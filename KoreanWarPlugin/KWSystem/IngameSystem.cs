using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Data;
using KoreanWarPlugin.Info;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.KWSystem
{
    public class IngameSystem
    {
        public static void OnPlayerKilled(UnturnedPlayer _uPlayer_Death) // 유저가 사망했을때 처리
        {
            PlayerComponent pc = _uPlayer_Death.Player.GetComponent<PlayerComponent>();
            pc.fireMode = 255;
            pc.isKnockDown = false;
            pc.isEnterRestrictArea = false;
            KillRecordInfo killRecordInfo = new KillRecordInfo("", "", "", "", "", false, false, false, false, (CSteamID)0, null);
            PlayerInfo deathInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer_Death.CSteamID);
            // 사망한 유저 정보 처리
            deathInfo.isDeployed = false;
            if (PluginManager.roundInfo.killRecordList.ContainsKey(_uPlayer_Death.CSteamID))
            {
                killRecordInfo = PluginManager.roundInfo.killRecordList[_uPlayer_Death.CSteamID];
                PlayerTeamRecordInfo deathRecordInfo = deathInfo.team ? PluginManager.teamInfo.playerRecordInfoList[_uPlayer_Death.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[_uPlayer_Death.CSteamID].team_1_RecordInfo;
                if (deathRecordInfo != null) deathRecordInfo.deathCount++;
                // 죽인 유저 정보 처리
                SteamPlayer steamPlayer = Provider.clients.FirstOrDefault(x => x.playerID == killRecordInfo.killerID);
                if (steamPlayer != null && killRecordInfo.killerTeam != killRecordInfo.deathTeam)
                {
                    PlayerTeamRecordInfo killerRecordInfo = killRecordInfo.killerTeam ? PluginManager.teamInfo.playerRecordInfoList[killRecordInfo.killerCsteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[killRecordInfo.killerCsteamID].team_1_RecordInfo;
                    if (killerRecordInfo != null) killerRecordInfo.killCount++;
                    GiveScoreAndCredit(steamPlayer, EScoreGainType.EnemyKill, 100, 5, _uPlayer_Death.DisplayName);
                }
                // 라운드 정보 갱신
                if (PluginManager.roundInfo.roundType != ERoundType.Free)
                {
                    if (deathInfo.team) PluginManager.roundInfo.ChangeScore(true, 1);
                    else PluginManager.roundInfo.ChangeScore(false, 1);
                    if (!PluginManager.instance.isRoundStart) return;
                }
                // 킬로그 갱신
                RefreshUIKillLog(new KillLog(killRecordInfo));
                // 기록 제거
                PluginManager.roundInfo.killRecordList.Remove(_uPlayer_Death.CSteamID);
            }
            // 죽은 유저에게 사망 UI 활성화
            ITransportConnection tc_Death = _uPlayer_Death.Player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectImageURL(47, tc_Death, false, "I_DeathKillerIcon", $"{killRecordInfo.killerAvatarUrl}");
            EffectManager.sendUIEffectText(47, tc_Death, false, "T_DeathKillerIcon", $"{killRecordInfo.killerName}");
            EffectManager.sendUIEffectImageURL(47, tc_Death, false, "I_DeathCause", $"{killRecordInfo.deathCauseUrl}");
            EffectManager.sendUIEffectText(47, tc_Death, false, "T_DeathCause", $"{killRecordInfo.deathCause}");
            UISystem.SetUIState_Death(_uPlayer_Death, tc_Death);
            PluginManager.instance.StartCoroutine(Cor_OnDeath(_uPlayer_Death, tc_Death));
            EffectManager.sendUIEffectVisibility(47, tc_Death, false, "L_VHealthBar", false);
        }
        public static void RefillAmmoFromVehicle(InteractableVehicle _vehicle,Player _player,ItemGunAsset _gunAsset) // 현재 소지한 무기에 맞게 트렁크에서 탄약을 보충
        {
            List<InventorySearch> inventorySearches = new List<InventorySearch>();
            inventorySearches = _player.inventory.search(EItemType.MAGAZINE, _gunAsset.magazineCalibers, false);
            List<InventorySearch> trunkSearches = new List<InventorySearch>();
            foreach (ushort caliber in _gunAsset.magazineCalibers) { _vehicle.trunkItems.search(trunkSearches, EItemType.MAGAZINE, caliber, false); }
            InventorySearch[] ammoTypeSearch = trunkSearches.Distinct(new InventorySearchComparer()).ToArray(); // 특정 아이템코드의 탄약을 한개씩 반환
            bool ischanged = false;
            foreach (InventorySearch search in ammoTypeSearch)
            {
                if (inventorySearches.FirstOrDefault(yy => yy.jar.item.id == search.jar.item.id) == default)
                {
                    if (_player.inventory.tryAddItem(search.jar.item, false))
                    {
                        byte trunkIndex = _vehicle.trunkItems.getIndex(search.jar.x, search.jar.y);
                        _vehicle.trunkItems.removeItem(trunkIndex);
                        ischanged = true;
                    }
                }
            }
            // 변경이 있었다면 다른 유저에게 정보 갱신
            if (ischanged) 
            {
                foreach (Passenger passenger in _vehicle.turrets)
                {
                    if (passenger.player == null || passenger.player.player == _player) continue;
                    ItemGunAsset gunAsset = new Item(passenger.turret.itemID, false).GetAsset<ItemGunAsset>();
                    if (gunAsset == null) continue;
                    bool hasCommonCaliber = _gunAsset.magazineCalibers.Intersect(gunAsset.magazineCalibers).Any();
                    if (hasCommonCaliber)
                    {
                        UpdateUIGun_RestAmmo(passenger.player.transportConnection, gunAsset.magazineCalibers, passenger.player.player.inventory);
                    }
                }
            }
        }
        public static void UpdateHealthBar(UnturnedPlayer _uPlayer, byte _health)
        {
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectText(47, tc, false, "T_HealthBar", $"{_health}");
            string amount = "";
            if (_health != 0)
            {
                amount = "".PadLeft(_health - 1);
                amount += ".";
            }
            EffectManager.sendUIEffectText(47, tc, false, "TF_HealthBarFill", $"{amount}");
        }
        public static void UpdateVehicleHealthBar(Player _player, InteractableVehicle _vehicle)
        {
            ITransportConnection tc = _player.channel.GetOwnerTransportConnection();
            int health = (int)((float)_vehicle.health / (float)_vehicle.asset.health * 100);
            EffectManager.sendUIEffectText(47, tc, false, "T_VHealthBar", $"{health}");
            string amount = "";
            if (health != 0)
            {
                amount = "".PadLeft(health - 1);
                amount += ".";
            }
            EffectManager.sendUIEffectText(47, tc, false, "TF_VHealthBarFill", $"{amount}");
        }
        public static void ActivateGunInfoUI(ITransportConnection _tc, ushort _itemID, byte[] _state, ushort[] _caliber, PlayerInventory _pInventory)
        {
            UpdateUIGun_Ammo(_tc, _state);
            UpdateUIGun_RestAmmo(_tc, _caliber, _pInventory);
            UpdateUIGun_Magazine(_tc, (ushort)(_state[8] + 256 * _state[9]));
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            WeaponInfoPreset weaponInfoPreset = configuration.weaponInfoPresets.FirstOrDefault(x => x.id == _itemID);
            if (weaponInfoPreset != default)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_GunInfo_GunIcon", weaponInfoPreset.iconUrl);
                EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_GunName", weaponInfoPreset.name);
            }
            else
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_GunInfo_GunIcon", "");
                EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_GunName", "정보없음");
            }
            EffectManager.sendUIEffectVisibility(47, _tc, false, "L_GunInfo", true);
        }
        public static void UpdateUIGun_Ammo(ITransportConnection _tc, byte[] _state)
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_Now", $"{_state[10]}");
        }
        public static void UpdateUIGun_RestAmmo(ITransportConnection _tc, ushort[] _caliber, PlayerInventory _pInventory)
        {
            ushort amount = 0;
            List<InventorySearch> searches = new List<InventorySearch>();
            searches = _pInventory.search(EItemType.MAGAZINE, _caliber, false);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMagazineInfo", true);
            if (_pInventory.player.equipment.isTurret)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(_pInventory.player);
                if (uPlayer.IsInVehicle)
                {
                    foreach (ushort caliber in _caliber)
                    {
                        searches = uPlayer.CurrentVehicle.trunkItems.search(searches, EItemType.MAGAZINE, caliber, false);
                    }

                    Dictionary<ushort, int> magazineList = new Dictionary<ushort, int>(); // 탄창 정보 <아이디,개수>
                    foreach (InventorySearch search in searches)
                    {
                        ushort id = search.jar.item.id;
                        if (!magazineList.ContainsKey(id)) magazineList.Add(id, 0);
                        magazineList[id] += search.jar.item.amount;
                    }
                    magazineList = magazineList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                    PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
                    int index = 0;
                    foreach (var dir in magazineList)
                    {
                        MagazineInfoPreset magazinePreset = configuration.magazineInfoPresets.FirstOrDefault(x => x.id == dir.Key);
                        if (magazinePreset == default) continue;
                        EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_MagazineInfo_{index}", $"{magazinePreset.iconUrl}");
                        EffectManager.sendUIEffectText(47, _tc, false, $"T_MagazineInfo_{index}", $"{dir.Value}");
                        EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_MagazineInfo_{index}", true);
                        index++;
                    }
                }
            }
            foreach (InventorySearch search in searches)
            {
                amount += search.jar.item.amount;
            }
            EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_Rest", $"{amount}");
        }
        public static void UpdateUIGun_RestAmmo(ITransportConnection _tc, ushort[] _caliber, PlayerInventory _pInventory,InteractableVehicle _vehicle) // 차량에 탑승할때 수동 갱신
        {
            ushort amount = 0;
            List<InventorySearch> searches = new List<InventorySearch>();
            searches = _pInventory.search(EItemType.MAGAZINE, _caliber, false);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMagazineInfo", true);
            foreach (ushort caliber in _caliber)
            {
                searches = _vehicle.trunkItems.search(searches, EItemType.MAGAZINE, caliber, false);
            }
            Dictionary<ushort, int> magazineList = new Dictionary<ushort, int>(); // 탄창 정보 <아이디,개수>
            foreach (InventorySearch search in searches)
            {
                ushort id = search.jar.item.id;
                if (!magazineList.ContainsKey(id)) magazineList.Add(id, 0);
                magazineList[id] += search.jar.item.amount;
            }
            magazineList = magazineList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            int index = 0;
            foreach (var dir in magazineList)
            {
                MagazineInfoPreset magazinePreset = configuration.magazineInfoPresets.FirstOrDefault(x => x.id == dir.Key);
                if (magazinePreset == default) continue;
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_MagazineInfo_{index}", $"{magazinePreset.iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_MagazineInfo_{index}", $"{dir.Value}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_MagazineInfo_{index}", true);
                index++;
            }
            foreach (InventorySearch search in searches)
            {
                amount += search.jar.item.amount;
            }
            EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_Rest", $"{amount}");
        }
        public static void UpdateUIGun_Magazine(ITransportConnection _tc, ushort _magazineID)
        {
            MagazineInfoPreset magazinePreset = PluginManager.instance.Configuration.Instance.magazineInfoPresets.FirstOrDefault(x => x.id == _magazineID);
            if (magazinePreset != default)
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_MagazineName", $"{magazinePreset.name}");
            }
            else
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_GunInfo_MagazineName", "");
            }
        }
        public static void RefreshUIKillLog(KillLog _killLog)
        {
            PluginManager.killLogs.Enqueue(_killLog);
            if (PluginManager.killLogs.Count > 5) PluginManager.killLogs.Dequeue();
            KillLog[] killLogArray = PluginManager.killLogs.ToArray();
            Array.Reverse(killLogArray);
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EPlayerUIState.InGame) continue;
                pc.killLogCount++;
                pc.killLogCount = (byte)Mathf.Clamp(pc.killLogCount, 0, 5);
                ITransportConnection tc = steamPlayer.transportConnection;
                for (int i = 0; i < pc.killLogCount; i++)
                {
                    if (killLogArray[i].deathTeam == pc.team)
                    {
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Death_0", $"{killLogArray[i].death}");
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Death_1", "");
                    }
                    else
                    {
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Death_0", "");
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Death_1", $"{killLogArray[i].death}");
                    }
                    if (killLogArray[i].killerTeam == pc.team)
                    {
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Killer_0", $"{killLogArray[i].killer}");
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Killer_1", "");
                    }
                    else
                    {
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Killer_0", "");
                        EffectManager.sendUIEffectText(47, tc, false, $"T_KillLog_{i}Killer_1", $"{killLogArray[i].killer}");
                    }
                    if (killLogArray[i].isImageLarge)
                    {
                        EffectManager.sendUIEffectImageURL(47, tc, false, $"I_KillLogL_{i}", $"{killLogArray[i].iconUrl}");
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"I_KillLogS_{i}", false);
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"I_KillLogL_{i}", true);
                    }
                    else
                    {
                        EffectManager.sendUIEffectImageURL(47, tc, false, $"I_KillLogS_{i}", $"{killLogArray[i].iconUrl}");
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"I_KillLogS_{i}", true);
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"I_KillLogL_{i}", false);
                    }
                    EffectManager.sendUIEffectVisibility(47, tc, false, $"P_KillLog_{i}", true);
                }
            }
        }
        public static void RefreshVehicleSupplyInfoToCrews(VehicleDeployInfo _vDeployInfo)
        {
            if (!_vDeployInfo.vehicle.anySeatsOccupied) return;
            foreach (Passenger passenger in _vDeployInfo.vehicle.passengers)
            {
                if (passenger.player == null) continue;
                ITransportConnection tc = passenger.player.transportConnection;
                RefreshVehicleSupplyInfo(_vDeployInfo, tc);
            }
        }
        public static void RefreshVehicleSupplyInfo(VehicleDeployInfo _vDeployInfo,ITransportConnection _tc)
        {
            if (_vDeployInfo.isSupplied) return;
            if (_vDeployInfo.isSupplying)
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_VehicleSupply", $"{_vDeployInfo.supplyElapsedTime}");
            }
            else
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_VehicleSupply", $"{(int)(_vDeployInfo.supplyCooltime - DateTime.UtcNow).TotalSeconds}");
            }
        }
        public static void GiveScoreAndCredit(SteamPlayer _steamPlayer, EScoreGainType _type, ushort _score, ushort _credit, string _target)
        {
            GiveScoreAndCredit(UnturnedPlayer.FromSteamPlayer(_steamPlayer), _type, _score, _credit, _target);
        }
        public static void GiveScoreAndCredit(UnturnedPlayer _uPlayer, EScoreGainType _type, ushort _score, ushort _credit, string _target)
        {
            if (PluginManager.roundInfo.roundType == ERoundType.Free) return;
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            // 점수 갱신
            if (_score != 0 && pc.isJoinedTeam)
            {
                PlayerTeamRecordInfo pTeamRecordInfo = pc.team ? PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_1_RecordInfo;
                LevelSystem.GainExperience(pc, _uPlayer, pTeamRecordInfo, _score);
            }
            // 크레딧 갱신
            if (_credit != 0)
            {
                PluginManager.playerDatabase.GiveCredit(_uPlayer, _credit);
            }
            // UI 활성화
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
            if (pc.localUIState == EPlayerUIState.InGame)
            {
                string preText = "";
                string middleText = "";
                string scoreText = "";
                string creditText = "";
                if (_score != 0) scoreText = $" {_score}xp";
                if (_credit != 0) creditText = $" {_credit}c";
                switch (_type)
                {
                    case EScoreGainType.EnemyKill:
                        preText = "적 사살";
                        middleText = $" {_target}";
                        break;
                    case EScoreGainType.ObjectiveCapture:
                        preText = "거점 점령";
                        break;
                    case EScoreGainType.ObjectiveNeturalize:
                        preText = "거점 중립화";
                        break;
                    case EScoreGainType.VehicleDestroy:
                        preText = "차량 파괴";
                        middleText = $" {_target}";
                        break;
                    case EScoreGainType.FriendlyRevive:
                        preText = "아군 소생";
                        break;
                    case EScoreGainType.FriendlySupply:
                        preText = "아군 보급";
                        break;
                    case EScoreGainType.FriendlyDeploy:
                        preText = "아군 수송차량에 배치";
                        break;
                }
                EffectManager.sendUIEffectText(47, tc, false, $"P_ScoreGain_{pc.scoreGainCount}", $"<color=#FFC067>{preText}</color><color=#FF4F4F>{middleText}</color><color=White>{scoreText}</color><color=#76FF6E>{creditText}</color>");
                EffectManager.sendUIEffectVisibility(47, tc, false, $"P_ScoreGain_{pc.scoreGainCount}", true);
                // 컴포넌트 정보 갱신
                pc.scoreGainCount++;
                if (pc.scoreGainCount >= 8) pc.scoreGainCount = 0;
            }
        }
        public static IEnumerator Cor_OnDeath(UnturnedPlayer _uPlayer, ITransportConnection _tc) // 사망 시 실행되는 코르틴 기능
        {
            SteamPlayer steamPlayer = _uPlayer.SteamPlayer();
            byte timer = PluginManager.instance.Configuration.Instance.respawnTimer;
            int elapsedTime = timer;
            while (elapsedTime >= 0)
            {
                if (!PluginManager.instance.isRoundStart) yield break;
                EffectManager.sendUIEffectText(47, _tc, false, "T_DeathTimer", $"{elapsedTime}");
                yield return new WaitForSeconds(1f);
                elapsedTime--;
            }
            if (Provider.clients.FirstOrDefault(x=>x.playerID == steamPlayer.playerID) == default) // 유저가 나갔다면 리턴
            {
                yield break;
            }
            PlayerData data = PluginManager.playerDatabase.FindData(_uPlayer.CSteamID);
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
            PlayerTeamRecordInfo pTeamRecordInfo = playerInfo.team ? PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_1_RecordInfo;
            UISystem.SetUIState_Loadout(_uPlayer, _tc, data, playerInfo, pTeamRecordInfo);
            _uPlayer.Player.life.ServerRespawn(false);
        }
        public static IEnumerator Cor_Invincible(UnturnedPlayer _uPlayer, ITransportConnection _tc) // 무적 시 실행되는 기능
        {
            SteamPlayer steamPlayer = _uPlayer.SteamPlayer();
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Invincible", true);
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            pc.isInvincible = true;
            byte timer = 5;
            int elapsedTime = timer;
            while (elapsedTime >= 0)
            {
                if (!PluginManager.instance.isRoundStart)
                {
                    pc.isInvincible = false;
                    yield break;
                }
                yield return new WaitForSeconds(1f);
                elapsedTime--;
            }
            if (Provider.clients.FirstOrDefault(x => x.playerID == steamPlayer.playerID) == default) // 유저가 나갔다면 리턴
            {
                yield break;
            }
            pc.isInvincible = false;
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Invincible", false);

        }
        public static IEnumerator Cor_OnRedeploy(UnturnedPlayer _uPlayer, ITransportConnection _tc) // 재배치 시 실행되는 기능
        {
            SteamPlayer steamPlayer = _uPlayer.SteamPlayer();
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Redeploying", true);
            _uPlayer.Player.equipment.dequip();
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            byte timer = PluginManager.instance.Configuration.Instance.respawnTimer;
            int elapsedTime = timer;
            while (elapsedTime >= 0)
            {
                if (!PluginManager.instance.isRoundStart || _uPlayer.Dead || pc.isKnockDown)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Redeploying", false);
                    pc.isRedeploying = false;
                    yield break;
                }
                EffectManager.sendUIEffectText(47, _tc, false, "T_Redeploying", $"{elapsedTime}");
                yield return new WaitForSeconds(1f);
                elapsedTime--;
            }
            if (Provider.clients.FirstOrDefault(x => x.playerID == steamPlayer.playerID) == default || _uPlayer.Dead) // 유저가 나갔다면 리턴
            {
                pc.isRedeploying = false;
                yield break;
            }
            PlayerData data = PluginManager.playerDatabase.FindData(_uPlayer.CSteamID);
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
            PlayerTeamRecordInfo pTeamRecordInfo = playerInfo.team ? PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_1_RecordInfo;
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            _uPlayer.Player.teleportToLocationUnsafe(configuration.spawnPos, configuration.spawnRot);
            playerInfo.isDeployed = false;
            pc.isRedeploying = false;
            UISystem.SetUIState_Loadout(_uPlayer, _tc, data, playerInfo, pTeamRecordInfo);
            yield return new WaitForSeconds(1f);
        }
    }
    public class InventorySearchComparer : IEqualityComparer<InventorySearch>
    {
        public bool Equals(InventorySearch x, InventorySearch y)
        {
            if (x == null || y == null)
                return false;
            return x.jar.item.id == x.jar.item.id;
        }
        public int GetHashCode(InventorySearch obj)
        {
            if (obj == null)
                return 0;
            return obj.jar.item.id.GetHashCode();
        }
    }
}

using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Data;
using KoreanWarPlugin.Info;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KoreanWarPlugin.KWSystem
{
    public class DeploySystem
    {
        public static void Deploy(UnturnedPlayer _uPlayer, bool _team, bool _vLeaderDeploy) // 장비 선택 후 배치
        {
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
            PlayerRecordInfo pRecordInfo = PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID];
            PlayerTeamRecordInfo pTeamRecordInfo = _team ? pRecordInfo.team_0_RecordInfo : pRecordInfo.team_1_RecordInfo;
            PlayerData data = PluginManager.playerDatabase.FindData(_uPlayer.CSteamID);
            if (playerInfo.classPrestIndex == ushort.MaxValue || playerInfo.creditCost > data.credit || !playerInfo.isDeployable) return; // 선택된 클래스가 없다면 리턴
            data.credit -= playerInfo.creditCost;
            PluginManager.playerDatabase.UpdateData();
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            ClassPresetTable classPresetTable = configuration.classPresets[playerInfo.classPrestIndex];
            EquipmentPresetTable equipmentPresetTable = configuration.equipmentPresets[classPresetTable.equipmentInstanceID];
            ClothPresetTable clothPresetTable = equipmentPresetTable.clothPresetList[playerInfo.equipmentIndex];
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
            if (_vLeaderDeploy) // 차량그룹 리더가 배치를 시도한경우
            {
                VehicleGroupInfo vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(playerInfo.vGroupInstanceID, _team);
                if (vGroupInfo != null)
                {
                    // 차량 스폰위치 확보 시도
                    SpawnPreset spawnPoint = FindVehicleSpawnPoint(_team, PluginManager.roundInfo.currentMapPreset);
                    if (spawnPoint == null)
                    {
                        // 스폰 위치가 전부 막힌 관계로 리턴
                        UISystem.SendPopUpInfo(tc, "현재 모든 차량 스폰 위치가 막힌 상태입니다.\n다른 유저가 스폰 위치를 떠날때까지 기다려야 합니다.");
                        EffectManager.sendUIEffectVisibility(47, tc, false, "P_Loading", false);
                        return;
                    }
                    foreach (SeatInfo seat in vGroupInfo.seats)
                    {
                        if (seat.cSteamID == CSteamID.NonSteamGS) continue;
                        PlayerInfo crewInfo = PluginManager.teamInfo.GetPlayerInfo(seat.cSteamID);
                        if (crewInfo == null)
                        {
                            UISystem.SendPopUpInfo(tc, "에러가 발생했습니다. \n에러코드 #001\n관리자에게 문의해주세요.");
                            EffectManager.sendUIEffectVisibility(47, tc, false, "P_Loading", false);
                            return;
                        }
                        if (!crewInfo.isDeployable)
                        {
                            UISystem.SendPopUpInfo(tc, $"전투배치가 불가능한 승무원이 있습니다.\n{crewInfo.displayName}");
                            EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
                            return;
                        }
                    }
                    VehiclePresetTable vPreset = configuration.vehiclePresets[vGroupInfo.vPresetIndex];
                    InteractableVehicle interactableVehicle = VehicleManager.spawnVehicleV2(vPreset.itemID, spawnPoint.position, Quaternion.Euler(0, spawnPoint.rotation, 0));
                    VehicleManager.ServerSetVehicleLock(interactableVehicle, CSteamID.NonSteamGS, _uPlayer.Player.quests.groupID, true); // 차량 락 지정
                    GiveVehicleAmmo(interactableVehicle, vPreset, false);
                    if (vPreset.isDeployable)
                    {
                        SteamPlayerID steamPlayerID = _uPlayer.SteamPlayer().playerID;
                        if (_team) PluginManager.teamInfo.team_0_spawnableVehicle.Add(new SpawnableVehicleInfo(steamPlayerID, interactableVehicle, vGroupInfo.vehicleTypePreset, _team));
                        else PluginManager.teamInfo.team_1_spawnableVehicle.Add(new SpawnableVehicleInfo(steamPlayerID, interactableVehicle, vGroupInfo.vehicleTypePreset, _team));
                        ActiveDeployMarker_VehicleAllToEveryone(_team);
                    }
                    if (_uPlayer.Dead) _uPlayer.Player.life.ServerRespawn(false);
                    foreach (SeatInfo seat in vGroupInfo.seats)
                    {
                        if (seat.cSteamID == CSteamID.NonSteamGS) continue;
                        UnturnedPlayer crew = UnturnedPlayer.FromCSteamID(seat.cSteamID);
                        if (crew.CSteamID != _uPlayer.CSteamID) Deploy(crew, _team, false);
                        VehicleManager.ServerForcePassengerIntoVehicle(crew.Player, interactableVehicle);
                    }
                    // 배치된 차량그룹 정보 추가
                    PluginManager.teamInfo.AddDeployVehicleInfo(interactableVehicle, vGroupInfo.vTypeIndex, vPreset, vGroupInfo.vehicleTypePreset, interactableVehicle, _team);
                    // 차량그룹 정보 제거
                    PluginManager.teamInfo.OnVehicleDeploy(vGroupInfo, _team);
                    // 정보 갱신
                    VehicleGroupSystem.RefreshUIVehicleGroupAllToEveryone(_team);
                    VehicleGroupSystem.RefreshUIVehicleDeployAllToEveryone(_team);
                }
            }
            // 스폰 전 스폰 가능 상태인지 체크
            if (6 <= playerInfo.spawnIndex && playerInfo.spawnIndex <= 15) // 차량 스폰인경우
            {
                List<SpawnableVehicleInfo> spawnableVehicleList = _team ? PluginManager.teamInfo.team_0_spawnableVehicle : PluginManager.teamInfo.team_1_spawnableVehicle;
                if(PluginManager.roundInfo.restrictArea_Vehicles.ContainsKey(spawnableVehicleList[playerInfo.spawnIndex - 6].vehicle))
                {
                    UISystem.SendPopUpInfo(tc, "배치하려는 차량이 현재 제한 구역에 있습니다.");
                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
                    return;
                }
            }
            // 유저가 소유한 모든 장비 제거
            for (byte i = 0; i < 7; i++)
            {
                for (int k = _uPlayer.Inventory.items[i].items.Count - 1; k >= 0; k--)
                {
                    _uPlayer.Inventory.removeItem(i, (byte)k);
                }
            }
            // 장구류
            ushort shirt = 0, pants = 0, head = 0, backpack = 0, vest = 0, mask = 0, glasses = 0;
            // 레벨별로 차등해서 장구류 제공
            if (clothPresetTable.equipment_Head.Length != 0)
            {
                if (clothPresetTable.equipment_Head.Length > pTeamRecordInfo.level) head = clothPresetTable.equipment_Head[pTeamRecordInfo.level];
                else head = clothPresetTable.equipment_Head[clothPresetTable.equipment_Head.Length - 1];
            }
            if (clothPresetTable.equipment_Mask.Length != 0)
            {
                if (clothPresetTable.equipment_Mask.Length > pTeamRecordInfo.level) mask = clothPresetTable.equipment_Mask[pTeamRecordInfo.level];
                else mask = clothPresetTable.equipment_Mask[clothPresetTable.equipment_Mask.Length - 1];
            }
            if (clothPresetTable.equipment_Glasses.Length != 0)
            {
                if (clothPresetTable.equipment_Glasses.Length > pTeamRecordInfo.level) glasses = clothPresetTable.equipment_Glasses[pTeamRecordInfo.level];
                else glasses = clothPresetTable.equipment_Glasses[clothPresetTable.equipment_Glasses.Length - 1];
            }
            if (clothPresetTable.equipment_Vest.Length != 0)
            {
                if (clothPresetTable.equipment_Vest.Length > pTeamRecordInfo.level) vest = clothPresetTable.equipment_Vest[pTeamRecordInfo.level];
                else vest = clothPresetTable.equipment_Vest[clothPresetTable.equipment_Vest.Length - 1];
            }
            if (clothPresetTable.equipment_shirt.Length != 0)
            {
                if (clothPresetTable.equipment_shirt.Length > pTeamRecordInfo.level) shirt = clothPresetTable.equipment_shirt[pTeamRecordInfo.level];
                else shirt = clothPresetTable.equipment_shirt[clothPresetTable.equipment_shirt.Length - 1];
            }
            if (clothPresetTable.equipment_Pant.Length != 0)
            {
                if (clothPresetTable.equipment_Pant.Length > pTeamRecordInfo.level) pants = clothPresetTable.equipment_Pant[pTeamRecordInfo.level];
                else pants = clothPresetTable.equipment_Pant[clothPresetTable.equipment_Pant.Length - 1];
            }
            if (clothPresetTable.equipment_Backpack.Length != 0)
            {
                if (clothPresetTable.equipment_Backpack.Length > pTeamRecordInfo.level) backpack = clothPresetTable.equipment_Backpack[pTeamRecordInfo.level];
                else backpack = clothPresetTable.equipment_Backpack[clothPresetTable.equipment_Backpack.Length - 1];
            }
            _uPlayer.Player.clothing.updateClothes(shirt, 100, new byte[0], pants, 100, new byte[0], head, 100, new byte[0], backpack, 100, new byte[0], vest, 100, new byte[0], mask, 100, new byte[0], glasses, 100, new byte[0]);
            bool equipGun = playerInfo.vGroupInstanceID == ushort.MaxValue ? true : false; // 차량에 타지 않으면 무기들 들고 차량에 타면 못들게 하기
            // 로드아웃 제공
            GiveLoadout(_uPlayer, playerInfo, classPresetTable, false, false);
            // 죽은 경우 리스폰
            if (_uPlayer.Dead) _uPlayer.Player.life.ServerRespawn(false);
            bool isVehicleSpawn = false;
            if (playerInfo.vGroupInstanceID == ushort.MaxValue) // 차량에서 배치하는것이 아닌경우
            {
                Vector3 spawnPos = Vector3.zero;
                float spawnRot = 0;
                if(0 <= playerInfo.spawnIndex && playerInfo.spawnIndex <= 4) // 거점 스폰인경우
                {
                    // 조건에 따라 스폰 위치 할당
                    SpawnPreset[] spawnPresets = null;
                    if (playerInfo.team && PluginManager.roundInfo.objectives[playerInfo.spawnIndex].team_1_Players.Count > 0)
                        spawnPresets = PluginManager.roundInfo.objectives[playerInfo.spawnIndex].team_0_spawn;
                    else if (!playerInfo.team && PluginManager.roundInfo.objectives[playerInfo.spawnIndex].team_0_Players.Count > 0)
                        spawnPresets = PluginManager.roundInfo.objectives[playerInfo.spawnIndex].team_1_spawn;
                    else
                        spawnPresets = PluginManager.roundInfo.objectives[playerInfo.spawnIndex].objectiveSpawn;
                    // 랜덤으로 스폰위치 선택 후 위치 이동
                    int random = Random.Range(0, spawnPresets.Length);
                    SpawnPreset spawnPreset = _team ? spawnPresets[random] : spawnPresets[random];
                    spawnPos = _team ? spawnPreset.position : spawnPreset.position;
                    spawnRot = _team ? spawnPreset.rotation : spawnPreset.rotation;
                }
                else if(playerInfo.spawnIndex == 5) // 기지스폰인 경우
                {
                    SpawnPreset[] spawnPresets = _team ? PluginManager.roundInfo.currentMapPreset.baseSpawnPos_0 : PluginManager.roundInfo.currentMapPreset.baseSpawnPos_1;
                    int random = Random.Range(0, spawnPresets.Length);
                    SpawnPreset spawnPreset = _team ? spawnPresets[random] : spawnPresets[random];
                    spawnPos = _team ? spawnPreset.position : spawnPreset.position;
                    spawnRot = _team ? spawnPreset.rotation : spawnPreset.rotation;
                }
                else if(6 <= playerInfo.spawnIndex && playerInfo.spawnIndex <= 15) // 차량 스폰인경우
                {
                    List<SpawnableVehicleInfo> spawnableVehicleList = _team ? PluginManager.teamInfo.team_0_spawnableVehicle : PluginManager.teamInfo.team_1_spawnableVehicle;
                    _uPlayer.Player.GetComponent<PlayerComponent>().isVehicleSpawn = true;
                    VehicleManager.ServerForcePassengerIntoVehicle(_uPlayer.Player, spawnableVehicleList[playerInfo.spawnIndex - 6].vehicle);
                    SteamPlayer vehicleOwner = Provider.clients.FirstOrDefault(x => x.playerID == spawnableVehicleList[playerInfo.spawnIndex - 6].steamPlayerID);
                    if (vehicleOwner != default)
                    {
                        if(spawnableVehicleList[playerInfo.spawnIndex - 6].steamPlayerID != _uPlayer.SteamPlayer().playerID) IngameSystem.GiveScoreAndCredit(vehicleOwner, EnumTable.EScoreGainType.FriendlyDeploy, 10, 2, "");
                    }
                    isVehicleSpawn = true;
                }
                if (!isVehicleSpawn)
                {
                    if (playerInfo.primaryIndex != ushort.MaxValue) _uPlayer.Player.equipment.ServerEquip(0, 0, 0);
                    else if (playerInfo.secondaryIndex != ushort.MaxValue) _uPlayer.Player.equipment.ServerEquip(1, 0, 0);
                    _uPlayer.Player.teleportToLocationUnsafe(spawnPos, spawnRot);
                }
                _uPlayer.Player.stance.checkStance(EPlayerStance.STAND, true);
            }
            // 무적상태 적용
            if(!isVehicleSpawn) PluginManager.instance.StartCoroutine(IngameSystem.Cor_Invincible(_uPlayer, tc));
            _uPlayer.Heal(100);
            playerInfo.isDeployed = true;
            if (pRecordInfo.teamChangeableDatetime < DateTime.UtcNow) pRecordInfo.teamChangeableDatetime = DateTime.UtcNow.AddSeconds(PluginManager.instance.Configuration.Instance.teamChangeDelay);
            // 설정 갱신
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            pc.isMedic = classPresetTable.isMedic;
            // 인게임 UI 상태로 전환
            UISystem.SetUIState_InGame(_uPlayer, _uPlayer.Player.channel.GetOwnerTransportConnection());
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
        }
        public static void GiveVehicleAmmo(InteractableVehicle _vehicle, VehiclePresetTable _vPreset, bool _clearStorage)
        {
            if (_clearStorage)
            {
                int count = _vehicle.trunkItems.items.Count;
                for (int i = 0; i < count; i++)
                {
                    _vehicle.trunkItems.removeItem(0);
                }
            }
            foreach (VAmmoPresetTable ammoPreset in _vPreset.ammos)
            {
                for (int i = 0; i < ammoPreset.amount; i++) { _vehicle.trunkItems.tryAddItem(new Item(ammoPreset.itemID, true), false); }
            }
        }
        public static void GiveLoadout(UnturnedPlayer _uPlayer, bool _removeInventory, bool _autoEquip)
        {
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
            ClassPresetTable classPresetTable = PluginManager.instance.Configuration.Instance.classPresets[playerInfo.classPrestIndex];
            GiveLoadout(_uPlayer, playerInfo, classPresetTable, _removeInventory, _autoEquip);
        }
        public static void GiveLoadout(UnturnedPlayer _uPlayer,PlayerInfo _playerInfo,ClassPresetTable _classPresetTable, bool _removeInventory, bool _autoEquip)
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            if (_removeInventory)
            {
                // 유저가 소유한 모든 장비 제거
                for (byte i = 0; i < 7; i++)
                {
                    for (int k = _uPlayer.Inventory.items[i].items.Count - 1; k >= 0; k--)
                    {
                        _uPlayer.Inventory.removeItem(i, (byte)k);
                    }
                }
            }
            byte magazineAmount = 0;
            // 병과 전용 기본 아이템 제공
            foreach (ushort itemID in _classPresetTable.loadoutList) { _uPlayer.Inventory.tryAddItem(new Item(itemID, true), false); }
            // 주무기
            if (_playerInfo.primaryIndex != ushort.MaxValue)
            {
                PrimaryPresetTable primaryPresetTable = configuration.primaryPresets[_playerInfo.primaryIndex];
                Item gunItem = new Item(primaryPresetTable.itemID, true);
                if (_playerInfo.sightIndex != ushort.MaxValue) // 조준경 배정
                {
                    LoadoutTable sightTable = configuration.attachmentPresets[_playerInfo.sightIndex];
                    gunItem.state[0] = (byte)sightTable.itemID;
                    gunItem.state[1] = (byte)(sightTable.itemID / 256);
                }
                if (_playerInfo.tacticalIndex != ushort.MaxValue) // 전술장비 배정
                {
                    LoadoutTable tacticalTable = configuration.attachmentPresets[_playerInfo.tacticalIndex];
                    gunItem.state[2] = (byte)tacticalTable.itemID;
                    gunItem.state[3] = (byte)(tacticalTable.itemID / 256);
                }
                if (_playerInfo.gripIndex != ushort.MaxValue) // 손잡이 배정
                {
                    LoadoutTable gripTable = configuration.attachmentPresets[_playerInfo.gripIndex];
                    gunItem.state[4] = (byte)gripTable.itemID;
                    gunItem.state[5] = (byte)(gripTable.itemID / 256);
                }
                if (_playerInfo.magazineIndex != ushort.MaxValue) // 탄창 배정
                {
                    LoadoutTable magazineTable = configuration.attachmentPresets[_playerInfo.magazineIndex];
                    Item magazineItem = new Item(magazineTable.itemID, true);
                    gunItem.state[8] = (byte)magazineTable.itemID;
                    gunItem.state[9] = (byte)(magazineTable.itemID / 256);
                    gunItem.state[10] = magazineItem.amount;
                    _uPlayer.Inventory.items[0].tryAddItem(gunItem);
                    if (magazineTable.amount_equipment.Length == 0) magazineAmount = magazineTable.amount;
                    else magazineAmount = magazineTable.amount_equipment[_playerInfo.equipmentIndex];
                    for (int i = 0; i < magazineAmount; i++) { _uPlayer.Inventory.tryAddItem(magazineItem, false); }
                }
                if (_autoEquip)
                {
                    if(_playerInfo.primaryIndex != ushort.MaxValue) _uPlayer.Player.equipment.ServerEquip(0, 0, 0);
                    else if (_playerInfo.secondaryIndex != ushort.MaxValue) _uPlayer.Player.equipment.ServerEquip(1, 0, 0);
                }
            }
            // 보조무기
            if (_playerInfo.secondaryIndex != ushort.MaxValue)
            {
                magazineAmount = 0;
                SecondaryPresetTable secondaryPresetTable = configuration.secondaryPresets[_playerInfo.secondaryIndex];
                _uPlayer.Inventory.items[1].tryAddItem(new Item(secondaryPresetTable.itemID, true));
                if (secondaryPresetTable.amount_equipment.Length == 0) magazineAmount = secondaryPresetTable.magazineAmount;
                else magazineAmount = secondaryPresetTable.amount_equipment[_playerInfo.equipmentIndex];
                for (int i = 0; i < magazineAmount; i++) { _uPlayer.Inventory.tryAddItem(new Item(secondaryPresetTable.magazineItemID, true), false); }
            }
            // 폭팔물
            if (_playerInfo.explosive_0Index != ushort.MaxValue)
            {
                magazineAmount = 0;
                SecondaryPresetTable explosivePresetTable = configuration.explosivePresets[_playerInfo.explosive_0Index];
                _uPlayer.Inventory.tryAddItem(new Item(explosivePresetTable.itemID, true), false);
                if (explosivePresetTable.magazineItemID != 0)
                {
                    if (explosivePresetTable.amount_equipment.Length == 0) magazineAmount = explosivePresetTable.magazineAmount;
                    else magazineAmount = explosivePresetTable.amount_equipment[_playerInfo.equipmentIndex];
                    for (int i = 0; i < magazineAmount; i++) { _uPlayer.Inventory.tryAddItem(new Item(explosivePresetTable.magazineItemID, true), false); }
                }
            }
            if (_playerInfo.explosive_1Index != ushort.MaxValue)
            {
                magazineAmount = 0;
                SecondaryPresetTable explosivePresetTable = configuration.explosivePresets[_playerInfo.explosive_1Index];
                _uPlayer.Inventory.tryAddItem(new Item(explosivePresetTable.itemID, true), false);
                if (explosivePresetTable.magazineItemID != 0)
                {
                    if (explosivePresetTable.amount_equipment.Length == 0) magazineAmount = explosivePresetTable.magazineAmount;
                    else magazineAmount = explosivePresetTable.amount_equipment[_playerInfo.equipmentIndex];
                    for (int i = 0; i < magazineAmount; i++) { _uPlayer.Inventory.tryAddItem(new Item(explosivePresetTable.magazineItemID, true), false); }
                }
            }
            // 특수장비
            if (_playerInfo.utility_0Index != ushort.MaxValue) _uPlayer.Inventory.tryAddItem(new Item(configuration.utilityPresets[_playerInfo.utility_0Index].itemID, true), false);
            if (_playerInfo.utility_1Index != ushort.MaxValue) _uPlayer.Inventory.tryAddItem(new Item(configuration.utilityPresets[_playerInfo.utility_1Index].itemID, true), false);

        }
        public static SpawnPreset FindVehicleSpawnPoint(bool _team, MapPreset _mapPreset)
        {
            SpawnPreset[] spawnPresets = _team ? _mapPreset.vehicleSpawnPos_0 : _mapPreset.vehicleSpawnPos_1;
            foreach (SpawnPreset spawnPreset in spawnPresets)
            {
                LayerMask layerMask = LayerMask.GetMask("Vehicle") | LayerMask.GetMask("Player");
                Collider[] colliders = Physics.OverlapBox(spawnPreset.position + Vector3.up * 5, new Vector3(5, 5, 5), Quaternion.Euler(0, spawnPreset.rotation, 0), layerMask);
                if (colliders.Length == 0)
                {
                    return spawnPreset;
                }
            }
            return null;
        }
        public static void SetUpDeployMarker_Static(ITransportConnection _tc, bool _team,PlayerInfo _playerInfo) // 모든 정적 마커 세팅
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarker_Static", true);
            string textPos = _team ? PluginManager.roundInfo.team_0_baseTextPos : PluginManager.roundInfo.team_1_baseTextPos;
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Marker_Base", true);
            EffectManager.sendUIEffectText(47, _tc, false, "T_Marker_Base", $"{textPos}");
            SetUpDeployMarker_Objective(_tc, _team); // 거점 마커 세팅
            SetUpInfoMarker_Objective(_tc, _team);
        }
        public static void SetUpDeployMarker_Objective(ITransportConnection _tc, bool _team) // 모든 스폰 거점 세팅
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                ObjectiveInfo objective = PluginManager.roundInfo.objectives[i];
                string textPos = objective.textPos_Center;
                EffectManager.sendUIEffectText(47, _tc, false, $"T_Marker_Objective_{i}", $"{textPos}");
            }
        }
        public static void SetUpInfoMarker_Objective(ITransportConnection _tc, bool _team) // 모든 거점 위치 마커 세팅 후 활성화
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                string textPos = PluginManager.roundInfo.objectives[i].textPos_Center;
                EffectManager.sendUIEffectText(47, _tc, false, $"T_Marker_ObjectivePos_{i}", $"{textPos}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_Marker_ObjectivePos_{i}", true);
                RefreshUIMapObjectiveTeam(_tc, _team, i);
            }
        }
        public static void ActiveDeployMarker_Static(ITransportConnection _tc, bool _team, PlayerInfo _playerInfo)
        {
            // 스폰 마커 활성화 시키기
            string textPos = _team ? PluginManager.roundInfo.team_0_baseTextPos : PluginManager.roundInfo.team_1_baseTextPos;
            if (_playerInfo.vGroupInstanceID != ushort.MaxValue && _playerInfo.spawnIndex != 5)
            {
                _playerInfo.spawnIndex = 5;
                _playerInfo.spawnInstaceID_Dynamic = ushort.MaxValue;
                EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarkerSelect", true);
                EffectManager.sendUIEffectVisibility(47, _tc, false, "A_Marker_Base_Select", true);
            }
            ActiveDeployMarker_ObjectiveAll(_tc, _team, _playerInfo);
        }
        public static void ActiveDeployMarker_ObjectiveToEveryone(bool _team, byte _index) // 모든 팀원에게 거점 스폰 마커 활성화 여부 갱신
        {
            if (PluginManager.teamInfo.playerInfoList.Count == 0) return;
            EnumTable.EObjectiveTeam eTeam = _team ? EnumTable.EObjectiveTeam.Team_0 : EnumTable.EObjectiveTeam.Team_1;
            bool active = eTeam == PluginManager.roundInfo.objectives[_index].team ? true : false;
            foreach (var playerInfoDir in PluginManager.teamInfo.playerInfoList)
            {
                if (playerInfoDir.Value.team != _team) continue;
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerInfoDir.Key);
                if (uPlayer == null) continue;
                PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
                if (playerInfoDir.Value.vGroupInstanceID != ushort.MaxValue) active = false;
                EffectManager.sendUIEffectVisibility(47, tc, false, $"P_Marker_Objective_{_index}", active);
                if(!active && playerInfoDir.Value.spawnIndex == _index) // 거점이 비활성화 되었는데 유저가 해당 거점을 선택한 상태인 경우 비활성화 시키기
                {
                    playerInfoDir.Value.spawnIndex = byte.MaxValue;
                    playerInfoDir.Value.spawnInstaceID_Dynamic = ushort.MaxValue;
                    EffectManager.sendUIEffectVisibility(47, tc, false, "Trigger_RemoveMarkerSelect", true);
                    UISystem.RefreshDeployState(playerInfoDir.Value, tc);
                }
            }
        }
        public static void ActiveDeployMarker_ObjectiveAll(ITransportConnection _tc, bool _team,PlayerInfo _playerInfo)
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                ActiveDeployMarker_Objective(_tc, _team, i, _playerInfo);
            }
        }
        public static void ActiveDeployMarker_Objective(ITransportConnection _tc, bool _team, byte _index,PlayerInfo _playerInfo) // 거점 체크 후 활성화
        {
            ObjectiveInfo objective = PluginManager.roundInfo.objectives[_index];
            EnumTable.EObjectiveTeam eTeam = _team ? EnumTable.EObjectiveTeam.Team_0 : EnumTable.EObjectiveTeam.Team_1;
            if (objective.team == eTeam && _playerInfo.vGroupInstanceID == ushort.MaxValue)
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_Marker_Objective_{_index}", true);
            }
            else
            {
                bool active = eTeam == objective.team ? true : false;
                if (!active && _playerInfo.spawnIndex == _index) // 거점이 비활성화 되었는데 유저가 해당 거점을 이미 선택한 경우
                {
                    _playerInfo.spawnIndex = byte.MaxValue;
                    _playerInfo.spawnInstaceID_Dynamic = ushort.MaxValue;
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarkerSelect", true);
                    UISystem.RefreshDeployState(_playerInfo, _tc);
                }
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_Marker_Objective_{_index}", false);
            }
        }
        public static void ActiveDeployMarker_VehicleAllToEveryone(bool _team)
        {
            //List<PlayerInfo> playerList = _team ? PluginManager.teamInfo.team_0_Players : PluginManager.teamInfo.team_1_Players;
            foreach (var playerInfoDir in PluginManager.teamInfo.playerInfoList)
            {
                if (playerInfoDir.Value.team != _team) continue;
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerInfoDir.Key);
                if (uPlayer == null) continue;
                PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
                ActiveDeployMarker_VehicleAll(tc, _team, playerInfoDir.Value);
            }
        }
        public static void ActiveDeployMarker_VehicleAll(ITransportConnection _tc,bool _team,PlayerInfo _playerInfo) // 모든 스폰가능 차량 개수 및 위치 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarker_Dynamic", true);
            if(_playerInfo.vGroupInstanceID != ushort.MaxValue)
            {
                if(6 <= _playerInfo.spawnIndex)
                {
                    _playerInfo.spawnIndex = byte.MaxValue;
                    _playerInfo.spawnInstaceID_Dynamic = ushort.MaxValue;
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarkerSelect", true);
                    UISystem.RefreshDeployState(_playerInfo, _tc);
                }
                return;
            }
            List<SpawnableVehicleInfo> spawnableVehicleList = _team ? PluginManager.teamInfo.team_0_spawnableVehicle : PluginManager.teamInfo.team_1_spawnableVehicle;
            bool isVehicleFound = false;
            for (byte i = 0; i < spawnableVehicleList.Count; i++)
            {
                EffectManager.sendUIEffectText(47, _tc, false, $"T_Marker_Dynamic_{i}", $"{spawnableVehicleList[i].textPos}");
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_Marker_Dynamic_{i}", spawnableVehicleList[i].vTypePreset.iconUrl);
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_Marker_Dynamic_{i}", true);
                if (isVehicleFound) continue;
                if(_playerInfo.spawnInstaceID_Dynamic != ushort.MaxValue && spawnableVehicleList[i].instanceID == _playerInfo.spawnInstaceID_Dynamic)
                {
                    _playerInfo.spawnIndex = (byte)(i + 6);
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarkerSelect", true);
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_Marker_Dynamic_{i}_Select", true);
                    isVehicleFound = true;
                }
            }
            if (_playerInfo.spawnInstaceID_Dynamic != ushort.MaxValue && !isVehicleFound)
            {
                _playerInfo.spawnIndex = byte.MaxValue;
                _playerInfo.spawnInstaceID_Dynamic = ushort.MaxValue;
                EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMarkerSelect", true);
                UISystem.RefreshDeployState(_playerInfo, _tc);
            }
        }
        public static void UpdateMarkerPosition_VehicleToEveryone(bool _team, byte _index) // 모두에게 스폰가능 차량 위치 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam || pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                UpdateMarkerPosition_Vehicle(tc, _team, _index);
            }
        }
        public static void UpdateMarkerPosition_Vehicle(ITransportConnection _tc, bool _team, byte _index) // 스폰가능 차량의 위치 갱신
        {
            List<SpawnableVehicleInfo> spawnableVehicleList = _team ? PluginManager.teamInfo.team_0_spawnableVehicle : PluginManager.teamInfo.team_1_spawnableVehicle;
            EffectManager.sendUIEffectText(47, _tc, false, $"T_Marker_Dynamic_{_index}", $"{spawnableVehicleList[_index].textPos}");
        }
        public static void RefreshUIMapObjectiveTeamToEveryone(byte _index)
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIMapObjectiveTeam(tc, pc.team, _index);
            }
        }
        public static void RefreshUIMapObjectiveTeamAll(ITransportConnection _tc, bool _team)
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++) { RefreshUIMapObjectiveTeam(_tc, _team, i); }
        }
        public static void RefreshUIMapObjectiveTeam(ITransportConnection _tc, bool _team,byte _index)
        {
            ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[_index];
            int objTeam = 0;
            switch (objectiveInfo.team)
            {
                case EnumTable.EObjectiveTeam.Team_0:
                    objTeam = _team ? 0 : 2;
                    break;
                case EnumTable.EObjectiveTeam.Netural:
                    objTeam = 1;
                    break;
                case EnumTable.EObjectiveTeam.Team_1:
                    objTeam = _team ? 2 : 0;
                    break;
            }
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_ObjectivePos_{_index}_{objTeam}", true);
        }
        public static string SpawnMarkerPositon(Vector3 _spawnPos)
        {
            Vector2 spawnPos = new Vector2(_spawnPos.x, _spawnPos.z);
            return SpawnMarkerPositon(spawnPos);
        }
        public static string SpawnMarkerPositon(Vector2 _spawnPos)
        {
            // 마커 위치 계산
            float mapSize = (float)PluginManager.roundInfo.currentMapPreset.mapSize;
            Vector2 mapOrigionPos = PluginManager.roundInfo.currentMapPreset.mapPositon;
            Vector2 markerPos = _spawnPos - mapOrigionPos;
            float uiRatio = 600 / mapSize;
            Vector2Int uiMarkerPos = new Vector2Int(Mathf.Clamp((int)(markerPos.x * uiRatio), 40, 560), Mathf.Clamp((int)(markerPos.y * uiRatio) - 4, 36, 556));
            string textPos = "";
            for (int i = 0; i < uiMarkerPos.y; i++) { textPos += "\n"; }
            textPos += ".".PadLeft(uiMarkerPos.x);
            return textPos;
        }
    }
}
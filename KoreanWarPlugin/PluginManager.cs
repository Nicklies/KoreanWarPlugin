using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using KoreanWarPlugin.KWSystem;
using Rocket.API.Serialisation;
using Rocket.Unturned.Chat;
using Steamworks;
using KoreanWarPlugin.Database;
using KoreanWarPlugin.Data;
using KoreanWarPlugin.Info;
using KoreanWarPlugin.Configuration.Preset;
using UnityEngine;
using System.Collections;
using Rocket.Unturned.Events;
using static KoreanWarPlugin.EnumTable;
using HarmonyLib;
using System.IO;
using Environment = System.Environment;
using KoreanWarPlugin.HarmonyFix;
using KoreanWarPlugin.Queue;
using Rocket.Core;

namespace KoreanWarPlugin
{
    public class PluginManager : RocketPlugin<PluginConfiguration>
    {
        public static PluginManager instance { get; private set; }
        // 데이터 베이스
        public static PlayerDatabase playerDatabase { get; private set; }
        public static TeamDatabase teamDatabase { get; private set; }
        // 정보
        public static TeamInfo teamInfo { get; set; }
        public static RoundInfo roundInfo { get; set; }
        public static Dictionary<string, string> icons { get; set; } // 아이콘 정보
        public static Queue<KillLog> killLogs { get; set; }
        public bool isRoundStart { get; set; } // 라운드 시작 여부
        public bool isVoteEnd { get; set; } // 투표가 끝났는지 여부
        public byte voteTimer { get; set; } // 투표 종료 시간
        public byte voteIniTime { get; set; } // 투표 시간 초기값
        // 코르틴
        public Coroutine freeModeEnd { get; set; } // 자유모드 종료 카운트 코르틴
        public Coroutine loopCoroutine { get; set; } // 루프 코르틴
        public Coroutine voteCoroutine { get; set; } // 투표 코르틴
        public Coroutine roundStartCoroutine { get; set; } // 게임시작 코르틴
        //대기열
        public IngamePopUpQueueManager ingamePopUpQueue { get; set; }
        public Harmony patcher;
        protected override void Load()
        {
            instance = this;
            // 아이콘 정보 배정
            icons = new Dictionary<string, string>
            {
                { "player", "https://drive.google.com/uc?id=10H5Je6mcPcY8TsjioeBv_tuoSawzTC0O" }, // 플레이어 아이콘
                { "lock", "https://drive.google.com/uc?id=1Ye_pzViuhhF07dmx_3QBs0ThaT0kmwtl" }, // 잠금 아이콘
                { "time", "https://drive.google.com/uc?id=1J-Zu5k-f_yZsMCjid7tNbDr8Vv5gqS35" }, // 시간 아이콘
                { "punch", "https://drive.google.com/uc?id=1bGKRcvThEanX0Lwj6l8ofd9CWItLESeA" }, // 펀치 아이콘
                { "roadkill", "https://drive.google.com/uc?id=1Q6Y5eA9F4vTMqB_8IBPyX8g8UeNkwAW4" }, // 로드킬 아이콘
                { "landmine", "https://drive.google.com/uc?id=1UsXMzKCVRNluYM0xIVjd2-nOISxNXW7Q" }, // 지뢰 아이콘
                { "explosion", "https://drive.google.com/uc?id=12CJ1FDXV6COQneB9V8xmLlt98GcRF-2w" }, // 폭발 아이콘
                { "burn", "https://drive.google.com/uc?id=1I9kNCpZ1sIXgHZtmiFLKTX6P0yIjo1GA" }, // 화재 아이콘
                { "fall", "https://drive.google.com/uc?id=16JbTe2OLLEiD4TrnmakIlBOgB4ZjJpRn" }, // 추락 아이콘
                { "bleeding", "https://drive.google.com/uc?id=1Dl6fl1sHehPfmYs6wV1RWNZo-c2F9u1g" }, // 출혈 아이콘
                { "machinegun", "https://drive.google.com/uc?id=1UNGTpGz1fUzUyFjBcaJfDflzrSMz0a-w" }, // 기관총 아이콘
                { "cannon", "https://drive.google.com/uc?id=1pjamN_WeZPUHdj5aT76wf9kiyqd-uptm" }, // 대포 아이콘
                { "oxygen", "https://drive.google.com/uc?id=1Duld04aG9eSxG5-UIkWb6D1OlevR1ixx" }, // 산소 아이콘
                { "shred", "https://drive.google.com/uc?id=1a5mDbcjxN319JnRgWMWJHDRQwmalm_hW" }, // 철조망 아이콘
                { "noPlayer", "https://drive.google.com/uc?id=1Li3cuWr2FEU8PMmCZgQy2Y-CWiMBWb_V" }, // 사람 없음 아이콘
                { "VehicleDestroy", "https://drive.google.com/uc?id=1GxAjGJDvVMbxnJUxum94CVs264RNAKGq" }, // 차량파괴 아이콘
                { "Approve", "https://drive.google.com/uc?id=1uIOUFue-T-nayYq2bPm9CBMh-WjevDxB" }, // 확인 아이콘
                { "arena", "https://drive.google.com/uc?id=1Ep6sAMcOV9BTpni4x2Az9o52hrlplmYe" }, // 아레나 아이콘
            };
            // 하모니
            Harmony.DEBUG = true;
            patcher = new Harmony("net.korean.war");
            patcher.PatchAll();
            // 데이터베이스 초기화
            playerDatabase = new PlayerDatabase();
            playerDatabase.Reload();
            playerDatabase.UpdateData();
            teamDatabase = new TeamDatabase();
            teamDatabase.Reload();
            teamDatabase.UpdateData();
            // 정보 초기화
            teamInfo = new TeamInfo();
            roundInfo = new RoundInfo();
            killLogs = new Queue<KillLog>();
            voteTimer = 0;
            voteIniTime = 15;
            isRoundStart = false;
            // 코르틴
            freeModeEnd = null;
            // 대기열 초기화
            ingamePopUpQueue = new IngamePopUpQueueManager();
            // 이벤트 등록
            U.Events.OnPlayerConnected += (player) =>
            {
                UISystem.PlayerJoinStart(player);
                playerDatabase.FindData(player.CSteamID);
                playerDatabase.UpdateData();
                player.Events.OnUpdateHealth += Events_OnUpdateHealth;
                player.Player.equipment.onEquipRequested += OnEquipRequested;
                player.Player.equipment.onDequipRequested += OnDequipRequested;
                player.Player.inventory.onDropItemRequested += OnDropItemRequested;
                player.Events.OnDeath += Events_OnDeath;
                player.Player.stance.onStanceUpdated += () => OnStanceUpdated(player);
                player.Player.life.onStaminaUpdated += (stamina) => OnStaminaUpdated(player);
                VehicleGroupSystem.RefreshVehicleTypeStateAllToEveryone();
            };
            BarricadeManager.onOpenStorageRequested += OnOpenStorageRequested;
            PlayerEquipment.OnUseableChanged_Global += PlayerEquipment_OnUseableChanged_Global;
            InteractableVehicle.OnHealthChanged_Global += InteractableVehicle_OnHealthChanged_Global;
            InteractableVehicle.OnPassengerAdded_Global += InteractableVehicle_OnPassengerAdded_Global;
            InteractableVehicle.OnPassengerRemoved_Global += InteractableVehicle_OnPassengerRemoved_Global;
            UseableGun.OnReloading_Global += UseableGun_OnReloading_Global;
            UseableGun.onBulletSpawned += UseableGun_onBulletSpawned;
            UseableGun.onProjectileSpawned += UseableGun_onProjectileSpawned;
            UseableGun.onChangeSightRequested += UseableGun_onChangeSightRequested;
            UseableGun.onChangeTacticalRequested += UseableGun_onChangeTacticalRequested;
            UseableGun.onChangeGripRequested += UseableGun_onChangeGripRequested;
            UseableGun.onChangeBarrelRequested += UseableGun_onChangeBarrelRequested;
            UseableConsumeable.onPerformingAid += UseableConsumeable_onPerformingAid;
            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
            EffectManager.onEffectTextCommitted += OnEffectTextCommitted;
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
            VehicleManager.OnVehicleExploded += OnVehicleExploded;
            VehicleManager.onDamageVehicleRequested += OnDamagedVehicleRequested;
            VehicleManager.OnToggleVehicleLockRequested += VehicleManager_OnToggleVehicleLockRequested;
            VehicleManager.onSwapSeatRequested += VehicleManager_onSwapSeatRequested;
            VehicleManager.onEnterVehicleRequested += VehicleManager_onEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested += VehicleManager_onExitVehicleRequested;
            Level.onLevelLoaded += OnLevelLoaded;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            DamageTool.damagePlayerRequested += OnPlayerDamaged;
            // 플러그인 로드 성공 알림
            Rocket.Core.Logging.Logger.Log(Configuration.Instance.LoadMessage);
            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
        }
        private void OnOpenStorageRequested(CSteamID _instigator, InteractableStorage _storage, ref bool _shouldAllow)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(_storage.transform);
            if (drop != null)
            {
                if (Configuration.Instance.supplyBarriacde.Contains(drop.asset.id))
                {
                    _shouldAllow = false;
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(_instigator);
                    PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                    if (pc.isKnockDown)
                    {
                        return;
                    }
                    if (pc.supplyCooltime > DateTime.UtcNow)
                    {
                        UnturnedChat.Say(uPlayer, $"남은 보급 쿨타임 {(pc.supplyCooltime - DateTime.UtcNow).Seconds}초");
                        return;
                    }
                    DeploySystem.GiveLoadout(uPlayer, true, true);
                    pc.supplyCooltime = DateTime.UtcNow.AddSeconds(Configuration.Instance.supplyCooltime_Inf);
                    BarricadeManager.tryGetInfo(drop.model, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region);
                    BarricadeManager.destroyBarricade(drop, x, y, plant);
                }
            }
        }
        private void UseableGun_onChangeBarrelRequested(PlayerEquipment equipment, UseableGun gun, Item oldItem, ItemJar newItem, ref bool shouldAllow)
        {
            shouldAllow = false;
        }

        private void UseableGun_onChangeGripRequested(PlayerEquipment equipment, UseableGun gun, Item oldItem, ItemJar newItem, ref bool shouldAllow)
        {
            shouldAllow = false;
        }

        private void UseableGun_onChangeTacticalRequested(PlayerEquipment equipment, UseableGun gun, Item oldItem, ItemJar newItem, ref bool shouldAllow)
        {
            shouldAllow = false;
        }

        private void UseableGun_onChangeSightRequested(PlayerEquipment equipment, UseableGun gun, Item oldItem, ItemJar newItem, ref bool shouldAllow)
        {
            shouldAllow = false;
        }

        private void UseableConsumeable_onPerformingAid(Player instigator, Player target, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            //123
            PlayerComponent targetPc = target.GetComponent<PlayerComponent>();
            if (targetPc.isKnockDown)
            {
                PlayerComponent healerPc = instigator.GetComponent<PlayerComponent>();
                if (!Configuration.Instance.reviveItemPresets.Contains(asset.id) || !healerPc.isMedic)
                {
                    shouldAllow = false;
                    return;
                }
                targetPc.isKnockDown = false;
                target.life.serverSetBleeding(false);
                ITransportConnection tc = target.channel.GetOwnerTransportConnection();
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_Down", false);
                IngameSystem.GiveScoreAndCredit(UnturnedPlayer.FromPlayer(instigator), EScoreGainType.FriendlyRevive, 5, 5, "");
                UnturnedPlayer uPlayer_Target = UnturnedPlayer.FromPlayer(target);
                if (roundInfo.killRecordList.ContainsKey(uPlayer_Target.CSteamID)) roundInfo.killRecordList.Remove(uPlayer_Target.CSteamID);
            }
        }

        private void OnStaminaUpdated(UnturnedPlayer _uPlayer)
        {
            if(_uPlayer.Player.life.stamina < 30)
            {
                _uPlayer.Player.life.serverModifyStamina(100);
            }
        }
        private void VehicleManager_OnToggleVehicleLockRequested(InteractableVehicle vehicle, ref bool shouldAllow)
        {
            shouldAllow = false;
            Enumerable.Range(3, 10).ToArray();
        }
        private void PlayerEquipment_OnUseableChanged_Global(PlayerEquipment _equipment) // 플레이어의 장비가 변경되었을 시 실행
        {
            // 총기 스테이트 각 인덱스별 정보
            // 0,1 = 조준경 아이디
            // 2,3 = 텍티컬 아이디
            // 4,5 = 그립 아이디
            // 6,7 = 배럴 아이디
            // 8,9 = 탄창 아이디
            // 10 = 탄약 수
            // 11 = 조정간 0 = 안전 1 = 단발 2 = 연사 3 = 점사
            // 12 = ???
            // 13~17 부착물 내구도
            ITransportConnection tc = _equipment.player.channel.GetOwnerTransportConnection();
            PlayerInventory inventory = _equipment.player.inventory;
            if (_equipment.itemID != 0)
            {
                if (_equipment.asset.type == EItemType.GUN) // 장비한 아이템이 총기라면
                {
                    ItemGunAsset asset = new Item(_equipment.itemID, false).GetAsset<ItemGunAsset>();
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(_equipment.player);
                    if (uPlayer.CurrentVehicle != null && uPlayer.IsInVehicle)
                    {
                        IngameSystem.RefillAmmoFromVehicle(uPlayer.CurrentVehicle, _equipment.player, asset);
                    }
                    IngameSystem.ActivateGunInfoUI(tc, _equipment.asset.id, _equipment.state, asset.magazineCalibers, inventory);
                    return;
                }
            }
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_GunInfo", false);
            _equipment.player.GetComponent<PlayerComponent>().fireMode = 255;
        }
        private void OnVehicleExploded(InteractableVehicle _vehicle)
        {
            VehicleDeployInfo vDeployInfo = teamInfo.GetVehicleDeployInfo(_vehicle, out bool team);
            if (vDeployInfo == default) return; // 유저가 배치한 차량이 아닌 경우 리턴
            vDeployInfo.state = EDeployVehicleState.Destroyed;
            vDeployInfo.removeDateTime = DateTime.UtcNow.AddSeconds(vDeployInfo.vTypePreset.respawnTime);
            vDeployInfo.isStateChanged = true;
            if (vDeployInfo.supplyCoroutine != null)
            {
                StopCoroutine(vDeployInfo.supplyCoroutine);
                vDeployInfo.supplyCoroutine = null;
            }
            if (roundInfo.roundType != ERoundType.Free) roundInfo.ChangeScore(team, vDeployInfo.vTypePreset.destroyCost);
            // 스폰 가능 차량인 경우 정보 처리
            if (vDeployInfo.vPreset.isDeployable)
            {
                teamInfo.RemoveSpawnableVehicle(_vehicle, team);
            }
            IngameSystem.RefreshUIKillLog(new KillLog(vDeployInfo.killerName, vDeployInfo.vPreset.name, icons["VehicleDestroy"], false, false, team, !team));

            if (roundInfo.killRecordList_Vehicle.ContainsKey(_vehicle))
            {
                VehicleKillRecordInfo killRecord = roundInfo.killRecordList_Vehicle[_vehicle];
                SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID == killRecord.killer);
                if (steamPlayer == null)
                {
                    IngameSystem.GiveScoreAndCredit(steamPlayer, EScoreGainType.VehicleDestroy, vDeployInfo.vTypePreset.reward_score, vDeployInfo.vTypePreset.reward_credit, vDeployInfo.vPreset.name);
                }
                roundInfo.killRecordList_Vehicle.Remove(_vehicle);
            }
        }
        private void OnDamagedVehicleRequested(CSteamID _killerID, InteractableVehicle _vehicle, ref ushort _pendingTotalDamage, ref bool _canRepair, ref bool _shouldAllow, EDamageOrigin _damageOrigin)
        {
            UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(_killerID);
            string killerName = "";
            if(killer != null)
            {
                bool killerTeam = killer.Player.GetComponent<PlayerComponent>().team;
                if (killer.Player.quests.groupID == _vehicle.lockedGroup)
                {
                    _shouldAllow = false;
                    return;
                }
                killerName = killer.DisplayName;
            }
            switch (_damageOrigin)
            {
                case EDamageOrigin.Unknown:
                    break;
                case EDamageOrigin.Mega_Zombie_Boulder:
                    break;
                case EDamageOrigin.Vehicle_Bumper:
                    break;
                case EDamageOrigin.Horde_Beacon_Self_Destruct:
                    break;
                case EDamageOrigin.Trap_Wear_And_Tear:
                    break;
                case EDamageOrigin.Carepackage_Timeout:
                    break;
                case EDamageOrigin.Plant_Harvested:
                    break;
                case EDamageOrigin.Charge_Self_Destruct:
                    break;
                case EDamageOrigin.Zombie_Swipe:
                    break;
                case EDamageOrigin.Grenade_Explosion:
                    break;
                case EDamageOrigin.Rocket_Explosion:
                    break;
                case EDamageOrigin.Food_Explosion:
                    break;
                case EDamageOrigin.Vehicle_Explosion:
                    killerName = "Car Explosion";
                    break;
                case EDamageOrigin.Charge_Explosion:
                    break;
                case EDamageOrigin.Trap_Explosion:
                    killerName = "LandMine";
                    break;
                case EDamageOrigin.Bullet_Explosion:
                    break;
                case EDamageOrigin.Radioactive_Zombie_Explosion:
                    break;
                case EDamageOrigin.Flamable_Zombie_Explosion:
                    break;
                case EDamageOrigin.Zombie_Electric_Shock:
                    break;
                case EDamageOrigin.Zombie_Stomp:
                    break;
                case EDamageOrigin.Zombie_Fire_Breath:
                    break;
                case EDamageOrigin.Sentry:
                    break;
                case EDamageOrigin.Useable_Gun:
                    break;
                case EDamageOrigin.Useable_Melee:
                    break;
                case EDamageOrigin.Punch:
                    break;
                case EDamageOrigin.Animal_Attack:
                    break;
                case EDamageOrigin.Kill_Volume:
                    break;
                case EDamageOrigin.Vehicle_Collision_Self_Damage:
                    break;
                case EDamageOrigin.Lightning:
                    break;
                case EDamageOrigin.VehicleDecay:
                    break;
                default:
                    break;
            }
            if (_vehicle.health - _pendingTotalDamage <= 0) // 차량이 파괴될것이라면
            {
                VehicleDeployInfo vDeployInfo = null;
                vDeployInfo = teamInfo.team_0_VehicleDeploys.FirstOrDefault(x => x.vehicleInstanceID == _vehicle.instanceID);
                if (vDeployInfo == default) vDeployInfo = teamInfo.team_1_VehicleDeploys.FirstOrDefault(x => x.vehicleInstanceID == _vehicle.instanceID);

                if (vDeployInfo == default) return; // 유저가 배치한 차량이 아닌 경우 리턴
                vDeployInfo.killerName = killerName;
                if (killer != null)
                {
                    if (roundInfo.killRecordList_Vehicle.ContainsKey(_vehicle)) roundInfo.killRecordList_Vehicle.Remove(_vehicle);
                    roundInfo.killRecordList_Vehicle.Add(_vehicle, new VehicleKillRecordInfo(killer.SteamPlayer().playerID));
                }
            }
        }
        private void InteractableVehicle_OnHealthChanged_Global(InteractableVehicle _vehicle)
        {
            foreach (Passenger passenger in _vehicle.passengers)
            {
                if (passenger.player == null) continue;
                IngameSystem.UpdateVehicleHealthBar(passenger.player.player, _vehicle);
            }
        }
        private void VehicleManager_onEnterVehicleRequested(Player _player, InteractableVehicle _vehicle, ref bool _shouldAllow) // 차량 입장 전 실행
        {
            PlayerComponent pc = _player.GetComponent<PlayerComponent>();
            if (pc.isRedeploying || pc.isKnockDown)
            {
                _shouldAllow = false;
                return;
            }
            VehicleDeployInfo vDeployInfo = teamInfo.GetVehicleDeployInfo(_vehicle, out bool _team);
            if (vDeployInfo == null) return;
            if (vDeployInfo.vTypePreset.classPlayerOnly)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(_player);
                PlayerInfo playerInfo = teamInfo.GetPlayerInfo(uPlayer.CSteamID);
                if (vDeployInfo.vTypePreset.classIndex != playerInfo.classIndex)
                {
                    _shouldAllow = false;
                    UnturnedChat.Say(uPlayer, "전용병과를 가진 인원만 탑승 가능합니다.");
                    return;
                }
            }
        }
        private void InteractableVehicle_OnPassengerAdded_Global(InteractableVehicle _vehicle, int _seat) // 차량 입장 후 실행
        {
            Player player = _vehicle.passengers[_seat].player.player;
            ITransportConnection tc = player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_VHealthBar", true);
            IngameSystem.UpdateVehicleHealthBar(player, _vehicle);
            if (_vehicle.passengers[_seat].turret != null)
            {
                ItemGunAsset gunAsset = new Item(_vehicle.passengers[_seat].turret.itemID, true).GetAsset<ItemGunAsset>();
                if (gunAsset != null)
                {
                    IngameSystem.UpdateUIGun_RestAmmo(tc, gunAsset.magazineCalibers, player.inventory, _vehicle);
                }
            }
            VehicleDeployInfo vDeployInfo = teamInfo.GetVehicleDeployInfo(_vehicle, out bool _team);
            if (vDeployInfo == null) return;
            if (roundInfo.allyArea_Vehicles.Contains(vDeployInfo))
            {
                if (vDeployInfo.isSupplied) return;
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleSupply", true);
                if (vDeployInfo.isSupplying) EffectManager.sendUIEffectVisibility(47, tc, false, "P_VehicleSupply_On", true);
                else EffectManager.sendUIEffectVisibility(47, tc, false, "P_VehicleSupply_Off", true);
                IngameSystem.RefreshVehicleSupplyInfo(vDeployInfo, tc);
            }
        }
        private void VehicleManager_onExitVehicleRequested(Player _player, InteractableVehicle _vehicle, ref bool _shouldAllow, ref Vector3 _pendingLocation, ref float _pendingYaw) // 차량 퇴출 전 실행
        {
            _vehicle.findPlayerSeat(_player, out byte _seat);
            if(_vehicle.passengers[_seat].turret != null) IngameSystem.ReturnAmmoToVehicle(_vehicle, _player, _seat);
        }
        private void InteractableVehicle_OnPassengerRemoved_Global(InteractableVehicle _vehicle, int _seat, Player _player) // 차량 퇴출 후 실행
        {
            if (!_vehicle.anySeatsOccupied) // 차량에 아무도 없다면
            {
                VehicleDeployInfo vDeployInfo = teamInfo.GetVehicleDeployInfo(_vehicle.instanceID, out bool _team);
                if (vDeployInfo != null)
                {
                    vDeployInfo.isStateChanged = true;
                    vDeployInfo.state = EDeployVehicleState.Abandon;
                    vDeployInfo.removeDateTime = DateTime.UtcNow.AddSeconds(vDeployInfo.vTypePreset.abandonTime);
                }
            }
            ITransportConnection tc = _player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_VHealthBar", false);
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleSupply", false);
        }
        private void VehicleManager_onSwapSeatRequested(Player _player, InteractableVehicle _vehicle, ref bool _shouldAllow, byte _fromSeatIndex, ref byte _toSeatIndex)
        {
            _vehicle.findPlayerSeat(_player, out byte _seat);
            if (_vehicle.passengers[_seat].turret != null) IngameSystem.ReturnAmmoToVehicle(_vehicle, _player, _seat);
        }
        private void OnPluginKeyTick(Player _player, uint _simulation, byte _key, bool _state)
        {
            if (_player.equipment.itemID != 0)
            {
                if (_player.equipment.asset.type == EItemType.GUN)
                {
                    PlayerComponent pc = _player.GetComponent<PlayerComponent>();
                    if (_player.equipment.state[11] == pc.fireMode) return;
                    ITransportConnection tc = _player.channel.GetOwnerTransportConnection();
                    switch (_player.equipment.state[11])
                    {
                        case 0:
                            EffectManager.sendUIEffectVisibility(47, tc, false, "I_FireMode_Safety", true);
                            break;
                        case 1:
                            EffectManager.sendUIEffectVisibility(47, tc, false, "I_FireMode_Semi", true);
                            break;
                        case 2:
                            EffectManager.sendUIEffectVisibility(47, tc, false, "I_FireMode_Auto", true);
                            break;
                        case 3:
                            EffectManager.sendUIEffectVisibility(47, tc, false, "I_FireMode_Burst", true);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void OnStanceUpdated(UnturnedPlayer _uPlayer)
        {
        }
        private void OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool _canDamage)
        {
            if (!isRoundStart) return;
            PlayerComponent pc = parameters.player.GetComponent<PlayerComponent>();
            if (pc.isInvincible || !pc.isJoinedTeam)
            {
                _canDamage = false;
                return;
            }
            if ((ulong)parameters.killer == 0) return;
            UnturnedPlayer uPlayer_Killer = UnturnedPlayer.FromCSteamID(parameters.killer);
            UnturnedPlayer uPlayer_Death = UnturnedPlayer.FromPlayer(parameters.player);
            bool killerTeam = false;
            bool deathTeam = parameters.player.GetComponent<PlayerComponent>().team;
            if (uPlayer_Killer != null)
            {
                killerTeam = uPlayer_Killer.Player.GetComponent<PlayerComponent>().team;
                if (killerTeam == deathTeam && uPlayer_Death.CSteamID != parameters.killer)
                {
                    _canDamage = false;
                    return;
                }
            }
            float damage = parameters.damage;
            float times = parameters.times;
            if (parameters.respectArmor)
            {
                times *= DamageTool.getPlayerArmor(parameters.limb, parameters.player);
            }
            if (parameters.applyGlobalArmorMultiplier)
            {
                times *= Provider.modeConfigData.Players.Armor_Multiplier;
            }
            int num = Mathf.FloorToInt(parameters.damage * parameters.times);
            if (parameters.player.life.health - num <= 0 && !pc.isKnockDown)
            {
                // 살해자에게 적 무력화 정보 제공
                if(uPlayer_Killer != null)
                {
                    if(parameters.killer != uPlayer_Death.CSteamID) IngameSystem.GiveScoreAndCredit(uPlayer_Killer, EScoreGainType.EnemyDown, 0, 0, uPlayer_Death.DisplayName);
                }
                // 피해입은 유저 무력화 처리
                ITransportConnection tc = parameters.player.channel.GetOwnerTransportConnection();
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_Down", true);
                pc.isKnockDown = true;
                _canDamage = false;
                uPlayer_Death.Player.life.serverSetBleeding(true);
                parameters.player.equipment.dequip();
                parameters.player.stance.checkStance(EPlayerStance.PRONE, true);
                parameters.player.life.ReceiveHealth(40);
                parameters.player.movement.forceRemoveFromVehicle();
                // 킬 기록 추가
                KillRecordInfo killRecordInfo = IngameSystem.CreateKillRecordInfo(parameters);
                if (roundInfo.killRecordList.ContainsKey(uPlayer_Death.CSteamID)) roundInfo.killRecordList.Remove(uPlayer_Death.CSteamID);
                if(killRecordInfo != null) roundInfo.killRecordList.Add(uPlayer_Death.CSteamID, killRecordInfo);
            }
        }

        private void Events_OnDeath(UnturnedPlayer uPlayer_Death, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            IngameSystem.OnPlayerKilled(uPlayer_Death, cause,limb,murderer);
        }
        private void UseableGun_OnReloading_Global(UseableGun gun) // 장전 시 실행
        {
            PlayerEquipment equipment = gun.player.equipment;
            ITransportConnection tc = equipment.player.channel.GetOwnerTransportConnection();
            byte amount = gun.player.equipment.state[10];
            EffectManager.sendUIEffectText(47, tc, false, "T_GunInfo_Now", $"{amount}");
            IngameSystem.UpdateUIGun_Magazine(tc, (ushort)(equipment.state[8] + 256 * equipment.state[9]));
            IngameSystem.UpdateUIGun_RestAmmo(tc, gun.equippedGunAsset.magazineCalibers, gun.player.inventory);
        }
        private void UseableGun_onProjectileSpawned(UseableGun sender, GameObject projectile)
        {
            byte amount = sender.player.equipment.state[10];
            ITransportConnection tc = sender.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectText(47, tc, false, "T_GunInfo_Now", $"{amount}");
        }
        private void UseableGun_onBulletSpawned(UseableGun gun, BulletInfo bullet) // 총 쏠때 실행
        {
            byte amount = gun.player.equipment.state[10];
            ITransportConnection tc = gun.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectText(47, tc, false, "T_GunInfo_Now", $"{amount}");
        }
        public void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            //if(item.GetAsset().type == EItemType.GUN)
            shouldAllow = false;
            
        }
        private void Events_OnUpdateHealth(UnturnedPlayer _uPlayer, byte _health) // 체력 변동 시 실행
        {
            IngameSystem.UpdateHealthBar(_uPlayer, _health);
        }
        public void OnEquipRequested(PlayerEquipment _equipment, ItemJar _jar, ItemAsset _asset, ref bool _shouldAllow)
        {
            PlayerComponent pc = _equipment.player.GetComponent<PlayerComponent>();
            if (pc.isRedeploying || pc.isKnockDown) _shouldAllow = false;
        }
        public void OnDequipRequested(PlayerEquipment _equipment, ref bool _shouldAllow) // 아이템 장착 해제 시 실행
        {
        }
        public void OnLevelLoaded(int level)
        {
            // 그룹 데이터 확인
            if (teamDatabase.Data_Team.Count == 0) teamDatabase.AddData(new TeamData(GroupManager.generateUniqueGroupID(), GroupManager.generateUniqueGroupID())); // 팀 데이터가 없는경우 새로 생성
            if (teamDatabase.Data_Team[0].team_0_ID == default) teamDatabase.Data_Team[0].team_0_ID = GroupManager.generateUniqueGroupID(); // 0 팀의 아이디가 부재일 시 생성 
            if (teamDatabase.Data_Team[0].team_1_ID == default) teamDatabase.Data_Team[0].team_1_ID = GroupManager.generateUniqueGroupID(); // 1 팀의 아이디가 부재일 시 생성
            teamDatabase.UpdateData();
            // 그룹 생성
            teamInfo.team_0 = GroupManager.getOrAddGroup(teamDatabase.Data_Team[0].team_0_ID, Configuration.Instance.teamPresets[Configuration.Instance.teamPresetIndex_0].teamName, out bool _0created);
            teamInfo.team_1 = GroupManager.getOrAddGroup(teamDatabase.Data_Team[0].team_1_ID, Configuration.Instance.teamPresets[Configuration.Instance.teamPresetIndex_1].teamName, out bool _1created);
            // 팀 정보 배정
            TeamPresetTable teamPreset = Configuration.Instance.teamPresets[Configuration.Instance.teamPresetIndex_0];
            teamInfo.teamPreset_0 = teamPreset;
            // 0팀 병과 정보 배정
            for (int i = 0; i < teamPreset.classList.Length; i++)
            {
                switch (Configuration.Instance.classPresets[teamPreset.classList[i]].classType)
                {
                    case EClassType.infantary:
                        teamInfo.team_0_ClassInf.Add(new ClassInfo(Configuration.Instance.classPresets[teamPreset.classList[i]]));
                        break;
                    case EClassType.driver:
                        teamInfo.team_0_ClassDriver.Add(new ClassInfo(Configuration.Instance.classPresets[teamPreset.classList[i]]));
                        break;
                }
            }
            // 0 팀 차량 정보 배정
            for (int i = 0; i < teamPreset.vehicleTypeList.Length; i++) { teamInfo.team_0_VehicleTypes.Add(new VehicleTypeInfo(Configuration.Instance.vehicleTypePresets[teamPreset.vehicleTypeList[i]])); }
            // 1 팀 병과 정보 배정
            teamPreset = Configuration.Instance.teamPresets[Configuration.Instance.teamPresetIndex_1];
            teamInfo.teamPreset_1 = teamPreset;
            for (int i = 0; i < teamPreset.classList.Length; i++)
            {
                switch (Configuration.Instance.classPresets[teamPreset.classList[i]].classType)
                {
                    case EClassType.infantary:
                        teamInfo.team_1_ClassInf.Add(new ClassInfo(Configuration.Instance.classPresets[teamPreset.classList[i]]));
                        break;
                    case EClassType.driver:
                        teamInfo.team_1_ClassDriver.Add(new ClassInfo(Configuration.Instance.classPresets[teamPreset.classList[i]]));
                        break;
                }
            }
            // 1 팀 차량 정보 배정
            for (int i = 0; i < teamPreset.vehicleTypeList.Length; i++) { teamInfo.team_1_VehicleTypes.Add(new VehicleTypeInfo(Configuration.Instance.vehicleTypePresets[teamPreset.vehicleTypeList[i]])); }

            loopCoroutine = StartCoroutine(TimeLoop());
            teamInfo.OnRoundStart();
            roundInfo.OnRoundStart();
        }
        public void OnPlayerDisconnected(UnturnedPlayer _uPlayer)
        {
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            if (pc.isKnockDown) IngameSystem.OnPlayerKilled(_uPlayer, new EDeathCause(), new ELimb(), CSteamID.NonSteamGS);
            if(pc.isEnterFinished) roundInfo.playerCount--;
            if (isRoundStart)
            {
                if (roundInfo.roundType == ERoundType.Free && pc.isEnterFinished)
                {
                    if (roundInfo.playerCount < Configuration.Instance.freeModeReadyCount && roundInfo.isFreeModeReady)
                    {
                        roundInfo.isFreeModeReady = false;
                        if (freeModeEnd != null) StopCoroutine(freeModeEnd);
                        freeModeEnd = null;
                        RoundSystem.RefreshUIFreeModeInfoToEveryone();
                    }
                }
                if (pc.localUIState == EPlayerUIState.TeamSelect || !pc.isJoinedTeam) return;
                PlayerInfo playerInfo = teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
                if (playerInfo == null) return; // 팀에 배정된 상태가 아니면 리턴
                bool team = playerInfo.team;
                // 병과 해제
                if (playerInfo.classPrestIndex != ushort.MaxValue) // 병과가 선택되있는 경우
                {
                    switch (playerInfo.classType)
                    {
                        case EClassType.infantary:
                            List<ClassInfo> classInfList = team ? teamInfo.team_0_ClassInf : teamInfo.team_1_ClassInf;
                            classInfList[playerInfo.classIndex].playerCount--;
                            break;
                        case EClassType.driver:
                            List<ClassInfo> classDriverList = team ? teamInfo.team_0_ClassDriver : teamInfo.team_1_ClassDriver;
                            classDriverList[playerInfo.classIndex].playerCount--;
                            break;
                    }
                    ClassSystem.RefreshUIClassPlayerCountToEveryone(team, playerInfo.classIndex, playerInfo.classType);
                }
                if (playerInfo.vGroupInstanceID != ushort.MaxValue) // 플레이어가 차량그룹에 있는경우
                {
                    teamInfo.RemovePlayerFromVehicleGroup(_uPlayer.CSteamID, playerInfo, team);
                    VehicleGroupSystem.RefreshUIVehicleGroupAllToEveryone(team);
                }
                if(pc.team) teamInfo.team_0_PlayerCount--;
                else teamInfo.team_1_PlayerCount--;
                // 기록 정보 갱신
                PlayerTeamRecordInfo pTeamRecordInfo = pc.team ? teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_0_RecordInfo : teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_1_RecordInfo;
                pTeamRecordInfo.isActive = false;
                // 팀에서 퇴장
                teamInfo.RemovePlayer(_uPlayer, team);
                UISystem.RefreshUITeamPlayersUIToEveryone(team);
                VehicleGroupSystem.RefreshVehicleTypeStateAllToEveryone();

            }
            else
            {
                roundInfo.votePlayerCount--;
                if (roundInfo.playerVote.ContainsKey(_uPlayer.CSteamID))
                {
                    roundInfo.voteCount[roundInfo.playerVote[_uPlayer.CSteamID]]--;
                    RoundSystem.RefreshUIVoteCountInfoToEveryone(roundInfo.playerVote[_uPlayer.CSteamID]);
                    roundInfo.playerVote.Remove(_uPlayer.CSteamID);
                }
                if (roundInfo.votePlayerCount <= roundInfo.playerVote.Count) { roundInfo.EndVote(); }
                RoundSystem.RefreshUIVotePlayerCountToEveryone();
            }
        }
        public void OnEffectButtonClicked(Player _player, string _buttonName) // 유저가 버튼 UI 선택 시 실행
        {
            PlayerComponent pc = _player.GetComponent<PlayerComponent>();
            if(_buttonName == "B_PreLoadEnd")
            {
                UISystem.PlayerJoinFinish(_player);
                return;
            }
            ITransportConnection tc = _player.channel.GetOwnerTransportConnection();
            switch (pc.localUIState)
            {
                case EPlayerUIState.TeamSelect:
                    ButtonSystem.OnButtonClick_TeamSelect(UnturnedPlayer.FromPlayer(_player), tc, _buttonName);
                    break;
                case EPlayerUIState.Loadout:
                    ButtonSystem.OnBuuttonClick_Loadout(UnturnedPlayer.FromPlayer(_player), tc, _buttonName);
                    break;
                case EPlayerUIState.Death:
                    break;
                case EPlayerUIState.RoundEnd:
                    ButtonSystem.OnBuuttonClick_RoundEnd(UnturnedPlayer.FromPlayer(_player), tc, _buttonName);
                    break;
            }
        }
        public void OnEffectTextCommitted(Player _player, string _buttonName, string _text)
        {
            PlayerComponent pc = _player.GetComponent<PlayerComponent>();
            switch (_buttonName)
            {
                case "IF_Chat":
                    pc.chatText = _text;
                    break;
                case "IF_VGroupLock":
                    pc.passwordText = _text;
                    break;
                case "IF_Password":
                    pc.enterPasswordText = _text;
                    break;
            }
        }
        public void StartCoroutine_Loop()
        {
            if (loopCoroutine != null) StopCoroutine(loopCoroutine);
            loopCoroutine = StartCoroutine(TimeLoop());
        }
        public void StopCoroutine_Loop()
        {
            if (loopCoroutine != null)
            {
                StopCoroutine(loopCoroutine);
                loopCoroutine = null;
            }
        }
        public void StartCoroutine_VoteStart()
        {
            voteTimer = voteIniTime;
            if (voteCoroutine != null) StopCoroutine(voteCoroutine);
            voteCoroutine = StartCoroutine(Cor_VoteStartTimer());
        }
        public void StartCoroutine_RoundStart()
        {
            voteTimer = voteIniTime;
            if (voteCoroutine != null)
            {
                StopCoroutine(voteCoroutine);
                voteCoroutine = null;
            }
            if (roundStartCoroutine != null) StopCoroutine(roundStartCoroutine);
            roundStartCoroutine = StartCoroutine(Cor_RoundStartTimer());
        }
        public IEnumerator Cor_VoteStartTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                voteTimer--;
                if (voteTimer == 0)
                {
                    roundInfo.EndVote();
                    RoundSystem.RefreshUIVoteTimerToEveryone();
                    break;
                }
                RoundSystem.RefreshUIVoteTimerToEveryone();
            }
        }
        public IEnumerator Cor_RoundStartTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                voteTimer--;
                if (voteTimer == 0)
                {
                    roundInfo.OnRoundStart();
                    RoundSystem.RefreshUIRoundStartTimerToEveryone();
                    break;
                }
                RoundSystem.RefreshUIRoundStartTimerToEveryone();
            }
        }
        public IEnumerator TimeLoop()
        {
            PluginConfiguration configuration = Configuration.Instance;
            int timer = 0;
            while (true)
            {
                LightingManager.time = 1800;
                yield return new WaitForSeconds(1f);
                if(isRoundStart)
                {
                    // 라운드 종류별로 처리
                    switch (roundInfo.roundType)
                    {
                        case ERoundType.Free: // 자유모드
                            break;
                        case ERoundType.CaptureTheFlag: // 깃발 점령전
                            // 거점 정보 처리
                            AreaSystem.UpdateObjectiveCapture();
                            // 거점 점수 변동 처리
                            if (10 <= timer)
                            {
                                timer = 0;
                                AreaSystem.UpdateObjectivePoint();
                                if (!isRoundStart) continue;
                            }
                            else timer++;
                            break;
                        case ERoundType.Battle: // 공방전
                            AreaSystem.UpdateObjectiveCapture();
                            break;
                    }
                    // 차량 대기 시간 처리
                    if (teamInfo.team_0_vehicleTimer.Count != 0)
                    {
                        List<byte> keysToRemove = new List<byte>();
                        foreach (var pair in teamInfo.team_0_vehicleTimer)
                        {
                            if (teamInfo.team_0_vehicleTimer[pair.Key] <= DateTime.UtcNow)
                            {
                                keysToRemove.Add(pair.Key);
                            }
                            VehicleGroupSystem.RefreshUIVehicleTypeTimerToEveryone(true, pair.Key, (pair.Value - DateTime.UtcNow).TotalSeconds);
                        }
                        if (keysToRemove.Count != 0)
                        {
                            foreach (var key in keysToRemove)
                            {
                                teamInfo.team_0_vehicleTimer.Remove(key);
                            }
                        }
                    }
                    if (teamInfo.team_1_vehicleTimer.Count != 0)
                    {
                        List<byte> keysToRemove = new List<byte>();
                        foreach (var pair in teamInfo.team_1_vehicleTimer)
                        {
                            if (teamInfo.team_1_vehicleTimer[pair.Key] <= DateTime.UtcNow)
                            {
                                keysToRemove.Add(pair.Key);
                            }
                            VehicleGroupSystem.RefreshUIVehicleTypeTimerToEveryone(false, pair.Key, (pair.Value - DateTime.UtcNow).TotalSeconds);
                        }
                        if (keysToRemove.Count != 0)
                        {
                            foreach (var key in keysToRemove)
                            {
                                teamInfo.team_1_vehicleTimer.Remove(key);
                            }
                        }
                    }
                    // 배치 차량 처리
                    if (teamInfo.team_0_VehicleDeploys.Count != 0)
                    {
                        List<VehicleDeployInfo> vehiclesToRemove = new List<VehicleDeployInfo>();
                        for (byte i = 0; i < teamInfo.team_0_VehicleDeploys.Count; i++)
                        {
                            VehicleDeployInfo vDeployInfo = teamInfo.team_0_VehicleDeploys[i];
                            if (vDeployInfo.isStateChanged) // 상태가 바뀌었는지 확인
                            {
                                VehicleGroupSystem.RefreshUIVehicleDeployStateToEveryone(i, true, vDeployInfo);
                                vDeployInfo.isStateChanged = false;
                            }
                            if (vDeployInfo.state != EDeployVehicleState.Normal)
                            {
                                if (vDeployInfo.removeDateTime <= DateTime.UtcNow)
                                {
                                    vehiclesToRemove.Add(vDeployInfo);
                                    if (vDeployInfo.state == EDeployVehicleState.Abandon)
                                    {
                                        InteractableVehicle vehicle = VehicleManager.findVehicleByNetInstanceID(vDeployInfo.vehicleInstanceID);
                                        if (vDeployInfo.vPreset.isDeployable) teamInfo.RemoveSpawnableVehicle(vehicle, true);
                                        VehicleManager.askVehicleDestroy(vehicle);
                                    }
                                }
                                else { VehicleGroupSystem.RefreshUIVehicleDeployTimerToEveryone(true, i, vDeployInfo, (vDeployInfo.removeDateTime - DateTime.UtcNow).TotalSeconds); }
                            }
                        }
                        if (vehiclesToRemove.Count != 0)
                        {
                            teamInfo.RemoveVehicleDeploys(vehiclesToRemove.ToArray(), true);
                        }
                    }
                    if (teamInfo.team_1_VehicleDeploys.Count != 0)
                    {
                        List<VehicleDeployInfo> vehiclesToRemove = new List<VehicleDeployInfo>();
                        for (byte i = 0; i < teamInfo.team_1_VehicleDeploys.Count; i++)
                        {
                            VehicleDeployInfo vDeployInfo = teamInfo.team_1_VehicleDeploys[i];
                            if (vDeployInfo.isStateChanged) // 상태가 바뀌었는지 확인
                            {
                                VehicleGroupSystem.RefreshUIVehicleDeployStateToEveryone(i, false, vDeployInfo);
                                vDeployInfo.isStateChanged = false;
                            }
                            if (vDeployInfo.state != EDeployVehicleState.Normal)
                            {
                                if (vDeployInfo.removeDateTime <= DateTime.UtcNow)
                                {
                                    vehiclesToRemove.Add(vDeployInfo);
                                    if (vDeployInfo.state == EDeployVehicleState.Abandon)
                                    {
                                        InteractableVehicle vehicle = VehicleManager.findVehicleByNetInstanceID(vDeployInfo.vehicleInstanceID);
                                        if (vDeployInfo.vPreset.isDeployable) teamInfo.RemoveSpawnableVehicle(vehicle, false);
                                        VehicleManager.askVehicleDestroy(vehicle);
                                    }
                                }
                                else { VehicleGroupSystem.RefreshUIVehicleDeployTimerToEveryone(false, i, vDeployInfo, (vDeployInfo.removeDateTime - DateTime.UtcNow).TotalSeconds); }
                            }
                        }
                        if (vehiclesToRemove.Count != 0)
                        {
                            teamInfo.RemoveVehicleDeploys(vehiclesToRemove.ToArray(), false);
                        }
                    }
                    // 스폰가능 차량 처리
                    for (byte i = 0; i < teamInfo.team_0_spawnableVehicle.Count; i++)
                    {
                        SpawnableVehicleInfo spawnableVehicle = teamInfo.team_0_spawnableVehicle[i];
                        Vector2Int newPos = new Vector2Int(Mathf.RoundToInt(spawnableVehicle.vehicle.transform.position.x), Mathf.RoundToInt(spawnableVehicle.vehicle.transform.position.z));
                        if (spawnableVehicle.lastPos == newPos) continue;
                        spawnableVehicle.lastPos = newPos;
                        spawnableVehicle.textPos = DeploySystem.SpawnMarkerPositon(spawnableVehicle.vehicle.transform.position);
                        DeploySystem.UpdateMarkerPosition_VehicleToEveryone(true, i);
                    }
                    for (byte i = 0; i < teamInfo.team_1_spawnableVehicle.Count; i++)
                    {
                        SpawnableVehicleInfo spawnableVehicle = teamInfo.team_1_spawnableVehicle[i];
                        Vector2Int newPos = new Vector2Int(Mathf.RoundToInt(spawnableVehicle.vehicle.transform.position.x), Mathf.RoundToInt(spawnableVehicle.vehicle.transform.position.z));
                        if (spawnableVehicle.lastPos == newPos) continue;
                        spawnableVehicle.lastPos = newPos;
                        spawnableVehicle.textPos = DeploySystem.SpawnMarkerPositon(spawnableVehicle.vehicle.transform.position);
                        DeploySystem.UpdateMarkerPosition_VehicleToEveryone(false, i);
                    }
                    // 입장 제한구역 처리
                    AreaSystem.UpdateAreaRestrict();
                }
            }
        }
    }
    /*
    해야 할거
    1. 차량 파괴되면 트렁크내 아이템 제거
    2. 특정 아이템은 죽을때 떨구게 하기
    4. 탄약 보급 구역이랑 제한구역 분리하기
    6. 특정 병과에게 레벨 제한 조건 걸기
    7. 상대 팀이 더 많으면 팀 제한 무시하고 들가게 만들기
    8. 투표 중 사람이 부족하면 맵에 경고 붙이기
    9. 차량 무적화 및 무적 유저 공격 시 무적 임을 알리는 UI 띄우기
    10. 서버 밸런스 고려하기 / 거점 점령, 적 사살 등 모든 상황에 점수 얼마나 줄지 / 탄약 얼마나 줄지 등등
    11. sks 격발음, 로켓 장비음 추가
    12. 유저 다운 시 의무병에겐 부상병 위치를, 부상병에겐 의무병 위치 알려주기
    13. 아군 보급 시 보상 주게 하기 / 각 이벤트 별로 주어지는 보상들 콘피그에서 관리하게 만들기
    나중에 해도 되는거
    1. 차량 그룹 정보 등 모든 정보를 다이렉토리로 변경하기
    2. 버튼를 누르면 작업이 완료되기 전까지 다른 버튼 눌러도 기능 실행되지 않게 하기 (대기열쪽만 먼저 했음 나머지는 나중에 해도 됨 아마도)
    3. 다운 됫을대 제대로 눕게 만들기
    4. 리스폰 할때 서있게 하기
    버그
    1. 차량 배치 시 탄약 제공 안되는 버그 아직도 있음
    2. 등록안된 차량이 거점에 들어가면 루프 멈추는 버그 있음
    기타정보
    1. 하나의 버그에 의해 다른 버그가 발생하는 경우가 있을수도 있음
    2. 라운드를 완전히 다시 시작시키는 기능을 추가하는것도 좋을거 같음
    */
}
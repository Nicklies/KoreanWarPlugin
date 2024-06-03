using KoreanWarPlugin.Configuration.Preset;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoreanWarPlugin.Queue;
using SDG.NetTransport;
using KoreanWarPlugin.KWSystem;
using UnityEngine;

namespace KoreanWarPlugin.Info
{
    public class TeamInfo
    {
        public TeamPresetTable teamPreset_0 { get; set; }
        public TeamPresetTable teamPreset_1 { get; set; }
        public GroupInfo team_0 { get; set; }
        public GroupInfo team_1 { get; set; }
        // 프리셋 정보
        public List<ClassInfo> team_0_ClassInf { get; set; } // 팀 0 보병 병과 정보
        public List<ClassInfo> team_1_ClassInf { get; set; } // 팀 1 보병 병과 정보
        public List<ClassInfo> team_0_ClassDriver { get; set; } // 팀 0 운전병 병과 정보
        public List<ClassInfo> team_1_ClassDriver { get; set; } // 팀 1 운전병 병과 정보
        public List<VehicleTypeInfo> team_0_VehicleTypes { get; set; } // 팀 0 차량종류 정보
        public List<VehicleTypeInfo> team_1_VehicleTypes { get; set; } // 팀 1 차량종류 정보
        // 인게임 정보
        public byte team_0_PlayerCount { get; set; } // 팀 0 플레이어 인원 수
        public byte team_1_PlayerCount { get; set; } // 팀 1 플레이어 인원 수
        public Dictionary<CSteamID, PlayerInfo> playerInfoList { get; set; } // 팀 0 플레이어 정보 / 플레이어가 팀을 나가면 사라질 정보
        public Dictionary<CSteamID, PlayerRecordInfo> playerRecordInfoList { get; set; } // 팀 0 플레이어 정보 / 플레이어가 팀을 나가면 사라질 정보
        public ScoreBoardInfo scoreBoardInfo { get; set; } // 게임이 끝난 후 유저 결과 정보를 해당 클래스에 저장 후 사용
        public List<VehicleGroupInfo> team_0_VehicleGroups { get; set; } // 팀 0 차량 그룹 정보
        public List<VehicleGroupInfo> team_1_VehicleGroups { get; set; } // 팀 1 차량 그룹 정보
        public List<VehicleDeployInfo> team_0_VehicleDeploys { get; set; } // 팀 0 투입 차량 정보
        public List<VehicleDeployInfo> team_1_VehicleDeploys { get; set; } // 팀 1 투입 차량 정보
        public List<SpawnableVehicleInfo> team_0_spawnableVehicle { get; set; } // 팀 0 스폰가능 차량
        public List<SpawnableVehicleInfo> team_1_spawnableVehicle { get; set; } // 팀 1 스폰가능 차량
        // 대기열
        public TeamJoinRequestManager teamJoinRequest { get; set; } // 팀 배정 신청대기열
        public ClassRequestManager team_0_ClassRequest { get; set; } // 팀 0 병과 신청대기열
        public ClassRequestManager team_1_ClassRequest { get; set; } // 팀 1 병과 신청대기열
        public VehicleRequestManager team_0_NewVehicleRequest { get; set; } // 팀 0 신규차량 신청대기열
        public VehicleRequestManager team_1_NewVehicleRequest { get; set; } // 팀 1 신규차량 신청대기열
        // 인스턴스 아이디 정보
        public ushort team_0_vGroupInstanceID { get; set; } // 팀 0 차량그룹 인스턴스 아이디
        public ushort team_1_vGroupInstanceID { get; set; } // 팀 1 차량그룹 인스턴스 아이디
        public ushort team_0_spawnableVInstanceID { get; set; } // 팀 0 스폰가능차량 인스턴스 아이디
        public ushort team_1_spawnableVInstanceID { get; set; } // 팀 1 스폰가능차량 인스턴스 아이디
        // 대기 시간 정보
        public Dictionary<byte, DateTime> team_0_vehicleTimer { get; set; } // 팀 0 차종 인덱스별 잠금해제까지 남은시간
        public Dictionary<byte, DateTime> team_1_vehicleTimer { get; set; } // 팀 1 차종 인덱스별 잠금해제까지 남은시간

        public void Initialize()
        {
            teamPreset_0 = new TeamPresetTable();
            teamPreset_1 = new TeamPresetTable();
            team_0_ClassInf = new List<ClassInfo>();
            team_1_ClassInf = new List<ClassInfo>();
            team_0_ClassDriver = new List<ClassInfo>();
            team_1_ClassDriver = new List<ClassInfo>();
            team_0_VehicleTypes = new List<VehicleTypeInfo>();
            team_1_VehicleTypes = new List<VehicleTypeInfo>();
            playerInfoList = new Dictionary<CSteamID, PlayerInfo>();
            playerRecordInfoList = new Dictionary<CSteamID, PlayerRecordInfo>();
            scoreBoardInfo = new ScoreBoardInfo();
            team_0_PlayerCount = 0;
            team_1_PlayerCount = 0;
            team_0_VehicleGroups = new List<VehicleGroupInfo>();
            team_1_VehicleGroups = new List<VehicleGroupInfo>();
            team_0_VehicleDeploys = new List<VehicleDeployInfo>();
            team_1_VehicleDeploys = new List<VehicleDeployInfo>();
            team_0_spawnableVehicle = new List<SpawnableVehicleInfo>();
            team_1_spawnableVehicle = new List<SpawnableVehicleInfo>();
            teamJoinRequest = new TeamJoinRequestManager();
            team_0_ClassRequest = new ClassRequestManager();
            team_1_ClassRequest = new ClassRequestManager();
            team_0_NewVehicleRequest = new VehicleRequestManager();
            team_1_NewVehicleRequest = new VehicleRequestManager();
            team_0_vehicleTimer = new Dictionary<byte, DateTime>();
            team_1_vehicleTimer = new Dictionary<byte, DateTime>();
            team_0_spawnableVInstanceID = 0;
            team_1_spawnableVInstanceID = 0;
            team_0_vGroupInstanceID = 0;
            team_1_vGroupInstanceID = 0;
        }
        public TeamInfo()
        {
            Initialize();
        }
        public void RemovePlayer(UnturnedPlayer _uPlayer, bool _team)
        {
            playerInfoList.Remove(_uPlayer.CSteamID);
        }
        public PlayerInfo GetPlayerInfo(CSteamID _cSteamID)
        {
            if (!playerInfoList.ContainsKey(_cSteamID)) return null;
            else return playerInfoList[_cSteamID];
        }
        public VehicleGroupInfo GetVehicleGroupInfo(ushort _instanceID, bool _team)
        {
            if (_team) return team_0_VehicleGroups.Find(x => x.instanceID == _instanceID);
            else return team_1_VehicleGroups.Find(x => x.instanceID == _instanceID);
        }
        public VehicleDeployInfo GetVehicleDeployInfo(InteractableVehicle _vehicle, out bool team)
        {
            VehicleDeployInfo vehicleDeployInfo = GetVehicleDeployInfo(_vehicle.instanceID, out bool _team);
            team = _team;
            return vehicleDeployInfo;
        }
        public VehicleDeployInfo GetVehicleDeployInfo(uint _vInstanceID, out bool _team)
        {
            VehicleDeployInfo vDeployInfo = null;
            _team = false;
            vDeployInfo = team_0_VehicleDeploys.FirstOrDefault(x => x.vehicleInstanceID == _vInstanceID);
            if (vDeployInfo != default)
            {
                _team = true;
                return vDeployInfo;
            }
            vDeployInfo = team_1_VehicleDeploys.FirstOrDefault(x => x.vehicleInstanceID == _vInstanceID);
            if (vDeployInfo != default)
            {
                return vDeployInfo;
            }
            return null;
        }
        public VehicleGroupInfo CreateNewVehicleGroup(CSteamID _cSteamID, VehicleTypePresetTable _preset, byte _vTypeIndex, bool _team)
        {
            VehicleGroupInfo newGroup = new VehicleGroupInfo(_preset, 0, _vTypeIndex, _cSteamID);
            newGroup.instanceID = _team ? PluginManager.teamInfo.team_0_vGroupInstanceID++ : PluginManager.teamInfo.team_1_vGroupInstanceID++;
            if (_team)
            {
                team_0_VehicleGroups.Add(newGroup);
                return newGroup;
            }
            else
            {
                team_1_VehicleGroups.Add(newGroup);
                return newGroup;
            }
        }
        public bool FindVehicleGroup(ushort _instanceID, bool _team, out VehicleGroupInfo _vGroupInfo, out byte _vIndex)
        {
            List<VehicleGroupInfo> vGroupList = _team ? team_0_VehicleGroups : team_1_VehicleGroups;
            VehicleGroupInfo vGroupInfo = vGroupList.FirstOrDefault(x => x.instanceID == _instanceID);
            if (vGroupInfo != default)
            {
                _vGroupInfo = vGroupInfo;
                _vIndex = (byte)vGroupList.IndexOf(vGroupInfo);
                return true;
            }
            else
            {
                _vGroupInfo = null;
                _vIndex = (byte)vGroupList.IndexOf(vGroupInfo);
                return false;
            }
        }
        public bool RemovePlayerFromVehicleGroup(CSteamID _cSteamID, PlayerInfo _playerInfo, bool _team) // 플레이어를 차량그룹에서 제거
        {
            VehicleGroupInfo vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(_playerInfo.vGroupInstanceID, _team);
            SeatInfo seatInfo = vGroupInfo.seats.FirstOrDefault(x => x.cSteamID == _cSteamID);
            ITransportConnection tc = UnturnedPlayer.FromCSteamID(_cSteamID).Player.channel.GetOwnerTransportConnection();
            vGroupInfo.crewCount--;
            if (seatInfo != default)
            {
                if (vGroupInfo.crewCount <= 0) // 승무원이 없는경우 그룹 제거
                {
                    if (_team)
                    {
                        team_0_VehicleGroups.Remove(vGroupInfo);
                        team_0_VehicleTypes[vGroupInfo.vTypeIndex].vehicleCount--;
                    }
                    else
                    {
                        team_1_VehicleGroups.Remove(vGroupInfo);
                        team_1_VehicleTypes[vGroupInfo.vTypeIndex].vehicleCount--;
                    }
                    VehicleGroupSystem.RefreshVehicleTypeCountToEveryone(_team, _playerInfo.vTypeIndex); // 차종 개수 정보 갱신
                    VehicleGroupSystem.RefreshUIVehicleGroupAllToEveryone(_team); // 차량그룹 UI 갱신
                }
                else // 승무원이 아직 남은 경우
                {
                    CSteamID oldID = seatInfo.cSteamID;
                    seatInfo.cSteamID = CSteamID.NonSteamGS;
                    if (oldID == vGroupInfo.leaderID) // 나간 인원이 리더라면 새로운 리더를 지정
                    {
                        vGroupInfo.SetNewLeaderByIndex();
                        PlayerInfo newLeaderInfo = PluginManager.teamInfo.GetPlayerInfo(vGroupInfo.leaderID);
                        ITransportConnection newLeaderTc = UnturnedPlayer.FromCSteamID(vGroupInfo.leaderID).Player.channel.GetOwnerTransportConnection();
                        // 지휘관 교체 후 비용 정산
                        LoadoutSystem.RefreshDeployCost(newLeaderInfo, newLeaderTc);
                    }
                    VehicleGroupSystem.RefreshUIVehicleGroupInfoToMembers(vGroupInfo);
                }
                LoadoutSystem.SetLoadoutType_Inventory(tc);
                EffectManager.sendUIEffectVisibility(47, tc, false, "P_LoadoutType_1Block_1", true);
                _playerInfo.vGroupInstanceID = ushort.MaxValue;
                _playerInfo.vTypeIndex = byte.MaxValue;
                LoadoutSystem.RefreshDeployCost(_playerInfo, tc);
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(_cSteamID);
                uPlayer.Player.GetComponent<PlayerComponent>().passwordText = "";
                PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                if (pc.chatType == EnumTable.EChatType.Vehicle)
                {
                    pc.chatType = EnumTable.EChatType.Team;
                    UISystem.ChangeChatType(pc, tc);
                }
                // 거점 정보 갱신
                DeploySystem.ActiveDeployMarker_Static(tc, _team, _playerInfo);
                DeploySystem.ActiveDeployMarker_VehicleAll(tc, _team, _playerInfo);
                return true;
            }
            return false;
        }
        public void RemoveVehicleDeploys(VehicleDeployInfo[] _vehicleDeployInfos,bool _team)
        {
            foreach (VehicleDeployInfo vDeployInfo in _vehicleDeployInfos)
            {
                RemoveVehicleDeploy(vDeployInfo, _team, false);
            }
            VehicleGroupSystem.RefreshUIVehicleDeployAllToEveryone(_team);
        }
        public void RemoveVehicleDeploy(VehicleDeployInfo _vDeployInfo, bool _team, bool _resfreshUI)
        {
            if(_vDeployInfo.supplyCoroutine != null)
            {
                PluginManager.instance.StopCoroutine(_vDeployInfo.supplyCoroutine);
                _vDeployInfo.supplyCoroutine = null;
            }
            if (_team)
            {
                PluginManager.teamInfo.team_0_VehicleDeploys.Remove(_vDeployInfo);
                team_0_VehicleTypes[_vDeployInfo.vTypeIndex].vehicleCount--;
            }
            else
            {
                PluginManager.teamInfo.team_1_VehicleDeploys.Remove(_vDeployInfo);
                team_1_VehicleTypes[_vDeployInfo.vTypeIndex].vehicleCount--;
            }
            VehicleGroupSystem.RefreshVehicleTypeCountToEveryone(_team, _vDeployInfo.vTypeIndex);
            if (_resfreshUI) VehicleGroupSystem.RefreshUIVehicleDeployAllToEveryone(_team);
        }
        public void RemoveSpawnableVehicle(InteractableVehicle _vehicle,bool _team)
        {
            SpawnableVehicleInfo spawnableVehicle = null;
            if (_team) spawnableVehicle = team_0_spawnableVehicle.FirstOrDefault(x => x.vehicle == _vehicle);
            else spawnableVehicle = team_1_spawnableVehicle.FirstOrDefault(x => x.vehicle == _vehicle);
            RemoveSpawnableVehicle(spawnableVehicle, _team);
        }
        public void RemoveSpawnableVehicle(SpawnableVehicleInfo _spawnableVehicle, bool _team)
        {
            if(_spawnableVehicle != default)
            {
                if (_team)team_0_spawnableVehicle.Remove(_spawnableVehicle);
                else team_1_spawnableVehicle.Remove(_spawnableVehicle);
                // 스폰가능 차량 갱신
                DeploySystem.ActiveDeployMarker_VehicleAllToEveryone(_team);
            }
        }
        public void OnVehicleDeploy(VehicleGroupInfo _vGroupInfo, bool _team) // 차량이 배치 되었을 시
        {
            foreach (SeatInfo seatInfo in _vGroupInfo.seats)
            {
                if (seatInfo.cSteamID == CSteamID.NonSteamGS) continue;
                PlayerInfo playerInfo = GetPlayerInfo(seatInfo.cSteamID);
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(seatInfo.cSteamID);
                ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleInfo", false);
                EffectManager.sendUIEffectVisibility(47, tc, false, "P_LoadoutType_1Block_1", true);
                playerInfo.vGroupInstanceID = ushort.MaxValue;
                playerInfo.vTypeIndex = byte.MaxValue;
                LoadoutSystem.SetLoadoutType_Inventory(tc);
            }
            if (_team) team_0_VehicleGroups.Remove(_vGroupInfo);
            else team_1_VehicleGroups.Remove(_vGroupInfo);
        }
        public void AddDeployVehicleInfo(InteractableVehicle _vehicle,byte _vTypeIndex, VehiclePresetTable _vPreset, VehicleTypePresetTable _vTypePreset, InteractableVehicle _interactableVehicle, bool _team)
        {
            VehicleDeployInfo vehicleDeployInfo = new VehicleDeployInfo(_vehicle, _vTypeIndex, _vPreset, _vTypePreset, _interactableVehicle);
            if (_team) team_0_VehicleDeploys.Add(vehicleDeployInfo);
            else team_1_VehicleDeploys.Add(vehicleDeployInfo);
        }
        public void OnRoundEnd()
        {

        }
        public void OnRoundStart()
        {
            // 정보 초기화
            playerInfoList.Clear();
            playerRecordInfoList.Clear();
            team_0_PlayerCount = 0;
            team_1_PlayerCount = 0;
            team_0_vGroupInstanceID = 0;
            team_1_vGroupInstanceID = 0;
            team_0_spawnableVInstanceID = 0;
            team_1_spawnableVInstanceID = 0;
            team_0_vehicleTimer = new Dictionary<byte, DateTime>();
            team_1_vehicleTimer = new Dictionary<byte, DateTime>();
            team_0_VehicleDeploys.Clear();
            team_1_VehicleDeploys.Clear();
            team_0_VehicleGroups.Clear();
            team_1_VehicleGroups.Clear();
            team_0_spawnableVehicle.Clear();
            team_1_spawnableVehicle.Clear();
            foreach (ClassInfo classInfo in team_0_ClassInf) { classInfo.playerCount = 0; }
            foreach (ClassInfo classInfo in team_1_ClassInf) { classInfo.playerCount = 0; }
            foreach (ClassInfo classInfo in team_0_ClassDriver) { classInfo.playerCount = 0; }
            foreach (ClassInfo classInfo in team_1_ClassDriver) { classInfo.playerCount = 0; }
            foreach (VehicleTypeInfo vTypeInfo in team_0_VehicleTypes) { vTypeInfo.vehicleCount = 0; }
            foreach (VehicleTypeInfo vTypeInfo in team_1_VehicleTypes) { vTypeInfo.vehicleCount = 0; }
            // 대기 시간 초기화
            for (byte i = 0; i < team_0_VehicleTypes.Count; i++)
            {
                if (team_0_VehicleTypes[i].presetInfo.timer != 0)
                {
                    team_0_vehicleTimer.Add(i, DateTime.UtcNow.AddSeconds(team_0_VehicleTypes[i].presetInfo.timer));
                }
            }
            for (byte i = 0; i < team_1_VehicleTypes.Count; i++)
            {
                if (team_1_VehicleTypes[i].presetInfo.timer != 0)
                {
                    team_1_vehicleTimer.Add(i, DateTime.UtcNow.AddSeconds(team_1_VehicleTypes[i].presetInfo.timer));
                }
            }
            PluginManager.instance.isRoundStart = true;
        }
        public void UpdateClassMaxPlayerCount() // 모든 병과의 최대 병과 수 갱신
        {
            int playerCount = Provider.clients.Count;
            for (byte i = 0; i < team_0_ClassInf.Count; i++)
            {
                if (team_0_ClassInf[i].presetInfo.playerMax == 0) continue;
                team_0_ClassInf[i].UpdateVariable(playerCount, i, true);
            }
            for (byte i = 0; i < team_0_ClassDriver.Count; i++)
            {
                if (team_0_ClassDriver[i].presetInfo.playerMax == 0) continue;
                team_0_ClassDriver[i].UpdateVariable(playerCount, i, true);
            }
            for (byte i = 0; i < team_1_ClassInf.Count; i++)
            {
                if (team_1_ClassInf[i].presetInfo.playerMax == 0) continue;
                team_1_ClassInf[i].UpdateVariable(playerCount, i, false);
            }
            for (byte i = 0; i < team_1_ClassDriver.Count; i++)
            {
                if (team_1_ClassDriver[i].presetInfo.playerMax == 0) continue;
                team_1_ClassDriver[i].UpdateVariable(playerCount, i, false);
            }
        }
    }
}
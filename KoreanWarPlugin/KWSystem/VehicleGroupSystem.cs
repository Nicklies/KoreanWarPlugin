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

namespace KoreanWarPlugin.KWSystem
{
    public class VehicleGroupSystem // 배치단계의 차량그룹, 차종 관련 시스템
    {
        public static void TryPassword_VehicleGroup(UnturnedPlayer _uPlayer, PlayerComponent _pc, ITransportConnection _tc, bool _team) // 차량 그룹에 비밀번호 입력후 접속 시도
        {
            if (!PluginManager.teamInfo.FindVehicleGroup(_pc.passwordVGroupSelect, _team, out VehicleGroupInfo _vGroup, out byte _vIndex))
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_Password", "차량방이 더이상 존재하지 않습니다.");
                return;
            }
            if (_vGroup.isLocked)
            {
                if (_pc.enterPasswordText.Length < 4)
                {
                    EffectManager.sendUIEffectText(47, _tc, false, "T_Password", "최소 4자리의 번호를 입력하세요.");
                    return;
                }
                if (_pc.enterPasswordText != _vGroup.password)
                {
                    EffectManager.sendUIEffectText(47, _tc, false, "T_Password", "잘못된 비밀번호 입니다.");
                    return;
                }
            }
            UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
            if (_team) PluginManager.teamInfo.team_0_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, byte.MaxValue, _pc.passwordVGroupSelect, _team, EnumTable.ERequestType.Join, OnVehicleRequestEnd);
            else PluginManager.teamInfo.team_1_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, byte.MaxValue, _pc.passwordVGroupSelect, _team, EnumTable.ERequestType.Join, OnVehicleRequestEnd);
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"L_Password", false);
            _pc.enterPasswordText = "";
        }
        public static void AssignVehicleCost(ITransportConnection _tc,PlayerInfo _playerInfo,VehicleGroupInfo _vGroupInfo)
        {
            if(_playerInfo.cSteamID == _vGroupInfo.leaderID) // 비교하는 대상이 리더라면 새로 임명된 리더로 간주하고 비용 추가
            {
                _playerInfo.creditCost += PluginManager.instance.Configuration.Instance.vehiclePresets[_vGroupInfo.vPresetIndex].creditCost;
            }
            else // 리더가 아닐 시 기존에 리더였다가 바뀐것으로 간주하고 비용차감
            {
                _playerInfo.creditCost -= PluginManager.instance.Configuration.Instance.vehiclePresets[_vGroupInfo.vPresetIndex].creditCost;
            }
            LoadoutSystem.RefreshDeployCost(_playerInfo, _tc);
        }
        public static void OnVehicleRequestEnd(CSteamID _cSteamID, byte _index, ushort _vGroupInstanceID, bool _team, EnumTable.ERequestType _type, bool _success) // 대기열에서 신규차량신청이 처리되었을 시
        {
            if (!PluginManager.instance.isRoundStart) return;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(_cSteamID);
            if (uPlayer == null) return;
            ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
            PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID);
            if (!_success || playerInfo.isDeployed)
            {
                UISystem.RefreshDeployState(PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID), tc);
                pc.isProcessing = false;
                // 실패 이유 보내기
                return;
            }
            List<VehicleGroupInfo> vehicleGroupList = new List<VehicleGroupInfo>();
            VehicleGroupInfo vGroupInfo = null;
            byte vGroupIndex = byte.MaxValue;
            switch (_type)
            {
                case EnumTable.ERequestType.Create: // 차량 생성 로직
                    List<VehicleTypeInfo> vehicleTypeList = _team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;
                    if (playerInfo.vGroupInstanceID != ushort.MaxValue) // 이전에 배정된 차량그룹이 있다면 퇴출시키기
                    {
                        if (PluginManager.teamInfo.RemovePlayerFromVehicleGroup(_cSteamID, playerInfo, _team)) { }
                        else { break; }
                    }
                    else // 이전에 배정된 차량그룹이 없다면
                    {
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"L_VehicleInfo", true);
                    }
                    vGroupInfo = PluginManager.teamInfo.CreateNewVehicleGroup(_cSteamID, vehicleTypeList[_index].presetInfo, _index, _team);
                    playerInfo.vGroupInstanceID = vGroupInfo.instanceID;
                    vehicleTypeList[_index].vehicleCount++;
                    playerInfo.vTypeIndex = _index;
                    RefreshVehicleTypeCountToEveryone(_team, _index); // 신규차량 탭 정보 갱신
                    RefreshUIVehicleGroupAllToEveryone(_team); // 대기차량 탭 정보 갱신
                    RefreshUIVehicleGroupInfoToMembers(vGroupInfo); // 차량그룹 정보 탭 갱신
                    AssignVehicleCost(tc, playerInfo, vGroupInfo);
                    LoadoutSystem.RefreshDeployCost(playerInfo, tc);
                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleType", false);
                    // 거점 정보 갱신
                    DeploySystem.ActiveDeployMarker_Static(tc, _team, playerInfo);
                    DeploySystem.ActiveDeployMarker_VehicleAll(tc, _team, playerInfo);
                    if (_team) PluginManager.teamInfo.team_0_ClassRequest.RequestClass(_cSteamID, vehicleTypeList[_index].presetInfo.classIndex, _team, true, EnumTable.EClassType.driver, ClassSystem.OnClassRequestEnd);
                    else PluginManager.teamInfo.team_1_ClassRequest.RequestClass(_cSteamID, vehicleTypeList[_index].presetInfo.classIndex, _team, true, EnumTable.EClassType.driver, ClassSystem.OnClassRequestEnd);
                    break;
                case EnumTable.ERequestType.Join: // 차량 그룹 입장 로직
                    vehicleGroupList = _team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
                    if (playerInfo.vGroupInstanceID != ushort.MaxValue) // 이전에 배정된 차량그룹이 있다면 퇴출시키기
                    {
                        if (PluginManager.teamInfo.RemovePlayerFromVehicleGroup(_cSteamID, playerInfo, _team)) { }
                        else { break; }
                    }
                    else // 이전에 배정된 차량그룹이 없다면
                    {
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"L_VehicleInfo", true);
                    }
                    vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(_vGroupInstanceID, _team, out byte _vIndex);
                    playerInfo.vGroupInstanceID = vGroupInfo.instanceID;
                    playerInfo.vTypeIndex = vGroupInfo.vTypeIndex;
                    vGroupInfo.crewCount++;
                    foreach (SeatInfo seat in vGroupInfo.seats) // 좌석 배정
                    {
                        if (seat.cSteamID != CSteamID.NonSteamGS) continue; // 좌석에 이미 누가 있는경우 넘김
                        seat.cSteamID = uPlayer.CSteamID;
                        break;
                    }
                    RefreshUIVehicleGroupAllToEveryone(_team); // 대기차량 탭 정보 갱신
                    //RefreshUIVehicleGroupToEveryone(_vIndex, _team); // 대기차량 탭 정보 갱신
                    RefreshUIVehicleGroupInfoToMembers(vGroupInfo); // 차량그룹 정보 탭 갱신
                    // 거점 정보 갱신
                   DeploySystem.ActiveDeployMarker_Static(tc, _team, playerInfo);
                    DeploySystem.ActiveDeployMarker_VehicleAll(tc, _team, playerInfo);
                    if (_team) PluginManager.teamInfo.team_0_ClassRequest.RequestClass(_cSteamID, vGroupInfo.vehicleTypePreset.classIndex, _team, true, EnumTable.EClassType.driver, ClassSystem.OnClassRequestEnd);
                    else PluginManager.teamInfo.team_1_ClassRequest.RequestClass(_cSteamID, vGroupInfo.vehicleTypePreset.classIndex, _team, true, EnumTable.EClassType.driver, ClassSystem.OnClassRequestEnd);
                    break;
                case EnumTable.ERequestType.Switch: // 좌석 변경 로직
                    vehicleGroupList = _team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
                    vGroupInfo = vehicleGroupList.FirstOrDefault(x => x.instanceID == _vGroupInstanceID);
                    vGroupInfo.ChangeSeat(uPlayer.CSteamID, _index);
                    RefreshUIVehicleGroupInfoToMembers_Member(vGroupInfo);
                    break;
                case EnumTable.ERequestType.Exile: // 좌석 추방 로직
                    vehicleGroupList = _team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
                    vGroupInfo = vehicleGroupList.FirstOrDefault(x => x.instanceID == _vGroupInstanceID);
                    vGroupIndex = (byte)vehicleGroupList.IndexOf(vGroupInfo);
                    vGroupInfo.ExileSeat(_index, _team);
                    RefreshUIVehicleGroupInfoToMembers_Member(vGroupInfo);
                    RefreshUIVehicleGroupToEveryone(vGroupIndex, _team);
                    break;
                case EnumTable.ERequestType.ChangeVehicle: // 차량 교체 로직
                    vehicleGroupList = _team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
                    vGroupInfo = vehicleGroupList.FirstOrDefault(x => x.instanceID == _vGroupInstanceID);
                    ushort[] selectVehicleList = vGroupInfo.vehicleTypePreset.vehicleList;
                    vGroupIndex = (byte)vehicleGroupList.IndexOf(vGroupInfo);
                    ushort beforevPresetIndex = vGroupInfo.vPresetIndex; // 교체 전 차량의 인덱스
                    vGroupInfo.vPresetIndex = selectVehicleList[_index]; // 인덱스 변경
                    VehiclePresetTable oldvPresetTable = PluginManager.instance.Configuration.Instance.vehiclePresets[beforevPresetIndex];
                    VehiclePresetTable newVehiclePreset = PluginManager.instance.Configuration.Instance.vehiclePresets[vGroupInfo.vPresetIndex];
                    // 좌석 정보 갱신
                    SeatInfo[] oldSeatInfo = vGroupInfo.seats; // 기존 정보
                    vGroupInfo.seats = new SeatInfo[newVehiclePreset.seats.Length];
                    for (byte i = 0; i < vGroupInfo.seats.Length; i++) vGroupInfo.seats[i] = new SeatInfo();
                    for (byte i = 0; i < oldSeatInfo.Length; i++)
                    {
                        if (i > newVehiclePreset.seats.Length - 1) // 현재 확인하는 좌석 인덱스가 새로 배정된 차량의 총 좌석수를 초과한 경우
                        {
                            if (oldSeatInfo[i].cSteamID == CSteamID.NonSteamGS) continue; // 비어있는 좌석인경우 넘기기
                            else // 누가 있는경우
                            {
                                // 먼저 빈 자리가 있는지 찾아서 넣으려고 시도
                                bool isEmptySeatExist = false; // 비어있는 자리가 있는지 여부
                                for (byte seatIndex = 0; seatIndex < vGroupInfo.seats.Length; seatIndex++)
                                {
                                    if (vGroupInfo.seats[seatIndex].cSteamID == CSteamID.NonSteamGS) // 빈자리가 있는 경우
                                    {
                                        vGroupInfo.seats[seatIndex].cSteamID = oldSeatInfo[i].cSteamID;
                                        isEmptySeatExist = true;
                                        break;
                                    }
                                }
                                if (isEmptySeatExist) continue;
                                // 빈자리가 없는경우 추방 수행
                                CSteamID exilePlayerCSteamID = CSteamID.NonSteamGS;
                                PlayerInfo exilePlayerInfo = null;
                                if (oldSeatInfo[i].cSteamID == vGroupInfo.leaderID) // 해당 자리 인원이 리더인 경우 마지막 자리를 추방하고 그 자리를 리더에게 제공
                                {
                                    exilePlayerCSteamID = vGroupInfo.seats[vGroupInfo.seats.Length - 1].cSteamID;
                                    exilePlayerInfo = PluginManager.teamInfo.GetPlayerInfo(exilePlayerCSteamID);
                                    //정보 제공 후 리더에게 자리 배정
                                    vGroupInfo.seats[vGroupInfo.seats.Length - 1].cSteamID = vGroupInfo.leaderID;
                                }
                                else
                                {
                                    exilePlayerCSteamID = oldSeatInfo[i].cSteamID;
                                    exilePlayerInfo = PluginManager.teamInfo.GetPlayerInfo(exilePlayerCSteamID);
                                }
                                exilePlayerInfo.vGroupInstanceID = ushort.MaxValue;
                                exilePlayerInfo.vTypeIndex = byte.MaxValue;
                                ITransportConnection exilePlayerTc = UnturnedPlayer.FromCSteamID(exilePlayerCSteamID).Player.channel.GetOwnerTransportConnection();
                                EffectManager.sendUIEffectVisibility(47, exilePlayerTc, false, "L_VehicleInfo", false);
                                PlayerComponent exilePc = UnturnedPlayer.FromCSteamID(exilePlayerInfo.cSteamID).Player.GetComponent<PlayerComponent>();
                                if (exilePc.localChatType == EnumTable.EChatType.Vehicle)
                                {
                                    exilePc.localChatType = EnumTable.EChatType.Team;
                                    UISystem.ChangeChatType(exilePc, exilePlayerTc);
                                }
                                UISystem.SendPopUpInfo(exilePlayerTc, "좌석 부족으로 인해 자동 퇴장 되었습니다.");
                                // 거점 정보 갱신
                                DeploySystem.ActiveDeployMarker_Static(exilePlayerTc, _team, exilePlayerInfo);
                                DeploySystem.ActiveDeployMarker_VehicleAll(tc, _team, playerInfo);
                            }
                        }
                        else
                        {
                            vGroupInfo.seats[i] = oldSeatInfo[i];
                        }
                    }
                    // 리더 비용 갱신
                    //playerInfo.creditPoint -= oldvPresetTable.creditCost;
                    //playerInfo.creditPoint += newVehiclePreset.creditCost;
                    LoadoutSystem.RefreshDeployCost(playerInfo, tc);
                    vGroupInfo.maxSeats = (byte)vGroupInfo.seats.Length;
                    LoadoutSystem.RefreshLoadout_Vehicle(tc, selectVehicleList[_index]);
                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_SelectItem", false);
                    RefreshUIVehicleGroupInfoToMembers(vGroupInfo);
                    RefreshUIVehicleGroupToEveryone(vGroupIndex, _team);
                    break;
            }
            pc.isProcessing = false;
            UISystem.RefreshDeployState(PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID), tc);
        }
        public static void RefreshVehicleType(ITransportConnection _tc, bool _team, UnturnedPlayer _uPlayer) // 신규차량 탭 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveVehicleType", true); // 트리거 활성화
            List<VehicleTypeInfo> vehicleList = _team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;
            for (byte i = 0; i < vehicleList.Count; i++)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleType_{i}", $"{vehicleList[i].presetInfo.iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleTypeName_{i}", $"{vehicleList[i].presetInfo.name}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_VehicleType_{i}", true);
            }
            // 조건문 갱신
            RefreshVehicleTypeStateAll(_tc, _team, _uPlayer);
        }
        public static void RefreshVehicleTypeStateAllToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                RefreshVehicleTypeStateAll(steamPlayer.transportConnection, pc.team, uPlayer);
            }
        }
        public static void RefreshVehicleTypeStateAll(ITransportConnection _tc, bool _team, UnturnedPlayer _uPlayer) // 신규차량 탭에서 레벨, 돈 등 조건에 따라 선택 못하는지 갱신
        {
            List<VehicleTypeInfo> vehicleList = _team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;
            for (byte i = 0; i < vehicleList.Count; i++)
            {
                RefreshVehicleTypeState(_tc, _team, i, _uPlayer);
            }
        }
        public static void RefreshVehicleTypeStateToEveryone(bool _team, byte _vTypeIndex) // 모든 유저에게 한개 인덱스 차종의 레벨, 돈 등 조건에 따라 선택 못하는지 갱신
        {
            foreach (var playerInfoDir in PluginManager.teamInfo.playerInfoList)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerInfoDir.Key);
                if (uPlayer == null) continue;
                PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
                RefreshVehicleTypeState(tc, _team, _vTypeIndex, uPlayer);
            }
        }
        public static void RefreshVehicleTypeState(ITransportConnection _tc, bool _team, byte _vIndex, UnturnedPlayer _uPlayer)
        {
            List<VehicleTypeInfo> infoList = _team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;

            EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleType_{_vIndex}Block_0", false);
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleType_{_vIndex}Block_1", false);
            uint creditCost = PluginManager.instance.Configuration.Instance.vehiclePresets[infoList[_vIndex].presetInfo.vehicleList[0]].creditCost;
            Dictionary<byte, DateTime> timerList = _team ? PluginManager.teamInfo.team_0_vehicleTimer : PluginManager.teamInfo.team_1_vehicleTimer;
            if (timerList.ContainsKey(_vIndex))
            {
                if (timerList[_vIndex] > DateTime.UtcNow)
                {
                    double _time = (timerList[_vIndex] - DateTime.UtcNow).TotalSeconds;
                    int minutes = (int)(_time / 60);
                    int seconds = (int)(_time % 60);
                    string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleType_{_vIndex}Block_0", timeText);
                    EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleType_{_vIndex}Block_0", $"{PluginManager.icons["time"]}");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleType_{_vIndex}Block_0", true);
                    return;
                }
            }
            if (Provider.clients.Count < infoList[_vIndex].presetInfo.playerMinCount)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleType_{_vIndex}Block_0", $"{PluginManager.icons["noPlayer"]}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleType_{_vIndex}Block_0", $"최소 {infoList[_vIndex].presetInfo.playerMinCount}명이 접속해야함");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleType_{_vIndex}Block_0", true);
                return;
            }
            PlayerTeamRecordInfo pTeamRecordInfo = _team ? PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_1_RecordInfo;
            PlayerData pData = PluginManager.playerDatabase.FindData(_uPlayer.CSteamID);
            if (infoList[_vIndex].presetInfo.levelLimit > pTeamRecordInfo.level)
            {
                LevelPreset levelPreset = _team ? PluginManager.teamInfo.teamPreset_0.levelPresets[infoList[_vIndex].presetInfo.levelLimit] : PluginManager.teamInfo.teamPreset_1.levelPresets[infoList[_vIndex].presetInfo.levelLimit];
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleType_{_vIndex}Block_0", $"LV.{infoList[_vIndex].presetInfo.levelLimit} {levelPreset.name} 달성 시 해제");
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleType_{_vIndex}Block_0", $"{levelPreset.iconUrl}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleType_{_vIndex}Block_0", true);
                return;
            }
            if (pData.credit < creditCost)
            {
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleType_{_vIndex}Block_1", $"최소 {creditCost} 필요");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleType_{_vIndex}Block_1", true);
                return;
            }
        }
        public static void RefreshVehicleTypeCountAll(ITransportConnection _tc, bool _team) // 각 차종별 모든 배치된 차량의 개수 갱신
        {
            List<VehicleTypeInfo> vehicleList = _team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;
            for (byte i = 0; i < vehicleList.Count; i++)
            {
                RefreshVehicleTypeCount(_tc, _team, i, vehicleList[i]);
            }
        }
        public static void RefreshVehicleTypeCountToEveryone(bool _team, byte _vTypeIndex)
        {
            List<VehicleTypeInfo> vehicleList = _team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_1_VehicleTypes;
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshVehicleTypeCount(tc, _team, _vTypeIndex, vehicleList[_vTypeIndex]);
            }
        }
        public static void RefreshVehicleTypeCount(ITransportConnection _tc, bool _team, byte _index, VehicleTypeInfo _vehicleInfo) // 해당 차량종류의 배치된 차량 개수 갱신
        {
            EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleTypeCount_{_index}", $"{_vehicleInfo.vehicleCount}/{_vehicleInfo.presetInfo.vehicleMax}");
        }
        public static void RefreshUIVehicleTypeTimerToEveryone(bool _team, byte _vTypeIndex, double _time) // 해당 차량종류의 대기시간 표시
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                if (_time > 0)
                {
                    int minutes = (int)(_time / 60);
                    int seconds = (int)(_time % 60);
                    string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");
                    ITransportConnection tc = steamPlayer.transportConnection;
                    EffectManager.sendUIEffectText(47, tc, false, $"T_VehicleType_{_vTypeIndex}Block_0", timeText);
                }
                else
                {
                    RefreshVehicleTypeStateToEveryone(_team, _vTypeIndex);
                }
            }
        }
        public static void RefreshUIVehicleDeployTimerToEveryone(bool _team, byte _vDeployIndex, VehicleDeployInfo _vDeployInfo, double _time) // 해당 배치차량의 제거시간 표시
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;
                if (_time > 0)
                {
                    int minutes = (int)(_time / 60);
                    int seconds = (int)(_time % 60);
                    string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");
                    ITransportConnection tc = steamPlayer.transportConnection;
                    EffectManager.sendUIEffectText(47, tc, false, $"T_VehicleDeploy_{_vDeployIndex}", timeText);
                }
            }
        }
        public static void RefreshUIVehicleGroupAllToEveryone(bool _team) // 같은 팀 유저에게 모든 대기차량 정보 초기화 후  갱신
        {
            foreach (var playerInfoDir in PluginManager.teamInfo.playerInfoList)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerInfoDir.Key);
                if (uPlayer == null) continue;
                PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;

                ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
                RefreshUIVehicleGroupAll(tc, playerInfoDir.Value.vGroupInstanceID, _team);
            }
        }
        public static void RefreshUIVehicleGroupAll(ITransportConnection _tc, ushort _vGroupInstanceID, bool _team) // 모든 대기차량 정보 초기화 후 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveClass_VehicleGroup", true);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveVehicleGroupBlock", true);

            List<VehicleGroupInfo> vehicleGroupList = _team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
            for (int i = 0; i < vehicleGroupList.Count; i++)
            {
                VehiclePresetTable vehiclePreset = PluginManager.instance.Configuration.Instance.vehiclePresets[vehicleGroupList[i].vPresetIndex];
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleGroup_{i}", $"{vehicleGroupList[i].vehicleTypePreset.iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleGroupName_{i}", $"{vehiclePreset.name}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleGroupCount_{i}", $"{vehicleGroupList[i].crewCount}/{vehicleGroupList[i].maxSeats}");
                if (vehicleGroupList[i].instanceID == _vGroupInstanceID) EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleGroup_{i}Block_0", true);
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_VehicleGroup_{i}", true);
                if (vehicleGroupList[i].isLocked) EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleGroupIcon_{i}", $"{PluginManager.icons["lock"]}");
                else EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleGroupIcon_{i}", $"{PluginManager.icons["player"]}");
            }
        }
        public static void RefreshUIVehicleGroupToEveryone(byte _vIndex, bool _team) // 같은 팀 유저에게 특정 대기차량의 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;

                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIVehicleGroup(tc, _vIndex, _team);
            }
        }
        public static void RefreshUIVehicleGroup(ITransportConnection _tc, byte _vIndex, bool _team) // 대기차량의 정보 갱신
        {
            VehicleGroupInfo vGroupInfo = _team ? PluginManager.teamInfo.team_0_VehicleGroups[_vIndex] : PluginManager.teamInfo.team_1_VehicleGroups[_vIndex];
            VehiclePresetTable vehiclePreset = PluginManager.instance.Configuration.Instance.vehiclePresets[vGroupInfo.vPresetIndex];
            EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleGroupCount_{_vIndex}", $"{vGroupInfo.crewCount}/{vGroupInfo.maxSeats}");
            EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleGroupName_{_vIndex}", $"{vehiclePreset.name}");
            if (vGroupInfo.isLocked) EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleGroupIcon_{_vIndex}", $"{PluginManager.icons["lock"]}");
            else EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleGroupIcon_{_vIndex}", $"{PluginManager.icons["player"]}");
        }
        public static void RefreshUIVehicleDeployAllToEveryone(bool _team) // 같은 팀 유저에게 모든 대기차량 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;

                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIVehicleDeployAll(tc, _team);
            }
        }
        public static void RefreshUIVehicleDeployAll(ITransportConnection _tc,bool _team) // 모든 배치된 차량 정보 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveClass_VehicleDeploy", true);

            List<VehicleDeployInfo> vDeployList = _team ? PluginManager.teamInfo.team_0_VehicleDeploys : PluginManager.teamInfo.team_1_VehicleDeploys;
            for (byte i = 0; i < vDeployList.Count; i++)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleDeploy_{i}", $"{vDeployList[i].vTypePreset.iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleDeployName_{i}", $"{vDeployList[i].vPreset.name}");
                RefreshUIVehicleDeployState(_tc, i, vDeployList[i]);
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_VehicleDeploy_{i}", true);
            }
        }
        public static void RefreshUIVehicleDeployStateToEveryone(byte _vDeployIndex, bool _team, VehicleDeployInfo _vDeployInfo) // 같은 팀 유저에게 해당 배치된 차량의 상태 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.localUIState != EnumTable.EPlayerUIState.Loadout) continue;

                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIVehicleDeployState(tc, _vDeployIndex, _vDeployInfo);
            }
        }
        public static void RefreshUIVehicleDeployState(ITransportConnection _tc, byte _vDeployIndex, VehicleDeployInfo _vDeployInfo) // 해당 배치된 차량의 상태 정보 갱신
        {
            switch (_vDeployInfo.state)
            {
                case EnumTable.EDeployVehicleState.Normal:
                    EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleDeployIcon_{_vDeployIndex}", $"{PluginManager.icons["lock"]}");
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleDeploy_{_vDeployIndex}", "배치됨");
                    break;
                case EnumTable.EDeployVehicleState.Abandon:
                    EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleDeployIcon_{_vDeployIndex}", $"{PluginManager.icons["noPlayer"]}");
                    break;
                case EnumTable.EDeployVehicleState.Destroyed:
                    EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleDeployIcon_{_vDeployIndex}", $"{PluginManager.icons["explosion"]}");
                    break;
            }
            if(_vDeployInfo.state != EnumTable.EDeployVehicleState.Normal)
            {
                double time = (_vDeployInfo.removeDateTime - DateTime.UtcNow).TotalSeconds;
                int minutes = (int)(time / 60);
                int seconds = (int)(time % 60);
                string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleDeploy_{_vDeployIndex}", $"{timeText}");
            }
        }
        public static void RefreshUIVehicleGroupInfoToMembers_Vehicle(VehicleGroupInfo _vGroupInfo) // 차량 그룹내 모든 유저에게 차량 정보만 갱신
        {
            foreach (SeatInfo seat in _vGroupInfo.seats)
            {
                if (seat.cSteamID != CSteamID.NonSteamGS)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(seat.cSteamID);
                    RefreshVehicleGroupInfo_Vehicle(uPlayer, _vGroupInfo);
                }
            }
        }
        public static void RefreshUIVehicleGroupInfoToMembers_Member(VehicleGroupInfo _vGroupInfo) // 차량 그룹내 모든 유저에게 유저 정보만 갱신
        {
            foreach (SeatInfo seat in _vGroupInfo.seats)
            {
                if (seat.cSteamID != CSteamID.NonSteamGS)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(seat.cSteamID);
                    RefreshUIVehicleGroupInfo_Member(uPlayer, _vGroupInfo);
                }
            }
        }
        public static void RefreshUIVehicleGroupInfoToMembers(VehicleGroupInfo _vGroupInfo) // 차량 그룹내 모든 유저에게 정보 갱신
        {
            foreach (SeatInfo seat in _vGroupInfo.seats)
            {
                if (seat.cSteamID != CSteamID.NonSteamGS)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(seat.cSteamID);
                    RefreshVehicleGroupInfo_Vehicle(uPlayer, _vGroupInfo);
                    RefreshUIVehicleGroupInfo_Member(uPlayer, _vGroupInfo);
                    // 리더는 배치 정보 갱신
                    if (_vGroupInfo.leaderID == seat.cSteamID) UISystem.RefreshDeployState(PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID), uPlayer.Player.channel.GetOwnerTransportConnection());
                }
            }
        }
        public static void RefreshVehicleGroupInfo_Vehicle(UnturnedPlayer _uPlayer, VehicleGroupInfo _vGroupInfo) // 유저가 속한 차량그룹에서 차량정보 갱신
        {
            VehiclePresetTable vehiclePreset = PluginManager.instance.Configuration.Instance.vehiclePresets[_vGroupInfo.vPresetIndex];
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
            // 차량 정보 갱신
            EffectManager.sendUIEffectImageURL(47, tc, false, "I_VehicleInfo", $"{vehiclePreset.iconUrl}");
            EffectManager.sendUIEffectText(47, tc, false, "T_VehicleInfoName", $"{vehiclePreset.name}");
            // 정보 갱신
            if (_vGroupInfo.leaderID == _uPlayer.CSteamID) // 리더라면 차량 로드아웃 잠금해제 및 차량 옵션 활성화
            {
                LoadoutSystem.RefreshLoadout_Vehicle(tc, _vGroupInfo.vPresetIndex);
                EffectManager.sendUIEffectVisibility(47, tc, false, "P_LoadoutType_1Block_1", false);
                EffectManager.sendUIEffectVisibility(47, tc, false, "P_VehicleOption", true);
                if (_vGroupInfo.isLocked) // 차량이 잠긴 상태라면
                {
                    EffectManager.sendUIEffectText(47, tc, false, "T_VehicleOptionUnlock", $"{_vGroupInfo.password}");
                    EffectManager.sendUIEffectVisibility(47, tc, false, "BP_VehicleOptionLock", false);
                    EffectManager.sendUIEffectVisibility(47, tc, false, "BP_VehicleOptionUnlock", true);
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(47, tc, false, "BP_VehicleOptionLock", true);
                    EffectManager.sendUIEffectVisibility(47, tc, false, "BP_VehicleOptionUnlock", false);
                }
            }
            else
            {
                EffectManager.sendUIEffectVisibility(47, tc, false, "P_LoadoutType_1Block_1", true);
                EffectManager.sendUIEffectVisibility(47, tc, false, "P_VehicleOption", false);
                LoadoutSystem.SetLoadoutType_Inventory(tc);
            }
        }
        public static void RefreshUIVehicleGroupInfo_Member(UnturnedPlayer _uPlayer, VehicleGroupInfo _vGroupInfo) // 유저가 속한 차량그룹에서 유저정보 갱신
        {
            VehiclePresetTable vehiclePreset = PluginManager.instance.Configuration.Instance.vehiclePresets[_vGroupInfo.vPresetIndex];
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();

            EffectManager.sendUIEffectText(47, tc, false, "T_VehicleInfoCount", $"{_vGroupInfo.crewCount}/{_vGroupInfo.maxSeats}");
            // 승무원 정보 갱신
            RefreshVehicleGroupMemberInfo(_uPlayer, tc, _vGroupInfo, vehiclePreset);
        }
        public static void RefreshVehicleGroupMemberInfo(UnturnedPlayer _uPlayer, ITransportConnection _tc, VehicleGroupInfo _vGroupInfo, VehiclePresetTable _vehiclePreset) // 차량 그룹내 승무원 정보 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveVGroupPlayer", true);
            for (int i = 0; i < _vGroupInfo.seats.Length; i++)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_VehicleCrew_{i}", $"{_vehiclePreset.seats[i].iconUrl}");
                if (_vGroupInfo.seats[i].cSteamID != CSteamID.NonSteamGS) // 좌석에 사람이 있는 경우
                {
                    UnturnedPlayer player = UnturnedPlayer.FromCSteamID(_vGroupInfo.seats[i].cSteamID);
                    if (_vGroupInfo.seats[i].cSteamID == _vGroupInfo.leaderID) EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleCrewLeader_{i}", $"{player.DisplayName}"); // 좌석의 유저가 리더라면
                    else EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleCrewMember_{i}", $"{player.DisplayName}");
                    if (_uPlayer.CSteamID == _vGroupInfo.leaderID) { if (_vGroupInfo.seats[i].cSteamID != _uPlayer.CSteamID) EffectManager.sendUIEffectVisibility(47, _tc, false, $"B_SeatExile_{i}", true); } // 자신이 리더라면
                }
                else // 좌석에 사람이 없는경우
                {
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_VehicleCrewMember_{i}", $"{_vehiclePreset.seats[i].name}");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"B_SeatChange_{i}", true);
                }
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_VehicleCrew_{i}", true);
            }
        }
    }
}

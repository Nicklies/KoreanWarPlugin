using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Info;
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
    public class ButtonSystem
    {
        public static void OnButtonClick_TeamSelect(UnturnedPlayer _uPlayer, ITransportConnection _tc, string _buttonName) // 팀 선택 창에서 버튼 선택 시 실행되는 함수 
        {
            switch (_buttonName)
            {
                case "B_Team_0": // 1팀 선택
                    PluginManager.teamInfo.teamJoinRequest.RequestTeamJoin(_uPlayer.CSteamID, true, UISystem.OnTeamJoinRequestEnd);
                    return;
                case "B_Team_1": // 2팀 선택
                    PluginManager.teamInfo.teamJoinRequest.RequestTeamJoin(_uPlayer.CSteamID, false, UISystem.OnTeamJoinRequestEnd);
                    return;
                case "B_Option": // 옵션 선택
                    return;
                case "B_Discord": // 디스코드 입장 선택
                    _uPlayer.Player.sendBrowserRequest("DiscordLink", PluginManager.instance.Configuration.Instance.discordUrl);
                    return;
            }
        }
        public static void OnBuuttonClick_Loadout(UnturnedPlayer _uPlayer, ITransportConnection _tc, string _buttonName)
        {
            PlayerComponent pc = _uPlayer.GetComponent<PlayerComponent>();
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            bool team = playerInfo.team;
            switch (_buttonName)
            {
                case "B_TeamSwitch": // 팀 변경
                    if (pc.isProcessing) return;
                    UISystem.LeaveTeam(_uPlayer, playerInfo, _tc, team);
                    // 로딩 해제
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "L_Loading", false);
                    return;
                case "B_Skin": // 스킨 탭 선택
                    return;
                case "B_ClassType_0": // 보병 병과 탭
                    ClassSystem.SetClassType_Infantary(pc, _tc);
                    return;
                case "B_ClassType_1": // 운전병 병과 탭
                    ClassSystem.SetClassType_Driver(pc, _tc);
                    return;
                case "B_ChatSend": // 채팅 보내기
                    UISystem.SendChat(_uPlayer, pc, playerInfo);
                    return;
                case "B_Deploy": // 전투 배치
                    if (pc.isProcessing) return;
                    pc.isProcessing = true;
                    bool isvLeaderDeploy = false; // 차량 리더로써 배치하는 것인지 여부
                    if (playerInfo.vGroupInstanceID != ushort.MaxValue)
                    {
                        VehicleGroupInfo vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(playerInfo.vGroupInstanceID, team);
                        if (vGroupInfo.leaderID == _uPlayer.CSteamID) isvLeaderDeploy = true;
                    }
                    DeploySystem.Deploy(_uPlayer, team, isvLeaderDeploy);
                    pc.isProcessing = false;
                    return;
                case "B_PasswordLeave": // 차량 비밀번호창 닫기
                    pc.enterPasswordText = "";
                    return;
                case "B_PasswordEnter": // 차량 그룹 비밀번호 입력 후 입장 시도
                    VehicleGroupSystem.TryPassword_VehicleGroup(_uPlayer, pc, _tc, team);
                    return;
                case "B_ChatType": // 채팅 종류 변환
                    pc.localChatType++;
                    if (EnumTable.EChatType.Vehicle < pc.localChatType || playerInfo.vGroupInstanceID == ushort.MaxValue && EnumTable.EChatType.Vehicle == pc.localChatType) pc.localChatType = EnumTable.EChatType.Team;
                    UISystem.ChangeChatType(pc, _tc);
                    return;
                case "B_Marker_Base": // 기지 스폰 마커 선택
                    playerInfo.spawnIndex = 5;
                    playerInfo.spawnInstaceID_Dynamic = ushort.MaxValue;
                    UISystem.RefreshDeployState(playerInfo, _tc);
                    return;
            }
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                if (_buttonName == $"B_Marker_Objective_{i}")
                {
                    playerInfo.spawnIndex = i;
                    playerInfo.spawnInstaceID_Dynamic = ushort.MaxValue;
                    UISystem.RefreshDeployState(playerInfo, _tc);
                }
            }
            List<SpawnableVehicleInfo> spawnableVehicleList = team ? PluginManager.teamInfo.team_0_spawnableVehicle : PluginManager.teamInfo.team_1_spawnableVehicle;
            for (byte i = 0; i < spawnableVehicleList.Count; i++)
            {
                if (_buttonName == $"B_Marker_Dynamic_{i}")
                {
                    playerInfo.spawnIndex = (byte)(i + 6);
                    playerInfo.spawnInstaceID_Dynamic = spawnableVehicleList[i].instanceID;
                    UISystem.RefreshDeployState(playerInfo, _tc);
                }
            }
            if (playerInfo.classIndex != byte.MaxValue) // 병과가 선택된 상태라면
            {
                bool isButtonCorrect = false;
                string loadoutDefaultName = "주무기";
                EnumTable.ELoadoutType type = EnumTable.ELoadoutType.primary;
                // 로드아웃 선택창
                switch (_buttonName)
                {
                    case "B_Primary": // 주무기 
                        isButtonCorrect = true;
                        break;
                    case "B_Sight": // 조준경
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.sight;
                        loadoutDefaultName = "조준경";
                        break;
                    case "B_Tactical": // 전술장비 
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.tactical;
                        loadoutDefaultName = "전술장비";
                        break;
                    case "B_Magazine": // 탄창 
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.magazine;
                        loadoutDefaultName = "탄창";
                        break;
                    case "B_Grip": // 손잡이
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.grip;
                        loadoutDefaultName = "손잡이";
                        break;
                    case "B_Secondary": // 보조무기
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.secondary;
                        loadoutDefaultName = "보조무기";
                        break;
                    case "B_Explosive_0": // 폭팔물1
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.explosive_0;
                        loadoutDefaultName = "폭팔물";
                        break;
                    case "B_Explosive_1": // 폭팔물2
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.explosive_1;
                        loadoutDefaultName = "폭팔물";
                        break;
                    case "B_Equip": // 장구류
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.equipment;
                        loadoutDefaultName = "장구류";
                        break;
                    case "B_Utility_0": // 특수장비1
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.utility_0;
                        loadoutDefaultName = "특수장비";
                        break;
                    case "B_Utility_1": // 특수장비2
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.utility_1;
                        loadoutDefaultName = "특수장비";
                        break;
                }
                if (isButtonCorrect)
                {
                    playerInfo.selectItemType = type;
                    EffectManager.sendUIEffectText(47, _tc, false, "T_SelectItemName", $"{loadoutDefaultName}");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_ToggleSelectItem", true);
                    LoadoutSystem.RefreshUISelectItem(playerInfo, _tc, team, type);
                    return;
                }
                // 인벤토리 아이템 제거
                switch (_buttonName)
                {
                    case "B_PrimaryRemove": // 주무기 제거
                        isButtonCorrect = true;
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "L_SelectItem", false);
                        break;
                    case "B_SightRemove": // 조준경 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.sight;
                        break;
                    case "B_TacticalRemove": // 전술장비 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.tactical;
                        break;
                    case "B_MagazineRemove": // 탄창 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.magazine;
                        break;
                    case "B_GripRemove": // 손잡이 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.grip;
                        break;
                    case "B_SecondaryRemove": // 보조무기 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.secondary;
                        break;
                    case "B_Explosive_0Remove": // 폭팔물1 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.explosive_0;
                        break;
                    case "B_Explosive_1Remove": // 폭팔물2 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.explosive_1;
                        break;
                    case "B_EquipRemove": // 장구류 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.equipment;
                        break;
                    case "B_Utility_0Remove": // 특수장비1 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.utility_0;
                        break;
                    case "B_Utility_1Remove": // 특수장비2 제거
                        isButtonCorrect = true;
                        type = EnumTable.ELoadoutType.utility_1;
                        break;
                }
                if (isButtonCorrect)
                {
                    UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                    LoadoutSystem.RefreshLoadout_Inventory(configuration, playerInfo, _tc, ushort.MaxValue, type);
                    LoadoutSystem.RefreshDeployCost(playerInfo, _tc);
                    UISystem.RefreshDeployState(playerInfo, _tc);
                    return;
                }
                ushort[] selectItemList = null;
                // 장비 선택창에서 아이템 선택
                switch (playerInfo.selectItemType)
                {
                    case EnumTable.ELoadoutType.primary:
                        selectItemList = configuration.classPresets[playerInfo.classPrestIndex].primaryList;
                        break;
                    case EnumTable.ELoadoutType.sight:
                        if (playerInfo.primaryIndex != ushort.MaxValue) selectItemList = configuration.primaryPresets[playerInfo.primaryIndex].sights;
                        break;
                    case EnumTable.ELoadoutType.tactical:
                        if (playerInfo.primaryIndex != ushort.MaxValue) selectItemList = configuration.primaryPresets[playerInfo.primaryIndex].tacticals;
                        break;
                    case EnumTable.ELoadoutType.magazine:
                        if (playerInfo.primaryIndex != ushort.MaxValue) selectItemList = configuration.primaryPresets[playerInfo.primaryIndex].magazines;
                        break;
                    case EnumTable.ELoadoutType.grip:
                        if (playerInfo.primaryIndex != ushort.MaxValue) selectItemList = configuration.primaryPresets[playerInfo.primaryIndex].grips;
                        break;
                    case EnumTable.ELoadoutType.secondary:
                        selectItemList = configuration.classPresets[playerInfo.classPrestIndex].secondaryList;
                        break;
                    case EnumTable.ELoadoutType.explosive_0:
                        selectItemList = configuration.classPresets[playerInfo.classPrestIndex].explosiveList;
                        break;
                    case EnumTable.ELoadoutType.explosive_1:
                        selectItemList = configuration.classPresets[playerInfo.classPrestIndex].explosiveList;
                        break;
                    case EnumTable.ELoadoutType.equipment:
                        LoadoutTable[] loadoutList = configuration.equipmentPresets[configuration.classPresets[playerInfo.classPrestIndex].equipmentInstanceID].clothPresetList;
                        selectItemList = new ushort[loadoutList.Length];
                        for (ushort i = 0; i < loadoutList.Length; i++) selectItemList[i] = i;
                        break;
                    case EnumTable.ELoadoutType.utility_0:
                        selectItemList = configuration.classPresets[playerInfo.classPrestIndex].utilityList;
                        break;
                    case EnumTable.ELoadoutType.utility_1:
                        selectItemList = configuration.classPresets[playerInfo.classPrestIndex].utilityList;
                        break;
                }
                if (selectItemList != null && selectItemList.Length > 0)
                {
                    for (int i = 0; i < selectItemList.Length; i++)
                    {
                        if (_buttonName == $"B_SelectItem_{i}")
                        {
                            LoadoutTable[] loadoutTableList = null;
                            byte beforeCost = 0; // 이전에 선택한 장비의 비용
                            // 보급 한계치를 넘는지 확인
                            switch (playerInfo.selectItemType)
                            {
                                case EnumTable.ELoadoutType.primary:
                                    loadoutTableList = configuration.primaryPresets;
                                    if (playerInfo.primaryIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.primaryIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.sight:
                                    loadoutTableList = configuration.attachmentPresets;
                                    if (playerInfo.sightIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.sightIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.tactical:
                                    loadoutTableList = configuration.attachmentPresets;
                                    if (playerInfo.tacticalIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.tacticalIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.magazine:
                                    loadoutTableList = configuration.attachmentPresets;
                                    if (playerInfo.tacticalIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.tacticalIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.grip:
                                    loadoutTableList = configuration.attachmentPresets;
                                    if (playerInfo.gripIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.gripIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.secondary:
                                    loadoutTableList = configuration.secondaryPresets;
                                    if (playerInfo.secondaryIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.secondaryIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.explosive_0:
                                    loadoutTableList = configuration.explosivePresets;
                                    if (playerInfo.explosive_0Index != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.explosive_0Index].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.explosive_1:
                                    loadoutTableList = configuration.explosivePresets;
                                    if (playerInfo.explosive_1Index != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.explosive_1Index].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.equipment:
                                    loadoutTableList = configuration.equipmentPresets[configuration.classPresets[playerInfo.classPrestIndex].equipmentInstanceID].clothPresetList;
                                    if (playerInfo.equipmentIndex != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.equipmentIndex].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.utility_0:
                                    loadoutTableList = configuration.utilityPresets;
                                    if (playerInfo.utility_0Index != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.utility_0Index].supplyCost;
                                    break;
                                case EnumTable.ELoadoutType.utility_1:
                                    loadoutTableList = configuration.utilityPresets;
                                    if (playerInfo.utility_1Index != ushort.MaxValue) beforeCost = loadoutTableList[playerInfo.utility_1Index].supplyCost;
                                    break;
                            }
                            if (playerInfo.supplyCost + loadoutTableList[selectItemList[i]].supplyCost - beforeCost > playerInfo.supplyPoint_Max) return;
                            UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                            LoadoutSystem.RefreshLoadout_Inventory(configuration, playerInfo, _tc, selectItemList[i], playerInfo.selectItemType);
                            EffectManager.sendUIEffectVisibility(47, _tc, false, "L_SelectItem", false);
                            LoadoutSystem.RefreshDeployCost(playerInfo, _tc);
                            UISystem.RefreshDeployState(playerInfo, _tc);
                            return;
                        }
                    }
                }
            }
            if (playerInfo.vGroupInstanceID != ushort.MaxValue) // 차량 그룹에 속한 상태인 경우
            {
                if (PluginManager.teamInfo.FindVehicleGroup(playerInfo.vGroupInstanceID, team, out VehicleGroupInfo _vGroupInfo, out byte vIndex))
                {
                    switch (_buttonName)
                    {
                        case "B_VehicleOptionExit": // 차량그룹 퇴장
                            UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                            PluginManager.teamInfo.RemovePlayerFromVehicleGroup(_uPlayer.CSteamID, playerInfo, team);
                            EffectManager.sendUIEffectVisibility(47, _tc, false, "L_VehicleInfo", false);
                            VehicleGroupSystem.RefreshUIVehicleGroupAllToEveryone(team);
                            UISystem.RefreshDeployState(playerInfo, _tc);
                            return;
                        case "B_VehicleL": // 로드아웃 차량 선택
                            EffectManager.sendUIEffectText(47, _tc, false, "T_SelectItemName", $"차량");
                            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_ToggleSelectVehicle", true);
                            LoadoutSystem.RefreshUISelectVehicle(playerInfo, _vGroupInfo, _tc, team);
                            return;
                        case "B_VehicleOptionLock": // 차량 잠금 선택
                            if (pc.passwordText.Length < 4) return; // 비밀번호가 4자리가 아니면 리턴
                            _vGroupInfo.password = pc.passwordText;
                            _vGroupInfo.isLocked = true;
                            pc.passwordText = "";
                            VehicleGroupSystem.RefreshUIVehicleGroupToEveryone(vIndex, team);
                            VehicleGroupSystem.RefreshVehicleGroupInfo_Vehicle(_uPlayer, _vGroupInfo);
                            return;
                        case "B_VehicleOptionUnlock": // 차량 잠금해제 선택
                            _vGroupInfo.password = "";
                            _vGroupInfo.isLocked = false;
                            VehicleGroupSystem.RefreshUIVehicleGroupToEveryone(vIndex, team);
                            VehicleGroupSystem.RefreshVehicleGroupInfo_Vehicle(_uPlayer, _vGroupInfo);
                            return;
                    }
                    if (!pc.isProcessing) // 작업 수행중인 경우 차량요청 관련 작업 사용 불가
                    {
                        ushort[] selectVehicleList = _vGroupInfo.vehicleTypePreset.vehicleList;
                        if (selectVehicleList != null && selectVehicleList.Length > 0)
                        {
                            for (byte i = 0; i < selectVehicleList.Length; i++)
                            {
                                if (_buttonName == $"B_SelectVehicle_{i}") // 차량 선택 버튼을 누른경우
                                {
                                    if (selectVehicleList[i] == _vGroupInfo.vPresetIndex) return; // 이미 선택된 차량인 경우 리턴
                                    UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                                    if (team) PluginManager.teamInfo.team_0_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, playerInfo.vGroupInstanceID, team, EnumTable.ERequestType.ChangeVehicle, VehicleGroupSystem.OnVehicleRequestEnd);
                                    else PluginManager.teamInfo.team_1_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, playerInfo.vGroupInstanceID, team, EnumTable.ERequestType.ChangeVehicle, VehicleGroupSystem.OnVehicleRequestEnd);
                                    return;
                                }
                            }
                        }
                        for (byte i = 0; i < 6; i++)
                        {
                            if (_buttonName == $"B_SeatChange_{i}") // 좌석 변경 버튼을 누른경우
                            {
                                if (_vGroupInfo.seats[i].cSteamID == CSteamID.NonSteamGS)
                                {
                                    UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                                    if (team) PluginManager.teamInfo.team_0_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, playerInfo.vGroupInstanceID, team, EnumTable.ERequestType.Switch, VehicleGroupSystem.OnVehicleRequestEnd);
                                    else PluginManager.teamInfo.team_1_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, playerInfo.vGroupInstanceID, team, EnumTable.ERequestType.Switch, VehicleGroupSystem.OnVehicleRequestEnd);
                                    return;
                                }
                            }
                        }
                        for (byte i = 0; i < 6; i++)
                        {
                            if (_buttonName == $"B_SeatExile_{i}") // 좌석 추방 버튼을 누른경우
                            {
                                if (_vGroupInfo.seats[i].cSteamID != CSteamID.NonSteamGS)
                                {
                                    UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                                    if (team) PluginManager.teamInfo.team_0_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, playerInfo.vGroupInstanceID, team, EnumTable.ERequestType.Exile, VehicleGroupSystem.OnVehicleRequestEnd);
                                    else PluginManager.teamInfo.team_1_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, playerInfo.vGroupInstanceID, team, EnumTable.ERequestType.Exile, VehicleGroupSystem.OnVehicleRequestEnd);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (!pc.isProcessing) // 작업 중인경우 모든 요청관련 작업 수행 불가
            {
                List<ClassInfo> classInfList = team ? PluginManager.teamInfo.team_0_ClassInf : PluginManager.teamInfo.team_1_ClassInf;
                List<ClassInfo> classDriverList = team ? PluginManager.teamInfo.team_0_ClassDriver : PluginManager.teamInfo.team_1_ClassDriver;
                // 보병 병과 선택 버튼
                for (byte i = 0; i < classInfList.Count; i++)
                {
                    if (_buttonName == $"B_SelectClass_{i}")
                    {
                        UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                        if (team) PluginManager.teamInfo.team_0_ClassRequest.RequestClass(_uPlayer.CSteamID, i, team, false, EnumTable.EClassType.infantary, ClassSystem.OnClassRequestEnd);
                        else PluginManager.teamInfo.team_1_ClassRequest.RequestClass(_uPlayer.CSteamID, i, team, false, EnumTable.EClassType.infantary, ClassSystem.OnClassRequestEnd);
                        return;
                    }
                }
                // 운전병 병과 선택 버튼
                for (byte i = 0; i < classDriverList.Count; i++)
                {
                    if (_buttonName == $"B_SelectCrew_{i}")
                    {
                        UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                        if (team) PluginManager.teamInfo.team_0_ClassRequest.RequestClass(_uPlayer.CSteamID, i, team, false, EnumTable.EClassType.driver, ClassSystem.OnClassRequestEnd);
                        else PluginManager.teamInfo.team_1_ClassRequest.RequestClass(_uPlayer.CSteamID, i, team, false, EnumTable.EClassType.driver, ClassSystem.OnClassRequestEnd);
                        return;
                    }
                }
                List<VehicleTypeInfo> vehicleTypeList = team ? PluginManager.teamInfo.team_0_VehicleTypes : PluginManager.teamInfo.team_0_VehicleTypes;
                // 신규 차량 선택 버튼
                for (byte i = 0; i < vehicleTypeList.Count; i++)
                {
                    if (_buttonName == $"B_VehicleType_{i}")
                    {
                        if (PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID).vTypeIndex == i) return;
                        UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                        if (team) PluginManager.teamInfo.team_0_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, ushort.MaxValue, team, EnumTable.ERequestType.Create, VehicleGroupSystem.OnVehicleRequestEnd);
                        else PluginManager.teamInfo.team_1_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, i, ushort.MaxValue, team, EnumTable.ERequestType.Create, VehicleGroupSystem.OnVehicleRequestEnd);
                        return;
                    }
                }
                List<VehicleGroupInfo> vGroupList = team ? PluginManager.teamInfo.team_0_VehicleGroups : PluginManager.teamInfo.team_1_VehicleGroups;
                // 대기차량 입장
                for (int i = 0; i < vGroupList.Count; i++)
                {
                    if (_buttonName == $"B_VehicleGroup_{i}")
                    {
                        if (vGroupList[i].isLocked)
                        {
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"L_Password", true);
                            pc.passwordVGroupSelect = vGroupList[i].instanceID;
                            return;
                        }
                        UISystem.RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.Loading);
                        if (team) PluginManager.teamInfo.team_0_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, byte.MaxValue, vGroupList[i].instanceID, team, EnumTable.ERequestType.Join, VehicleGroupSystem.OnVehicleRequestEnd);
                        else PluginManager.teamInfo.team_1_NewVehicleRequest.RequestVehicle(_uPlayer.CSteamID, byte.MaxValue, vGroupList[i].instanceID, team, EnumTable.ERequestType.Join, VehicleGroupSystem.OnVehicleRequestEnd);
                    }
                }
            }
        }
        public static void OnBuuttonClick_RoundEnd(UnturnedPlayer _uPlayer, ITransportConnection _tc, string _buttonName)
        {
            if (PluginManager.instance.isVoteEnd) return;
            // 맵 투표 선택
            for (byte i = 0; i < 6; i++)
            {
                if (_buttonName == $"B_MapVote_{i}")
                {
                    PluginManager.roundInfo.PlayerSendVote(_uPlayer, i);
                    return;
                }
            }
            // 게임모드 투표 선택
            for (byte i = 6; i < PluginManager.roundInfo.voteCount.Length; i++)
            {
                if (_buttonName == $"B_GameModeVote_{i - 6}")
                {
                    PluginManager.roundInfo.PlayerSendVote(_uPlayer, i);
                    return;
                }
            }
        }
    }
}

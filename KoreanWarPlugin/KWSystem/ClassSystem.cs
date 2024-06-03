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
    public class ClassSystem // 병과 관련 시스템
    {
        public static void SetClassType_Infantary(PlayerComponent _pc, ITransportConnection _tc) // 보병 병과 UI창으로 변환
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_ClassType_0Block", true);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_ClassType_1Block", false);
        }
        public static void SetClassType_Driver(PlayerComponent _pc, ITransportConnection _tc) // 운전병 병과 UI창으로 변환
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_ClassType_0Block", false);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_ClassType_1Block", true);
        }
        public static void LoadClassInfo(ITransportConnection _tc, bool _team) // 팀의 모든 병과 정보를 불러옴
        {
            // 병과 리스트 UI 초기화
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveClass_Inf", true);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveClass_Driver", true);
            // 보병 병과 정보 불러오기
            List<ClassInfo> classInfoList = _team ? PluginManager.teamInfo.team_0_ClassInf : PluginManager.teamInfo.team_1_ClassInf;
            for (int i = 0; i < classInfoList.Count; i++)
            {
                ClassPresetTable classPreset = classInfoList[i].presetInfo;
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_SelectClass_{i}", true);
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClass_{i}", classPreset.iconUrl);
                EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectClassName_{i}", classPreset.name);
            }
            // 운전병 병과 정보 불러오기
            classInfoList = _team ? PluginManager.teamInfo.team_0_ClassDriver : PluginManager.teamInfo.team_1_ClassDriver;
            for (int i = 0; i < classInfoList.Count; i++)
            {
                ClassPresetTable classPreset = classInfoList[i].presetInfo; // 병과 정보
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_SelectCrew_{i}", true);
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectCrew_{i}", classPreset.iconUrl);
                EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectCrewName_{i}", classPreset.name);
            }
        }
        public static void OnClassRequestEnd(CSteamID _cSteamID, byte _index, bool _team, bool _vGroupClass, EnumTable.EClassType _classType, bool _success) // 대기열에서 병과신청이 처리되었을 시
        {
            if (!PluginManager.instance.isRoundStart) return;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(_cSteamID);
            if (uPlayer == null) return;
            PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID);
            ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
            if (playerInfo == null)
            {
                pc.isJoinedTeam = false;
                return;
            }
            if (!_success || playerInfo.isDeployed)
            {
                UISystem.RefreshDeployState(PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID), tc);
                pc.isProcessing = false;
                // 실패 이유 보내기
                return;
            }
            List<ClassInfo> classInfList = _team ? PluginManager.teamInfo.team_0_ClassInf : PluginManager.teamInfo.team_1_ClassInf;
            List<ClassInfo> classDriverList = _team ? PluginManager.teamInfo.team_0_ClassDriver : PluginManager.teamInfo.team_1_ClassDriver;
            ClassInfo classInfo = null; // 선택한 병과 정보
            string classTypeName = "";
            switch (_classType)
            {
                case EnumTable.EClassType.infantary:
                    classInfo = classInfList[_index];
                    classTypeName = "P_SelectClass_";
                    break;
                case EnumTable.EClassType.driver:
                    classInfo = classDriverList[_index];
                    classTypeName = "P_SelectCrew_";
                    break;
            }
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_SelectItem", false);
            if (playerInfo.classPrestIndex != ushort.MaxValue) // 이전에 선택된 병과가 있다면 선택 해제시키기
            {
                switch (playerInfo.classType)
                {
                    case EnumTable.EClassType.infantary:
                        classInfList[playerInfo.classIndex].playerCount--;
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"P_SelectClass_{playerInfo.classIndex}Block_0", false);
                        RefreshUIClassPlayerCountToEveryone(_team, playerInfo.classIndex, EnumTable.EClassType.infantary);
                        break;
                    case EnumTable.EClassType.driver:
                        classDriverList[playerInfo.classIndex].playerCount--;
                        EffectManager.sendUIEffectVisibility(47, tc, false, $"P_SelectCrew_{playerInfo.classIndex}Block_0", false);
                        RefreshUIClassPlayerCountToEveryone(_team, playerInfo.classIndex, EnumTable.EClassType.driver);
                        break;
                }
            }
            else // 이전에 선택된 병과가 없다면
            {
                EffectManager.sendUIEffectVisibility(47, tc, false, $"L_Loadout", true);
                EffectManager.sendUIEffectVisibility(47, tc, false, $"L_Deploy", true);
            }
            if (!_vGroupClass && playerInfo.vGroupInstanceID != ushort.MaxValue) // 보병 선택 중 차량그룹에 속해있는 상태일경우
            {
                PluginManager.teamInfo.RemovePlayerFromVehicleGroup(_cSteamID, playerInfo, _team);
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleInfo", false);
                VehicleGroupSystem.RefreshUIVehicleGroupAllToEveryone(_team);
            }
            EffectManager.sendUIEffectVisibility(47, tc, false, $"{classTypeName}{_index}Block_0", true);
            classInfo.playerCount++;
            playerInfo.classIndex = _index;
            playerInfo.classPrestIndex = _classType == EnumTable.EClassType.infantary ? classInfList[_index].presetInfo.instanceID : classDriverList[_index].presetInfo.instanceID;
            playerInfo.classType = _classType;
            PlayerTeamRecordInfo pTeamRecordInfo = _team ? PluginManager.teamInfo.playerRecordInfoList[uPlayer.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[uPlayer.CSteamID].team_1_RecordInfo;
            RefreshSupplyCost(playerInfo, pTeamRecordInfo);
            playerInfo.supplyCost = 0;
            pc.isProcessing = false;
            RefreshUIClassPlayerCountToEveryone(_team, _index, _classType);
            LoadoutSystem.AssignDefaultInventory(playerInfo, tc, _team);
            UISystem.RefreshDeployState(playerInfo, tc);
            LoadoutSystem.SetLoadoutType_Inventory(tc);
        }
        public static void RefreshUIClassPlayerCountAllToEveryone(bool _team, EnumTable.EClassType _type) // 같은팀 모든 유저에게 병과 인원 수 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.uIState != EnumTable.EPlayerUIState.Loadout) continue;

                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIClassPlayerCountAll(tc, _team, _type);
            }
        }
        public static void RefreshUIClassPlayerCountAll(ITransportConnection _tc, bool _team, EnumTable.EClassType _type) // 모든 병과 인원 수 정보 갱신
        {
            TeamInfo teamInfo = PluginManager.teamInfo;
            string text = "";
            byte index = 0;
            var calssInfoList = new List<ClassInfo>();
            string textName = "";
            switch (_type)
            {
                case EnumTable.EClassType.infantary: // 보병 병과 정보 갱신
                    if (_team) calssInfoList = teamInfo.team_0_ClassInf;
                    else calssInfoList = teamInfo.team_1_ClassInf;
                    textName = "T_SelectClassCount_";
                    break;
                case EnumTable.EClassType.driver: // 운전병 병과 정보 갱신
                    if (_team) calssInfoList = teamInfo.team_0_ClassDriver;
                    else calssInfoList = teamInfo.team_1_ClassDriver;
                    textName = "T_SelectCrewCount_";
                    break;
            }
            foreach (var classInfo in calssInfoList)
            {
                if (classInfo.presetInfo.playerMax == 0) // 유저 수 제한이 없다면
                {
                    text = $"{classInfo.playerCount}";
                    EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClassIcon_{index}", $"{PluginManager.icons["player"]}");
                }
                else // 유저 수 제한이 있다면
                {
                    text = $"{classInfo.playerCount}/{classInfo.presetInfo.playerMax}";
                    if (classInfo.playerCount >= classInfo.presetInfo.playerMax) EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClassIcon_{index}", $"{PluginManager.icons["lock"]}");
                    else EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClassIcon_{index}", $"{PluginManager.icons["player"]}");
                }
                EffectManager.sendUIEffectText(47, _tc, false, $"{textName}{index}", text);
                index++;
            }
        }
        public static void RefreshUIClassPlayerCountToEveryone(bool _team, byte _index, EnumTable.EClassType _type) // 같은팀 유저에게 한개 인덱스의 병과 인원 수 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.team != _team || pc.uIState != EnumTable.EPlayerUIState.Loadout) continue;

                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIClassPlayerCount(tc, _team, _index, _type);
            }
        }
        public static void RefreshUIClassPlayerCount(ITransportConnection _tc, bool _team, byte _index, EnumTable.EClassType _type) // 한개 인덱스의 병과 인원 수 정보 갱신
        {
            TeamInfo teamInfo = PluginManager.teamInfo;
            string text = "";
            var classInfoList = new List<ClassInfo>();
            string textName = "";
            switch (_type)
            {
                case EnumTable.EClassType.infantary: // 보병 병과 정보 갱신
                    if (_team) classInfoList = teamInfo.team_0_ClassInf;
                    else classInfoList = teamInfo.team_1_ClassInf;
                    textName = "T_SelectClassCount_";
                    break;
                case EnumTable.EClassType.driver: // 운전병 병과 정보 갱신
                    if (_team) classInfoList = teamInfo.team_0_ClassDriver;
                    else classInfoList = teamInfo.team_1_ClassDriver;
                    textName = "T_SelectCrewCount_";
                    break;
            }
            if (classInfoList[_index].presetInfo.playerMax == 0)
            {
                text = $"{classInfoList[_index].playerCount}";
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClassIcon_{_index}", $"{PluginManager.icons["player"]}");
            }
            else
            {
                text = $"{classInfoList[_index].playerCount}/{classInfoList[_index].maxPlayerCount}";
                if (classInfoList[_index].playerCount >= classInfoList[_index].presetInfo.playerMax) EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClassIcon_{_index}", $"{PluginManager.icons["lock"]}");
                else EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectClassIcon_{_index}", $"{PluginManager.icons["player"]}");
            }
            EffectManager.sendUIEffectText(47, _tc, false, $"{textName}{_index}", text);
        }
        public static void RefreshSupplyCost(PlayerInfo _playerInfo, PlayerTeamRecordInfo _pTeamRecordInfo)
        {
            if (_playerInfo.classIndex == byte.MaxValue) return;
            ClassPresetTable classPreset = null;
            switch (_playerInfo.classType)
            {
                case EnumTable.EClassType.infantary:
                    classPreset = _playerInfo.team ? PluginManager.teamInfo.team_0_ClassInf[_playerInfo.classIndex].presetInfo : PluginManager.teamInfo.team_1_ClassInf[_playerInfo.classIndex].presetInfo;
                    break;
                case EnumTable.EClassType.driver:
                    classPreset = _playerInfo.team ? PluginManager.teamInfo.team_0_ClassDriver[_playerInfo.classIndex].presetInfo : PluginManager.teamInfo.team_1_ClassDriver[_playerInfo.classIndex].presetInfo;
                    break;
            }
            _playerInfo.supplyPoint_Max = (ushort)(classPreset.supplyPoint + _pTeamRecordInfo.level);

        }
    }
}

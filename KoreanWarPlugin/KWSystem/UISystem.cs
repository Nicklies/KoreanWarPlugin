using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.API;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using KoreanWarPlugin.Configuration;
using KoreanWarPlugin.Configuration.Preset;
using UnityEngine;
using KoreanWarPlugin.Info;
using KoreanWarPlugin.Data;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.KWSystem
{
    public class UISystem // 일반적인 기능 시스템
    {
        public static void PlayerJoinStart(UnturnedPlayer _uPlayer) // 처음 서버 입장 시 실행되는 함수
        {
            // 스폰으로 이동
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            _uPlayer.Player.teleportToLocationUnsafe(configuration.spawnPos, configuration.spawnRot);
            // 모든 기본 UI 비활성화
            _uPlayer.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowFood);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowHealth);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowOxygen);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowStamina);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowVirus);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowWater);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowDeathMenu);
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowVehicleStatus);
            // 로비 UI 활성화
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffect(4700, 47, tc, false);
            LoadImage(tc);
            // 접속 했을때 해당 유저 정보가 이미 있다면 제거하기
            if (PluginManager.teamInfo.playerInfoList.ContainsKey(_uPlayer.CSteamID)) PluginManager.teamInfo.playerInfoList.Remove(_uPlayer.CSteamID);
        }
        public static void PlayerJoinFinish(Player _player) // 로딩 완료 후 입장 시 실행되는 함수
        {
            PlayerComponent pc = _player.GetComponent<PlayerComponent>();
            pc.isEnterFinished = true;
            // 로비 UI 활성화
            ITransportConnection tc = _player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffectVisibility(47, tc, false, "Section_PreLoad", false);
            // 팀 선택 UI 정보 제공
            EffectManager.sendUIEffectImageURL(47, tc, false, $"I_Team_0", PluginManager.teamInfo.teamPreset_0.teamImageUrl);
            EffectManager.sendUIEffectImageURL(47, tc, false, $"I_Team_1", PluginManager.teamInfo.teamPreset_1.teamImageUrl);
            EffectManager.sendUIEffectText(47, tc, false, $"T_TeamName_0", PluginManager.teamInfo.teamPreset_0.teamName);
            EffectManager.sendUIEffectText(47, tc, false, $"T_TeamName_1", PluginManager.teamInfo.teamPreset_1.teamName);
            // 거점 이펙트 갱신
            for (int i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                Guid effectGuid = Guid.Parse(PluginManager.instance.Configuration.Instance.objectiveEffectGuid[i]);
                TriggerEffectParameters triggerEffect = new TriggerEffectParameters(effectGuid);
                triggerEffect.position = PluginManager.roundInfo.objectives[i].position;
                triggerEffect.SetUniformScale(1f);
                triggerEffect.SetRelevantPlayer(_player.channel.GetOwnerTransportConnection());
                EffectManager.triggerEffect(triggerEffect);
            }
            // 접속 인원 수 갱신
            PluginManager.roundInfo.playerCount++;
            if (PluginManager.instance.isRoundStart) { SetUIState_TeamSelection(_player, tc); }
            else 
            {
                SetUIState_RoundEnd(_player, tc);
                PluginManager.roundInfo.votePlayerCount++;
                RoundSystem.RefreshUIVotePlayerCountToEveryone();
            }
            if (PluginManager.roundInfo.roundType == ERoundType.Free)
            {
                if (PluginManager.roundInfo.playerCount >= PluginManager.instance.Configuration.Instance.freeModeReadyCount)
                {
                    if (!PluginManager.roundInfo.isFreeModeReady)
                    {
                        PluginManager.roundInfo.isFreeModeReady = true;
                        PluginManager.instance.freeModeEnd = PluginManager.instance.StartCoroutine(RoundSystem.Cor_FreeModeEnd());
                        RoundSystem.RefreshUIFreeModeInfoToEveryone();
                    }
                }
            }
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
        }
        public static void LoadImage(ITransportConnection _tc) // 서버 입장 후 모든 이미지 로드
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            foreach (string iconUrl in PluginManager.icons.Values) // 아이콘 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", iconUrl); // 아이콘
            }
            foreach (TeamPresetTable preset in configuration.teamPresets) // 팀 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.teamImageUrl);
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.teamIconUrl);
                foreach (LevelPreset level in preset.levelPresets) { EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", level.iconUrl); } // 계급 아이콘
            }
            foreach (WeaponInfoPreset preset in configuration.weaponInfoPresets)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 무기 아이콘
            }
            foreach (ClassPresetTable preset in configuration.classPresets) // 병과 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 병과 아이콘
            }
            foreach (PrimaryPresetTable preset in configuration.primaryPresets) // 주무기 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 주무기 아이콘
            }
            foreach (LoadoutTable preset in configuration.attachmentPresets) // 부착물 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 부착물 아이콘
            }
            foreach (SecondaryPresetTable preset in configuration.secondaryPresets) // 보조무기 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 보조무기 아이콘
            }
            foreach (LoadoutTable preset in configuration.explosivePresets) // 폭팔물 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 폭팔물 아이콘
            }
            foreach (EquipmentPresetTable preset in configuration.equipmentPresets) // 장구류 정보
            {
                foreach (ClothPresetTable cloth in preset.clothPresetList) EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", cloth.iconUrl); // 장구류 아이콘
            }
            foreach (LoadoutTable preset in configuration.utilityPresets) // 특수장비 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 특수장비 아이콘
            }
            foreach (VehicleTypePresetTable preset in configuration.vehicleTypePresets) // 차종 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 차종 아이콘
            }
            foreach (VehiclePresetTable preset in configuration.vehiclePresets) // 차량 정보
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 차량 아이콘
                foreach (SeatPresetTable seat in preset.seats) { EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", seat.iconUrl); } // 좌석 아이콘
            }
            foreach (MapPreset preset in configuration.mapPresets)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.mapIconUrl); // 맵 아이콘
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.mapImageUrl); // 맵 이미지
            }
            foreach (GameModePreset preset in configuration.gameModePresets)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 게임모드 아이콘
            }
            foreach (MagazineInfoPreset preset in configuration.magazineInfoPresets)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, "I_ImageLoading", preset.iconUrl); // 탄창 아이콘
            }
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_PreLoad", PluginManager.icons["Approve"]); // 확인용 아이콘 보내기
        }
        public static async void OnTeamJoinRequestEnd(CSteamID _cSteamID, bool _team, bool _success) // 플레이어에게 팀을 배정하는 함수
        {
            if (!PluginManager.instance.isRoundStart) return;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(_cSteamID);
            ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
            if (!_success) 
            {
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
                return; 
            }
            CSteamID groupID = _team ? PluginManager.teamInfo.team_0.groupID : PluginManager.teamInfo.team_1.groupID; // 배정될 팀의 아이디 참조
            if (!uPlayer.Player.quests.ServerAssignToGroup(groupID, EPlayerGroupRank.MEMBER, true)) // 그룹에 배정 시도 후 실패하면 리턴
            {
                SendPopUpInfo(tc, "팀 배정에 실패하였습니다. 지속적으로 접속에 실패 할 경우 관리자에게 문의해주세요.");
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
                return;
            }
            PlayerComponent pc = uPlayer.GetComponent<PlayerComponent>();
            PlayerData data = PluginManager.playerDatabase.FindData(uPlayer.CSteamID);
            PlayerInfo playerInfo = new PlayerInfo(uPlayer, data, _team);
            if (PluginManager.teamInfo.playerInfoList.ContainsKey(uPlayer.CSteamID)) PluginManager.teamInfo.playerInfoList.Remove(uPlayer.CSteamID);
            PluginManager.teamInfo.playerInfoList.Add(uPlayer.CSteamID, playerInfo); // 유저 정보 생성
            if (!PluginManager.teamInfo.playerRecordInfoList.ContainsKey(uPlayer.CSteamID)) // 유저 기록 정보가 없다면 생성
                PluginManager.teamInfo.playerRecordInfoList.Add(uPlayer.CSteamID, new PlayerRecordInfo(uPlayer.DisplayName, _team));
            if (_team) PluginManager.teamInfo.team_0_PlayerCount++;
            else PluginManager.teamInfo.team_1_PlayerCount++;
            
            await LoadAvatar(uPlayer, playerInfo);
            pc.isJoinedTeam = true;
            pc.team = _team;
            PlayerRecordInfo pRecordInfo = PluginManager.teamInfo.playerRecordInfoList[uPlayer.CSteamID];
            PlayerTeamRecordInfo pTeamRecordInfo = _team ? pRecordInfo.team_0_RecordInfo : pRecordInfo.team_1_RecordInfo;
            pRecordInfo.lastSelectedTeam = _team;
            pTeamRecordInfo.isActive = true;
            EffectManager.sendUIEffectImageURL(47, tc, false, "I_Map", $"{PluginManager.roundInfo.currentMapPreset.mapImageUrl}"); // 지도 갱신
            ClassSystem.LoadClassInfo(tc, _team); // 병과 정보 갱신
            PluginManager.teamInfo.UpdateClassMaxPlayerCount(); // 유저 수에 따라 병과 최대인원 수 갱신
            RefreshUITeamPlayersUIToEveryone(_team); // 팀내 유저 인원 정보 갱신
            RoundSystem.RefreshUIRoundInfo(tc, _team); // 라운드 정보 갱신
            RoundSystem.ActiveObjectiveInfo(tc, _team);
            DeploySystem.SetUpDeployMarker_Static(tc, _team, playerInfo); // 정적 스폰 마커 세팅
            SetUIState_Loadout(uPlayer, tc, data, playerInfo, pTeamRecordInfo);
            EffectManager.sendUIEffectVisibility(47, tc, false, "Trigger_RemoveMarkerSelect", true);
            EffectManager.sendUIEffectVisibility(47, tc, false, "L_Loading", false);
        }
        public static void SetUIState_TeamSelection(Player _player, ITransportConnection _tc) // 팀 선택 UI 상태로 변환
        {
            PlayerComponent pc = _player.GetComponent<PlayerComponent>();
            pc.localUIState = EnumTable.EPlayerUIState.TeamSelect;
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_SetUIState_Team", true);
            RefreshUITeamPlayers(_tc, true);
            RefreshUITeamPlayers(_tc, false);
            RefreshUITeamLimitState(_tc);
        }
        public static void SetUIState_Loadout(UnturnedPlayer _uPlayer, ITransportConnection _tc,PlayerData data,PlayerInfo _playerInfo, PlayerTeamRecordInfo _pTeamRecordInfo) // 로드아웃 UI 상태로 변환
        {
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            pc.localUIState = EnumTable.EPlayerUIState.Loadout;
            _uPlayer.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            if (pc.localChatType == EnumTable.EChatType.Vehicle) ChangeChatType(pc, _tc);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_SetUIState_Loadout", true);
            ClassSystem.SetClassType_Infantary(pc, _tc);
            ClassSystem.RefreshUIClassPlayerCountAll(_tc, _playerInfo.team, EnumTable.EClassType.infantary); // 보병 인원 수 갱신
            ClassSystem.RefreshUIClassPlayerCountAll(_tc, _playerInfo.team, EnumTable.EClassType.driver); // 운전병 인원 수 갱신
            ClassSystem.RefreshSupplyCost(_playerInfo, _pTeamRecordInfo);
            VehicleGroupSystem.RefreshVehicleType(_tc, _playerInfo.team, _uPlayer); // 차종 정보 갱신
            VehicleGroupSystem.RefreshVehicleTypeCountAll(_tc, _playerInfo.team); // 차종별 배치된 차량 개수 정보 갱신
            LevelSystem.RefreshUILevel(_pTeamRecordInfo, _tc, _playerInfo.team); // 레벨 정보 갱신
            LevelSystem.RefreshUIExperience(_pTeamRecordInfo, _tc);
            LoadoutSystem.RefreshUICredit(_tc, data);
            LoadoutSystem.RefreshDeployCost(_playerInfo, _tc);
            VehicleGroupSystem.RefreshUIVehicleGroupAll(_tc, ushort.MaxValue, _playerInfo.team); // 팀의 차량그룹 정보 갱신
            VehicleGroupSystem.RefreshUIVehicleDeployAll(_tc, _playerInfo.team); // 팀의 배치된 차량 정보 갱신
            DeploySystem.ActiveDeployMarker_ObjectiveAll(_tc, _playerInfo.team, _playerInfo); // 거점 스폰 체크 및 활성화
            DeploySystem.ActiveDeployMarker_VehicleAll(_tc, _playerInfo.team, _playerInfo); // 차량 스폰 체크 및 활성화
            DeploySystem.RefreshUIMapObjectiveTeamAll(_tc, _playerInfo.team); // 지도상 거점의 점령상태 갱신
            RefreshDeployState(_playerInfo, _tc);
        }
        public static void SetUIState_InGame(UnturnedPlayer _uPlayer, ITransportConnection _tc) // 인게임 UI 상태로 변환
        {
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            pc.killLogCount = 0;
            pc.scoreGainCount = 0;
            pc.localUIState = EnumTable.EPlayerUIState.InGame;
            _uPlayer.Player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_SetUIState_Ingame", true);
            IngameSystem.UpdateHealthBar(_uPlayer, _uPlayer.Health);
        }
        public static void SetUIState_Death(UnturnedPlayer _uPlayer, ITransportConnection _tc) // 사망 UI 상태로 변환
        {
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            pc.localUIState = EnumTable.EPlayerUIState.InGame;
            _uPlayer.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_SetUIState_Death", true);
        }
        public static void SetUIState_RoundEnd(Player _player, ITransportConnection _tc) // 사망 UI 상태로 변환
        {
            PlayerComponent pc = _player.GetComponent<PlayerComponent>();
            pc.localUIState = EPlayerUIState.RoundEnd;
            _player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            if (!pc.isJoinedTeam || PluginManager.roundInfo.roundType == ERoundType.Free) EffectManager.sendUIEffectText(47, _tc, false, "T_Victory", "게임종료");
            else
            {
                if (pc.team == PluginManager.roundInfo.winner) { EffectManager.sendUIEffectText(47, _tc, false, "T_Victory", "승리"); }
                else { EffectManager.sendUIEffectText(47, _tc, false, "T_Victory", "패배"); }
            }
            RoundSystem.ActiveUIRoundEndInfo(_tc);
            if (!PluginManager.instance.isVoteEnd) 
            {
                RoundSystem.RefreshUIVotePlayerCount(_tc);
                RoundSystem.RefreshUIVoteTimer(_tc);
            }
            RoundSystem.RefreshUIScoreboard(_tc);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_SetUIState_RoundEnd", true);
        }
        static async Task LoadAvatar(UnturnedPlayer _uPlayer, PlayerInfo _playerInfo)
        {
            using (var client = new WebClient())
            {
                var result = await client.DownloadStringTaskAsync($"https://steamcommunity.com/profiles/{_uPlayer.CSteamID}/?xml=1");
                var avatar = Regex.Match(result, @"<avatarFull><!\[CDATA\[(.*)\]\]></avatarFull>").Groups[1].Value;
                _playerInfo.avatarUrl = avatar;
            }
        }
        public static void LeaveTeam(UnturnedPlayer _uPlayer, PlayerInfo _playerInfo, ITransportConnection _tc, bool _team) // 팀에서 나간 후 팀 선택창으로 이동하는 함수
        {
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            PlayerTeamRecordInfo pTeamRecordInfo = _team ? PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[_uPlayer.CSteamID].team_1_RecordInfo;
            if (_playerInfo.classPrestIndex != ushort.MaxValue) // 기존에 선택된 병과가 있다면 선택 해제
            {
                switch (_playerInfo.classType)
                {
                    case EnumTable.EClassType.infantary:
                        List<ClassInfo> classInfList = _team ? PluginManager.teamInfo.team_0_ClassInf : PluginManager.teamInfo.team_1_ClassInf;
                        classInfList[_playerInfo.classIndex].playerCount--;
                        break;
                    case EnumTable.EClassType.driver:
                        List<ClassInfo> classDriverList = _team ? PluginManager.teamInfo.team_0_ClassDriver : PluginManager.teamInfo.team_1_ClassDriver;
                        classDriverList[_playerInfo.classIndex].playerCount--;
                        break;
                }
                ClassSystem.RefreshUIClassPlayerCountToEveryone(_team, _playerInfo.classIndex, _playerInfo.classType);
            }
            if (_playerInfo.vGroupInstanceID != ushort.MaxValue) // 차량 그룹에 속해있는 경우 퇴장
            {
                PluginManager.teamInfo.RemovePlayerFromVehicleGroup(_uPlayer.CSteamID, _playerInfo, _team);
            }
            if (_team) PluginManager.teamInfo.team_0_PlayerCount--;
            else PluginManager.teamInfo.team_1_PlayerCount--;
            // 플레이어 정보 초기화
            PluginManager.teamInfo.RemovePlayer(_uPlayer, _team);
            _uPlayer.Player.quests.leaveGroup(true);
            _playerInfo.classPrestIndex = ushort.MaxValue;
            _playerInfo.classIndex = 255;
            pc.isJoinedTeam = false;
            pTeamRecordInfo.isActive = false;
            // 병과 정보 갱신
            PluginManager.teamInfo.UpdateClassMaxPlayerCount();
            // UI 변경
            SetUIState_TeamSelection(_uPlayer.Player, _tc);
            RefreshUITeamPlayersUIToEveryone(_team);
        }
        public static void SendChat(UnturnedPlayer _uPlayer, PlayerComponent _pc,PlayerInfo _playerInfo) // 채팅을 전송하는 함수
        {
            if (string.IsNullOrWhiteSpace(_pc.chatText)) return;
            string chat = $"{_uPlayer.DisplayName}: {_pc.chatText}";
            switch (_pc.localChatType)
            {
                case EnumTable.EChatType.Team:
                    List<SteamPlayer> steamPlayers = Provider.clients;
                    foreach (SteamPlayer steamPlayer in steamPlayers)
                    {
                        PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                        if (pc.team != _playerInfo.team) continue;
                        ChatManager.serverSendMessage("[G] " + chat, Color.white, _uPlayer.SteamPlayer(), steamPlayer, EChatMode.GLOBAL, _uPlayer.SteamProfile.AvatarIcon.ToString(), true);
                    }
                    Rocket.Core.Logging.Logger.Log("[Group] " + chat);
                    break;
                case EnumTable.EChatType.All:
                    ChatManager.serverSendMessage(chat, Color.white, _uPlayer.SteamPlayer(), null, EChatMode.GLOBAL, _uPlayer.SteamProfile.AvatarIcon.ToString(), true);
                    Rocket.Core.Logging.Logger.Log("[World] " + chat);
                    break;
                case EnumTable.EChatType.Vehicle:
                    VehicleGroupInfo vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(_playerInfo.vGroupInstanceID, _playerInfo.team);
                    foreach (SeatInfo seat in vGroupInfo.seats)
                    {
                        if (seat.cSteamID == CSteamID.NonSteamGS) continue;
                        ChatManager.serverSendMessage("[V] " + chat, Color.white, _uPlayer.SteamPlayer(), UnturnedPlayer.FromCSteamID(seat.cSteamID).SteamPlayer(), EChatMode.GLOBAL, _uPlayer.SteamProfile.AvatarIcon.ToString(), true);
                    }
                    Rocket.Core.Logging.Logger.Log("[Vehicle] " + chat);
                    break;
            }
            _pc.chatText = "";
        }
        public static void SendPopUpInfo(ITransportConnection _tc, string _text)
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_PopUpInfo", _text);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "L_PopUpInfo", true);
        }
        public static void ChangeChatType(PlayerComponent _pc,ITransportConnection _tc)
        {
            switch (_pc.localChatType)
            {
                case EnumTable.EChatType.Team:
                    _pc.localChatType = EnumTable.EChatType.Team;
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_ChatType", "Team");
                    break;
                case EnumTable.EChatType.All:
                    _pc.localChatType = EnumTable.EChatType.All;
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_ChatType", "All");
                    break;
                case EnumTable.EChatType.Vehicle:
                    _pc.localChatType = EnumTable.EChatType.Vehicle;
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_ChatType", "Vehicle");
                    break;
            }
        }
        public static void RefreshUITeamPlayersUIToEveryone(bool _team) // 모든 유저에게 팀 화면에서 보이는 유저 수 및 이름 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                PlayerComponent pc = player.Player.GetComponent<PlayerComponent>();
                if (pc.localUIState == EnumTable.EPlayerUIState.TeamSelect)
                {
                    ITransportConnection tc = player.Player.channel.GetOwnerTransportConnection();
                    RefreshUITeamPlayers(tc, _team);
                    RefreshUITeamLimitState(tc);
                }
            }
        }
        public static void RefreshUITeamPlayers(ITransportConnection _tc, bool _team) // 팀 화면에서 보이는 유저 수 및 이름 갱신
        {
            if (_team)
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemovePlayerNameTeam_0", true);
                EffectManager.sendUIEffectText(47, _tc, false, $"T_TeamCount_0", PluginManager.teamInfo.team_0_PlayerCount.ToString());
                PlayerInfo[] team_0_Infos = PluginManager.teamInfo.playerInfoList.Values.Where(x => x.team == _team).ToArray();
                for (int i = 0; i < team_0_Infos.Length; i++)
                {
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerList_0_{i}", team_0_Infos[i].displayName);
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"T_PlayerList_0_{i}", true);
                }
            }
            else
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemovePlayerNameTeam_1", true);
                EffectManager.sendUIEffectText(47, _tc, false, $"T_TeamCount_1", PluginManager.teamInfo.team_1_PlayerCount.ToString());
                PlayerInfo[] team_1_Infos = PluginManager.teamInfo.playerInfoList.Values.Where(x => x.team == _team).ToArray();
                for (int i = 0; i < team_1_Infos.Length; i++)
                {
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerList_1_{i}", team_1_Infos[i].displayName);
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"T_PlayerList_1_{i}", true);
                }
            }
        }
        public static void RefreshUITeamLimitState(ITransportConnection _tc) // 팀 화면에서 팀 선택 제한 상태 갱신 
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            if (PluginManager.teamInfo.team_0_PlayerCount - PluginManager.teamInfo.team_1_PlayerCount >= configuration.teamRestrictCount)
                EffectManager.sendUIEffectVisibility(47, _tc, false, "P_TeamBlock_0", true);
            else EffectManager.sendUIEffectVisibility(47, _tc, false, "P_TeamBlock_0", false);

            if (PluginManager.teamInfo.team_1_PlayerCount - PluginManager.teamInfo.team_0_PlayerCount >= configuration.teamRestrictCount)
                EffectManager.sendUIEffectVisibility(47, _tc, false, "P_TeamBlock_1", true);
            else EffectManager.sendUIEffectVisibility(47, _tc, false, "P_TeamBlock_1", false);
        }
        public static void RefreshDeployState(PlayerInfo _playerInfo, ITransportConnection _tc) // 유저의 상태 점검 후 전투배치 가능 여부를 갱신
        {
            if (_playerInfo.supplyCost > _playerInfo.supplyPoint_Max) // 이용 가능한 보급 포인트를 초과한경우 전투배치 금지
            {
                RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.MaxSupply);
                _playerInfo.isDeployable = false;
                return;
            }
            PlayerData data = PluginManager.playerDatabase.FindData(_playerInfo.cSteamID);
            if (_playerInfo.creditCost > data.credit)
            {
                RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.MaxCredit);
                _playerInfo.isDeployable = false;
                return;
            }
            if (_playerInfo.vGroupInstanceID != ushort.MaxValue) // 유저가 차량그룹에 속한 경우
            {
                VehicleGroupInfo vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(_playerInfo.vGroupInstanceID, _playerInfo.team);
                if (vGroupInfo.leaderID != _playerInfo.cSteamID) // 리더가 아니면 전투배치 금지
                {
                    RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.NotLeader);
                    _playerInfo.isDeployable = true;
                    return;
                }
                else // 리더일 때
                {
                    if(vGroupInfo.crewCount < vGroupInfo.vehicleTypePreset.crewMinCount)
                    {
                        RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.vGroupMinPlayers);
                        _playerInfo.isDeployable = false;
                        return;
                    }
                }
            }
            if (_playerInfo.spawnIndex == byte.MaxValue)
            {
                RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.SpawnPointSelect);
                _playerInfo.isDeployable = false;
                return;
            }
            RefreshUIDeployBlockState(_tc, EnumTable.EReadyBlockType.None);
            _playerInfo.isDeployable = true;
        }
        public static void RefreshUIDeployBlockState(ITransportConnection _tc,EnumTable.EReadyBlockType _type) // 전투배치 금지 여부를 갱신
        {
            switch (_type)
            {
                case EnumTable.EReadyBlockType.None:
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", false);
                    break;
                case EnumTable.EReadyBlockType.Loading:
                    EffectManager.sendUIEffectText(47, _tc, false, "T_DeployBlock", "변경사항 적용중...");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", true);
                    break;
                case EnumTable.EReadyBlockType.NotLeader:
                    EffectManager.sendUIEffectText(47, _tc, false, "T_DeployBlock", "차량 지휘관이 배치가능");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", true);
                    break;
                case EnumTable.EReadyBlockType.MaxSupply:
                    EffectManager.sendUIEffectText(47, _tc, false, "T_DeployBlock", "보급 포인트 최대치 초과");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", true);
                    break;
                case EnumTable.EReadyBlockType.MaxCredit:
                    EffectManager.sendUIEffectText(47, _tc, false, "T_DeployBlock", "크레딧 한도 초과");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", true);
                    break;
                case EnumTable.EReadyBlockType.vGroupMinPlayers:
                    EffectManager.sendUIEffectText(47, _tc, false, "T_DeployBlock", "차량 인원수 부족");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", true);
                    break;
                case EnumTable.EReadyBlockType.SpawnPointSelect:
                    EffectManager.sendUIEffectText(47, _tc, false, "T_DeployBlock", "선택된 스폰 포인트 없음");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_DeployBlock", true);
                    break;
            }
        }
    }
}
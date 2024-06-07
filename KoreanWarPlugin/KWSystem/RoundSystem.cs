using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Info;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using System.Threading.Tasks;

namespace KoreanWarPlugin.KWSystem
{
    public class RoundSystem
    {
        public static void RoundEnd()
        {
            PluginManager.instance.isRoundStart = false;
            if (PluginManager.roundInfo.playerCount < PluginManager.instance.Configuration.Instance.freeModeReadyCount)
            {
                PluginManager.instance.isVoteEnd = true;
                PluginManager.roundInfo.roundType = EnumTable.ERoundType.Free;
                PluginManager.roundInfo.currentMapIndex = PluginManager.instance.Configuration.Instance.freeModeMapIndex;
                PluginManager.roundInfo.currentMapPreset = PluginManager.instance.Configuration.Instance.mapPresets[PluginManager.roundInfo.currentMapIndex];
                PluginManager.instance.StartCoroutine_RoundStart();
            }
            else
            {
                PluginManager.instance.isVoteEnd = false;
                PluginManager.roundInfo.ReloadVote();
            }
            PluginManager.teamInfo.OnRoundEnd();
            List<PlayerTeamRecordInfo> team_0_resultList = new List<PlayerTeamRecordInfo>();
            List<PlayerTeamRecordInfo> team_1_resultList = new List<PlayerTeamRecordInfo>();
            foreach (var recordInfoDir in PluginManager.teamInfo.playerRecordInfoList)
            {
                if(recordInfoDir.Value.team_0_RecordInfo.isActive) team_0_resultList.Add(recordInfoDir.Value.team_0_RecordInfo);
                if (recordInfoDir.Value.team_1_RecordInfo.isActive) team_1_resultList.Add(recordInfoDir.Value.team_1_RecordInfo);
            }
            PluginManager.teamInfo.scoreBoardInfo.team_0_Results = team_0_resultList.OrderByDescending(x => x.score).ToArray();
            PluginManager.teamInfo.scoreBoardInfo.team_1_Results = team_1_resultList.OrderByDescending(x => x.score).ToArray();
            byte playerCount = 0;
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.isEnterFinished) playerCount++;
            }
            PluginManager.roundInfo.votePlayerCount = playerCount;
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isEnterFinished) continue;
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                uPlayer.Player.equipment.dequip();
                uPlayer.Player.movement.forceRemoveFromVehicle();
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                UISystem.SetUIState_RoundEnd(uPlayer.Player, tc);
            }
        }
        public static void ActiveUIRoundEndInfo(ITransportConnection _tc) // 투표창 UI 처음 등장할때 실행
        {
            // 투표 종료전이면 투표창, 종료후면 정보창 띄우기
            if (!PluginManager.instance.isVoteEnd)
            {
                // 맵 투표, 게임 모드 투표 갱신
                RefreshUIVoteMapInfo(_tc); // 맵 투표 정보 불러오기
                RefreshUIVoteGamemodeInfo(_tc); // 게임모드 투표 정보 불러오기
                RefreshUIVoteCountInfoAll(_tc); // 투표한 인원 수 정보 불러오기
                EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Vote", true);
            }
            else 
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, "P_RoundInfo", true);
            }
        }
        public static void RefreshUIVoteMapInfoToEveryone() // 모든 사람에게 맵 투표 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIVoteMapInfo(tc);
            }
        }
        public static void RefreshUIVoteMapInfo(ITransportConnection _tc) // 맵 투표 정보 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveMapVote", true);
            if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.Free) return; // 자유모드일때는 게임모드를 먼저 선택하게 하기위해 맵을 보여주지 않게 함

            GameModePreset gameMode = PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType];
            for (byte i = 0; i < gameMode.maps.Length; i++)
            {
                MapPreset mapPreset = PluginManager.instance.Configuration.Instance.mapPresets[gameMode.maps[i]];
                EffectManager.sendUIEffectText(47, _tc, false, $"T_MapVote_Name_{i}", $"{mapPreset.name}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_MapVote_Player_{i}", $"{mapPreset.playerCount}명 이상");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_MapVote_GameMode_{i}", $"{gameMode.name}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_MapVote_MapSize_{i}", $"{mapPreset.mapSize}");
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_MapVote_{i}", $"{mapPreset.mapIconUrl}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_MapVote_{i}", true);
            }
            // 맵 정보 갱신 할때 맵 선택 꺼지게 하고 게임모드 투표 방지 갱신
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveVoteSelect", true);
            RefreshUIVoteGamemodeBlock(_tc);
        }
        public static void RefreshUIVoteGamemodeInfo(ITransportConnection _tc)
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveGameModeVote", true);
            for (byte i = 0; i < PluginManager.instance.Configuration.Instance.gameModePresets.Length; i++) // 0 은 자유모드이기 때문에 스킵하고 다음 게임모드부터 불러옴
            {
                GameModePreset gameMode = PluginManager.instance.Configuration.Instance.gameModePresets[i];
                EffectManager.sendUIEffectText(47, _tc, false, $"T_GameModeVoteName_{i}", $"{gameMode.name}");
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_GameModeVote_{i}", $"{gameMode.iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_GameModeVotePlayers_{i}", $"최소 {gameMode.playerCount}명 권장");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_GameModeVoteDes_{i}", $"{gameMode.description}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_GameModeVote_{i}", true);
            }
        }
        public static void RefreshUIVoteCountInfoAllToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIVoteCountInfoAll(tc);
            }
        }
        public static void RefreshUIVoteCountInfoAll(ITransportConnection _tc)
        {
            for (byte i = 0; i < PluginManager.roundInfo.voteCount.Length; i++) 
            {
                RefreshUIVoteCountInfo(_tc, i);
            }
        }
        public static void RefreshUIVoteCountInfoToEveryone(byte _index)
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIVoteCountInfo(tc, _index);
            }
        }
        public static void RefreshUIVoteCountInfo(ITransportConnection _tc, byte _index)
        {
            if (_index < 6)
            {
                if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.Free) return;
                byte mapCount = (byte)PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType].maps.Length;
                if (mapCount < _index) return; // 현재 게임모드가 가진 맵 개수보다 인덱스가 높기 때문에 리턴
                EffectManager.sendUIEffectText(47, _tc, false, $"T_MapVote_Vote_{_index}", $"{PluginManager.roundInfo.voteCount[_index]}"); 
            }
            else { EffectManager.sendUIEffectText(47, _tc, false, $"T_GameModeVoteVote_{_index - 6}", $"{PluginManager.roundInfo.voteCount[_index]}"); }
        }
        public static void RefreshUIVoteTimerToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIVoteTimer(tc);
            }
        }
        public static void RefreshUIVoteTimer(ITransportConnection _tc)
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_Vote_Timer", $"{PluginManager.instance.voteTimer}");
        }
        public static void RefreshUIRoundStartTimerToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIRoundStartTimer(tc);
            }
        }
        public static void RefreshUIRoundStartTimer(ITransportConnection _tc)
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_Timer", $"{PluginManager.instance.voteTimer}");
        }
        public static void RefreshUIVoteGamemodeBlock(ITransportConnection _tc) // 게임모드 투표 방지 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveGamemodeBlock", true);
            for (byte i = 0; i < PluginManager.instance.Configuration.Instance.gameModePresets.Length; i++)
            {
                if((byte)PluginManager.roundInfo.roundType == i)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_GameModeVote_{i}Block", true);
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_GameModeVote_{i}Block", "선택됨");
                }
                else if (PluginManager.roundInfo.isGamemodeChanged)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_GameModeVote_{i}Block", true);
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_GameModeVote_{i}Block", "더 이상 변경불가");
                }
            }
        }
        public static void RefreshUIVotePlayerCountToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                RefreshUIVotePlayerCount(tc);
            }
        }
        public static void RefreshUIVotePlayerCount(ITransportConnection _tc) // 투표에 참여하고 있는 모든 유저 수 갱신
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_MapVote_Players", $"{PluginManager.roundInfo.votePlayerCount}");
        }
        public static void RefreshUIScoreboard(ITransportConnection _tc) // 스코어보드 정보 갱신
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "Trigger_RemoveRoundPlayerInfo", true);
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_RoundEnd_Team_0", $"{PluginManager.teamInfo.teamPreset_0.teamIconUrl}");
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_RoundEnd_Team_1", $"{PluginManager.teamInfo.teamPreset_1.teamIconUrl}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_RoundEnd_Team_0", $"{PluginManager.teamInfo.teamPreset_0.teamName}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_RoundEnd_Team_1", $"{PluginManager.teamInfo.teamPreset_1.teamName}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_MapName", $"{PluginManager.roundInfo.currentMapPreset.name}");
            if (PluginManager.roundInfo.roundType != EnumTable.ERoundType.Free)
                EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_GameMode", $"{PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType].name}");
            else EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_GameMode", $"자유모드");

            int index = 0;
            foreach (PlayerTeamRecordInfo pRecordInfo in PluginManager.teamInfo.scoreBoardInfo.team_0_Results)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_PlayerInfo_Rank_0_{index}", $"{PluginManager.teamInfo.teamPreset_0.levelPresets[pRecordInfo.level].iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Name_0_{index}", $"{pRecordInfo.name}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Kill_0_{index}", $"{pRecordInfo.killCount}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Death_0_{index}", $"{pRecordInfo.deathCount}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Score_0_{index}", $"{pRecordInfo.score}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_PlayerInfo_0_{index}", true);
                index++;
            }
            index = 0;
            foreach (PlayerTeamRecordInfo pRecordInfo in PluginManager.teamInfo.scoreBoardInfo.team_1_Results)
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_PlayerInfo_Rank_1_{index}", $"{PluginManager.teamInfo.teamPreset_1.levelPresets[pRecordInfo.level].iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Name_1_{index}", $"{pRecordInfo.name}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Kill_1_{index}", $"{pRecordInfo.killCount}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Death_1_{index}", $"{pRecordInfo.deathCount}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_PlayerInfo_Score_1_{index}", $"{pRecordInfo.score}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_PlayerInfo_1_{index}", true);
                index++;
            }
        }
        public static void RefreshUIRoundInfoToEveryone() // 모두에게 라운드 정보 갱신
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam || pc.localUIState == EnumTable.EPlayerUIState.TeamSelect) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIRoundInfo(tc, pc.team);
            }
        }
        public static void RefreshUIRoundInfo(ITransportConnection _tc, bool _team) // 라운드 정보 갱신
        {
            if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.Free) return;
            int score_team0 = (int)(((float)PluginManager.roundInfo.team_0_score / (float)PluginManager.roundInfo.team_0_scoreMax) * 100);
            int score_team1 = (int)(((float)PluginManager.roundInfo.team_1_score / (float)PluginManager.roundInfo.team_1_scoreMax) * 100);
            string score_team0_str = "", score_team1_str = "";
            string scoreText_Team_0 = "", scoreText_Team_1 = "";
            // 섬멸전, 깃발 점령에만 사용
            if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.CaptureTheFlag || PluginManager.roundInfo.roundType == EnumTable.ERoundType.Annihilation)
            {
                if (score_team0 != 0) score_team0_str = "".PadLeft(score_team0 - 1) + ".";
                if (score_team1 != 0) score_team1_str = "".PadLeft(score_team1 - 1) + ".";
                scoreText_Team_0 = PluginManager.roundInfo.team_0_score.ToString();
                scoreText_Team_1 = PluginManager.roundInfo.team_1_score.ToString();
            }
            else if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.Battle)
            {
                if (PluginManager.roundInfo.deffenseTeam)
                {
                    scoreText_Team_0 = "방어";
                    scoreText_Team_1 = PluginManager.roundInfo.team_1_score.ToString();
                }
                else
                {
                    scoreText_Team_0 = PluginManager.roundInfo.team_0_score.ToString();
                    scoreText_Team_1 = "방어";
                }
            }
            if (_team)
            {
                EffectManager.sendUIEffectText(47, _tc, false, "TF_TeamPointA", $"{score_team0_str}");
                EffectManager.sendUIEffectText(47, _tc, false, "TF_TeamPointB", $"{score_team1_str}");
                EffectManager.sendUIEffectText(47, _tc, false, "T_TeamPointA", scoreText_Team_0);
                EffectManager.sendUIEffectText(47, _tc, false, "T_TeamPointB", scoreText_Team_1);
            }
            else
            {
                EffectManager.sendUIEffectText(47, _tc, false, "TF_TeamPointA", $"{score_team1_str}");
                EffectManager.sendUIEffectText(47, _tc, false, "TF_TeamPointB", $"{score_team0_str}");
                EffectManager.sendUIEffectText(47, _tc, false, "T_TeamPointA", $"{scoreText_Team_1}");
                EffectManager.sendUIEffectText(47, _tc, false, "T_TeamPointB", $"{scoreText_Team_0}");
            }
        }
        public static void ActiveObjectiveInfo(ITransportConnection _tc, bool _team)
        {
            if (PluginManager.roundInfo.roundType != EnumTable.ERoundType.Free) EffectManager.sendUIEffectVisibility(47, _tc, false, "L_RoundInfo", true);
            else
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, "L_RoundInfo_Free", true);
                RefreshUIFreeModeInfo(_tc);
            }
            // 팀 아이콘 갱신
            string team_A_Icon = _team ? PluginManager.teamInfo.teamPreset_0.teamIconUrl : PluginManager.teamInfo.teamPreset_1.teamIconUrl;
            string team_B_Icon = _team ? PluginManager.teamInfo.teamPreset_1.teamIconUrl : PluginManager.teamInfo.teamPreset_0.teamIconUrl;
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_TeamA", $"{team_A_Icon}");
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_TeamB", $"{team_B_Icon}");
            // 공방전, 깃발 점령전이면 거점 정보 활성화
            if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.CaptureTheFlag || PluginManager.roundInfo.roundType == EnumTable.ERoundType.Battle)
            {
                for (int i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_Objective_{i}", true);
                }
                RefreshObjectiveStateAll(_tc, _team);
                RefreshObjectiveInfoAll(_tc, _team);
                RefreshObjectiveAlarmAll(_tc);
            }
            // 섬멸전, 깃발 점령전이면 점수 정보 활성화
            if (PluginManager.roundInfo.roundType == EnumTable.ERoundType.Annihilation || PluginManager.roundInfo.roundType == EnumTable.ERoundType.CaptureTheFlag)
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, "P_TeamPointFillbar", true);
            }
        }
        public static void RefreshObjectInfoAllToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshObjectiveInfoAll(tc, pc.team);
            }
        }
        public static void RefreshObjectInfoToEveryone(byte _index)
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshObjectiveInfo(tc, pc.team, _index);
            }
        }
        public static void RefreshObjectiveInfoAll(ITransportConnection _tc, bool _team)
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++) { RefreshObjectiveInfo(_tc, _team, i); }
        }
        public static void RefreshObjectiveInfo(ITransportConnection _tc, bool _team,byte _index)
        {
            ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[_index];
            string team_0_Amount = "";
            string team_1_Amount = "";
            if (objectiveInfo.point <= 99) { if (_team) team_0_Amount = ".".PadLeft(99 - objectiveInfo.point); else team_1_Amount = ".".PadLeft(99 - objectiveInfo.point); }
            else if (objectiveInfo.point >= 101) { if (!_team) team_0_Amount = ".".PadLeft(objectiveInfo.point - 101); else team_1_Amount = ".".PadLeft(objectiveInfo.point - 101); }
            EffectManager.sendUIEffectText(47, _tc, false, $"T_Objective_{_index}_0", team_0_Amount);
            EffectManager.sendUIEffectText(47, _tc, false, $"T_Objective_{_index}_1", team_1_Amount);
        }
        public static void RefreshObjectiveStateAllToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshObjectiveStateAll(tc, pc.team);
            }
        }
        public static void RefreshObjectiveStateAll(ITransportConnection _tc, bool _team)
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++) { RefreshObjectiveState(_tc, _team, i); }
        }
        public static void RefreshObjectiveStateToEveryone(byte _index)
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshObjectiveState(tc, pc.team, _index);
            }
        }
        public static void RefreshObjectiveState(ITransportConnection _tc, bool _team, byte _index)
        {
            ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[_index];
            byte teamIndex = (byte)objectiveInfo.team;
            if(!_team)
            {
                if (teamIndex == 0) teamIndex = 2;
                else if (teamIndex == 2) teamIndex = 0;
            }
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_Objective_{_index}_{teamIndex}", true);
            if(objectiveInfo.locked) EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_ObjectiveLock_{_index}", true);
            else EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_ObjectiveLock_{_index}", false);
        }
        public static void RefreshObjectivetAlarmAllToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshObjectiveAlarmAll(tc);
            }
        }
        public static void RefreshObjectiveAlarmAll(ITransportConnection _tc)
        {
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++) { RefreshObjectiveAlarm(_tc, PluginManager.roundInfo.objectives[i].isCapturing, i); }
        }
        public static void RefreshObjectiveAlarmToEveryone(bool _capture, byte _index)
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (!pc.isJoinedTeam) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshObjectiveAlarm(tc, _capture, _index);
            }
        }
        public static void RefreshObjectiveAlarm(ITransportConnection _tc, bool _capture,byte _index)
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"A_Objective_{_index}", _capture);
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"A_ObjectivePos_{_index}", _capture);
        }
        public static void ActiveObjectiveHighlightInfo(ITransportConnection _tc,bool _team, byte _index)
        {
            string objectiveStr = "";
            switch (_index)
            {
                case 0:
                    objectiveStr = "A";
                    break;
                case 1:
                    objectiveStr = "B";
                    break;
                case 2:
                    objectiveStr = "C";
                    break;
                case 3:
                    objectiveStr = "D";
                    break;
                case 4:
                    objectiveStr = "E";
                    break;
                default:
                    break;
            }
            EffectManager.sendUIEffectText(47, _tc, false, "T_ObjectiveHightlight", objectiveStr);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "L_ObjectiveHighlight", true);
            RefreshObjectiveHighlightInfo(_tc, _team, _index);
            RefreshObjectiveHighlightState(_tc, _team, _index);
            RefreshObjectiveHighlightAlarm(_tc, _index);
        }
        public static void RefreshObjectiveHighlightInfo(ITransportConnection _tc, bool _team, byte _index)
        {
            ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[_index];
            string team_0_Amount = "";
            string team_1_Amount = "";
            if (objectiveInfo.point <= 99) { if (_team) team_0_Amount = ".".PadLeft(99 - objectiveInfo.point); else team_1_Amount = ".".PadLeft(99 - objectiveInfo.point); }
            else if (objectiveInfo.point >= 101) { if (!_team) team_0_Amount = ".".PadLeft(objectiveInfo.point - 101); else team_1_Amount = ".".PadLeft(objectiveInfo.point - 101); }
            EffectManager.sendUIEffectText(47, _tc, false, "TF_ObjectiveHighlight_0", team_0_Amount);
            EffectManager.sendUIEffectText(47, _tc, false, "TF_ObjectiveHighlight_1", team_1_Amount);
            int team_0_PlayerCount = _team ? objectiveInfo.team_0_Players.Count : objectiveInfo.team_1_Players.Count;
            int team_1_PlayerCount = _team ? objectiveInfo.team_1_Players.Count : objectiveInfo.team_0_Players.Count;
            EffectManager.sendUIEffectText(47, _tc, false, "T_ObjectiveHighlight_0", $"{team_0_PlayerCount}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_ObjectiveHighlight_1", $"{team_1_PlayerCount}");
        }
        public static void RefreshObjectiveHighlightState(ITransportConnection _tc, bool _team, byte _index)
        {
            ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[_index];
            byte teamIndex = (byte)objectiveInfo.team;
            if (!_team)
            {
                if (teamIndex == 0) teamIndex = 2;
                else if (teamIndex == 2) teamIndex = 0;
            }
            EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_ObjectiveHighlight_{teamIndex}", true);
        }
        public static void RefreshObjectiveHighlightAlarm(ITransportConnection _tc, byte _index)
        {
            ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[_index];
            if (objectiveInfo.isCapturing) EffectManager.sendUIEffectVisibility(47, _tc, false, "A_ObjectiveHighlight", true);
            else EffectManager.sendUIEffectVisibility(47, _tc, false, "A_ObjectiveHighlight", false);
        }
        public static void RefreshUIFreeModeInfoToEveryone()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState == EnumTable.EPlayerUIState.TeamSelect) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                RefreshUIFreeModeInfo(tc);
            }
        }
        public static void RefreshUIFreeModeInfo(ITransportConnection _tc) // 자유모드 상태에서의 정보 갱신
        {
            if (!PluginManager.roundInfo.isFreeModeReady)
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_Free_0", "플레이어 기다리는중...");
                EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_Free_1", $"{PluginManager.instance.Configuration.Instance.freeModeReadyCount}명 접속 시 게임 시작");
            }
            else
            {
                EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_Free_0", "게임 시작 중...");
                EffectManager.sendUIEffectText(47, _tc, false, "T_RoundInfo_Free_1", "잠시 후 게임이 시작됩니다.");
            }
        }
        public static IEnumerator Cor_FreeModeEnd()
        {
            int elapsedTime = 5;
            while (elapsedTime >= 0)
            {
                if (!PluginManager.roundInfo.isFreeModeReady)
                {
                    yield break;
                }
                yield return new WaitForSeconds(1f);
                elapsedTime--;
            }
            if (!PluginManager.roundInfo.isFreeModeReady)
            {
                yield break;
            }
            PluginManager.roundInfo.OnRoundOver(false);
        }
    }
}

using KoreanWarPlugin.Configuration.Preset;
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
using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.KWSystem
{
    public class AreaSystem // 구역 관련 시스템
    {
        public static void UpdateObjectiveCapture() // 거점 점령 상태 갱신
        {
            LayerMask playerMask = LayerMask.GetMask("Player");
            for (byte i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                if (PluginManager.roundInfo.objectives[i].locked) continue;
                ObjectiveInfo objectiveInfo = PluginManager.roundInfo.objectives[i];
                Collider[] colliders = Physics.OverlapBox(objectiveInfo.position, objectiveInfo.size, objectiveInfo.quaternion, playerMask);
                // 현재 거점에 있는 유저 정보
                List<SteamPlayer> team_0_steamPlayers = new List<SteamPlayer>();
                List<SteamPlayer> team_1_steamPlayers = new List<SteamPlayer>();
                // 거점에 있다가 떠난 유저 정보
                List<SteamPlayer> team_0_LeavePlayers = new List<SteamPlayer>();
                List<SteamPlayer> team_1_LeavePlayers = new List<SteamPlayer>();
                // 거점에 새로 들어온 유저 정보
                List<SteamPlayer> team_0_NewPlayers = new List<SteamPlayer>();
                List<SteamPlayer> team_1_NewPlayers = new List<SteamPlayer>();
                if (colliders.Length != 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        if (collider.TryGetComponent(out Player player))
                        {
                            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                            SteamPlayer steamPlayer = uPlayer.SteamPlayer();
                            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID);
                            if (playerInfo == null) continue;
                            if (!playerInfo.isDeployed) continue;
                            if (playerInfo.team)
                            {
                                team_0_steamPlayers.Add(steamPlayer);
                            }
                            else
                            {
                                team_1_steamPlayers.Add(steamPlayer);
                            }
                        }
                    }
                }
                // 유저 정보 갱신
                team_0_LeavePlayers = objectiveInfo.team_0_Players.Except(team_0_steamPlayers).ToList();
                team_1_LeavePlayers = objectiveInfo.team_1_Players.Except(team_1_steamPlayers).ToList();
                team_0_NewPlayers = team_0_steamPlayers.Except(objectiveInfo.team_0_Players).ToList();
                team_1_NewPlayers = team_1_steamPlayers.Except(objectiveInfo.team_1_Players).ToList();
                objectiveInfo.team_0_Players = team_0_steamPlayers;
                objectiveInfo.team_1_Players = team_1_steamPlayers;
                // 점수 변동 처리
                byte beforePoint = objectiveInfo.point;
                //byte newPoint = (byte)Mathf.Clamp(objectiveInfo.point + (team_1_Players.Count - team_0_Players.Count) * 30, 0, 200);
                byte newPoint = (byte)Mathf.Clamp(objectiveInfo.point + (team_1_steamPlayers.Count - team_0_steamPlayers.Count), 0, 200);
                objectiveInfo.point = newPoint;
                bool isStateChange = false;
                #region yee
                /*
                if (beforePoint != newPoint) // 점수 변동이 있었다면
                {
                    if (team_1_Players.Count - team_0_Players.Count < 0)
                    {
                        foreach (CSteamID cSteamID in team_0_Players)
                        {
                            if (objectiveInfo.team_0_effort.ContainsKey(cSteamID)) { objectiveInfo.team_0_effort[cSteamID] = (ushort)Mathf.Clamp(++objectiveInfo.team_0_effort[cSteamID], 0, 100); }
                            else
                            {
                                objectiveInfo.team_0_effort.Add(cSteamID, 0);
                                objectiveInfo.team_0_effort[cSteamID] = (ushort)Mathf.Clamp(++objectiveInfo.team_0_effort[cSteamID], 0, 100);
                            }
                        }
                    }
                    else if (team_1_Players.Count - team_0_Players.Count > 0)
                    {
                        foreach (CSteamID cSteamID in team_1_Players)
                        {
                            if (objectiveInfo.team_1_effort.ContainsKey(cSteamID)) { objectiveInfo.team_1_effort[cSteamID] = (ushort)Mathf.Clamp(++objectiveInfo.team_1_effort[cSteamID], 0, 100); }
                            else
                            {
                                objectiveInfo.team_1_effort.Add(cSteamID, 0);
                                objectiveInfo.team_1_effort[cSteamID] = (ushort)Mathf.Clamp(++objectiveInfo.team_1_effort[cSteamID], 0, 100);
                            }
                        }
                    }
                    if (!objectiveInfo.isCapturing) // 거점을 점령중인 상태엿는지
                    {
                        objectiveInfo.isCapturing = true;
                        RoundSystem.RefreshObjectiveCaptureStateToEveryone(objectiveInfo.isCapturing, i);
                    }
                    // 점수 및 점령상태에 따라 거점의 점령상태 갱신
                    if (objectiveInfo.team == EObjectiveTeam.Team_0)
                    {
                        if (objectiveInfo.point >= 100)
                        {
                            objectiveInfo.team = EObjectiveTeam.Netural; isStateChange = true;
                            DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(true, i);
                            DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                            PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, false);
                            foreach (var effort in objectiveInfo.team_1_effort)
                            {
                                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(effort.Key);
                                if (uPlayer != null) { IngameSystem.GiveScoreAndCredit(uPlayer, EScoreGainType.ObjectiveNeturalize, effort.Value, (ushort)(effort.Value / 10), ""); }
                            }
                            objectiveInfo.team_0_effort.Clear();
                            objectiveInfo.team_1_effort.Clear();
                        }
                    }
                    else if (objectiveInfo.team == EObjectiveTeam.Team_1)
                    {
                        if (objectiveInfo.point <= 100)
                        {
                            objectiveInfo.team = EObjectiveTeam.Netural; isStateChange = true;
                            DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(false, i);
                            DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                            PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, true);
                            foreach (var effort in objectiveInfo.team_0_effort)
                            {
                                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(effort.Key);
                                if (uPlayer != null) { IngameSystem.GiveScoreAndCredit(uPlayer, EScoreGainType.ObjectiveNeturalize, effort.Value, (ushort)(effort.Value / 10), ""); }
                            }
                            objectiveInfo.team_0_effort.Clear();
                            objectiveInfo.team_1_effort.Clear();
                        }
                    }
                    else if (objectiveInfo.point == 0)
                    {
                        objectiveInfo.team = EObjectiveTeam.Team_0; isStateChange = true;
                        objectiveInfo.isCapturing = false;
                        RoundSystem.RefreshObjectiveCaptureStateToEveryone(objectiveInfo.isCapturing, i);
                        DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(true, i);
                        DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                        PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, true);

                        foreach (var effort in objectiveInfo.team_0_effort)
                        {
                            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(effort.Key);
                            if (uPlayer != null) { IngameSystem.GiveScoreAndCredit(uPlayer, EScoreGainType.ObjectiveCapture, effort.Value, (ushort)(effort.Value / 10), ""); }
                        }
                        objectiveInfo.team_0_effort.Clear();
                        objectiveInfo.team_1_effort.Clear();
                        if (PluginManager.roundInfo.roundType == ERoundType.Battle)
                        {
                            bool team = true;
                            if (objectiveInfo.team == EObjectiveTeam.Team_1) team = false;
                            if (PluginManager.roundInfo.deffenseTeam != team) // 공격팀이 점령한 경우 
                            {
                                if (!PluginManager.roundInfo.deffenseTeam)
                                {
                                    if (i + 1 >= PluginManager.roundInfo.objectives.Length)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i + 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i + 1));
                                    }
                                }
                                else
                                {
                                    if (i <= 0)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i - 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i - 1));
                                    }
                                }
                            }  
                        }
                    }
                    else if (objectiveInfo.point == 200)
                    {
                        objectiveInfo.team = EObjectiveTeam.Team_1; isStateChange = true;
                        objectiveInfo.isCapturing = false;
                        RoundSystem.RefreshObjectiveCaptureStateToEveryone(objectiveInfo.isCapturing, i);
                        DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(false, i);
                        DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                        PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, false);
                        foreach (var effort in objectiveInfo.team_1_effort)
                        {
                            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(effort.Key);
                            if (uPlayer != null) { IngameSystem.GiveScoreAndCredit(uPlayer, EScoreGainType.ObjectiveCapture, effort.Value, (ushort)(effort.Value / 10), ""); }
                        }
                        objectiveInfo.team_0_effort.Clear();
                        objectiveInfo.team_1_effort.Clear();
                        if (PluginManager.roundInfo.roundType == ERoundType.Battle)
                        {
                            bool team = true;
                            if (objectiveInfo.team == EObjectiveTeam.Team_1) team = false;
                            if (PluginManager.roundInfo.deffenseTeam != team) // 공격팀이 점령한 경우 
                            {
                                if (!PluginManager.roundInfo.deffenseTeam)
                                {
                                    if (i + 1 >= PluginManager.roundInfo.objectives.Length)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i + 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i + 1));
                                    }
                                }
                                else
                                {
                                    if (i <= 0)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i - 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i - 1));
                                    }
                                }
                            }
                        }
                    }
                    if (isStateChange) RoundSystem.RefreshObjectiveStateToEveryone(i);
                    RoundSystem.RefreshObjectInfoToEveryone(i);
                } // 점수 변동이 있었다면
                else
                {
                    if (objectiveInfo.isCapturing) // 거점을 점령중인 상태엿는지
                    {
                        objectiveInfo.isCapturing = false;
                        RoundSystem.RefreshObjectiveCaptureStateToEveryone(objectiveInfo.isCapturing, i);
                    }
                }
                */
                #endregion
                if (beforePoint != newPoint) // 점수 변동이 있었다면
                {
                    if (team_1_steamPlayers.Count - team_0_steamPlayers.Count < 0)
                    {
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers)
                        {
                            if (objectiveInfo.team_0_effort.ContainsKey(steamPlayer.playerID)) { objectiveInfo.team_0_effort[steamPlayer.playerID] = (ushort)Mathf.Clamp(++objectiveInfo.team_0_effort[steamPlayer.playerID], 0, 100); }
                            else
                            {
                                objectiveInfo.team_0_effort.Add(steamPlayer.playerID, 0);
                                objectiveInfo.team_0_effort[steamPlayer.playerID] = (ushort)Mathf.Clamp(objectiveInfo.team_0_effort[steamPlayer.playerID]+2, 0, 200);
                            }
                        }
                    }
                    else if (team_1_steamPlayers.Count - team_0_steamPlayers.Count > 0)
                    {
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers)
                        {
                            if (objectiveInfo.team_1_effort.ContainsKey(steamPlayer.playerID)) { objectiveInfo.team_1_effort[steamPlayer.playerID] = (ushort)Mathf.Clamp(++objectiveInfo.team_1_effort[steamPlayer.playerID], 0, 100); }
                            else
                            {
                                objectiveInfo.team_1_effort.Add(steamPlayer.playerID, 0);
                                objectiveInfo.team_1_effort[steamPlayer.playerID] = (ushort)Mathf.Clamp(objectiveInfo.team_1_effort[steamPlayer.playerID]+2, 0, 200);
                            }
                        }
                    }
                    if (!objectiveInfo.isCapturing) // 거점을 점령중인 상태엿는지
                    {
                        objectiveInfo.isCapturing = true;
                        RoundSystem.RefreshObjectiveAlarmToEveryone(objectiveInfo.isCapturing, i);
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                    }
                    // 점수 및 점령상태에 따라 거점의 점령상태 갱신    
                    if (objectiveInfo.team == EObjectiveTeam.Team_0)
                    {
                        if (objectiveInfo.point >= 100) // 0 팀 점령 => 중립화
                        {
                            objectiveInfo.team = EObjectiveTeam.Netural; isStateChange = true;
                            DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(true, i);
                            DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                            PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, false);
                            foreach (var effort in objectiveInfo.team_1_effort)
                            {
                                SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID == effort.Key);
                                if (steamPlayer != null) { IngameSystem.GiveScoreAndCredit(steamPlayer, EScoreGainType.ObjectiveNeturalize, effort.Value, (ushort)(effort.Value / 10), ""); }
                            }
                            objectiveInfo.team_0_effort.Clear();
                            objectiveInfo.team_1_effort.Clear();
                            // 거점 내 유저에게 하이라이트 거점 UI 상태 갱신
                            foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, true, i); }
                            foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, false, i); }
                        }
                    }
                    else if (objectiveInfo.team == EObjectiveTeam.Team_1)
                    {
                        if (objectiveInfo.point <= 100) // 1 팀 점령 => 중립화
                        {
                            objectiveInfo.team = EObjectiveTeam.Netural; isStateChange = true;
                            DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(false, i);
                            DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                            PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, true);
                            foreach (var effort in objectiveInfo.team_0_effort)
                            {
                                SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID == effort.Key);
                                if (steamPlayer != null) { IngameSystem.GiveScoreAndCredit(steamPlayer, EScoreGainType.ObjectiveNeturalize, effort.Value, (ushort)(effort.Value / 10), ""); }
                            }
                            objectiveInfo.team_0_effort.Clear();
                            objectiveInfo.team_1_effort.Clear();
                            // 거점 내 유저에게 하이라이트 거점 UI 상태 갱신
                            foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, true, i); }
                            foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, false, i); }
                        }
                    }
                    else if (objectiveInfo.point == 0) // 중립 => 0 팀 점령
                    {
                        objectiveInfo.team = EObjectiveTeam.Team_0; isStateChange = true;
                        objectiveInfo.isCapturing = false;
                        RoundSystem.RefreshObjectiveAlarmToEveryone(objectiveInfo.isCapturing, i);
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                        DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(true, i);
                        DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                        PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, true);

                        foreach (var effort in objectiveInfo.team_0_effort)
                        {
                            SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID == effort.Key);
                            if (steamPlayer != null) { IngameSystem.GiveScoreAndCredit(steamPlayer, EScoreGainType.ObjectiveCapture, effort.Value, (ushort)(effort.Value / 10), ""); }
                        }
                        objectiveInfo.team_0_effort.Clear();
                        objectiveInfo.team_1_effort.Clear();
                        // 거점 내 유저에게 하이라이트 거점 UI 상태 갱신
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, true, i); }
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, false, i); }
                        if (PluginManager.roundInfo.roundType == ERoundType.Battle)
                        {
                            bool team = true;
                            if (objectiveInfo.team == EObjectiveTeam.Team_1) team = false;
                            if (PluginManager.roundInfo.deffenseTeam != team) // 공격팀이 점령한 경우 
                            {
                                if (!PluginManager.roundInfo.deffenseTeam)
                                {
                                    if (i + 1 >= PluginManager.roundInfo.objectives.Length)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i + 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i + 1));
                                    }
                                }
                                else
                                {
                                    if (i <= 0)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i - 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i - 1));
                                    }
                                }
                            }
                            if (objectiveInfo.locked)
                            {
                                foreach (SteamPlayer steamPlayer in team_0_steamPlayers)
                                {
                                    ITransportConnection tc = steamPlayer.transportConnection;
                                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_ObjectiveHighlight", false);
                                }
                                foreach (SteamPlayer steamPlayer in team_1_steamPlayers)
                                {
                                    ITransportConnection tc = steamPlayer.transportConnection;
                                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_ObjectiveHighlight", false);
                                }
                            }
                        }
                    }
                    else if (objectiveInfo.point == 200) // 중립 => 1 팀 점령
                    {
                        objectiveInfo.team = EObjectiveTeam.Team_1; isStateChange = true;
                        objectiveInfo.isCapturing = false;
                        RoundSystem.RefreshObjectiveAlarmToEveryone(objectiveInfo.isCapturing, i);
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                        DeploySystem.ActiveDeployMarker_ObjectiveToEveryone(false, i);
                        DeploySystem.RefreshUIMapObjectiveTeamToEveryone(i);
                        PluginManager.instance.ingamePopUpQueue.RequestIngamePopUp(i, objectiveInfo.team, false);
                        foreach (var effort in objectiveInfo.team_1_effort)
                        {
                            SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID == effort.Key);
                            if (steamPlayer != null) { IngameSystem.GiveScoreAndCredit(steamPlayer, EScoreGainType.ObjectiveCapture, effort.Value, (ushort)(effort.Value / 10), ""); }
                        }
                        objectiveInfo.team_0_effort.Clear();
                        objectiveInfo.team_1_effort.Clear();
                        // 거점 내 유저에게 하이라이트 거점 UI 상태 갱신
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, true, i); }
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightState(steamPlayer.transportConnection, false, i); }
                        if (PluginManager.roundInfo.roundType == ERoundType.Battle)
                        {
                            bool team = true;
                            if (objectiveInfo.team == EObjectiveTeam.Team_1) team = false;
                            if (PluginManager.roundInfo.deffenseTeam != team) // 공격팀이 점령한 경우
                            {
                                if (!PluginManager.roundInfo.deffenseTeam)
                                {
                                    if (i + 1 >= PluginManager.roundInfo.objectives.Length)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i + 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i + 1));
                                    }
                                }
                                else
                                {
                                    if (i <= 0)
                                    {
                                        // 모든 거점을 점령했기 때문에 라운드 종료
                                        PluginManager.roundInfo.OnRoundOver(team);
                                    }
                                    else
                                    {
                                        objectiveInfo.locked = true;
                                        PluginManager.roundInfo.objectives[i - 1].locked = false;
                                        RoundSystem.RefreshObjectiveStateToEveryone((byte)(i - 1));
                                    }
                                }
                            }
                            if (objectiveInfo.locked)
                            {
                                foreach (SteamPlayer steamPlayer in team_0_steamPlayers)
                                {
                                    ITransportConnection tc = steamPlayer.transportConnection;
                                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_ObjectiveHighlight", false);
                                }
                                foreach (SteamPlayer steamPlayer in team_1_steamPlayers)
                                {
                                    ITransportConnection tc = steamPlayer.transportConnection;
                                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_ObjectiveHighlight", false);
                                }
                            }
                        }
                    }
                    if (isStateChange) RoundSystem.RefreshObjectiveStateToEveryone(i);
                    RoundSystem.RefreshObjectInfoToEveryone(i);
                } // 점수 변동이 있었다면
                else
                {
                    if (objectiveInfo.isCapturing) // 거점을 점령중인 상태엿는지
                    {
                        objectiveInfo.isCapturing = false;
                        RoundSystem.RefreshObjectiveAlarmToEveryone(objectiveInfo.isCapturing, i);
                        foreach (SteamPlayer steamPlayer in team_0_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                        foreach (SteamPlayer steamPlayer in team_1_steamPlayers) { RoundSystem.RefreshObjectiveHighlightAlarm(steamPlayer.transportConnection, i); }
                    }
                }
                // 거점 관련 유저 UI 처리
                // 거점에 새로 들어온 유저 정보 처리
                foreach (SteamPlayer steamPlayer in team_0_NewPlayers) 
                {
                    if (steamPlayer == null) continue;
                    ITransportConnection tc = steamPlayer.transportConnection;
                    RoundSystem.ActiveObjectiveHighlightInfo(tc, true, i);
                }
                foreach (SteamPlayer steamPlayer in team_1_NewPlayers) 
                {
                    if (steamPlayer == null) continue;
                    ITransportConnection tc = steamPlayer.transportConnection;
                    RoundSystem.ActiveObjectiveHighlightInfo(tc, false, i);
                }
                // 거점에 있는 유저 정보 처리
                foreach (SteamPlayer steamPlayer in team_0_steamPlayers)
                {
                    if (steamPlayer == null) continue;
                    ITransportConnection tc = steamPlayer.transportConnection;
                    RoundSystem.RefreshObjectiveHighlightInfo(tc, true, i);
                }
                foreach (SteamPlayer steamPlayer in team_1_steamPlayers)
                {
                    if (steamPlayer == null) continue;
                    ITransportConnection tc = steamPlayer.transportConnection;
                    RoundSystem.RefreshObjectiveHighlightInfo(tc, false, i);
                }
                // 거점에서 나간 유저 정보 처리
                foreach (SteamPlayer steamPlayer in team_0_LeavePlayers)
                {
                    if (steamPlayer == default) continue;
                    ITransportConnection tc = steamPlayer.transportConnection;
                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_ObjectiveHighlight", false);
                }
                foreach (SteamPlayer steamPlayer in team_1_LeavePlayers)
                {
                    if (steamPlayer == default) continue;
                    ITransportConnection tc = steamPlayer.transportConnection;
                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_ObjectiveHighlight", false);
                }
            }
        }
        public static void UpdateObjectivePoint() // 거점 점령에 의한 점수 변동 계산
        {
            ushort team_0_Count = 0;
            ushort team_1_Count = 0;
            foreach (ObjectiveInfo objectiveInfo in PluginManager.roundInfo.objectives)
            {
                if (objectiveInfo.team == EObjectiveTeam.Team_0) team_0_Count++;
                else if (objectiveInfo.team == EObjectiveTeam.Team_1) team_1_Count++;
            }
            // 승리 점수 계산
            PluginManager.roundInfo.ChangeScore(true, team_1_Count);
            PluginManager.roundInfo.ChangeScore(false, team_0_Count);
        }
        public static void UpdateAreaRestrict() // 접근 제한 구역 처리
        {
            LayerMask playerMask = LayerMask.GetMask("Player") + LayerMask.GetMask("Vehicle");
            // 현재 거점에 있는 유저 정보
            List<SteamPlayer> steamPlayers = new List<SteamPlayer>();
            // 거점에 있다가 떠난 유저 정보
            List<SteamPlayer> leavePlayers = new List<SteamPlayer>();
            // 거점에 새로 들어온 유저 정보
            List<SteamPlayer> newPlayers = new List<SteamPlayer>();
            // 현재 제한구역에 있는 차량
            List<InteractableVehicle> vehicles_Restrict = new List<InteractableVehicle>();
            // 제한구역에 있다가 떠난 차량 정보
            List<InteractableVehicle> leaveVehicles_Restrict = new List<InteractableVehicle>();
            // 제한구역에 새로 들어온 차량 정보
            List<InteractableVehicle> newVehicles_Restrict = new List<InteractableVehicle>();
            // 현재 아군 지역에 있는 차량
            List<VehicleDeployInfo> vehicles_Ally = new List<VehicleDeployInfo>();
            // 아군지역에 있다가 떠난 차량 정보
            List<VehicleDeployInfo> leaveVehicles_Ally = new List<VehicleDeployInfo>();
            // 아군지역에 새로 들어온 차량 정보
            List<VehicleDeployInfo> newVehicles_Ally = new List<VehicleDeployInfo>();
            // 확인된 차량 정보
            for (int i = 0; i < 2; i++)
            {
                bool team = i == 0 ? true : false;
                RestrictPreset restrictPreset = i == 0 ? PluginManager.roundInfo.currentMapPreset.baseRestrict_0 : PluginManager.roundInfo.currentMapPreset.baseRestrict_1;
                Collider[] colliders = Physics.OverlapBox(restrictPreset.position, restrictPreset.size, Quaternion.Euler(0, restrictPreset.rotation, 0), playerMask);
                if (colliders.Length != 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        if (collider.TryGetComponent(out Player player))
                        {
                            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                            PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                            if (pc.isRedeploying) continue;
                            SteamPlayer steamPlayer = uPlayer.SteamPlayer();
                            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID);
                            
                            if (playerInfo == null) continue;
                            if (!playerInfo.isDeployed) continue;
                            if (playerInfo.team != team) steamPlayers.Add(steamPlayer);
                        }
                        else if (collider.TryGetComponent(out InteractableVehicle vehicle))
                        {
                            if (vehicle.isDead) continue;
                            VehicleDeployInfo vDeployInfo = PluginManager.teamInfo.GetVehicleDeployInfo(vehicle, out bool _vTeam);
                            if (vDeployInfo == null) continue;
                            if (_vTeam != team) // 제한구역에 있다면 제한구역 리스트에 추가
                            {
                                if (vehicles_Restrict.Contains(vehicle)) continue;
                                vehicles_Restrict.Add(vehicle);
                                if (!vehicle.anySeatsOccupied) continue;
                                foreach (Passenger passenger in vehicle.passengers)
                                {
                                    if (passenger.player == null) continue;
                                    PlayerComponent pc = passenger.player.player.GetComponent<PlayerComponent>();
                                    if (pc.isRedeploying) continue;
                                    PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(UnturnedPlayer.FromSteamPlayer(passenger.player).CSteamID);
                                    if (playerInfo == null) continue;
                                    if (!playerInfo.isDeployed) continue;
                                    if (playerInfo.team != team) steamPlayers.Add(passenger.player);
                                }
                            }
                            else // 아군지역에 있다면 아군지역 리스트에 추가
                            {
                                if (vehicles_Ally.Contains(vDeployInfo)) continue;
                                vehicles_Ally.Add(vDeployInfo);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < PluginManager.roundInfo.objectives.Length; i++)
            {
                RestrictPreset restrictPreset = null;
                bool team = false;
                if (PluginManager.roundInfo.objectives[i].team == EObjectiveTeam.Team_0)
                {
                    restrictPreset = PluginManager.roundInfo.currentMapPreset.ObjectivePresets[i].team_0_Restrict;
                    team = true;
                }
                else if (PluginManager.roundInfo.objectives[i].team == EObjectiveTeam.Team_1)
                {
                    restrictPreset = PluginManager.roundInfo.currentMapPreset.ObjectivePresets[i].team_1_Restrict; 
                }
                else continue;
                Collider[] colliders = Physics.OverlapBox(restrictPreset.position, restrictPreset.size, Quaternion.Euler(0, restrictPreset.rotation, 0), playerMask);
                if (colliders.Length != 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        if (collider.TryGetComponent(out Player player))
                        {
                            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                            PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                            if (pc.isRedeploying) continue;
                            SteamPlayer steamPlayer = uPlayer.SteamPlayer();
                            PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID);
                            if (playerInfo == null) continue;
                            if (!playerInfo.isDeployed) continue;
                            if (playerInfo.team != team) steamPlayers.Add(steamPlayer);
                        }
                        else if (collider.TryGetComponent(out InteractableVehicle vehicle))
                        {
                            if (vehicle.isDead) continue;
                            if (vehicles_Restrict.Contains(vehicle)) continue;
                            VehicleDeployInfo vDeployInfo = PluginManager.teamInfo.GetVehicleDeployInfo(vehicle, out bool _vTeam);
                            if (vDeployInfo == null) continue;
                            if (_vTeam == team) continue;
                            vehicles_Restrict.Add(vehicle);
                            if (!vehicle.anySeatsOccupied) continue;
                            foreach (Passenger passenger in vehicle.passengers)
                            {
                                if (passenger.player == null) continue;
                                PlayerComponent pc = passenger.player.player.GetComponent<PlayerComponent>();
                                if (pc.isRedeploying) continue;
                                PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(UnturnedPlayer.FromSteamPlayer(passenger.player).CSteamID);
                                if (playerInfo == null) continue;
                                if (!playerInfo.isDeployed) continue;
                                if (playerInfo.team != team) steamPlayers.Add(passenger.player);
                            }
                        }
                    }
                }
            }
            // 구역 정보 갱신
            leavePlayers = PluginManager.roundInfo.restrictArea_Players.Keys.Except(steamPlayers).ToList();
            newPlayers = steamPlayers.Except(PluginManager.roundInfo.restrictArea_Players.Keys).ToList();
            leaveVehicles_Restrict = PluginManager.roundInfo.restrictArea_Vehicles.Keys.Except(vehicles_Restrict).ToList();
            newVehicles_Restrict = vehicles_Restrict.Except(PluginManager.roundInfo.restrictArea_Vehicles.Keys).ToList();
            leaveVehicles_Ally = PluginManager.roundInfo.allyArea_Vehicles.Except(vehicles_Ally).ToList();
            newVehicles_Ally = vehicles_Ally.Except(PluginManager.roundInfo.allyArea_Vehicles).ToList();
            PluginManager.roundInfo.allyArea_Vehicles = vehicles_Ally;
            // 제한구역에 새로 들어온 유저 정보 처리
            foreach (SteamPlayer steamPlayer in newPlayers)
            {
                if (steamPlayer == null) continue;
                if (!Provider.clients.Contains(steamPlayer)) continue;
                PluginManager.roundInfo.restrictArea_Players.Add(steamPlayer, 5);
                ITransportConnection tc = steamPlayer.transportConnection;
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                pc.isEnterRestrictArea = true;
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_AreaRestrict", true);
                RefreshUIAreaRestrictTimer(tc, 10);
            }
            // 제한구역에 있는 유저 정보 처리
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                if (steamPlayer == null) continue;
                if (!Provider.clients.Contains(steamPlayer)) 
                {
                    PluginManager.roundInfo.restrictArea_Players.Remove(steamPlayer);
                    continue;
                }
                ITransportConnection tc = steamPlayer.transportConnection;
                byte timer = PluginManager.roundInfo.restrictArea_Players[steamPlayer]--;
                if(timer == 0)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    uPlayer.Player.movement.forceRemoveFromVehicle();
                    uPlayer.Player.teleportToLocationUnsafe(PluginManager.instance.Configuration.Instance.spawnPos,0);
                    uPlayer.Damage(255, uPlayer.Position, EDeathCause.ARENA, ELimb.SPINE, (CSteamID)0);
                    PluginManager.roundInfo.restrictArea_Players.Remove(steamPlayer);
                }
                RefreshUIAreaRestrictTimer(tc, timer);
            }
            // 제한구역에서 나간 유저 정보 처리
            foreach (SteamPlayer steamPlayer in leavePlayers)
            {
                if (steamPlayer == default) continue;
                PluginManager.roundInfo.restrictArea_Players.Remove(steamPlayer);
                if (!Provider.clients.Contains(steamPlayer)) continue;
                ITransportConnection tc = steamPlayer.transportConnection;
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_AreaRestrict", false);
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                pc.isEnterRestrictArea = false;
            }
            // 제한구역에 새로 들어온 차량 처리
            foreach (InteractableVehicle vehicle in newVehicles_Restrict)
            {
                if (vehicle == null) continue;
                if (vehicle.isDead) continue;
                PluginManager.roundInfo.restrictArea_Vehicles.Add(vehicle, 30);
            }
            // 제한구역에 있는 차량 처리
            foreach (InteractableVehicle vehicle in vehicles_Restrict)
            {
                if (vehicle == null) continue;
                if (vehicle.isDead)
                {
                    PluginManager.roundInfo.restrictArea_Vehicles.Remove(vehicle);
                    continue;
                }
                byte timer = PluginManager.roundInfo.restrictArea_Vehicles[vehicle]--;
                if (timer == 0)
                {
                    PluginManager.roundInfo.restrictArea_Vehicles.Remove(vehicle);
                    VehicleManager.askVehicleDestroy(vehicle);
                    VehicleDeployInfo vDeployInfo = PluginManager.teamInfo.GetVehicleDeployInfo(vehicle, out bool _team);
                    if (vDeployInfo != null)
                    {
                        PluginManager.teamInfo.RemoveVehicleDeploy(vDeployInfo, _team, true);
                        if (vDeployInfo.vPreset.isDeployable)
                        {
                            PluginManager.teamInfo.RemoveSpawnableVehicle(vehicle, _team);
                        }
                    }
                    
                }
            }
            // 제한구역에서 나간 차량 처리
            foreach (InteractableVehicle vehicle in leaveVehicles_Restrict)
            {
                if (vehicle == null) continue;
                if (vehicle.isDead) continue;
                PluginManager.roundInfo.restrictArea_Vehicles.Remove(vehicle);
            }
            // 아군구역에 새로 들어온 차량 처리
            foreach (VehicleDeployInfo vDeployInfo in newVehicles_Ally)
            {
                string active = "";
                vDeployInfo.isSupplied = false;
                if(vDeployInfo.supplyCooltime <= DateTime.UtcNow) // 보급 쿨타임이 조건 충족 시
                {
                    vDeployInfo.isSupplying = true;
                    active = "On";
                    // 보급 코르틴 실행
                    if (vDeployInfo.supplyCoroutine != null) PluginManager.instance.StopCoroutine(vDeployInfo.supplyCoroutine);
                    vDeployInfo.supplyCoroutine = PluginManager.instance.StartCoroutine(VehicleDeployInfo.Cor_Supply(vDeployInfo));
                }
                else
                {
                    vDeployInfo.isSupplying = false;
                    active = "Off";
                }
                if (!vDeployInfo.vehicle.anySeatsOccupied) continue;
                foreach (Passenger passenger in vDeployInfo.vehicle.passengers)
                {
                    if (passenger.player == null) continue;
                    ITransportConnection tc = passenger.player.transportConnection;
                    EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleSupply", true);
                    EffectManager.sendUIEffectVisibility(47, tc, false, $"P_VehicleSupply_{active}", true);
                }
                IngameSystem.RefreshVehicleSupplyInfoToCrews(vDeployInfo);
            }
            // 아군구역에 있는 차량 처리
            foreach (VehicleDeployInfo vDeployInfo in vehicles_Ally)
            {
                if (vDeployInfo.isSupplying || vDeployInfo.isSupplied) continue; // 조건 미충족 시 건너뛰기
                if (vDeployInfo.supplyCooltime <= DateTime.UtcNow) // 보급 쿨타임이 조건 충족 시
                {
                    vDeployInfo.isSupplying = true;
                    // 보급 코르틴 실행
                    if (vDeployInfo.supplyCoroutine != null) PluginManager.instance.StopCoroutine(vDeployInfo.supplyCoroutine);
                    vDeployInfo.supplyCoroutine = PluginManager.instance.StartCoroutine(VehicleDeployInfo.Cor_Supply(vDeployInfo));
                    if (vDeployInfo.vehicle.anySeatsOccupied)
                    {
                        foreach (Passenger passenger in vDeployInfo.vehicle.passengers)
                        {
                            if (passenger.player == null) continue;
                            ITransportConnection tc = passenger.player.transportConnection;
                            EffectManager.sendUIEffectVisibility(47, tc, false, "P_VehicleSupply_On", true);
                        }
                    }
                }
                else
                {
                    // 보급 쿨타임 시간 갱신
                    IngameSystem.RefreshVehicleSupplyInfoToCrews(vDeployInfo);
                }
            }
            // 아군구역에서 나간 차량 처리
            foreach (VehicleDeployInfo vDeployInfo in leaveVehicles_Ally)
            {
                if (vDeployInfo.isSupplying)
                {
                    if (vDeployInfo.supplyCoroutine != null)
                    {
                        PluginManager.instance.StopCoroutine(vDeployInfo.supplyCoroutine);
                        vDeployInfo.supplyCoroutine = null;
                    }
                }
                if (vDeployInfo.vehicle.anySeatsOccupied)
                {
                    foreach (Passenger passenger in vDeployInfo.vehicle.passengers)
                    {
                        if (passenger.player == null) continue;
                        ITransportConnection tc = passenger.player.transportConnection;
                        EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleSupply", false);
                    }
                }
                vDeployInfo.isSupplying = false;
                vDeployInfo.isSupplied = false;
            }
        }
        public static void RefreshUIAreaRestrictTimer(ITransportConnection _tc, int _timer)
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_AreaRestrict", $"{_timer}");
        }
    }
}

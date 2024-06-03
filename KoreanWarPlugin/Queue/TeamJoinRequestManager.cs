using KoreanWarPlugin.Info;
using KoreanWarPlugin.KWSystem;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Queue
{
    public class TeamJoinRequestManager
    {
        public Queue<TeamJoinRequest> teamJoinRequestQueue = new Queue<TeamJoinRequest>();
        public TeamJoinRequest currentTeamJoinRequest;

        bool isProcessing;
        public void RequestTeamJoin(CSteamID _cSteamID, bool _team, Action<CSteamID, bool, bool> callback)
        {
            TeamJoinRequest newRequest = new TeamJoinRequest(_cSteamID, _team, callback);
            teamJoinRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }
        void TryProcessNext()
        {
            if (!isProcessing && teamJoinRequestQueue.Count > 0)
            {
                currentTeamJoinRequest = teamJoinRequestQueue.Dequeue();
                isProcessing = true;
                ProcessNewTeamJoinRequest();
            }
        }
        public void FinishedProcessing(bool _success)
        {
            isProcessing = false;
            currentTeamJoinRequest.callback(currentTeamJoinRequest.cSteamID, currentTeamJoinRequest.team, _success);
            TryProcessNext();
        }
        void ProcessNewTeamJoinRequest()
        {
            bool success = false;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(currentTeamJoinRequest.cSteamID);
            PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();

            if (uPlayer != null)
            {
                if (PluginManager.teamInfo.playerRecordInfoList.ContainsKey(uPlayer.CSteamID) || pc.isJoinedTeam)
                {
                    PlayerRecordInfo pRecordInfo = PluginManager.teamInfo.playerRecordInfoList[uPlayer.CSteamID];
                    if (currentTeamJoinRequest.team)
                    {
                        if (PluginManager.teamInfo.team_1_PlayerCount - PluginManager.teamInfo.team_0_PlayerCount >= PluginManager.instance.Configuration.Instance.teamRestrictCount) // 상대팀이 입장 불가능한 경우 무조건 허가
                        {
                            // 팀에 새로 배정된 것으로 간주하고 딜레이 리셋
                            pRecordInfo.teamChangeableDatetime = DateTime.UtcNow.AddSeconds(PluginManager.instance.Configuration.Instance.teamChangeDelay);
                            success = true; 
                        }
                        else if (pRecordInfo.teamChangeableDatetime > DateTime.UtcNow && pRecordInfo.lastSelectedTeam != currentTeamJoinRequest.team)
                        {
                            double _time = (pRecordInfo.teamChangeableDatetime - DateTime.UtcNow).TotalSeconds;
                            int minutes = (int)(_time / 60);
                            int seconds = (int)(_time % 60);
                            string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");
                            UISystem.SendPopUpInfo(uPlayer.Player.channel.GetOwnerTransportConnection(), $"팀 변경 가능시간까지 \n{timeText} 남음");
                        }
                        else if (PluginManager.teamInfo.team_0_PlayerCount - PluginManager.teamInfo.team_1_PlayerCount < PluginManager.instance.Configuration.Instance.teamRestrictCount) success = true;
                    }
                    else
                    {
                        if (PluginManager.teamInfo.team_0_PlayerCount - PluginManager.teamInfo.team_1_PlayerCount >= PluginManager.instance.Configuration.Instance.teamRestrictCount)
                        {
                            pRecordInfo.teamChangeableDatetime = DateTime.UtcNow.AddSeconds(PluginManager.instance.Configuration.Instance.teamChangeDelay);
                            success = true;
                        }
                        else if (pRecordInfo.teamChangeableDatetime > DateTime.UtcNow && pRecordInfo.lastSelectedTeam != currentTeamJoinRequest.team)
                        {
                            double _time = (pRecordInfo.teamChangeableDatetime - DateTime.UtcNow).TotalSeconds;
                            int minutes = (int)(_time / 60);
                            int seconds = (int)(_time % 60);
                            string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");
                            UISystem.SendPopUpInfo(uPlayer.Player.channel.GetOwnerTransportConnection(), $"팀 변경 가능시간까지 \n{timeText} 남음");
                        }
                        else if (PluginManager.teamInfo.team_1_PlayerCount - PluginManager.teamInfo.team_0_PlayerCount < PluginManager.instance.Configuration.Instance.teamRestrictCount) success = true;
                    }
                }
                else
                {
                    if (currentTeamJoinRequest.team)
                    {
                        if (PluginManager.teamInfo.team_0_PlayerCount - PluginManager.teamInfo.team_1_PlayerCount < PluginManager.instance.Configuration.Instance.teamRestrictCount) success = true;
                    }
                    else
                    {
                        if (PluginManager.teamInfo.team_1_PlayerCount - PluginManager.teamInfo.team_0_PlayerCount < PluginManager.instance.Configuration.Instance.teamRestrictCount) success = true;
                    }
                }
            }
            FinishedProcessing(success);
        }
    }
}

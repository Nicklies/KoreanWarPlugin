using KoreanWarPlugin.Info;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Queue
{
    public class ClassRequestManager // 병과 대기열
    {
        public Queue<ClassRequest> classRequestQueue = new Queue<ClassRequest>();
        public ClassRequest currentClassRequest;

        bool isProcessing;
        public void RequestClass(CSteamID _cSteamID, byte _index, bool _team, bool _vGroupClass, EnumTable.EClassType _type, Action<CSteamID, byte, bool, bool, EnumTable.EClassType, bool> callback)
        {
            ClassRequest newRequest = new ClassRequest(_cSteamID, _index, _team, _vGroupClass, _type, callback);
            classRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }
        void TryProcessNext()
        {
            if (!isProcessing && classRequestQueue.Count > 0)
            {
                currentClassRequest = classRequestQueue.Dequeue();
                isProcessing = true;
                ProcessClassRequest();
            }
        }
        public void FinishedProcessing(bool _success)
        {
            isProcessing = false;
            currentClassRequest.callback(currentClassRequest.cSteamID, currentClassRequest.index, currentClassRequest.team, currentClassRequest.vGroupClass, currentClassRequest.type, _success);
            TryProcessNext();
        }
        void ProcessClassRequest()
        {
            bool success = false;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(currentClassRequest.cSteamID);
            if (uPlayer != null)
            {
                PlayerComponent pc = uPlayer.Player.GetComponent<PlayerComponent>();
                if (pc.isJoinedTeam)
                {
                    List<ClassInfo> classList = new List<ClassInfo>();
                    switch (currentClassRequest.type)
                    {
                        case EnumTable.EClassType.infantary:
                            classList = currentClassRequest.team ? PluginManager.teamInfo.team_0_ClassInf : PluginManager.teamInfo.team_1_ClassInf;
                            break;
                        case EnumTable.EClassType.driver:
                            classList = currentClassRequest.team ? PluginManager.teamInfo.team_0_ClassDriver : PluginManager.teamInfo.team_1_ClassDriver;
                            break;
                    }
                    if (classList[currentClassRequest.index].presetInfo.playerMax == 0) success = true; // 병력 제한이 없다면 바로 통과
                    else
                    {
                        if (classList[currentClassRequest.index].playerCount >= classList[currentClassRequest.index].maxPlayerCount) success = false; // 인원을 초과한경우
                        else success = true;
                    }
                }
            }
            FinishedProcessing(success);
        }
    }
}
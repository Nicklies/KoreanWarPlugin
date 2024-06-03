using Rocket.Unturned.Chat;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.Queue
{
    public class IngamePopUpQueueManager
    {
        public Queue<IngamePopUpRequest> popUpRequestQueue = new Queue<IngamePopUpRequest>();
        public IngamePopUpRequest currentPopUpRequest;

        bool isProcessing;
        public void RequestIngamePopUp(byte _index,EObjectiveTeam _objectiveState,bool _invokeTeam)
        {
            IngamePopUpRequest newRequest = new IngamePopUpRequest(_index, _objectiveState, _invokeTeam);
            popUpRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }
        void TryProcessNext()
        {
            if (!PluginManager.instance.isRoundStart)
            {
                popUpRequestQueue.Clear();
                return;
            }
            if (!isProcessing && popUpRequestQueue.Count > 0)
            {
                currentPopUpRequest = popUpRequestQueue.Dequeue();
                isProcessing = true;
                PluginManager.instance.StartCoroutine(ProcessNewTeamJoinRequest());
            }
        }
        public void FinishedProcessing()
        {
            isProcessing = false;
            TryProcessNext();
        }
        IEnumerator ProcessNewTeamJoinRequest()
        {
            List<SteamPlayer> steamPlayers = Provider.clients;
            foreach (SteamPlayer steamPlayer in steamPlayers)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EnumTable.EPlayerUIState.InGame) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                string objective = "";
                switch (currentPopUpRequest.index)
                {
                    case 0: objective = "A"; break;
                    case 1: objective = "B"; break;
                    case 2: objective = "C"; break;
                    case 3: objective = "D"; break;
                    case 4: objective = "E"; break;
                }
                string teamStr = currentPopUpRequest.invokeTeam == pc.team ? "아군이" : "적군이";
                //string teamStrColor = currentPopUpRequest.invokeTeam == pc.team ? "" : "";
                string taken = currentPopUpRequest.objectiveState == EObjectiveTeam.Netural ? "무력화함" : "점령함";
                EffectManager.sendUIEffectText(47, tc, false, "T_ObjectiveTaken", $"<color=White>{teamStr}</color> <color=#FFC067>{objective}거점을</color> <color=White>{taken}</color>");
                EffectManager.sendUIEffectVisibility(47, tc, false, $"A_ObjectiveTaken", true);
            }
            yield return new WaitForSeconds(6f);
            FinishedProcessing();
        }
    }
}
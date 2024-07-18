using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.KWSystem;
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
using Random = UnityEngine.Random;

namespace KoreanWarPlugin.Info
{
    public class RoundInfo // 라운드 정보
    {
        public ERoundType roundType { get; set; } // 라운드 종류
        public ushort team_0_score { get; set; } // 팀 0 점수
        public ushort team_1_score { get; set; } // 팀 1 점수
        public ushort team_0_scoreMax { get; set; } // 팀 0 초기화 점수
        public ushort team_1_scoreMax { get; set; } // 팀 1 초기화 점수
        public Dictionary<SteamPlayer, byte> restrictArea_Players { get; set; } // 제한구역에 들어간 유저들
        public Dictionary<InteractableVehicle, byte> restrictArea_Vehicles { get; set; } // 제한구역에 들어간 차량들
        public List<VehicleDeployInfo> allyArea_Vehicles { get; set; } // 아군구역에 들어간 차량들
        public ObjectiveInfo[] objectives { get; set; }
        public MapPreset currentMapPreset { get; set; }
        public string team_0_baseTextPos { get; set; }
        public string team_1_baseTextPos { get; set; }
        public byte currentMapIndex { get; set; } // 현재 선택된 맵의 인덱스
        public byte[] voteCount { get; set; } // 투표별 인원 수 / 0 ~ 5 맵 투표, 6 ~ 게임모드 투표
        public Dictionary<CSteamID, byte> playerVote { get; set; } // 투표를 선택한 유저의 정보 / 유저가 이전에 어떤 투표를 눌렀는지 추적하기 위해 활용
        public bool isGamemodeChanged { get; set; } // 투표중 게임모드가 바뀐적이 있는지 여부
        public bool winner { get; set; } // 게임 종료 후 승리한 팀 정보
        public bool deffenseTeam { get; set; } // 공방전인경우 방어팀
        public bool isFreeModeReady { get; set; } // 자유모드 상태에서 인원수가 충족되 게임 시작이 가능한지 여부
        public byte votePlayerCount { get; set; }
        public byte playerCount { get; set; } // 현재 활동중인 유저 수
        public Dictionary<CSteamID, KillRecordInfo> killRecordList { get; set; } // 무력화 되었을 때 킬로그 기록용
        public Dictionary<InteractableVehicle, VehicleKillRecordInfo> killRecordList_Vehicle { get; set; } // 차량 파괴될 예정일때 킬로그 기록용
        public RoundInfo()
        {
            // 자유모드 용 맵 설정
            roundType = ERoundType.Free;
            currentMapIndex = PluginManager.instance.Configuration.Instance.freeModeMapIndex;
            currentMapPreset = PluginManager.instance.Configuration.Instance.mapPresets[currentMapIndex];
            team_0_score = 0;
            team_1_score = 0;
            team_0_scoreMax = 0;
            team_1_scoreMax = 0;
            restrictArea_Players = new Dictionary<SteamPlayer, byte>();
            restrictArea_Vehicles = new Dictionary<InteractableVehicle, byte>();
            allyArea_Vehicles = new List<VehicleDeployInfo>();
            objectives = new ObjectiveInfo[0];
            team_0_baseTextPos = "";
            team_1_baseTextPos = "";
            voteCount = new byte[0];
            playerVote = new Dictionary<CSteamID, byte>();
            isGamemodeChanged = false;
            votePlayerCount = 0;
            playerCount = 0;
            killRecordList = new Dictionary<CSteamID, KillRecordInfo>();
            killRecordList_Vehicle = new Dictionary<InteractableVehicle, VehicleKillRecordInfo>();
            winner = false;
            deffenseTeam = false;
            isFreeModeReady = false;
        }
        public void OnRoundStart()
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            PluginManager.instance.StartCoroutine_Loop();
            PluginManager.instance.isRoundStart = true;
            isFreeModeReady = false;
            // 게임모드 별로 정보 처리
            int playerCount = (PluginManager.roundInfo.playerCount / 2 + PluginManager.roundInfo.playerCount % 2);
            float mapSizeFactor = 0.0f;
            ushort maxTickets = 0;
            switch (roundType)
            {
                case ERoundType.CaptureTheFlag:
                    // 거점 초기화
                    objectives = new ObjectiveInfo[currentMapPreset.ObjectivePresets.Length];
                    for (int i = 0; i < currentMapPreset.ObjectivePresets.Length; i++)
                    { 
                        objectives[i] = new ObjectiveInfo(100, currentMapPreset.ObjectivePresets[i]);
                        objectives[i].locked = false;
                    }
                    // 점수 초기화
                    switch (currentMapPreset.mapSize)
                    {
                        case EMapSize.Small:
                            mapSizeFactor = 1.0f;
                            maxTickets = 250;
                            break;
                        case EMapSize.Medium:
                            mapSizeFactor = 1.5f;
                            maxTickets = 500;
                            break;
                        case EMapSize.Large:
                            mapSizeFactor = 2.0f;
                            maxTickets = 1000;
                            break;
                    }
                    team_0_score = (ushort)Mathf.Clamp(configuration.gameModePresets[0].scoreMultipier * mapSizeFactor * playerCount, 0, maxTickets);
                    team_0_scoreMax = team_0_score;
                    team_1_score = (ushort)Mathf.Clamp(configuration.gameModePresets[0].scoreMultipier * mapSizeFactor * playerCount, 0, maxTickets);
                    team_1_scoreMax = team_1_score;
                    break;
                case ERoundType.Battle:
                    deffenseTeam = Random.Range(0, 2) == 0 ? true : false;
                    // 거점 초기화
                    objectives = new ObjectiveInfo[currentMapPreset.ObjectivePresets.Length];
                    int pointByTeam = deffenseTeam ? 0 : 200;
                    for (int i = 0; i < currentMapPreset.ObjectivePresets.Length; i++)
                    {
                        objectives[i] = new ObjectiveInfo((byte)pointByTeam, currentMapPreset.ObjectivePresets[i]);
                        if (!deffenseTeam)
                        {
                            if (i == 0) objectives[i].locked = false;
                            else objectives[i].locked = true;
                        }
                        else
                        {
                            if (i == objectives.Length - 1) objectives[i].locked = false;
                            else objectives[i].locked = true;
                        }
                    }
                    // 점수 초기화
                    switch (currentMapPreset.mapSize)
                    {
                        case EMapSize.Small:
                            mapSizeFactor = 1.0f;
                            maxTickets = 50;
                            break;
                        case EMapSize.Medium:
                            mapSizeFactor = 2.0f;
                            maxTickets = 125;
                            break;
                        case EMapSize.Large:
                            mapSizeFactor = 3.0f;
                            maxTickets = 250;
                            break;
                    }
                    team_0_score = (ushort)Mathf.Clamp(configuration.gameModePresets[1].scoreMultipier * mapSizeFactor * playerCount, 0, maxTickets);
                    team_0_scoreMax = team_0_score;
                    team_1_score = (ushort)Mathf.Clamp(configuration.gameModePresets[1].scoreMultipier * mapSizeFactor * playerCount, 0, maxTickets);
                    team_1_scoreMax = team_1_score;
                    break;
                case ERoundType.Free:
                    objectives = new ObjectiveInfo[0];
                    if (playerCount >= PluginManager.instance.Configuration.Instance.freeModeReadyCount)
                    {
                        isFreeModeReady = true;
                        PluginManager.instance.freeModeEnd = PluginManager.instance.StartCoroutine(RoundSystem.Cor_FreeModeEnd());
                    }
                    break;
            }
            // 맵 초기화
            foreach (InteractableVehicle vehicle in VehicleManager.vehicles)
            {
                if (vehicle == null) continue;
                if (vehicle.isDead) continue;
                int count = vehicle.trunkItems.items.Count;
                for (int i = 0; i < count; i++)
                {
                    vehicle.trunkItems.removeItem(0);
                }
            }
            VehicleManager.askVehicleDestroyAll();
            BarricadeManager.askClearAllBarricades();
            StructureManager.askClearAllStructures();
            ItemManager.askClearAllItems();
            PluginManager.teamInfo.OnRoundStart();
            restrictArea_Players.Clear();
            restrictArea_Vehicles.Clear();
            allyArea_Vehicles.Clear();
            killRecordList.Clear();
            killRecordList_Vehicle.Clear();
            team_0_baseTextPos = DeploySystem.SpawnMarkerPositon(currentMapPreset.basePos_0);
            team_1_baseTextPos = DeploySystem.SpawnMarkerPositon(currentMapPreset.basePos_1);
            // 유저 정보 초기화
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                // 미접속 상태일 시 예외처리
                if (!pc.isEnterFinished) continue;
                pc.Initialize();
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                UISystem.SetUIState_TeamSelection(steamPlayer.player, tc);
                if (steamPlayer.player.life.isDead) 
                steamPlayer.player.life.ServerRespawn(false);
                else steamPlayer.player.teleportToLocationUnsafe(configuration.spawnPos, configuration.spawnRot);
                // 거점 이펙트 갱신
                foreach (string guid in PluginManager.instance.Configuration.Instance.objectiveEffectGuid)
                {
                    Guid effectGuid = Guid.Parse(guid);
                    EffectManager.ClearEffectByGuid(effectGuid, tc);
                }
                for (int i = 0; i < objectives.Length; i++)
                {
                    Guid effectGuid = Guid.Parse(PluginManager.instance.Configuration.Instance.objectiveEffectGuid[i]);
                    TriggerEffectParameters triggerEffect = new TriggerEffectParameters(effectGuid);
                    triggerEffect.position = objectives[i].position;
                    triggerEffect.SetUniformScale(1f);
                    triggerEffect.SetRelevantPlayer(steamPlayer);
                    EffectManager.triggerEffect(triggerEffect);
                }
            }
        }
        public void OnRoundOver(bool _winner)
        {
            PluginManager.instance.StopCoroutine_Loop();
            isGamemodeChanged = false;
            winner = _winner;
            RoundSystem.RoundEnd();
        }
        public void ChangeScore(bool _team, ushort _amount)
        {
            if (_amount == 0) return;
            // 공방전인경우 방어팀은 점수를 잃지 않으므로 리턴
            if (roundType == ERoundType.Battle && deffenseTeam == _team) return;
            //_amount = 300;
            if (_team)
            {
                team_0_score = (ushort)Mathf.Clamp(team_0_score - _amount, 0, ushort.MaxValue);
                if(team_0_score == 0)
                {
                    OnRoundOver(false);
                    return;
                }
            }
            else
            {
                team_1_score = (ushort)Mathf.Clamp(team_1_score - _amount, 0, ushort.MaxValue);
                if (team_1_score == 0)
                {
                    OnRoundOver(true);
                    return;
                }
            }
            RoundSystem.RefreshUIRoundInfoToEveryone();
        }
        public void PlayerSendVote(UnturnedPlayer _uPlayer, byte _index) // 유저가 투표 정보 전달
        {
            if (playerVote.ContainsKey(_uPlayer.CSteamID))
            {
                byte previousVoteIndex = playerVote[_uPlayer.CSteamID];
                if (previousVoteIndex == _index) return; // 이미 선택한 투표이므로 리턴
                voteCount[previousVoteIndex]--;
                playerVote[_uPlayer.CSteamID] = _index;
                RoundSystem.RefreshUIVoteCountInfoToEveryone(previousVoteIndex);
            }
            else playerVote.Add(_uPlayer.CSteamID, _index);
            voteCount[_index]++;
            if (votePlayerCount <= playerVote.Count) { EndVote(); }
            RoundSystem.RefreshUIVoteCountInfoToEveryone(_index);
        }
        public void ReloadVote() // 투표 정보 초기화
        {
            // 투표 코르틴 시작
            PluginManager.instance.StartCoroutine_VoteStart();
            playerVote = new Dictionary<CSteamID, byte>();
            byte mapCount = 6;
            byte gamemodeCount = (byte)PluginManager.instance.Configuration.Instance.gameModePresets.Length;
            voteCount = new byte[mapCount + gamemodeCount];
        }
        public void EndVote()
        {
            int maxCount = voteCount[0];
            List<byte> maxIndex = new List<byte>();
            // 투표를 많이 받은 인덱스 찾기
            if (voteCount != null && 0 < voteCount.Length)
            {
                for (byte i = 0; i < voteCount.Length; i++)
                {
                    if (voteCount[i] > maxCount) 
                    {
                        maxCount = voteCount[i];
                        maxIndex.Clear();
                        maxIndex.Add(i);
                    }
                    else if (voteCount[i] == maxCount)
                    {
                        maxIndex.Add(i);
                    }
                }
            }
            // 투표 결과 처리
            byte result = byte.MaxValue;
            if (maxCount == 0) // 아무것도 투표가 안되어 있다면 랜덤으로 맵을 선택해 진행 / 자유모드인 경우 게임모드 선택할 때 까지 초기화
            {
                if(roundType == ERoundType.Free)
                {
                    ReloadVote();
                    RoundSystem.RefreshUIVoteTimerToEveryone();
                    RoundSystem.RefreshUIVoteCountInfoAllToEveryone();
                }
                else
                {
                    // 랜덤으로 맵을 선택해 결과 적용
                    result = (byte)Random.Range(0, PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType].maps.Length);
                    PluginManager.instance.isVoteEnd = true;
                    currentMapIndex = PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType].maps[result];
                    currentMapPreset = PluginManager.instance.Configuration.Instance.mapPresets[PluginManager.roundInfo.currentMapIndex];
                    List<SteamPlayer> steamPlayers = Provider.clients;
                    // 투표가 종료 되었다면 라운드 시작
                    if (PluginManager.instance.isVoteEnd)
                    {
                        PluginManager.instance.StartCoroutine_RoundStart();
                    }
                }
            }
            else // 한명이라도 투표를 선택했을 시
            {
                if (maxIndex.Count < 2) result = maxIndex[0]; // 중복 투표가 없다면 결과 반환
                else // 중복 투표가 있다면 랜덤으로 하나만 선정해 결과 반환
                {
                    byte[] mapIndex = maxIndex.Where(x => x < 6).ToArray();
                    if (mapIndex.Length != 0) { result = mapIndex[Random.Range(0, mapIndex.Length)]; } // 중복투표에 맵이 포함되어 있다면 맵을 결과값으로 반환
                    else { result = maxIndex[Random.Range(0, maxIndex.Count)]; } // 중복투표에 맵이 없다면 게임모드만 있는걸로 간주하고 결과값 반환
                }
                if (result < 6) // 맵이 뽑힌경우
                {
                    PluginManager.instance.isVoteEnd = true;
                    currentMapIndex = PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType].maps[result];
                    currentMapPreset = PluginManager.instance.Configuration.Instance.mapPresets[PluginManager.roundInfo.currentMapIndex];
                }
                else // 게임모드가 뽑힌경우
                {
                    roundType = (ERoundType)(result - 6);
                    isGamemodeChanged = true;
                    ReloadVote();
                    RoundSystem.RefreshUIVoteMapInfoToEveryone();
                    RoundSystem.RefreshUIVoteCountInfoAllToEveryone();
                }
            }
            // 투표가 종료 되었다면 라운드 시작 코르틴 작동
            if (PluginManager.instance.isVoteEnd)
            {
                PluginManager.instance.StartCoroutine_RoundStart();
            }
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                PlayerComponent pc = steamPlayer.player.GetComponent<PlayerComponent>();
                if (pc.localUIState != EPlayerUIState.RoundEnd) continue;
                ITransportConnection tc = steamPlayer.player.channel.GetOwnerTransportConnection();
                // 하이라이트 황성화
                if (result != byte.MaxValue)
                {
                    if (result < 6) EffectManager.sendUIEffectVisibility(47, tc, false, $"P_MapVote_{result}Highlight", true);
                    else EffectManager.sendUIEffectVisibility(47, tc, false, $"P_GameModeVote_{result - 6}Highlight", true);
                }
                if (PluginManager.instance.isVoteEnd)
                {
                    // 유저에게 정보 제공
                    EffectManager.sendUIEffectText(47, tc, false, "T_RoundInfo_MapName", $"{PluginManager.roundInfo.currentMapPreset.name}");
                    EffectManager.sendUIEffectText(47, tc, false, "T_RoundInfo_GameMode", $"{PluginManager.instance.Configuration.Instance.gameModePresets[(int)PluginManager.roundInfo.roundType].name}");
                    RoundSystem.RefreshUIRoundStartTimer(tc);
                    EffectManager.sendUIEffectVisibility(47, tc, false, "A_VoteEnd", true);
                    EffectManager.sendUIEffectVisibility(47, tc, false, "P_MapVote_Block", true);
                }
            }
        }
    }
    public class Vote
    {
        public List<CSteamID> players { get; set; }
        public Vote()
        {
            players = new List<CSteamID>();
        }
    }
}
using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.KWSystem;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.Info
{
    public class ObjectiveInfo
    {
        public byte point; // 거점 별로 점령상태 0 ~ 99 100 101 ~ 200
        public EObjectiveTeam team; // 0 = 0 팀, 1 = 중립, 2 = 1 팀
        public Vector3 position;
        public Quaternion quaternion;
        public Vector3 size;
        public List<SteamPlayer> team_0_Players; // 거점 안에 있는 0 팀 유저들
        public List<SteamPlayer> team_1_Players; // 거점 안에 있는 1 팀 유저들
        public bool isCapturing;
        public bool locked; // 점령이 불가능한지 여부 / 공방전에서 사용
        public string textPos_Center;
        public string textPos_0;
        public string textPos_1;
        public SpawnPreset[] team_0_spawn;
        public SpawnPreset[] team_1_spawn;
        public Dictionary<SteamPlayerID, ushort> team_0_effort; // 거점에서 점수를 얻은 0팀 유저들
        public Dictionary<SteamPlayerID, ushort> team_1_effort; // 거점에서 점수를 얻은 1팀 유저들
        public ObjectiveInfo(byte _point, ObjectivePreset _objectivePreset)
        {
            point = _point;
            if (_point == 0) { team = EObjectiveTeam.Team_0; }
            else if (_point >= 1 && _point <= 199) { team = EObjectiveTeam.Netural; }
            else if (_point == 200) { team = EObjectiveTeam.Team_1; }
            else { point = 200; team = EObjectiveTeam.Team_1; }
            position = _objectivePreset.position;
            quaternion = _objectivePreset.rotation;
            size = _objectivePreset.size;
            team_0_Players = new List<SteamPlayer>();
            team_1_Players = new List<SteamPlayer>();
            isCapturing = false;
            locked = false;
            textPos_Center = DeploySystem.SpawnMarkerPositon(_objectivePreset.position);
            textPos_0 = DeploySystem.SpawnMarkerPositon(_objectivePreset.team_0_MarkerPos);
            textPos_1 = DeploySystem.SpawnMarkerPositon(_objectivePreset.team_1_MarkerPos);
            team_0_spawn = _objectivePreset.team_0_spawn;
            team_1_spawn = _objectivePreset.team_1_spawn;
            team_0_effort = new Dictionary<SteamPlayerID, ushort>();
            team_1_effort = new Dictionary<SteamPlayerID, ushort>();
        }
    }
}

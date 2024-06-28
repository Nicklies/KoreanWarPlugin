using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class MapPreset
    {
        public string name; // 맵 이름
        public string mapImageUrl; // 로드아웃에서 보일 맵 이미지
        public string mapIconUrl; // 투표화면에서 보일 아이콘 이미지
        public EMapSize mapSize; // 맵 사이즈 / Small, Medium, Large
        public byte[] roundType; // 라운드 종류
        public byte playerCount; // 권장 플레이어 수
        public ObjectivePreset[] ObjectivePresets; // 거점 개수와 위치값
        public Vector3 mapPositon; // 맵의 원점 위치
        public Vector3 basePos_0; // 0 팀 스폰 위치
        public Vector3 basePos_1; // 1 팀 스폰 위치
        public SpawnPreset[] baseSpawnPos_0; // 0 팀 스폰 위치
        public SpawnPreset[] baseSpawnPos_1; // 1 팀 스폰 위치
        public RestrictPreset baseRestrict_0; // 0 팀 입장 제한 구역
        public RestrictPreset baseRestrict_1; // 1 팀 입장 제한 구역
        public SpawnPreset[] vehicleSpawnPos_0; // 0 팀 차량 스폰 위치
        public SpawnPreset[] vehicleSpawnPos_1; // 1 팀 차량 스폰 위치
    }
    public class ObjectivePreset
    {
        public Vector3 position;
        public Vector3 size;
        public Quaternion rotation;
        public Vector3 team_0_MarkerPos;
        public Vector3 team_1_MarkerPos;

        public SpawnPreset[] team_0_spawn;
        public SpawnPreset[] team_1_spawn;
        public RestrictPreset team_0_Restrict;
        public RestrictPreset team_1_Restrict;
    }
    public class SpawnPreset
    {
        public Vector3 position;
        public float rotation;
    }
    public class RestrictPreset
    {
        public Vector3 position;
        public float rotation;
        public Vector3 size;
    }
}

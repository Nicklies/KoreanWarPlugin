using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class VehicleTypePresetTable // 차량 종류 프리셋 예) 경전차, 중전차 등을 정의
    {
        public string name;
        public ushort instanceID;
        public string iconUrl; // 아이콘 URL
        public int timer; // 게임 시작 후 해제까지 필요한 시간 / 0일 시 바로 이용 가능
        public byte levelLimit; // 최소 필요한 레벨 / 0일 시 제한 없음
        public byte vehicleMax; // 최대 배치 가능한 차량 개수
        public byte playerMinCount; // 최소 필요한 플레이어 인원 수 / 접속한 플레이어가 이보다 적으면 사용 불가
        public byte crewMinCount; // 배치하기 위해 필요한 최소 인원 수
        public ushort[] vehicleList; // 사용 가능한 차량 리스트
        public bool classPlayerOnly; // 전용병과를 가진 유저만 탑승 가능한지 여부
        public byte classIndex; // 차량 배정 시 선택되는 운전병 병과 인덱스
        public float respawnTime; // 파괴된 이후 재배치 가능까지 걸리는 시간
        public float abandonTime; // 버려진 이후 복구되기까지 걸리는 시간
        public ushort destroyCost; // 차량 파괴 시 차감되는 점수
        public ushort reward_score;
        public ushort reward_credit;
    }
}

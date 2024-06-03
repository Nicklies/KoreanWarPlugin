using KoreanWarPlugin.Info;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin
{
    public class PlayerComponent : UnturnedPlayerComponent
    {
        public EnumTable.EPlayerUIState uIState = EnumTable.EPlayerUIState.TeamSelect;
        public EnumTable.EChatType chatType = EnumTable.EChatType.Team;
        public bool isJoinedTeam = false; // 유저가 팀에 배정됬는지 여부
        public bool isRedeploying = false; // 유저가 재배치 중인지 여부
        public bool isInvincible = false; // 스폰 무적 상태인지 여부
        public bool isEnterRestrictArea = false; // 접근 제한 구역에 들어갔는지 여부
        public bool team = false; // 현재 유저가 배정된 팀 / 플레이어 정보에 접근하지 않고 어느팀인지만 확일할 경우 사용
        public bool isProcessing = false; // 특정 기능을 수행중인지 여부 / 해당 수행이 끝날때까지 다른 작업은 사용 불가능함
        public bool isVehicleSpawn = false; // 차에서 스폰 했는지 여부
        public bool isEnterFinished = false; // 초기 이미지 불러오기를 마쳣는지 여부
        public bool isKnockDown = false; // 유저가 다운상태인지 여부
        public bool isMedic = false; // 선택한 병과가 의생병인지 여부
        public DateTime supplyCooltime = DateTime.MinValue; // 보급 쿨타임
        public string chatText = ""; // 채팅내용
        public string passwordText = ""; // 비번내용
        public string enterPasswordText = ""; // 비번내용
        public ushort passwordVGroupSelect = ushort.MaxValue; // 비번이 걸린 차량을 선택했을때 그 차량의 인스턴스 아이디
        public byte killLogCount = 0; // 현재 유저에게 띄워진 킬로그 개수 / 5개가 최대
        public byte scoreGainCount = 0; // 현재 유저에게 띄워진 점수확보 UI 개수 / 8개가 최대
        public byte fireMode = byte.MaxValue;
        public void Initialize()
        {
            isJoinedTeam = false;
            isRedeploying = false;
            isEnterRestrictArea = false;
            team = false;
            isVehicleSpawn = false;
            isInvincible = false;
            isProcessing = false;
            isKnockDown = false;
            isMedic = false;
            chatText = "";
            passwordText = "";
            enterPasswordText = "";
            passwordVGroupSelect = ushort.MaxValue;
            killLogCount = 0;
            scoreGainCount = 0;
            fireMode = byte.MaxValue;
            supplyCooltime = DateTime.MinValue;
        }
    }
}
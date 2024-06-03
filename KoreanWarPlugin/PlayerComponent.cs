using KoreanWarPlugin.Info;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin
{
    // 서버 접속한 유저 객체에게 부여되는 클래스
    public class PlayerComponent : UnturnedPlayerComponent
    {
        // 로컬 UI 정보
        public EnumTable.EPlayerUIState localUIState { get; set; } // 로컬 UI 상태
        public EnumTable.EChatType localChatType { get; set; } // 로컬 채팅 종류
        public bool isEnterFinished { get; set; } // 초기 이미지 불러오기를 마쳣는지 여부
        // 유저 정보
        public bool isJoinedTeam { get; set; } // 유저가 팀에 배정됬는지 여부
        public bool isRedeploying { get; set; } // 유저가 재배치 중인지 여부
        public bool isInvincible { get; set; } // 스폰 무적 상태인지 여부
        public bool isEnterRestrictArea { get; set; } // 접근 제한 구역에 들어갔는지 여부
        public bool team { get; set; } // 현재 유저가 배정된 팀 / 플레이어 정보에 접근하지 않고 어느팀인지만 확일할 경우 사용
        public bool isProcessing { get; set; } // 특정 기능을 수행중인지 여부 / 해당 수행이 끝날때까지 다른 작업은 사용 불가능함
        public bool isVehicleSpawn { get; set; } // 차에서 스폰 했는지 여부
        public bool isKnockDown { get; set; } // 유저가 다운상태인지 여부
        public bool isMedic { get; set; } // 선택한 병과가 의생병인지 여부
        public DateTime supplyCooltime { get; set; } // 보급 쿨타임
        public string chatText { get; set; } // 채팅내용
        public string passwordText { get; set; } // 비번내용
        public string enterPasswordText { get; set; } // 비번내용
        public ushort passwordVGroupSelect { get; set; } // 비번이 걸린 차량을 선택했을때 그 차량의 인스턴스 아이디
        public byte killLogCount { get; set; } // 현재 유저에게 띄워진 킬로그 개수 / 5개가 최대
        public byte scoreGainCount { get; set; } // 현재 유저에게 띄워진 점수확보 UI 개수 / 8개가 최대
        public byte fireMode { get; set; }
        public void Initialize()
        {
            isJoinedTeam = false;
            isRedeploying = false;
            isInvincible = false;
            isEnterRestrictArea = false;
            team = false;
            isProcessing = false;
            isVehicleSpawn = false;
            isKnockDown = false;
            isMedic = false;
            supplyCooltime = DateTime.MinValue;
            chatText = "";
            passwordText = "";
            enterPasswordText = "";
            passwordVGroupSelect = ushort.MaxValue;
            killLogCount = 0;
            scoreGainCount = 0;
            fireMode = byte.MaxValue;
        }
        public PlayerComponent()
        {
            localUIState = EnumTable.EPlayerUIState.TeamSelect;
            localChatType = EnumTable.EChatType.Team;
            isEnterFinished = false;
            Initialize();
        }
    }
}
using KoreanWarPlugin.Data;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class PlayerInfo
    {
        public CSteamID cSteamID { get; set; }
        public string displayName { get; set; }
        public string avatarUrl { get; set; }
        public ushort experience { get; set; }
        public EnumTable.EClassType classType { get; set; }
        public EnumTable.ELoadoutType selectItemType { get; set; } // 선택한 아이템 선택창의 종류
        public bool team { get; set; } // 어느팀인지 여부 / true = 0팀, false = 1팀
        public bool isDeployed { get; set; } // 배치된 상태인지 여부
        public bool isDeployable { get; set; } // 전투배치가 가능한지 여부
        public byte spawnIndex { get; set; } // 스폰 위치 인덱스 / 0 ~ 4 = 거점, 5 = 기지, 6 ~ n = 차량, 255 = 선택안됨
        public byte vTypeIndex { get; set; } // 유저에게 배정된 차량종류 인덱스
        public byte classIndex { get; set; } // 선택한 병과의 인덱스 (리스트에서 보이는 순서)
        public uint creditCost { get; set; } // 전투배치 시 유저가 소모할 포인트
        public ushort classPrestIndex { get; set; } // 유저에게 배정된 병과 프리셋 인덱스
        public ushort spawnInstaceID_Dynamic { get; set; } // 스폰 차량 인스턴스 아이디
        public ushort vGroupInstanceID { get; set; } // 유저에게 배정된 차량그룹 인스턴스아이디
        public ushort supplyCost { get; set; } // 현재 유저가 적립한 보급 포인트
        public ushort supplyPoint_Max { get; set; } // 유저가 적립 가능한 최대 보급 포인트
        public ushort primaryIndex { get; set; } // 유저가 선택한 주무기 인덱스
        public ushort sightIndex { get; set; } // 유저가 선택한 조준경 인덱스
        public ushort tacticalIndex { get; set; } // 유저가 선택한 전술 인덱스
        public ushort magazineIndex { get; set; } // 유저가 선택한 탄창 인덱스
        public ushort gripIndex { get; set; } // 유저가 선택한 손잡이 인덱스
        public ushort secondaryIndex { get; set; } // 유저가 선택한 보조무기 인덱스
        public ushort explosive_0Index { get; set; } // 유저가 선택한 폭팔물0 인덱스
        public ushort explosive_1Index { get; set; } // 유저가 선택한 폭팔물1 인덱스
        public ushort equipmentIndex { get; set; } // 유저가 선택한 장구류 인덱스
        public ushort utility_0Index { get; set; } // 유저가 선택한 특수장비0 인덱스
        public ushort utility_1Index { get; set; } // 유저가 선택한 특수장비1 인덱스

        public PlayerInfo(UnturnedPlayer _uPlayer,PlayerData _data,bool _team)
        {
            cSteamID = _uPlayer.CSteamID;
            supplyCost = 0;
            supplyPoint_Max = 0;
            team = _team;
            isDeployed = false;
            isDeployable = true;
            spawnIndex = byte.MaxValue;
            spawnInstaceID_Dynamic = ushort.MaxValue;
            displayName = _uPlayer.DisplayName;
            avatarUrl = "";
            classPrestIndex = ushort.MaxValue;
            classIndex = byte.MaxValue;
            vTypeIndex = byte.MaxValue;
            vGroupInstanceID = ushort.MaxValue;
            primaryIndex = ushort.MaxValue;
            sightIndex = ushort.MaxValue;
            tacticalIndex = ushort.MaxValue;
            magazineIndex = ushort.MaxValue;
            gripIndex = ushort.MaxValue;
            secondaryIndex = ushort.MaxValue;
            explosive_0Index = ushort.MaxValue;
            explosive_1Index = ushort.MaxValue;
            equipmentIndex = ushort.MaxValue;
            utility_0Index = ushort.MaxValue;
            utility_1Index = ushort.MaxValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class ClassPresetTable
    {
        public EnumTable.EClassType classType; // 병과 타입
        public ushort instanceID; // 병과 개별 아이디
        public string name; // 병과 이름
        public string iconUrl; // 아이콘 URL
        public bool isMedic; // 소생 기능을 가진 병과인지 여부
        public byte levelLimit; // 최소 필요한 레벨 / 0일 시 제한 없음
        public byte playerMax; // 최대 유저 수 / 0일 시 제한 없음
        public byte supplyPoint; // 해당 병과가 가지는 보급포인트 최대량
        public float timer; // 게임 시작 후 해제까지 필요한 시간 / 0일 시 바로 이용 가능
        public ushort[] primaryList; // 병과가 사용 가능한 주무기 아이디 리스트
        public ushort[] secondaryList; // 병과가 사용 가능한 보조무기 아이디 리스트
        public ushort[] explosiveList; // 병과가 사용 가능한 폭팔물 아이디 리스트
        public ushort equipmentInstanceID; // 병과가 사용 가능한 장구류 프리셋 아이디
        public ushort[] utilityList; // 병과가 사용 가능한 특수장비 아이디 리스트
        public ushort primaryDefaultIndex; // 병과 선택 시 기본으로 선택되는 주무기 인덱스
        public ushort secondaryDefaultIndex; // 병과 선택 시 기본으로 선택되는 보조무기 인덱스
        public ushort explosive_0DefaultIndex; // 병과 선택 시 기본으로 선택되는 폭팔물0 인덱스
        public ushort explosive_1DefaultIndex; // 병과 선택 시 기본으로 선택되는 폭팔물1 인덱스
        public ushort equipmentDefaultIndex; // 병과 선택 시 기본으로 선택되는 장구류 인덱스
        public ushort utility_0DefaultIndex; // 병과 선택 시 기본으로 선택되는 특수장비0 인덱스
        public ushort utility_1DefaultIndex; // 병과 선택 시 기본으로 선택되는 특수장비1 인덱스
        public ushort[] loadoutList; // 전투배치 시 병과전용으로 제공되는 기본 아이템 리스트
    }
}

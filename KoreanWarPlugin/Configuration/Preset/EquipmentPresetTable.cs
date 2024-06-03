using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class EquipmentPresetTable
    {
        public ClothPresetTable[] clothPresetList; // 인덱스 별로 티어별 무장 정보를 나타냄
    }
    public class ClothPresetTable : LoadoutTable
    {
        // 배열 인덱스별로 각 레벨의 의류 아이디 선택 / 레벨이 배열 개수를 초과할 경우 마지막 아이템을 반환
        public ushort[] equipment_Head; // 병과가 착용할 모자
        public ushort[] equipment_Mask; // 병과가 착용할 마스크
        public ushort[] equipment_Glasses; // 병과가 착용할 안경
        public ushort[] equipment_Vest; // 병과가 착용할 조끼
        public ushort[] equipment_Backpack; // 병과가 착용할 가방
        public ushort[] equipment_shirt; // 병과가 착용할 상의
        public ushort[] equipment_Pant; // 병과가 착용할 하의
    }
}

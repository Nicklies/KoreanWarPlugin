using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class LoadoutTable
    {
        public string name;
        public string iconUrl;
        public byte supplyCost;
        public uint creditCost;
        public ushort itemID;
        public byte amount;
        public byte[] amount_equipment = new byte[0]; // 장구류 레벨에 따라 추가로 지급되는 수량
        public bool isDuplicatable = true; // 로드아웃에서 중복해서 장착 가능한지 여부
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class SecondaryPresetTable : LoadoutTable
    {
        public ushort magazineItemID; // 탄창 아이템 아이디
        public byte magazineAmount; // 탄창 개수
    }
}

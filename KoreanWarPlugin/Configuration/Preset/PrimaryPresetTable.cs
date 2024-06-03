using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class PrimaryPresetTable : LoadoutTable
    {
        public ushort[] sights;
        public ushort[] tacticals;
        public ushort[] magazines;
        public ushort[] grips;
        public ushort sightDefaultIndex; // 총기 선택 시 기본으로 부착되는 조준경 인덱스
        public ushort tacticalDefaultIndex; // 총기 선택 시 기본으로 부착되는 전술 인덱스
        public ushort magazineDefaultIndex; // 총기 선택 시 기본으로 부착되는 탄창 인덱스
        public ushort gripDefaultIndex; // 총기 선택 시 기본으로 부착되는 손잡이 인덱스
    }
}
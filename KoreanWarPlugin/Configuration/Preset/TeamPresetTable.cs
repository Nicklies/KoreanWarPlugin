using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class TeamPresetTable
    {
        public string teamName; // 팀 이름
        public string teamImageUrl; // 팀 이미지 URL
        public string teamIconUrl; // 팀 아이콘 URL

        public ushort[] classList; // 팀에 할당된 병과리스트
        public ushort[] vehicleTypeList; // 팀에 할당된 차종리스트
        public LevelPreset[] levelPresets; // 레벨별 정보
    }
}

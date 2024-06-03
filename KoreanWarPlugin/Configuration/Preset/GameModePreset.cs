using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class GameModePreset
    {
        public string name;
        public byte playerCount;
        public string description;
        public string iconUrl;
        public byte[] maps; // 해당 게임모드로 플레이 가능한 맵의 인덱스
    }
}

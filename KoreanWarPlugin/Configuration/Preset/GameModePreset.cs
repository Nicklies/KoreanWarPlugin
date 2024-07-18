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
        public int scoreMultipier; // 게임 게임모드로 시작 시 인원 당 제공될 점수 / 서버에 20 명이 있다면 두팀으로 나뉠것을 감안해 10명분의 점수로 제공됨 / 5 를 입력하면 50 이 제공되는 방식
        public int mapSizeFactorScoreMultipier; // 작은 맵에서 곱해지는
    }
}

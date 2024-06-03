using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class PlayerRecordInfo
    {
        public string name { get; set; }
        public PlayerTeamRecordInfo team_0_RecordInfo { get; set; }
        public PlayerTeamRecordInfo team_1_RecordInfo { get; set; }
        public DateTime teamChangeableDatetime { get; set; } // 팀 변경 가능한 시간
        public bool lastSelectedTeam { get; set; } // 마지막으로 선택한 팀
        public PlayerRecordInfo(string _name, bool _team)
        {
            name = _name;
            team_0_RecordInfo = new PlayerTeamRecordInfo(_name, _team ? true : false);
            team_1_RecordInfo = new PlayerTeamRecordInfo(_name, _team ? false : true);
            teamChangeableDatetime = DateTime.MinValue;
            lastSelectedTeam = false;
        }

    }
    public class PlayerTeamRecordInfo
    {
        public string name { get; set; }
        public byte level { get; set; }
        public ushort score { get; set; }
        public int killCount { get; set; }
        public int deathCount { get; set; }
        public bool isActive { get; set; }
        public PlayerTeamRecordInfo(string _name, bool _isActive)
        {
            name = _name;
            level = 0;
            score = 0;
            killCount = 0;
            deathCount = 0;
            isActive = _isActive;
        }
    }
}
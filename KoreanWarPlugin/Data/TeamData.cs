using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Data
{
    public class TeamData
    {
        public CSteamID team_0_ID { get; set; } // 팀 스팀 아이디
        public CSteamID team_1_ID { get; set; } // 팀 스팀 아이디

        public TeamData(CSteamID _team_0steamID, CSteamID _team_1steamID)
        {
            team_0_ID = _team_0steamID;
            team_1_ID = _team_1steamID;
        }
    }
}

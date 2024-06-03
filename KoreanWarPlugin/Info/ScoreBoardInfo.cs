using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class ScoreBoardInfo // 게임이 종료되면 해당 클래스로 결과 정보 전달
    {
        public PlayerTeamRecordInfo[] team_0_Results;
        public PlayerTeamRecordInfo[] team_1_Results;
        public ScoreBoardInfo()
        {
            team_0_Results = new PlayerTeamRecordInfo[0];
            team_1_Results = new PlayerTeamRecordInfo[0];
        }
    }
}

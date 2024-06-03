using KoreanWarPlugin.Data;
using KoreanWarPlugin.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Database
{
    public class TeamDatabase
    {
        private DataStorage<List<TeamData>> DataStorage_TeamData { get; set; }
        public List<TeamData> Data_Team;
        public TeamDatabase()
        {
            DataStorage_TeamData = new DataStorage<List<TeamData>>(PluginManager.instance.Directory, "TeamData.json");
        }

        public void Reload()
        {
            Data_Team = DataStorage_TeamData.Read();
            if (Data_Team == null)
            {
                Data_Team = new List<TeamData>();
                DataStorage_TeamData.Save(Data_Team);
            }
        }
        public void AddData(TeamData _data)
        {
            Data_Team.Add(_data);
        }
        public void RemoveData(TeamData _data)
        {
            Data_Team.Remove(_data);
        }
        public void UpdateData()
        {
            DataStorage_TeamData.Save(Data_Team);
        }
    }
}

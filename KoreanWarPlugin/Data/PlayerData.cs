using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Data
{
    public class PlayerData
    {
        public CSteamID cSteamID { get; set; }
        public uint credit { get; set; }

        public PlayerData(CSteamID _cSteamID)
        {
            cSteamID = _cSteamID;
            credit = 0;
        }
    }
}
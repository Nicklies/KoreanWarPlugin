using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class VehicleKillRecordInfo
    {
        public SteamPlayerID killer;

        public VehicleKillRecordInfo(SteamPlayerID _killer)
        {
            killer = _killer;
        }
    }
}

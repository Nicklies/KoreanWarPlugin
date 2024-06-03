using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class KillRecordInfo
    {
        public string killerAvatarUrl;
        public string killerName;
        public string deathName;
        public string deathCause;
        public string deathCauseUrl;
        public bool isHeadShot;
        public bool isImageLarge;
        public bool killerTeam;
        public bool deathTeam;
        public CSteamID killerCsteamID;
        public SteamPlayerID killerID;

        public KillRecordInfo(string _killerAvatarUrl, string _killerName,string _deathName,string _deathCause,string _deathCauseUrl, bool _isHeadShot,bool _isImageLarge,bool _killerTeam,bool _deathTeam,CSteamID _killerSteamID,SteamPlayerID _killerID)
        {
            killerAvatarUrl = _killerAvatarUrl;
            killerName = _killerName;
            deathName = _deathName;
            deathCause = _deathCause;
            deathCauseUrl = _deathCauseUrl;
            isHeadShot = _isHeadShot;
            isImageLarge = _isImageLarge;
            killerTeam = _killerTeam;
            deathTeam = _deathTeam;
            killerCsteamID = _killerSteamID;
            killerID = _killerID;
        }
    }
}

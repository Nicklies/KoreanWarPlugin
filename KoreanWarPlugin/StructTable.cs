using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Info;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin
{
    public struct ClassRequest
    {
        public Action<CSteamID, byte, bool, bool, EnumTable.EClassType, bool> callback;
        public CSteamID cSteamID;
        public byte index;
        public bool team;
        public bool vGroupClass;
        public EnumTable.EClassType type;
        public ClassRequest(CSteamID _cSteamID, byte _index, bool _team, bool _vGroupClass, EnumTable.EClassType _type, Action<CSteamID, byte, bool, bool, EnumTable.EClassType, bool> _callback)
        {
            cSteamID = _cSteamID;
            index = _index;
            team = _team;
            vGroupClass = _vGroupClass;
            type = _type;
            callback = _callback;
        }
    }
    public struct VehicleRequest
    {
        public Action<CSteamID, byte, ushort, bool, EnumTable.ERequestType, bool> callback;
        public CSteamID cSteamID;
        public byte index;
        public ushort vGroupInstanceID;
        public bool team;
        public EnumTable.ERequestType type;
        public VehicleRequest(CSteamID _cSteamID, byte _vTypeIndex, ushort _vGroupInstanceID, bool _team, EnumTable.ERequestType _type, Action<CSteamID, byte, ushort, bool, EnumTable.ERequestType, bool> _callback)
        {
            cSteamID = _cSteamID;
            index = _vTypeIndex;
            vGroupInstanceID = _vGroupInstanceID;
            team = _team;
            type = _type;
            callback = _callback;
        }
    }
    public struct TeamJoinRequest
    {
        public Action<CSteamID, bool, bool> callback;
        public CSteamID cSteamID;
        public bool team;
        public TeamJoinRequest(CSteamID _cSteamID,bool _team, Action<CSteamID, bool, bool> _callback)
        {
            cSteamID = _cSteamID;
            team = _team;
            callback = _callback;
        }
    }
    public struct IngamePopUpRequest
    {
        public byte index;
        public EObjectiveTeam objectiveState;
        public bool invokeTeam;
        public IngamePopUpRequest(byte _index, EObjectiveTeam _objectiveState,bool _invokeTeam)
        {
            index = _index;
            objectiveState = _objectiveState;
            invokeTeam = _invokeTeam;
        }
    }
    public struct KillLog
    {
        public string killer;
        public string death;
        public string iconUrl;
        public bool isImageLarge;
        public bool isHeadShot;
        public bool killerTeam;
        public bool deathTeam;
        public KillLog(string _killer, string _death, string _weaponIcon, bool _weaponLarge, bool _headShot, bool _deathTeam, bool _killerTeam)
        {
            killer = _killer;
            death = _death;
            iconUrl = _weaponIcon;
            isImageLarge = _weaponLarge;
            isHeadShot = _headShot;
            deathTeam = _deathTeam;
            killerTeam = _killerTeam;
        }
        public KillLog(KillRecordInfo _killRecordInfo)
        {
            killer = _killRecordInfo.killerName;
            death = _killRecordInfo.deathName;
            iconUrl = _killRecordInfo.deathCauseUrl;
            isImageLarge = _killRecordInfo.isImageLarge;
            isHeadShot = _killRecordInfo.isHeadShot;
            killerTeam = _killRecordInfo.killerTeam;
            deathTeam = _killRecordInfo.deathTeam;
        }
    }
}
using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.KWSystem;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KoreanWarPlugin.Info
{
    public class SpawnableVehicleInfo
    {
        public SteamPlayerID steamPlayerID;
        public ushort instanceID;
        public InteractableVehicle vehicle;
        public VehicleTypePresetTable vTypePreset;
        public Vector2Int lastPos;
        public string textPos;
        public SpawnableVehicleInfo(SteamPlayerID _steamPlayerID, InteractableVehicle _vehicle, VehicleTypePresetTable _vTypePreset, bool _team)
        {
            steamPlayerID = _steamPlayerID;
            instanceID = _team ? PluginManager.teamInfo.team_0_spawnableVInstanceID++ : PluginManager.teamInfo.team_1_spawnableVInstanceID++;
            vehicle = _vehicle;
            vTypePreset = _vTypePreset;
            lastPos = new Vector2Int(Mathf.RoundToInt(_vehicle.transform.position.x), Mathf.RoundToInt(_vehicle.transform.position.z));
            textPos = DeploySystem.SpawnMarkerPositon(_vehicle.transform.position);
        }
    }
}

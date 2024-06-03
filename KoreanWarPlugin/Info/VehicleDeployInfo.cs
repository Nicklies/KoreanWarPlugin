using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.KWSystem;
using Rocket.Unturned.Chat;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KoreanWarPlugin.EnumTable;

namespace KoreanWarPlugin.Info
{
    public class VehicleDeployInfo
    {
        public VehiclePresetTable vPreset;
        public VehicleTypePresetTable vTypePreset;
        public uint vehicleInstanceID;
        public InteractableVehicle vehicle;
        public byte vTypeIndex; // 해당 차량 그룹의 차량 종류 인덱스
        public EDeployVehicleState state;
        public bool isStateChanged; // 상태가 바뀌었는지 여부 / 상태가 바뀔 때에만 (예) 배치상태에서 -> 파괴된 상태로 전환됨) 해당 상태에 맞게 유저들의 UI 를 갱신해줌
        public bool isSupplyable { get; private set; } // 기지에서 보급 가능한지 여부
        public bool isSupplying; // 현재 보급중인 상태인지 여부
        public bool isSupplied; // 기지내에서 이미 보급이 됬는지 여부 / 보급된 후 기지를 나갈 때까지 true 로 유지
        public int supplyElapsedTime; // 보급진행중 남은시간
        public DateTime supplyCooltime; // 보급 쿨타임
        public DateTime removeDateTime;
        public string killerName; // 파괴될 경우 파괴한 유저의 이름 / 체력이 0 이 될때 저장되어 사용됨
        public Coroutine supplyCoroutine; // 보급 코르틴
        public VehicleDeployInfo(InteractableVehicle _vehicle,byte _vTypeIndex,VehiclePresetTable _vPreset,VehicleTypePresetTable _vTypePreset, InteractableVehicle _interactableVehicle)
        {
            vehicle = _vehicle;
            vPreset = _vPreset;
            vTypePreset = _vTypePreset;
            vTypeIndex = _vTypeIndex;
            vehicleInstanceID = _interactableVehicle.instanceID;
            state = EDeployVehicleState.Normal;
            isSupplyable = _vPreset.isSupplyable;
            isSupplying = false;
            isSupplied = true;
            supplyCooltime = DateTime.UtcNow.AddSeconds(_vPreset.supplyCooltime);
            isStateChanged = false;
            removeDateTime = DateTime.MinValue;
            killerName = "";
            supplyCoroutine = null;
        }
        public static IEnumerator Cor_Supply(VehicleDeployInfo _vDeployInfo) // 차량 재보급 코르틴
        {
            _vDeployInfo.supplyElapsedTime = 10;
            while (_vDeployInfo.supplyElapsedTime >= 0)
            {
                if (!PluginManager.instance.isRoundStart) yield break;
                IngameSystem.RefreshVehicleSupplyInfoToCrews(_vDeployInfo);
                yield return new WaitForSeconds(1f);
                _vDeployInfo.supplyElapsedTime--;
            }
            if (!PluginManager.instance.isRoundStart) yield break;
            if(_vDeployInfo.vPreset.isSupplyable) DeploySystem.GiveVehicleAmmo(_vDeployInfo.vehicle, _vDeployInfo.vPreset, true);
            _vDeployInfo.vehicle.askRepair(_vDeployInfo.vehicle.asset.healthMax);
            foreach (Passenger passenger in _vDeployInfo.vehicle.passengers)
            {
                if (passenger.player == null) continue;
                ITransportConnection tc = passenger.player.transportConnection;
                EffectManager.sendUIEffectVisibility(47, tc, false, "L_VehicleSupply", false);
                if (passenger.turret == null) continue;
                ItemGunAsset gunAsset = new Item(passenger.turret.itemID, true).GetAsset<ItemGunAsset>();
                if (gunAsset == null) continue;
                IngameSystem.UpdateUIGun_RestAmmo(tc, gunAsset.magazineCalibers, passenger.player.player.inventory);
            }
            _vDeployInfo.supplyCooltime = DateTime.UtcNow.AddSeconds(_vDeployInfo.vPreset.supplyCooltime);
            _vDeployInfo.isSupplying = false;
            _vDeployInfo.isSupplied = true;
        }
    }
}

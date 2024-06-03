using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Data;
using KoreanWarPlugin.Info;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace KoreanWarPlugin.KWSystem
{
    public class LoadoutSystem
    {
        public static void SetLoadoutType_Inventory(ITransportConnection _tc) // 인벤토리 장비 선택 UI창으로 변환
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_LoadoutType_0Block", true);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_LoadoutType_1Block", false);
        }
        public static void SetLoadoutType_Vehicle(ITransportConnection _tc) // 차량 장비 선택 UI창으로 변환
        {
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_LoadoutType_0Block", false);
            EffectManager.sendUIEffectVisibility(47, _tc, false, "P_LoadoutType_1Block", true);
        }
        public static void AssignDefaultInventory(PlayerInfo _playerInfo, ITransportConnection _tc, bool _team) // 병과 선택 후 기본 로드아웃 정보 세팅
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            ClassPresetTable classPreset = null;
            switch (_playerInfo.classType)
            {
                case EnumTable.EClassType.infantary:
                    classPreset = _team ? PluginManager.teamInfo.team_0_ClassInf[_playerInfo.classIndex].presetInfo : PluginManager.teamInfo.team_1_ClassInf[_playerInfo.classIndex].presetInfo;
                    break;
                case EnumTable.EClassType.driver:
                    classPreset = _team ? PluginManager.teamInfo.team_0_ClassDriver[_playerInfo.classIndex].presetInfo : PluginManager.teamInfo.team_1_ClassDriver[_playerInfo.classIndex].presetInfo;
                    break;
            }
            // 원활한 보급 포인트 계산을 위해 정보 초기화
            _playerInfo.primaryIndex = ushort.MaxValue;
            _playerInfo.sightIndex = ushort.MaxValue;
            _playerInfo.tacticalIndex = ushort.MaxValue;
            _playerInfo.magazineIndex = ushort.MaxValue;
            _playerInfo.gripIndex = ushort.MaxValue;
            _playerInfo.secondaryIndex = ushort.MaxValue;
            _playerInfo.explosive_0Index = ushort.MaxValue;
            _playerInfo.explosive_1Index = ushort.MaxValue;
            _playerInfo.equipmentIndex = ushort.MaxValue;
            _playerInfo.utility_0Index = ushort.MaxValue;
            _playerInfo.utility_1Index = ushort.MaxValue;
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.primaryDefaultIndex, EnumTable.ELoadoutType.primary);
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.secondaryDefaultIndex, EnumTable.ELoadoutType.secondary);
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.explosive_0DefaultIndex, EnumTable.ELoadoutType.explosive_0);
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.explosive_1DefaultIndex, EnumTable.ELoadoutType.explosive_1);
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.equipmentDefaultIndex, EnumTable.ELoadoutType.equipment);
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.utility_0DefaultIndex, EnumTable.ELoadoutType.utility_0);
            RefreshLoadout_Inventory(configuration, _playerInfo, _tc, classPreset.utility_1DefaultIndex, EnumTable.ELoadoutType.utility_1);
            RefreshDeployCost(_playerInfo, _tc);
        }
        public static void RefreshLoadout_Inventory(PluginConfiguration _configuration, PlayerInfo _playerInfo, ITransportConnection _tc, ushort _index, EnumTable.ELoadoutType _type) // 장비 정보를 할당하고 UI 갱신
        {
            LoadoutTable[] loadoutList = null;
            string loadoutName = "";
            string loadoutDefaultName = "";
            ushort beforeIndex = ushort.MaxValue; // 이전에 선택됬던 병과의 인덱스
            switch (_type)
            {
                case EnumTable.ELoadoutType.primary:
                    EffectManager.sendUIEffectVisibility(47, _tc, false, "P_PrimaryCredit", false); // 크레딧 UI 비활성화
                    loadoutList = _configuration.primaryPresets;
                    beforeIndex = _playerInfo.primaryIndex;
                    loadoutName = "Primary";
                    loadoutDefaultName = "주무기";
                    _playerInfo.primaryIndex = _index;
                    break;
                case EnumTable.ELoadoutType.sight:
                    loadoutList = _configuration.attachmentPresets;
                    beforeIndex = _playerInfo.sightIndex;
                    loadoutName = "Sight";
                    loadoutDefaultName = "조준경";
                    _playerInfo.sightIndex = _index;
                    break;
                case EnumTable.ELoadoutType.tactical:
                    loadoutList = _configuration.attachmentPresets;
                    beforeIndex = _playerInfo.tacticalIndex;
                    loadoutName = "Tactical";
                    loadoutDefaultName = "전술장비";
                    _playerInfo.tacticalIndex = _index;
                    break;
                case EnumTable.ELoadoutType.magazine:
                    loadoutList = _configuration.attachmentPresets;
                    beforeIndex = _playerInfo.magazineIndex;
                    loadoutName = "Magazine";
                    loadoutDefaultName = "탄창";
                    _playerInfo.magazineIndex = _index;
                    break;
                case EnumTable.ELoadoutType.grip:
                    loadoutList = _configuration.attachmentPresets;
                    beforeIndex = _playerInfo.gripIndex;
                    loadoutName = "Grip";
                    loadoutDefaultName = "손잡이";
                    _playerInfo.gripIndex = _index;
                    break;
                case EnumTable.ELoadoutType.secondary:
                    loadoutList = _configuration.secondaryPresets;
                    beforeIndex = _playerInfo.secondaryIndex;
                    loadoutName = "Secondary";
                    loadoutDefaultName = "보조무기";
                    _playerInfo.secondaryIndex = _index;
                    break;
                case EnumTable.ELoadoutType.explosive_0:
                    loadoutList = _configuration.explosivePresets;
                    beforeIndex = _playerInfo.explosive_0Index;
                    loadoutName = "Explosive_0";
                    loadoutDefaultName = "폭팔물1";
                    _playerInfo.explosive_0Index = _index;
                    break;
                case EnumTable.ELoadoutType.explosive_1:
                    loadoutList = _configuration.explosivePresets;
                    beforeIndex = _playerInfo.explosive_1Index;
                    loadoutName = "Explosive_1";
                    loadoutDefaultName = "폭팔물2";
                    _playerInfo.explosive_1Index = _index;
                    break;
                case EnumTable.ELoadoutType.equipment:
                    loadoutList = _configuration.equipmentPresets[_configuration.classPresets[_playerInfo.classPrestIndex].equipmentInstanceID].clothPresetList;
                    beforeIndex = _playerInfo.equipmentIndex;
                    loadoutName = "Equip";
                    loadoutDefaultName = "장구류";
                    _playerInfo.equipmentIndex = _index;
                    break;
                case EnumTable.ELoadoutType.utility_0:
                    loadoutList = _configuration.utilityPresets;
                    beforeIndex = _playerInfo.utility_0Index;
                    loadoutName = "Utility_0";
                    loadoutDefaultName = "특수장비1";
                    _playerInfo.utility_0Index = _index;
                    break;
                case EnumTable.ELoadoutType.utility_1:
                    loadoutList = _configuration.utilityPresets;
                    beforeIndex = _playerInfo.utility_1Index;
                    loadoutName = "Utility_1";
                    loadoutDefaultName = "특수장비2";
                    _playerInfo.utility_1Index = _index;
                    break;
            }
            if (_index != ushort.MaxValue) // 장비 선택 시
            {
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_{loadoutName}", $"{loadoutList[_index].iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_{loadoutName}Name", $"{loadoutList[_index].name}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_{loadoutName}Cost", $"{loadoutList[_index].supplyCost}");
                if (loadoutList[_index].creditCost != 0) // 크레딧 요구할 시
                {
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_{loadoutName}CreditCost", $"{loadoutList[_index].creditCost}");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_{loadoutName}Credit", true); // 크레딧 UI 활성화
                }
                if (beforeIndex == ushort.MaxValue) EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_{loadoutName}OnEquip", true);
                else 
                { 
                    //_playerInfo.supplyPoint -= loadoutList[beforeIndex].supplyCost;
                    //_playerInfo.creditPoint -= loadoutList[beforeIndex].creditCost;
                }
                // 비용 정산
                //_playerInfo.supplyPoint += loadoutList[_index].supplyCost;
                //_playerInfo.creditPoint += loadoutList[_index].creditCost;

                if (_type == EnumTable.ELoadoutType.primary) // 총 선택 시
                {
                    // 기본 부착물 추가
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, _configuration.primaryPresets[_index].sightDefaultIndex, EnumTable.ELoadoutType.sight);
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, _configuration.primaryPresets[_index].tacticalDefaultIndex, EnumTable.ELoadoutType.tactical);
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, _configuration.primaryPresets[_index].magazineDefaultIndex, EnumTable.ELoadoutType.magazine);
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, _configuration.primaryPresets[_index].gripDefaultIndex, EnumTable.ELoadoutType.grip);
                }
                else if (_type == EnumTable.ELoadoutType.equipment) // 장구류 선택 시
                {
                    if(_index == 0) // 비무장 선택 시
                    {
                        if(_playerInfo.explosive_0Index != ushort.MaxValue) RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.explosive_0);
                        if(_playerInfo.explosive_1Index != ushort.MaxValue) RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.explosive_1);
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Explosive_0_Block", true);
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Explosive_1_Block", true);
                    }
                    else if (_index == 1)
                    {
                        if (_playerInfo.explosive_1Index != ushort.MaxValue) RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.explosive_1);
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Explosive_0_Block", false);
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Explosive_1_Block", true);
                    }
                    else
                    {
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Explosive_0_Block", false);
                        EffectManager.sendUIEffectVisibility(47, _tc, false, "P_Explosive_1_Block", false);
                    }

                }
            }
            else // 제거 선택 시
            {
                EffectManager.sendUIEffectText(47, _tc, false, $"T_{loadoutName}Name", $"{loadoutDefaultName}");
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_{loadoutName}OnEquip", false);
                if (beforeIndex != ushort.MaxValue) 
                {
                    //_playerInfo.supplyPoint -= loadoutList[beforeIndex].supplyCost;
                    //_playerInfo.creditPoint -= loadoutList[beforeIndex].creditCost;
                }
                if (_type == EnumTable.ELoadoutType.primary) // 총 선택 시 부착물 제거
                {
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.sight);
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.tactical);
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.magazine);
                    RefreshLoadout_Inventory(_configuration, _playerInfo, _tc, ushort.MaxValue, EnumTable.ELoadoutType.grip);
                }
            }
            //EffectManager.sendUIEffectText(47, _tc, false, "T_Supply_Cost", $"{_playerInfo.supplyPoint}/{_playerInfo.supplyPoint_Max}");
        }
        public static void RefreshLoadout_Vehicle(ITransportConnection _tc, ushort _vPresetIndex) // 차량 정보를 할당하고 UI 갱신
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_VehicleL", $"{configuration.vehiclePresets[_vPresetIndex].iconUrl}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_VehicleLName", $"{configuration.vehiclePresets[_vPresetIndex].name}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_VehicleLCost", $"{configuration.vehiclePresets[_vPresetIndex].creditCost}");
        }
        public static void RefreshUISelectItem(PlayerInfo _playerInfo, ITransportConnection _tc, bool _team, EnumTable.ELoadoutType _type) // 아이템 선택창 UI 갱신
        {
            if (_playerInfo.primaryIndex == ushort.MaxValue) // 선택된 주무기가 없을 때 부착물 버튼을 선택했다면 리턴
                if (_type == EnumTable.ELoadoutType.sight || _type == EnumTable.ELoadoutType.tactical || _type == EnumTable.ELoadoutType.magazine || _type == EnumTable.ELoadoutType.grip) return;
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            ClassPresetTable classPreset = configuration.classPresets[_playerInfo.classPrestIndex];
            LoadoutTable[] selectItemList = null;
            ushort[] selectItemIndexList = null;
            ushort beforeItemSupplyCost = 0;
            uint beforeItemCreditost = 0;
            switch (_type)
            {
                case EnumTable.ELoadoutType.primary:
                    selectItemList = configuration.primaryPresets;
                    selectItemIndexList = classPreset.primaryList;
                    if (_playerInfo.primaryIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.primaryIndex].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.primaryIndex].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.sight:
                    selectItemList = configuration.attachmentPresets;
                    selectItemIndexList = configuration.primaryPresets[_playerInfo.primaryIndex].sights;
                    if (_playerInfo.sightIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.sightIndex].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.sightIndex].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.tactical:
                    selectItemList = configuration.attachmentPresets;
                    selectItemIndexList = configuration.primaryPresets[_playerInfo.primaryIndex].tacticals;
                    if (_playerInfo.tacticalIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.tacticalIndex].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.tacticalIndex].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.magazine:
                    selectItemList = configuration.attachmentPresets;
                    selectItemIndexList = configuration.primaryPresets[_playerInfo.primaryIndex].magazines;
                    if (_playerInfo.magazineIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.magazineIndex].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.magazineIndex].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.grip:
                    selectItemList = configuration.attachmentPresets;
                    selectItemIndexList = configuration.primaryPresets[_playerInfo.primaryIndex].grips;
                    if (_playerInfo.gripIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.gripIndex].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.gripIndex].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.secondary:
                    selectItemList = configuration.secondaryPresets;
                    selectItemIndexList = classPreset.secondaryList;
                    if (_playerInfo.secondaryIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.secondaryIndex].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.secondaryIndex].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.explosive_0:
                    selectItemList = configuration.explosivePresets;
                    selectItemIndexList = classPreset.explosiveList;
                    if (_playerInfo.explosive_0Index != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.explosive_0Index].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.explosive_0Index].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.explosive_1:
                    selectItemList = configuration.explosivePresets;
                    selectItemIndexList = classPreset.explosiveList;
                    if (_playerInfo.explosive_1Index != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.explosive_1Index].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.explosive_1Index].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.equipment:
                    selectItemList = configuration.equipmentPresets[configuration.classPresets[_playerInfo.classPrestIndex].equipmentInstanceID].clothPresetList;
                    if (_playerInfo.equipmentIndex != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.equipmentIndex].supplyCost;
                    }
                    for (int i = 0; i < selectItemList.Length; i++)
                    {
                        EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_SelectItem_{i}", true);
                        EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectItem_{i}", $"{selectItemList[i].iconUrl}");
                        EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectItem_{i}Cost", $"{selectItemList[i].supplyCost}");
                        EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectItem_{i}Name", $"{selectItemList[i].name}");
                        if (_playerInfo.supplyCost + selectItemList[i].supplyCost - beforeItemSupplyCost > _playerInfo.supplyPoint_Max) 
                        {
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_SelectItem_{i}Block_0Supply", true);
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectItem_{i}Block_0", true);
                        }
                    }
                    return;
                case EnumTable.ELoadoutType.utility_0:
                    selectItemList = configuration.utilityPresets;
                    selectItemIndexList = classPreset.utilityList;
                    if (_playerInfo.utility_0Index != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.utility_0Index].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.utility_0Index].creditCost;
                    }
                    break;
                case EnumTable.ELoadoutType.utility_1:
                    selectItemList = configuration.utilityPresets;
                    selectItemIndexList = classPreset.utilityList;
                    if (_playerInfo.utility_1Index != ushort.MaxValue)
                    {
                        beforeItemSupplyCost = selectItemList[_playerInfo.utility_1Index].supplyCost;
                        beforeItemCreditost = selectItemList[_playerInfo.utility_1Index].creditCost;
                    }
                    break;
            }
            for (int i = 0; i < selectItemIndexList.Length; i++)
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_SelectItem_{i}", true);
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectItem_{i}", $"{selectItemList[selectItemIndexList[i]].iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectItem_{i}Cost", $"{selectItemList[selectItemIndexList[i]].supplyCost}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectItem_{i}Name", $"{selectItemList[selectItemIndexList[i]].name}");
                if (selectItemList[selectItemIndexList[i]].creditCost != 0)
                {
                    EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectItem_{i}CreditCost", $"{selectItemList[selectItemIndexList[i]].creditCost}");
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectItem_{i}Credit", true);
                }
                PlayerData data = PluginManager.playerDatabase.FindData(_playerInfo.cSteamID);
                // 보급, 크레딧이 부족한 경우 클릭 막기
                if (_playerInfo.supplyCost + selectItemList[selectItemIndexList[i]].supplyCost - beforeItemSupplyCost > _playerInfo.supplyPoint_Max)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_SelectItem_{i}Block_0Supply", true);
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectItem_{i}Block_0", true);
                }
                else if (selectItemList[selectItemIndexList[i]].creditCost != 0
                    && _playerInfo.creditCost + selectItemList[selectItemIndexList[i]].creditCost - beforeItemCreditost > data.credit)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_SelectItem_{i}Block_0Credit", true);
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectItem_{i}Block_0", true);
                }
                else if (!selectItemList[selectItemIndexList[i]].isDuplicatable)
                {
                    if (_type == EnumTable.ELoadoutType.explosive_0 || _type == EnumTable.ELoadoutType.explosive_1)
                    {
                        if (_playerInfo.explosive_0Index == selectItemIndexList[i] || _playerInfo.explosive_1Index == selectItemIndexList[i])
                        {
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_SelectItem_{i}Block_0Duplicate", true);
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectItem_{i}Block_0", true);
                        }
                    }
                    else if (_type == EnumTable.ELoadoutType.utility_0 || _type == EnumTable.ELoadoutType.utility_1)
                    {
                        if (_playerInfo.utility_0Index == selectItemIndexList[i] || _playerInfo.utility_1Index == selectItemIndexList[i])
                        {
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"I_SelectItem_{i}Block_0Duplicate", true);
                            EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectItem_{i}Block_0", true);
                        }
                    }
                }
            }
        }
        public static void RefreshUISelectVehicle(PlayerInfo _playerInfo,VehicleGroupInfo _vGroupInfo, ITransportConnection _tc, bool _team) // 차량 선택창 UI 갱신
        {
            VehiclePresetTable[] vehiclePresetList = PluginManager.instance.Configuration.Instance.vehiclePresets;
            ushort[] selectVehicleIndexList = _vGroupInfo.vehicleTypePreset.vehicleList;
            for (int i = 0; i < selectVehicleIndexList.Length; i++)
            {
                EffectManager.sendUIEffectVisibility(47, _tc, false, $"BP_SelectVehicle_{i}", true);
                EffectManager.sendUIEffectImageURL(47, _tc, false, $"I_SelectVehicle_{i}", $"{vehiclePresetList[selectVehicleIndexList[i]].iconUrl}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectVehicle_{i}Cost", $"{vehiclePresetList[selectVehicleIndexList[i]].creditCost}");
                EffectManager.sendUIEffectText(47, _tc, false, $"T_SelectVehicle_{i}Name", $"{vehiclePresetList[selectVehicleIndexList[i]].name}");
                uint beforeItemCreditCost = PluginManager.instance.Configuration.Instance.vehiclePresets[_vGroupInfo.vPresetIndex].creditCost;
                PlayerData data = PluginManager.playerDatabase.FindData(_playerInfo.cSteamID);
                if (vehiclePresetList[selectVehicleIndexList[i]].creditCost != 0
                    && _playerInfo.creditCost + vehiclePresetList[selectVehicleIndexList[i]].creditCost - beforeItemCreditCost > data.credit)
                {
                    EffectManager.sendUIEffectVisibility(47, _tc, false, $"P_SelectVehicle_{i}Block", true);
                }
            }
        }
        public static void RefreshDeployCost(PlayerInfo _playerInfo, ITransportConnection _tc) // 전투 배치 시 필요한 비용 갱신
        {
            PluginConfiguration configuration = PluginManager.instance.Configuration.Instance;
            ushort supplyCost = 0;
            uint creditCost = 0;
            if (_playerInfo.primaryIndex != ushort.MaxValue) // 주무기 비용
            {
                supplyCost += configuration.primaryPresets[_playerInfo.primaryIndex].supplyCost;
                creditCost += configuration.primaryPresets[_playerInfo.primaryIndex].creditCost;
            }
            if (_playerInfo.sightIndex != ushort.MaxValue) // 조준경 비용
            {
                supplyCost += configuration.attachmentPresets[_playerInfo.sightIndex].supplyCost;
                creditCost += configuration.attachmentPresets[_playerInfo.sightIndex].creditCost;
            }
            if (_playerInfo.tacticalIndex != ushort.MaxValue) // 전술장비 비용
            {
                supplyCost += configuration.attachmentPresets[_playerInfo.tacticalIndex].supplyCost;
                creditCost += configuration.attachmentPresets[_playerInfo.tacticalIndex].creditCost;
            }
            if (_playerInfo.magazineIndex != ushort.MaxValue) // 탄창 비용
            {
                supplyCost += configuration.attachmentPresets[_playerInfo.magazineIndex].supplyCost;
                creditCost += configuration.attachmentPresets[_playerInfo.magazineIndex].creditCost;
            }
            if (_playerInfo.gripIndex != ushort.MaxValue) // 손잡이 비용
            {
                supplyCost += configuration.attachmentPresets[_playerInfo.gripIndex].supplyCost;
                creditCost += configuration.attachmentPresets[_playerInfo.gripIndex].creditCost;
            }
            if (_playerInfo.secondaryIndex != ushort.MaxValue) // 보조무기 비용
            {
                supplyCost += configuration.secondaryPresets[_playerInfo.secondaryIndex].supplyCost;
                creditCost += configuration.secondaryPresets[_playerInfo.secondaryIndex].creditCost;
            }
            if (_playerInfo.explosive_0Index != ushort.MaxValue) // 폭팔물1 비용
            {
                supplyCost += configuration.explosivePresets[_playerInfo.explosive_0Index].supplyCost;
                creditCost += configuration.explosivePresets[_playerInfo.explosive_0Index].creditCost;
            }
            if (_playerInfo.explosive_1Index != ushort.MaxValue) // 폭팔물2 비용
            {
                supplyCost += configuration.explosivePresets[_playerInfo.explosive_1Index].supplyCost;
                creditCost += configuration.explosivePresets[_playerInfo.explosive_1Index].creditCost;
            }
            if (_playerInfo.equipmentIndex != ushort.MaxValue) // 장구류 비용
            {
                supplyCost += configuration.equipmentPresets[configuration.classPresets[_playerInfo.classPrestIndex].equipmentInstanceID].clothPresetList[_playerInfo.equipmentIndex].supplyCost;
                creditCost += configuration.equipmentPresets[configuration.classPresets[_playerInfo.classPrestIndex].equipmentInstanceID].clothPresetList[_playerInfo.equipmentIndex].creditCost;
            }
            if (_playerInfo.utility_0Index != ushort.MaxValue) // 특수장비1 비용
            {
                supplyCost += configuration.explosivePresets[_playerInfo.utility_0Index].supplyCost;
                creditCost += configuration.explosivePresets[_playerInfo.utility_0Index].creditCost;
            }
            if (_playerInfo.utility_1Index != ushort.MaxValue) // 특수장비2 비용
            {
                supplyCost += configuration.explosivePresets[_playerInfo.utility_1Index].supplyCost;
                creditCost += configuration.explosivePresets[_playerInfo.utility_1Index].creditCost;
            }
            if(_playerInfo.vGroupInstanceID != ushort.MaxValue)
            {
                VehicleGroupInfo vGroupInfo = PluginManager.teamInfo.GetVehicleGroupInfo(_playerInfo.vGroupInstanceID, _playerInfo.team);
                if(vGroupInfo.leaderID == _playerInfo.cSteamID) creditCost += configuration.vehiclePresets[vGroupInfo.vPresetIndex].creditCost;
            }
            _playerInfo.supplyCost = supplyCost;
            _playerInfo.creditCost = creditCost;
            string supplyColor = _playerInfo.supplyCost > _playerInfo.supplyPoint_Max ? "#FF372D" : "White";
            PlayerData playerData = PluginManager.playerDatabase.FindData(_playerInfo.cSteamID);
            string creditColor = _playerInfo.creditCost > playerData.credit ? "#FF372D" : "#F0D41E";
            EffectManager.sendUIEffectText(47, _tc, false, "T_Supply_Cost", $"<color={supplyColor}>{supplyCost}/{_playerInfo.supplyPoint_Max}</color>");
            EffectManager.sendUIEffectText(47, _tc, false, "T_Credit_Cost", $"<color={creditColor}>{creditCost}</color>");
        }
        public static void RefreshUICredit(ITransportConnection _tc,PlayerData _data)
        {
            EffectManager.sendUIEffectText(47, _tc, false, "T_Point_Cost", $"{_data.credit}");
        }
    }
}

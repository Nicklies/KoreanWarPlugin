using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Configuration.Preset
{
    public class VehiclePresetTable : LoadoutTable
    {
        public SeatPresetTable[] seats; // 좌석 정보
        public VAmmoPresetTable[] ammos; // 탄약 정보
        public bool isDeployable = false;
        public bool isSupplyable; // 기지에서 보급 가능한지 여부
        public int supplyCooltime; // 보급 쿨타임
    }
    public class SeatPresetTable
    {
        public string iconUrl;
        public string name;
    }
    public class VAmmoPresetTable
    {
        public ushort itemID;
        public byte amount;
    }
}

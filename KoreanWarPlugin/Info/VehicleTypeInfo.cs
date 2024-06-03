using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoreanWarPlugin.Configuration.Preset;

namespace KoreanWarPlugin.Info
{
    public class VehicleTypeInfo // 차량종류 정보
    {
        public VehicleTypePresetTable presetInfo;
        public byte vehicleCount; // 배치된 차종 개수
        public VehicleTypeInfo(VehicleTypePresetTable _preset)
        {
            presetInfo = _preset;
            vehicleCount = 0;
        }
    }
}
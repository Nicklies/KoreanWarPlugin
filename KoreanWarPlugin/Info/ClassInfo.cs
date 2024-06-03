using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.KWSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Info
{
    public class ClassInfo
    {
        public ClassPresetTable presetInfo;
        public byte maxPlayerCount;
        public byte playerCount;

        public ClassInfo(ClassPresetTable _presetTable)
        {
            presetInfo = _presetTable;
            maxPlayerCount = _presetTable.playerMax;
            playerCount = 0;
        }
        public void UpdateVariable(int _playerCount, byte _index, bool _team) // 병과 최대 유저 수 갱신
        {
            byte beforePlayerCount = maxPlayerCount;
            maxPlayerCount = (byte)(presetInfo.playerMax + ((float)_playerCount / (float)PluginManager.instance.Configuration.Instance.classInterval));
            if (beforePlayerCount != maxPlayerCount) { ClassSystem.RefreshUIClassPlayerCountToEveryone(_team, _index, presetInfo.classType); }
        }
    }
}

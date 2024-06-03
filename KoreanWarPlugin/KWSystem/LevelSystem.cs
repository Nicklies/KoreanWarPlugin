using KoreanWarPlugin.Configuration.Preset;
using KoreanWarPlugin.Info;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using System.Threading.Tasks;

namespace KoreanWarPlugin.KWSystem
{
    public class LevelSystem
    {
        public static void GainExperience(PlayerComponent _pc,UnturnedPlayer _uPlayer, PlayerTeamRecordInfo _pTeamRecordInfo, ushort _amount)
        {
            _pTeamRecordInfo.score += _amount;
            if (_pTeamRecordInfo.score >= GetExpRequireForNextLevel(_pTeamRecordInfo.level, out bool _isMaxLevel)) // 다음 레벨까지 갈 수있는 경험치가 있다면
            {
                if(!_isMaxLevel && !_isMaxLevel) LevelUp(_uPlayer,_pTeamRecordInfo);
            }
            if(_pc.uIState == EnumTable.EPlayerUIState.Loadout)
            {
                ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
                RefreshUIExperience(_pTeamRecordInfo, tc);
            }
        }
        public static void LevelUp(UnturnedPlayer _uPlayer, PlayerTeamRecordInfo _pTeamRecordInfo)
        {
            PlayerComponent pc = _uPlayer.Player.GetComponent<PlayerComponent>();
            ITransportConnection tc = _uPlayer.Player.channel.GetOwnerTransportConnection();
            ushort expRequire = GetExpRequireForNextLevel(_pTeamRecordInfo.level,out bool _isMaxLevel);
            if (_isMaxLevel) return;
            _pTeamRecordInfo.level++;
            if (_pTeamRecordInfo.score < expRequire) // 경험치를 충족하지 않은 상태에서 레벨업을 했자면 최소 경험치량을 배정
            {
                _pTeamRecordInfo.score = expRequire;
            }
            // 레벨업 UI 띄우기
            if (pc.uIState == EnumTable.EPlayerUIState.InGame)
            {
                LevelPreset levelPreset = pc.team ? PluginManager.teamInfo.teamPreset_0.levelPresets[_pTeamRecordInfo.level] : PluginManager.teamInfo.teamPreset_1.levelPresets[_pTeamRecordInfo.level];
                EffectManager.sendUIEffectImageURL(47, tc, false, "I_LevelUp", $"{levelPreset.iconUrl}");
                EffectManager.sendUIEffectText(47, tc, false, "T_LevelUp_Rank", $"{levelPreset.name}");
                EffectManager.sendUIEffectVisibility(47, tc, false, "A_LevelUp", true);
            }
            else if (pc.uIState == EnumTable.EPlayerUIState.Loadout)
            {
                RefreshUILevel(_pTeamRecordInfo, tc, pc.team);
                PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(_uPlayer.CSteamID);
                ClassSystem.RefreshSupplyCost(playerInfo, _pTeamRecordInfo);
                LoadoutSystem.RefreshDeployCost(playerInfo, tc);
                VehicleGroupSystem.RefreshVehicleTypeStateAll(tc, pc.team, _uPlayer); // 차량 조건문 갱신
            }
        }
        public static ushort GetExpRequireForNextLevel(int _level, out bool _isMaxLevel) // 현재 레벨에서 다음레벨 달성까지 필요한 경험치량 반환
        {
            _level = Mathf.Clamp(_level, 0, PluginManager.instance.Configuration.Instance.levelExpPresets.Length + 1);
            if (PluginManager.instance.Configuration.Instance.levelExpPresets.Length <= _level)
            {
                _isMaxLevel = true;
                return PluginManager.instance.Configuration.Instance.levelExpPresets[PluginManager.instance.Configuration.Instance.levelExpPresets.Length - 1];
            }
            else
            {
                _isMaxLevel = false;
                return PluginManager.instance.Configuration.Instance.levelExpPresets[_level];
            }

        }
        public static ushort GetExpRequireForNextLevel(int _level)
        {
            _level = Mathf.Clamp(_level, 0, PluginManager.instance.Configuration.Instance.levelExpPresets.Length + 1);
            if (PluginManager.instance.Configuration.Instance.levelExpPresets.Length <= _level) 
            {
                return PluginManager.instance.Configuration.Instance.levelExpPresets[PluginManager.instance.Configuration.Instance.levelExpPresets.Length - 1];
            }
            else
            {
                return PluginManager.instance.Configuration.Instance.levelExpPresets[_level];
            }
        }
        public static void RefreshUILevel(PlayerTeamRecordInfo _pTeamRecordInfo, ITransportConnection _tc, bool _team) // 유저에게 레벨 정보 갱신
        {
            TeamPresetTable teamPreset = _team ? PluginManager.teamInfo.teamPreset_0 : PluginManager.teamInfo.teamPreset_1;
            string iconUrl = "";
            string name = "";
            if (teamPreset.levelPresets.Length > _pTeamRecordInfo.level)
            {
                name = teamPreset.levelPresets[_pTeamRecordInfo.level].name;
                iconUrl = teamPreset.levelPresets[_pTeamRecordInfo.level].iconUrl;
            }
            else
            {
                name = teamPreset.levelPresets[teamPreset.levelPresets.Length - 1].name;
                iconUrl = teamPreset.levelPresets[teamPreset.levelPresets.Length - 1].iconUrl;
            }
            EffectManager.sendUIEffectText(47, _tc, false, "T_LevelName", $"{name}");
            EffectManager.sendUIEffectImageURL(47, _tc, false, "I_Level", $"{iconUrl}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_Level", $"LV.{_pTeamRecordInfo.level}");
        }
        public static void RefreshUIExperience(PlayerTeamRecordInfo _pTeamRecordInfo, ITransportConnection _tc)
        {
            ushort expAmount = GetExpRequireForNextLevel(_pTeamRecordInfo.level, out bool _isMaxLevel);
            ushort preExpAmount = 0;
            int experience = 0;
            string experience_str = "";
            if (!_isMaxLevel)
            {
                if (_pTeamRecordInfo.level != 0) preExpAmount = GetExpRequireForNextLevel(_pTeamRecordInfo.level - 1);
                experience = (int)((float)(_pTeamRecordInfo.score - preExpAmount) / (float)(expAmount - preExpAmount) * 100);
                experience = Mathf.Clamp(experience, 0, 100);
                experience_str = $"{_pTeamRecordInfo.score} / {expAmount}";
            }
            else 
            {
                experience = 100;
                experience_str = "Max";
            }
            string experience_TF = "";
            if (experience != 0) experience_TF = "".PadLeft(experience - 1) + ".";
            EffectManager.sendUIEffectText(47, _tc, false, "TF_Experience", $"{experience_TF}");
            EffectManager.sendUIEffectText(47, _tc, false, "T_Experience", $"{experience_str}");
        }
    }
}

using KoreanWarPlugin.Info;
using KoreanWarPlugin.KWSystem;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Windows.Input;
namespace KoreanWarPlugin.Command
{
    class ConsoleCommand{}
    class DataReloadCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "reloadData";
        public string Help => "";
        public string Syntax => "name";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            PluginManager.playerDatabase.Reload();
            Rocket.Core.Logging.Logger.Log($"PlayerDataLoaded!");
        }
    }
    class LevelUpCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "levelUp";
        public string Help => "";
        public string Syntax => "name";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1) return;

            string cmd = command[0].ToLower(); // 소문자로 변환
            var target = UnturnedPlayer.FromName(cmd);
            PlayerComponent pc = target.Player.GetComponent<PlayerComponent>();
            PlayerTeamRecordInfo pTeamRecordInfo = pc.team ? PluginManager.teamInfo.playerRecordInfoList[target.CSteamID].team_0_RecordInfo : PluginManager.teamInfo.playerRecordInfoList[target.CSteamID].team_1_RecordInfo;
            if (target != null)
            { 
                LevelSystem.LevelUp(target, pTeamRecordInfo);
                ITransportConnection tc = target.Player.channel.GetOwnerTransportConnection();
                LevelSystem.RefreshUIExperience(pTeamRecordInfo, tc);
            }
        }
    }
    class GiveCreditCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "GiveCredit";
        public string Help => "";
        public string Syntax => "name";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2) return;
            string cmd = command[0].ToLower(); // 소문자로 변환
            var target = UnturnedPlayer.FromName(cmd);
            PlayerComponent pc = target.Player.GetComponent<PlayerComponent>();
            if (target != null)
            {
                bool isNumeric = Regex.IsMatch(command[1], @"^\d+$");
                if (!isNumeric) return;
                uint value = uint.Parse(command[1]);
                PluginManager.playerDatabase.GiveCredit(target, value);
            }
        }
    }
    class RoundStartCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "startRound";
        public string Help => "";
        public string Syntax => "name";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            PluginManager.teamInfo.OnRoundStart();
            PluginManager.roundInfo.currentMapIndex = 0;
            PluginManager.roundInfo.currentMapPreset = PluginManager.instance.Configuration.Instance.mapPresets[0];
        }
    }
}

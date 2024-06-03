using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanWarPlugin.Command
{
    class PermissonCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "pos";

        public string Help => "";

        public string Syntax => "name";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            /*
            if (command.Length < 1) return;
            string cmd = command[0].ToLower(); // 소문자로 변환
            var target = UnturnedPlayer.FromName(cmd);
            if (target != null)
            {
                UnturnedChat.Say($"{target.Position}, {target.Rotation}");
                TriggerEffectParameters triggerEffect = new TriggerEffectParameters(Guid.Parse("dc3ddb05d89945088180c83bacaad4df"));
                triggerEffect.position = target.Position;
                EffectManager.triggerEffect(triggerEffect)
            }*/
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;
        }
    }
}

using HarmonyLib;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using static KoreanWarPlugin.EnumTable;
using KoreanWarPlugin.Info;
using Rocket.Unturned;
using Steamworks;
using UnityEngine;
using KoreanWarPlugin.KWSystem;
using SDG.NetTransport;

namespace KoreanWarPlugin.HarmonyFix
{
    //https://harmony.pardeike.net/articles/patching-prefix.html 하모니 정보
    //https://github.com/Unturned-Datamining/Unturned-Datamining/blob/243cd5a3e97f2eb5f842a3d35fd2f4adc3074bad/Assembly-CSharp/SDG.Unturned/UseableMelee.cs#L456 언턴드 깃허브
    // https://docs.discordnet.dev/guides/int_framework/intro.html 디스코드 연동 정보
    [HarmonyPatch(typeof(InteractableVehicle), nameof(InteractableVehicle.tryAddPlayer))]
    public class TryAddPlayerPatch
    {
        [HarmonyPrefix]
        static void TryAddPlayerPrefix(ref bool __result, InteractableVehicle __instance, ref byte seat, Player player)
        {
            if (!__instance.anySeatsOccupied) // 차량에 아무도 없다면
            {
                VehicleDeployInfo vDeployInfo = PluginManager.teamInfo.GetVehicleDeployInfo(__instance, out bool _team);
                if (vDeployInfo != null)
                {
                    vDeployInfo.isStateChanged = true;
                    vDeployInfo.state = EDeployVehicleState.Normal;
                }
            }
        }
        [HarmonyPostfix]
        static void TryAddPlayerPostfix(ref bool __result, InteractableVehicle __instance, ref byte seat, Player player)
        {
            if (!__result) return;
            PlayerComponent pc = player.GetComponent<PlayerComponent>();
            if (pc.isVehicleSpawn) // 차량스폰인지 여부
            {
                __result = false;
                for (byte i = 1; i < __instance.passengers.Length; i++)
                {
                    if (__instance.passengers[i].player == null)
                    {
                        seat = i;
                        __result = true;
                        break;
                    }
                }
                pc.isVehicleSpawn = false;
            }
            // 탑승하려는 좌석에 터렛이 있다면 차고내에 탄약 받기
            if (__instance.passengers[seat].turret != null)
            {
                ItemGunAsset gunAsset = new Item(__instance.passengers[seat].turret.itemID, false).GetAsset<ItemGunAsset>();
                if (gunAsset != null)
                {
                    IngameSystem.RefillAmmoFromVehicle(__instance, player, gunAsset);
                    ITransportConnection tc = player.channel.GetOwnerTransportConnection();
                    IngameSystem.UpdateUIGun_RestAmmo(tc, gunAsset.magazineCalibers, player.inventory, __instance);
                }
            }
        }
    } // 차량 탑승 관련 패치
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearHat), new Type[] { typeof(ItemHatAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearHatPatch
    {
        static bool Prefix(ItemHatAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearShirt), new Type[] { typeof(ItemShirtAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearShirtPatch
    {
        static bool Prefix(ItemShirtAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearPants), new Type[] { typeof(ItemPantsAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearPantsPatch
    {
        static bool Prefix(ItemPantsAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearVest), new Type[] { typeof(ItemVestAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearVestPatch
    {
        static bool Prefix(ItemVestAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearBackpack), new Type[] { typeof(ItemBackpackAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearBackpackPatch
    {
        static bool Prefix(ItemBackpackAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearGlasses), new Type[] { typeof(ItemGlassesAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearGlassesPatch
    {
        static bool Prefix(ItemGlassesAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearMask), new Type[] { typeof(ItemMaskAsset), typeof(byte), typeof(byte[]), typeof(bool) })]
    public class AskWearMaskPatch
    {
        static bool Prefix(ItemMaskAsset asset, byte quality, byte[] state, bool playEffect) { return false; }
    }
    [HarmonyPatch(typeof(PlayerLife), nameof(PlayerLife.askStarve))]
    public class AskStarvePatch
    {
        static bool Prefix(byte amount) { return false; }
    }
    [HarmonyPatch(typeof(PlayerLife), nameof(PlayerLife.askDehydrate))]
    public class AskDehydratePatch
    {
        static bool Prefix(byte amount) { return false; }
    }
    [HarmonyPatch(typeof(InteractableVehicle), nameof(InteractableVehicle.stealBattery))]
    public class StealBatteryPatch
    {
        static bool Prefix(Player player) { return false; }
    }
    [HarmonyPatch(typeof(InteractableVehicle), nameof(InteractableVehicle.askBurnFuel))]
    public class AskBurnFuelPatch
    {
        static void Prefix(InteractableVehicle __instance, ushort amount)
        {
            if (__instance.fuel <= 50)
            {
                VehicleManager.sendVehicleFuel(__instance, __instance.asset.fuelMax);
            }
        }
    }
    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.ReceiveDragItem))]
    public class ReceiveDragItemPatch
    {
        static bool Prefix(PlayerInventory __instance ,byte page_0, byte x_0, byte y_0, byte page_1, byte x_1, byte y_1, byte rot_1)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(__instance.player);
            if (uPlayer.IsInVehicle) return false;
            else return true;
        }
    }
    /*
    [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damagePlayer))]
    public class DamagePlayerPatch
    {
        static void Prefix(DamagePlayerParameters parameters, ref EPlayerKill kill)
        {
            
            if ((ulong)parameters.killer == 0) return true;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
            PlayerComponent pc = parameters.player.GetComponent<PlayerComponent>();
            if (pc.isInvincible) return false;
            if (uPlayer == null) return true;
            bool killerTeam = uPlayer.Player.GetComponent<PlayerComponent>().team;
            bool victimTeam = parameters.player.GetComponent<PlayerComponent>().team;
            if (killerTeam == victimTeam && uPlayer.CSteamID != parameters.killer) {  return false; }
            return true;
            
        }
    }
    */
    [HarmonyPatch(typeof(PlayerLife), nameof(PlayerLife.ReceiveSuicideRequest))]
    public class ReceiveSuicideRequestPatch
    {
        static bool Prefix(PlayerLife __instance)
        {
            if (!PluginManager.instance.isRoundStart) return false;
            PlayerComponent pc = __instance.player.GetComponent<PlayerComponent>();
            if (!pc.isKnockDown)
            {
                if (!pc.isJoinedTeam || pc.isRedeploying || pc.isEnterRestrictArea || pc.isInvincible) return false;
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(__instance.player);
                if (uPlayer.IsInVehicle) return false;
                PlayerInfo playerInfo = PluginManager.teamInfo.GetPlayerInfo(uPlayer.CSteamID);
                if (playerInfo == null) return false;
                if (!playerInfo.isDeployed) return false;
                pc.isRedeploying = true;
                ITransportConnection tc = uPlayer.Player.channel.GetOwnerTransportConnection();
                PluginManager.instance.StartCoroutine(IngameSystem.Cor_OnRedeploy(uPlayer, tc));
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(UseableGun), nameof(UseableGun.ReceiveAttachMagazine))]
    public class ReceiveAttachMagazinePatch
    {
        static bool Prefix(UseableGun __instance, byte page, byte x, byte y, byte[] hash)
        {
            if (!__instance.player.equipment.isTurret) return true;
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(__instance.player);
            if (!uPlayer.IsInVehicle) return true;
            bool needHammer = __instance.player.equipment.state[10] == 0 ? true : false;
            InteractableVehicle vehicle = uPlayer.CurrentVehicle;
            ushort magId = (ushort)(__instance.player.equipment.state[8] + 256 * __instance.player.equipment.state[9]);
            Item magToEject = new Item(magId, __instance.player.equipment.state[10], __instance.player.equipment.state[17]); // 제거하려는 탄창 정보
            ItemGunAsset gunAsset = new Item(__instance.player.equipment.itemID, false).GetAsset<ItemGunAsset>();
            ItemMagazineAsset magAsset = magToEject.GetAsset<ItemMagazineAsset>();
            byte newAmount = __instance.player.equipment.state[10];
            byte index = __instance.player.inventory.getIndex(page, x, y);
            if (page != byte.MaxValue)
            {
                ItemJar magToReload = __instance.player.inventory.getItem(page, index); // 장전하려는 탄창 정보
                // 새로운 탄 장전
                __instance.player.inventory.removeItem(page, index);
                Buffer.BlockCopy(BitConverter.GetBytes(magToReload.item.id), 0, __instance.player.equipment.state, 8, 2);
                __instance.player.equipment.state[10] = magToReload.item.amount;
                __instance.player.equipment.state[17] = magToReload.item.quality;
                __instance.player.equipment.sendUpdateState();
            }
            else
            {
                __instance.player.equipment.state[8] = 0;
                __instance.player.equipment.state[9] = 0;
                __instance.player.equipment.state[10] = 0;
                __instance.player.equipment.state[17] = 0;
                __instance.player.equipment.sendUpdateState();
            }
            // 트렁크에 혹은 인벤토리에 사용한 탄창 넣기
            bool empty = false;
            if (gunAsset.shouldDeleteEmptyMagazines || magAsset.deleteEmpty)
            {
                if (newAmount == 0) empty = true;
            }
            if (!empty)
            {
                if (!vehicle.trunkItems.tryAddItem(magToEject))
                {
                    __instance.player.inventory.forceAddItem(magToEject, auto: true);
                }
            }
            // 유저 인벤토리에 탄약이 있는지 체크 후 트렁크에서 꺼내 제공
            IngameSystem.RefillAmmoFromVehicle(vehicle, __instance.player, __instance.equippedGunAsset);
            __instance.ServerPlayReload(needHammer);
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerStance), nameof(PlayerStance.checkStance), new Type[] { typeof(EPlayerStance), typeof(bool) })]
    public class CheckStancePatch
    {
        static bool Prefix(PlayerStance __instance,ref EPlayerStance newStance,bool all)
        {
            /*
            PlayerComponent pc = __instance.player.GetComponent<PlayerComponent>();
            if (pc.isKnockDown)
            {
                if (newStance == EPlayerStance.STAND || newStance == EPlayerStance.CROUCH)
                {
                    newStance = EPlayerStance.PRONE;
                    UnturnedChat.Say("1");
                }
            }*/
            return true;
        }
    }
    [HarmonyPatch(typeof(PlayerStance), nameof(PlayerStance.simulate))]
    public class StanceSimulatePatch
    {
        static bool Prefix(PlayerStance __instance,uint simulation, ref bool inputCrouch, ref bool inputProne, ref bool inputSprint)
        {
            PlayerComponent pc = __instance.player.GetComponent<PlayerComponent>();
            if (pc.isKnockDown)
            {
                //inputCrouch = false;
                //inputProne = true;
                //inputSprint = false;
                __instance.checkStance(EPlayerStance.PRONE);
                __instance.ReceiveStance(EPlayerStance.PRONE);
                //UnturnedChat.Say("2");
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(ObjectManager), nameof(ObjectManager.ReceiveToggleObjectBinaryStateRequest))]
    public class ReceiveObjectBinaryStatePatch
    {
        static bool Prefix(in ServerInvocationContext context, byte x, byte y, ushort index, bool isUsed)
        {
            LevelObject levelObject = ObjectManager.getObject(x, y, index);
            if (PluginManager.instance.Configuration.Instance.supplyObject.Contains(levelObject.asset.id))
            {
                Player player = context.GetPlayer();
                PlayerComponent pc = player.GetComponent<PlayerComponent>();
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                if (pc.isKnockDown)
                {
                    return false;
                }
                if (pc.supplyCooltime > DateTime.UtcNow)
                {
                    UnturnedChat.Say(uPlayer, $"남은 보급 쿨타임 {(pc.supplyCooltime - DateTime.UtcNow).Seconds}초");
                    return false;
                }
                DeploySystem.GiveLoadout(uPlayer, true, true);
                pc.supplyCooltime = DateTime.UtcNow.AddSeconds(PluginManager.instance.Configuration.Instance.supplyCooltime_Inf);
                return false;
            }
            return true;
        }
    }
    /*
    [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.explode), new Type[] { typeof(ExplosionParameters), typeof(List<EPlayerKill>) },new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
    public class ExplodePatch
    {
        static protected CSteamID killer_Landmine = CSteamID.NonSteamGS;
        public static void Prefix(ExplosionParameters parameters, ref List<EPlayerKill> kills)
        {
            if(parameters.damageOrigin == EDamageOrigin.Trap_Explosion)
            {
                LayerMask layerMask = LayerMask.GetMask("Barricade");
                Collider[] colliders = Physics.OverlapSphere(parameters.point, .01f, layerMask);
                BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(colliders[0].transform);
                killer_Landmine = (CSteamID)drop.GetServersideData().owner;
            }
        }
        public static void Postfix(ExplosionParameters parameters, ref List<EPlayerKill> kills)
        {
            if (killer_Landmine == CSteamID.NonSteamGS) return;
            int killCount = 0;
            foreach (EPlayerKill kill in kills)
            {
                if (kill == EPlayerKill.PLAYER) killCount++;
            }
            if (PluginManager.teamInfo.team_0_PlayerRecords.ContainsKey(killer_Landmine))
            {
                UnturnedChat.Say($"{PluginManager.teamInfo.team_0_PlayerRecords[killer_Landmine].name}가 {killCount}명을 죽임");
            }
            else if (PluginManager.teamInfo.team_1_PlayerRecords.ContainsKey(killer_Landmine))
            {
                UnturnedChat.Say($"{PluginManager.teamInfo.team_1_PlayerRecords[killer_Landmine].name}가 {killCount}명을 죽임");
            }
            killer_Landmine = CSteamID.NonSteamGS;
        }
    }*/
}
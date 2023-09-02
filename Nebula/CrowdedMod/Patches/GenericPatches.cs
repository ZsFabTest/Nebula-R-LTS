using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Networking.Rpc;
using UnityEngine;

namespace CrowdedMod.Patches {
    internal static class GenericPatches {
        // I did not find a use of this method, but still patching for future updates
        // maxExpectedPlayers is unknown, looks like server code tbh
        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
        public static class InvalidOptionsPatches
        {
            public static bool Prefix(GameOptionsData __instance, [HarmonyArgument(0)] int maxExpectedPlayers)
            {
                return __instance.MaxPlayers > maxExpectedPlayers ||
                       __instance.NumImpostors < 1 ||
                       __instance.NumImpostors + 1 > maxExpectedPlayers / 2 ||
                       __instance.KillDistance is < 0 or > 2 ||
                       __instance.PlayerSpeedMod is <= 0f or > 3f;
            }
        }

        // Will be patched with signatures later when BepInEx reveals it
        // [HarmonyPatch(typeof(InnerNetServer), nameof(InnerNetServer.HandleNewGameJoin))]
        // public static class InnerNetSerer_HandleNewGameJoin
        // {
        //     public static bool Prefix(InnerNetServer __instance, [HarmonyArgument(0)] InnerNetServer.Player client)
        //     {
        //         if (__instance.Clients.Count >= 15)
        //         {
        //             __instance.Clients.Add(client);
        //
        //             client.LimboState = LimboStates.PreSpawn;
        //             if (__instance.HostId == -1)
        //             {
        //                 __instance.HostId = __instance.Clients.ToArray()[0].Id;
        //
        //                 if (__instance.HostId == client.Id)
        //                 {
        //                     client.LimboState = LimboStates.NotLimbo;
        //                 }
        //             }
        //
        //             var writer = MessageWriter.Get(SendOption.Reliable);
        //             try
        //             {
        //                 __instance.WriteJoinedMessage(client, writer, true);
        //                 client.Connection.Send(writer);
        //                 __instance.BroadcastJoinMessage(client, writer);
        //             }
        //             catch (Il2CppException exception)
        //             {
        //                 Debug.LogError("[CM] InnerNetServer::HandleNewGameJoin MessageWriter 2 Exception: " +
        //                                exception.Message);
        //                 // ama too stupid for this 
        //                 // Debug.LogException(exception.InnerException, __instance);
        //             }
        //             finally
        //             {
        //                 writer.Recycle();
        //             }
        //
        //             return false;
        //         }
        //
        //         return true;
        //     }
        // }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static class GameOptionsMenu_Start
        {
            public static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentsInChildren<NumberOption>()
                    .First(o => o.Title == StringNames.GameNumImpostors)
                    // ReSharper disable once PossibleLossOfFraction
                    .ValidRange = new FloatRange(1, 63);
            }
        }
    }
}

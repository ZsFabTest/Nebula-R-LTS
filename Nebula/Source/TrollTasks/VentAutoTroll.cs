using AmongUs.GameOptions;
using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 自动钻通风管恶搞 - 靠近通风管自动使用
    /// </summary>
    internal static class VentAutoTroll
    {
        private static float LastVent;

        public static void Update()
        {
            if (!TrollTaskManager.IsEnabled) return;
            if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.inVent || LastVent <= 0f) return;
            LastVent -= Time.deltaTime;
        }

        [HarmonyPatch(typeof(Vent))]
        private static class VentPatch
        {
            [HarmonyPatch(nameof(Vent.SetOutline))]
            [HarmonyPostfix]
            private static void SetOutlinePostfix(Vent __instance, bool on, bool mainTarget)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (!on || !mainTarget || PlayerControl.LocalPlayer.inVent || LastVent > 0f ||
                    !PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
                __instance.Use();
                LastVent = 5f;
            }
        }
    }
}

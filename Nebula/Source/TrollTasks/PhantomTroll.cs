using AmongUs.GameOptions;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 幻影恶搞 - 使用技能后隐藏按钮直到冷却结束
    /// </summary>
    internal static class PhantomTroll
    {
        [HarmonyPatch(typeof(KillButton))]
        private static class KillButtonForPhantomPatch
        {
            public static bool IsShowPhantomButton = true;

            [HarmonyPatch(nameof(KillButton.DoClick))]
            [HarmonyPostfix]
            private static void DoClickPostfix(KillButton __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (PlayerControl.LocalPlayer.Data.RoleType == RoleTypes.Phantom)
                {
                    IsShowPhantomButton = false;
                    _ = new LateTask(() =>
                    {
                        IsShowPhantomButton = true;
                    }, PlayerControl.LocalPlayer.killTimer);
                }
            }
        }

        [HarmonyPatch(typeof(AbilityButton))]
        private static class AbilityButtonForPhantomPatch
        {
            [HarmonyPatch(nameof(AbilityButton.Update))]
            [HarmonyPostfix]
            private static void UpdatePostfix(AbilityButton __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (PlayerControl.LocalPlayer.Data.RoleType != RoleTypes.Phantom) return;
                if (!KillButtonForPhantomPatch.IsShowPhantomButton)
                {
                    __instance.gameObject.SetActive(false);
                }
                else
                {
                    __instance.gameObject.SetActive(true);
                }
            }
        }
    }
}

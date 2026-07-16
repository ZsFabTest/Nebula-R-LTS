using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 扫树叶任务恶搞 - 计时器加速10-50倍
    /// </summary>
    internal static class CaliDistributarTroll
    {
        [HarmonyPatch(typeof(SweepMinigame))]
        private static class SweepMinigamePatch
        {
            [HarmonyPatch(nameof(SweepMinigame.FixedUpdate))]
            [HarmonyPrefix]
            private static void FixedUpdatePrefix(SweepMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                System.Random numer = new System.Random();
                float num = (float)numer.Next(10, 50);
                __instance.timer += Time.fixedDeltaTime * num;
            }
        }
    }
}

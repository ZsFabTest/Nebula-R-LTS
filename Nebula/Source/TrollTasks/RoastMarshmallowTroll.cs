namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 烤棉花糖任务恶搞 - 120-300秒烤制时间
    /// </summary>
    internal static class RoastMarshmallowTroll
    {
        [HarmonyPatch(typeof(RoastMarshmallowFireMinigame))]
        private static class RoastMarshmallowFireMinigamePatch
        {
            [HarmonyPatch(nameof(RoastMarshmallowFireMinigame.Begin))]
            [HarmonyPrefix]
            private static void BeginPrefix(RoastMarshmallowFireMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                System.Random rd = new System.Random();
                float timetoast = (float)rd.Next(120, 300);
                __instance.timeToToasted = timetoast;
            }
        }
    }
}

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 捡贝壳任务恶搞 - 随机4-19个贝壳
    /// </summary>
    internal static class CollectShellsTroll
    {
        [HarmonyPatch(typeof(CollectShellsMinigame))]
        private static class CollectShellsMinigamePatch
        {
            [HarmonyPatch(nameof(CollectShellsMinigame.Begin))]
            [HarmonyPrefix]
            private static void BeginPrefix(CollectShellsMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                System.Random Rd = new System.Random();
                __instance.numShellsRange = (IntRange)Rd.Next(4, 20);
            }
        }
    }
}

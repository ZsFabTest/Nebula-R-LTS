namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 节点课程任务恶搞 - 随机20-26个节点
    /// </summary>
    internal static class CourseTroll
    {
        [HarmonyPatch(typeof(CourseMinigame))]
        private static class CourseMinigamePatch
        {
            [HarmonyPatch(nameof(CourseMinigame.Begin))]
            [HarmonyPrefix]
            private static void BeginPrefix(CourseMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                System.Random rd = new System.Random();
                __instance.NumPoints = rd.Next(20, 27);
            }
        }
    }
}

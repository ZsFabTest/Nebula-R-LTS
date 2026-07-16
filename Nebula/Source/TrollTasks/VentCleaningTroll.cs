using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 清理通风口任务恶搞 - 500-800个污渍
    /// </summary>
    internal static class VentCleaningTroll
    {
        [HarmonyPatch(typeof(VentCleaningMinigame))]
        private static class VentCleaningMinigamePatch
        {
            [HarmonyPatch(nameof(VentCleaningMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(VentCleaningMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;

                Transform TaskParent = __instance.transform.parent;
                for (int i = 0; i < TaskParent.childCount; i++)
                {
                    Transform child = TaskParent.GetChild(i);
                    if (child.name == "VentDirt(Clone)") UnityEngine.Object.Destroy(child);
                }
                System.Random Rd = new System.Random();
                __instance.numberOfDirts = Rd.Next(500, 800);
                for (int i = 0; i < __instance.numberOfDirts; i++) __instance.SpawnDirt();
            }
        }
    }
}

using Object = UnityEngine.Object;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 树叶清扫任务恶搞 - 550片树叶
    /// </summary>
    internal static class LeafTroll
    {
        [HarmonyPatch(typeof(LeafMinigame))]
        private static class LeafMinigamePatch
        {
            [HarmonyPatch(nameof(LeafMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(LeafMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                int leavesNum = 550;
                __instance.MyNormTask.taskStep = 0;
                __instance.MyNormTask.MaxStep = leavesNum;
                Transform TaskParent = __instance.transform.parent;
                for (int i = 0; i < TaskParent.childCount; i++)
                {
                    Transform child = TaskParent.GetChild(i);
                    if (child.name == "o2_leaf1(Clone)") Object.Destroy(child);
                }

                __instance.Leaves = new Collider2D[leavesNum];
                for (int i = 0; i < leavesNum; i++)
                {
                    LeafBehaviour leafBehaviour = Object.Instantiate(__instance.LeafPrefab);
                    leafBehaviour.transform.SetParent(__instance.transform);
                    leafBehaviour.Parent = __instance;
                    Vector2 localPosition = __instance.ValidArea.Next();
                    leafBehaviour.transform.localPosition = new Vector3(localPosition.x, localPosition.y, -1);
                    __instance.Leaves[i] = leafBehaviour.GetComponent<Collider2D>();
                }

                GameObject pointer = new("cursor");
                pointer.transform.SetParent(__instance.transform);
                pointer.layer = 4;
                CircleCollider2D collider2D = pointer.AddComponent<CircleCollider2D>();
                collider2D.radius = 1f;
            }

            [HarmonyPatch(nameof(LeafMinigame.FixedUpdate))]
            [HarmonyPostfix]
            private static void FixedUpdatePostfix(LeafMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                var cursor = __instance.transform.FindChild("cursor");
                if (cursor) cursor.position = __instance.myController.HoverPosition;
            }
        }
    }
}

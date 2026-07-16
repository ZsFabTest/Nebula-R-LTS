using System;
using System.Linq;
using Il2CppSystem.Text;
using Random = UnityEngine.Random;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 接电任务恶搞 - 混乱的箭头和地图图标
    /// </summary>
    internal static class DivertPowerTroll
    {
        private static bool _isIntermission;
        private static readonly int Outline = Shader.PropertyToID("_Outline");

        private static PlayerTask[] PlayerTasksArray =>
            PlayerControl.LocalPlayer?.myTasks?.ToArray() ?? new PlayerTask[0];

        [HarmonyPatch(typeof(ShipStatus))]
        private static class ShipStatusPatch
        {
            [HarmonyPatch(nameof(ShipStatus.Start))]
            [HarmonyPrefix]
            private static void StartPrefix()
            {
                _isIntermission = false;
            }
        }

        [HarmonyPatch(typeof(NormalPlayerTask))]
        private static class NormalPlayerTaskPatch
        {
            [HarmonyPatch(nameof(NormalPlayerTask.NextStep))]
            [HarmonyPostfix]
            private static void NextStepPostfix(NormalPlayerTask __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (__instance.TaskType != TaskTypes.DivertPower) return;

                if (PlayerTasksArray.Count(x => x.TaskType == TaskTypes.DivertPower && !x.IsComplete) == 0)
                {
                    _isIntermission = false;
                }

                if (__instance.taskStep == __instance.MaxStep)
                {
                    Transform arrowParent = __instance.Arrow.transform.parent;
                    for (int i = 0; i < arrowParent.childCount; i++)
                    {
                        if (arrowParent.GetChild(i))
                            UnityEngine.Object.Destroy(arrowParent.GetChild(i).gameObject);
                    }
                    return;
                }

                for (int i = 0; i < 500; i++)
                {
                    _isIntermission = true;
                    GameObject arrowObject = UnityEngine.Object.Instantiate(
                        __instance.Arrow.gameObject, __instance.Arrow.transform.parent);
                    ArrowBehaviour arrowBehavior = arrowObject.GetComponent<ArrowBehaviour>();
                    arrowObject.GetComponent<SpriteRenderer>().color = TrollTaskManager.RandomColor();
                    arrowBehavior.target = new Vector2(
                        Random.RandomRange(-30f, 30f), Random.RandomRange(-30f, 30f));
                }
            }
        }

        [HarmonyPatch(typeof(DivertPowerTask))]
        private static class DivertPowerTaskPatch
        {
            [HarmonyPatch(nameof(DivertPowerTask.AppendTaskText))]
            [HarmonyPrefix]
            private static bool AppendTaskTextPrefix(DivertPowerTask __instance, StringBuilder sb)
            {
                if (!TrollTaskManager.IsEnabled) return true;

                string divertLocation = DestroyableSingleton<TranslationController>.Instance
                    .GetString(__instance.StartAt);
                switch (__instance.TaskStep)
                {
                    case 0:
                        sb.AppendLine($"{divertLocation}: {DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.DivertPower)} (0/2)");
                        break;
                    case 1:
                        sb.AppendLine($"<color=yellow>???????: {DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.AcceptDivertedPower)} (1/2)</color>");
                        break;
                    case 2:
                        return true;
                    default:
                        sb.AppendLine($"{divertLocation}: {DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.DivertPower)} (0/2)");
                        break;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(DivertPowerMinigame))]
        private static class DivertPowerMinigamePatch
        {
            [HarmonyPatch(nameof(DivertPowerMinigame.Begin))]
            [HarmonyPrefix]
            private static void BeginPrefix(DivertPowerMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                System.Random random = new();
                __instance.SliderOrder = __instance.SliderOrder
                    .OrderBy(x => random.Next()).ToArray();
            }
        }

        [HarmonyPatch(typeof(MapTaskOverlay))]
        private static class MapBehaviourPatch
        {
            [HarmonyPatch(nameof(MapTaskOverlay.Show))]
            [HarmonyPostfix]
            private static void ShowPostfix(MapTaskOverlay __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;

                int divertTasks = PlayerTasksArray.Count(x => x.TaskType is TaskTypes.DivertPower);
                if (!_isIntermission)
                {
                    if (__instance.transform.childCount <= 100) return;

                    for (int i = 0; i < __instance.transform.childCount; i++)
                    {
                        Transform child = __instance.transform.GetChild(i);
                        if (!child || !child.name.StartsWith("Divert") || !child.name.Contains("Power")) continue;
                        child.gameObject.Destroy();
                    }
                    return;
                }

                GameObject powerIndicator = null;
                Transform mapIcons = __instance.transform;

                if (mapIcons.childCount > 100)
                {
                    for (int i = 0; i < mapIcons.childCount; i++)
                    {
                        Transform child = mapIcons.GetChild(i);
                        if (!child || !child.name.StartsWith("Divert") || !child.name.Contains("Power")) continue;
                        child.GetComponent<SpriteRenderer>().material.SetFloat(Outline, 1f);
                    }
                    return;
                }

                for (int i = 0; i < 250 * divertTasks; i++)
                {
                    foreach (SpriteRenderer renderer in mapIcons.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (renderer.name.StartsWith("Divert") && renderer.name.Contains("Power"))
                        {
                            powerIndicator = renderer.gameObject;
                            break;
                        }
                    }

                    if (!powerIndicator)
                    {
                        Transform child = mapIcons.GetChild(Random.RandomRangeInt(0, mapIcons.childCount));
                        if (child && child.name.StartsWith("Divert") && child.name.Contains("Power"))
                            powerIndicator = child.gameObject;
                    }
                    if (!powerIndicator) return;

                    GameObject newIndicator = UnityEngine.Object.Instantiate(powerIndicator, mapIcons);
                    newIndicator.transform.localPosition = new Vector3(
                        Random.RandomRange(-20f, 20f),
                        Random.RandomRange(-20f, 20f),
                        newIndicator.transform.localPosition.z);
                    newIndicator.GetComponent<SpriteRenderer>().material.SetFloat(Outline, 0f);
                }

                for (int i = 0; i < mapIcons.childCount; i++)
                {
                    Transform child = mapIcons.GetChild(i);
                    if (!child || !child.name.StartsWith("Divert") || !child.name.Contains("Power")) continue;
                    child.GetComponent<SpriteRenderer>().material.SetFloat(Outline, 1f);
                }
            }
        }
    }
}

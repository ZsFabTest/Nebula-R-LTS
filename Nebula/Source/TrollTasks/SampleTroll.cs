using System;
using Il2CppSystem.Text;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 检查样本任务恶搞 - 每步24小时，计时器加速递减
    /// </summary>
    internal static class SampleTroll
    {
        [HarmonyPatch(typeof(SampleMinigame))]
        private static class SampleMinigamePatch
        {
            [HarmonyPatch(nameof(SampleMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(SampleMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                SampleMinigame.ProcessingStrings = new StringNames[]
                {
                    StringNames.DoSomethingElse, StringNames.DoSomethingElse,
                };
                __instance.TimePerStep = 86400f;
            }
        }

        [HarmonyPatch(typeof(NormalPlayerTask))]
        private static class NormalPlayerTaskPatch
        {
            [HarmonyPatch(nameof(NormalPlayerTask.AppendTaskText))]
            [HarmonyPrefix]
            private static bool AppendTaskTextPrefix(NormalPlayerTask __instance, StringBuilder sb)
            {
                if (!TrollTaskManager.IsEnabled) return true;
                if (__instance.TaskType != TaskTypes.InspectSample) return true;
                if (!__instance.ShowTaskTimer || __instance.TimerStarted != NormalPlayerTask.TimerState.Started)
                    return true;

                string startAt = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.StartAt);
                string taskType = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.TaskType);
                TimeSpan time = TimeSpan.FromSeconds((int)__instance.TaskTimer);

                string painfulCounter = (int)__instance.TaskTimer switch
                {
                    >= 3600 => $"{time.Hours}h {time.Seconds}s",
                    >= 60 => $"{time.Minutes}m {time.Seconds}s",
                    _ => $"{time.Seconds}s"
                };

                sb.AppendLine($"<color=yellow>{startAt}: {taskType} ({painfulCounter})</color>");
                return false;
            }

            [HarmonyPatch(nameof(NormalPlayerTask.FixedUpdate))]
            [HarmonyPostfix]
            private static void FixedUpdatePostfix(NormalPlayerTask __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (__instance.TaskType != TaskTypes.InspectSample) return;

                __instance.TaskTimer -= (int)__instance.TaskTimer switch
                {
                    >= 3455 => 0,
                    >= 2600 => 1.8f,
                    >= 2400 => 2.2f,
                    >= 1700 => 2.7f,
                    >= 1000 => 3.4f,
                    >= 15 => 3.7f,
                    _ => 0
                };
            }
        }
    }
}

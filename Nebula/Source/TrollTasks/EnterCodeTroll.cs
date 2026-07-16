using System;
using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 输入代码任务恶搞 - 十位数超大目标代码
    /// </summary>
    internal static class EnterCodeTroll
    {
        [HarmonyPatch(typeof(EnterCodeMinigame))]
        private static class EnterCodePatch
        {
            private static string _targetNumberString;

            [HarmonyPatch(nameof(EnterCodeMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(EnterCodeMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                System.Random random = new();
                int targetNumberFirst = random.Next(0x3B9AC9FF, int.MaxValue);
                int targetNumberLast = random.Next(0x3B9AC9FF, int.MaxValue);

                _targetNumberString = $"{targetNumberFirst}{targetNumberLast}";
                __instance.targetNumber = BitConverter.ToInt32(__instance.MyNormTask.Data, 0);
                __instance.TargetText.text = _targetNumberString;
                __instance.TargetText.transform.localPosition += Vector3.down * 0.25f;
            }

            [HarmonyPatch(nameof(EnterCodeMinigame.EnterDigit))]
            [HarmonyPrefix]
            private static bool EnterDigitPrefix(EnterCodeMinigame __instance, int i)
            {
                if (!TrollTaskManager.IsEnabled) return true;
                if (__instance.animating || __instance.done) return false;

                if (__instance.NumberText.text.Length >= __instance.TargetText.text.Length)
                {
                    if (!Constants.ShouldPlaySfx()) return false;
                    SoundManager.Instance.PlaySound(__instance.RejectSound, false, 1f);
                    return false;
                }

                if (Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySound(__instance.NumberSound, false, 1f)
                        .pitch = Mathf.Lerp(0.8f, 1.2f, i / 9f);
                }

                __instance.numString += i.ToString();

                if (__instance.numString == _targetNumberString)
                    __instance.number = __instance.targetNumber;

                __instance.NumberText.text = new string('*', __instance.numString.Length);
                __instance.NumberText.enableAutoSizing = true;

                return false;
            }
        }
    }
}

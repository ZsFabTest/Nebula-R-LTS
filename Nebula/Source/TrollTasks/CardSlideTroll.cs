using UnityEngine;
using Random = UnityEngine.Random;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 刷卡任务恶搞 - 要求像素级精度
    /// </summary>
    [HarmonyPatch(typeof(CardSlideGame))]
    internal static class CardSlideTroll
    {
        private static bool PrevState = false;

        [HarmonyPatch(nameof(CardSlideGame.Begin))]
        [HarmonyPrefix]
        private static void BeginPrefix(CardSlideGame __instance)
        {
            if (!TrollTaskManager.IsEnabled) return;
            __instance.AcceptedTime = new FloatRange(0.5f, 0.5f);
        }

        [HarmonyPatch(nameof(CardSlideGame.Update))]
        [HarmonyPrefix]
        private static void UpdatePrefix(CardSlideGame __instance)
        {
            if (!TrollTaskManager.IsEnabled) return;
            bool CurrentState = __instance.redLight.color == Color.red;
            if (PrevState == CurrentState || !CurrentState) return;
            int randomNumber = Random.RandomRangeInt(0, 40);
            if (randomNumber == 0) __instance.AcceptedTime = new FloatRange(0.25f, 2f);
            else __instance.AcceptedTime = new FloatRange(0.495f, 0.505f);
        }
    }
}

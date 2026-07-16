using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 武器（小行星）任务恶搞 - 物理混乱+鼠标光标
    /// </summary>
    internal static class WeaponsTroll
    {
        [HarmonyPatch(typeof(WeaponsMinigame))]
        private static class WeaponsMinigamePatch
        {
            [HarmonyPatch(nameof(WeaponsMinigame.Begin))]
            [HarmonyPrefix]
            private static void BeginPrefix(WeaponsMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                GameObject cursor = new("cursor");
                cursor.transform.SetParent(__instance.transform);
                cursor.layer = 4;
                CircleCollider2D circleCollider2D = cursor.AddComponent<CircleCollider2D>();
                circleCollider2D.radius = 0.52f;
                WeaponsCustom weaponsCustom = cursor.AddComponent<WeaponsCustom>();
                weaponsCustom.weaponsMinigame = __instance;
            }
        }

        [HarmonyPatch(typeof(Asteroid))]
        private static class AsteroidPatch
        {
            [HarmonyPatch(nameof(Asteroid.Reset))]
            [HarmonyPostfix]
            private static void ResetPostfix(Asteroid __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (__instance.gameObject.GetComponent<Rigidbody2D>()) return;
                Rigidbody2D rigidbody2D = __instance.gameObject.AddComponent<Rigidbody2D>();
                rigidbody2D.gravityScale = 0f;
            }
        }

        internal class WeaponsCustom : MonoBehaviour
        {
            public WeaponsMinigame weaponsMinigame;
            public WeaponsCustom(IntPtr ptr) : base(ptr) { }

            public void Update()
            {
                if (weaponsMinigame) transform.position = weaponsMinigame.myController.HoverPosition;
            }
        }
    }
}

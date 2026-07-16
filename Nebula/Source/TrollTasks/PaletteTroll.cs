using Random = UnityEngine.Random;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 调色板恶搞 - 开局随机交换三对玩家颜色
    /// </summary>
    internal static class PaletteTroll
    {
        [HarmonyPatch(typeof(ShipStatus))]
        private static class ShipStatusPatch
        {
            [HarmonyPatch(nameof(ShipStatus.Start))]
            [HarmonyPostfix]
            private static void StartPostfix()
            {
                if (!TrollTaskManager.IsEnabled) return;

                for (int i = 0; i < 3; i++)
                {
                    int from = Random.RandomRangeInt(0, Palette.PlayerColors.Length);
                    int to = Random.RandomRangeInt(0, Palette.PlayerColors.Length);

                    (Color32 main, Color32 shadow, StringNames name) = (
                        Palette.PlayerColors[to],
                        Palette.ShadowColors[to],
                        Palette.ColorNames[to]);

                    Palette.PlayerColors[to] = Palette.PlayerColors[from];
                    Palette.ShadowColors[to] = Palette.ShadowColors[from];
                    Palette.ColorNames[to] = Palette.ColorNames[from];
                    Palette.PlayerColors[from] = main;
                    Palette.ShadowColors[from] = shadow;
                    Palette.ColorNames[from] = name;
                }
            }
        }
    }
}

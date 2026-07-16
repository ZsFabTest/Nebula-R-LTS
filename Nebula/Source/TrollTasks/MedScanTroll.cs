namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 体检任务恶搞 - 随机ID和90秒等待
    /// </summary>
    internal static class MedScanTroll
    {
        public static (string id, int bloodType) PlayerData;

        [HarmonyPatch(typeof(MedScanMinigame))]
        private static class MedScanMinigamePatch
        {
            [HarmonyPatch(nameof(MedScanMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(MedScanMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                if (PlayerData == default)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int id = new System.Random().Next(0, int.MaxValue);
                        PlayerData.id += id.ToString("X").PadLeft(8, '0');
                    }
                    PlayerData.bloodType = new System.Random().Next(0, 8);
                }

                __instance.completeString =
                   "Player Identity: " + PlayerControl.LocalPlayer.Data.ColorName + " Player " + PlayerData.id +
                   "\nIdentification Number: " + PlayerData.id +
                   "\nPlayer Name: " + PlayerControl.LocalPlayer.cosmetics.nameText.text +
                   "\nHeight: 3 feet, 6 inches" +
                   "\nWeight: 92 pounds" +
                   "\nColor: " + $"{PlayerControl.LocalPlayer.Data.ColorName} " +
                   "\nBlood Type: " + MedScanMinigame.BloodTypes[PlayerData.bloodType];
                __instance.ScanDuration = 90f;
            }
        }

        [HarmonyPatch(typeof(ShipStatus))]
        private static class ShipStatusPatch
        {
            [HarmonyPatch(nameof(ShipStatus.Start))]
            [HarmonyPrefix]
            private static void StartPrefix()
            {
                PlayerData = default;
            }
        }
    }
}

using TMPro;

namespace Nebula.Patches;

[HarmonyPatch]
public static class CredentialsPatch
{
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    private static class VersionShowerPatch
    {
        static void Postfix(VersionShower __instance)
        {
            RuntimePrefabs.TextPrefab = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(__instance.text);
            RuntimePrefabs.TextPrefab.enableAutoSizing = true;
            RuntimePrefabs.TextPrefab.text = "";
            RuntimePrefabs.TextPrefab.gameObject.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(RuntimePrefabs.TextPrefab.gameObject);
        }
    }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTrackerPatch
    {
        static void Postfix(PingTracker __instance)
        {
            __instance.text.alignment = AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ? TextAlignmentOptions.Top : TextAlignmentOptions.TopLeft;
            var position = __instance.GetComponent<AspectPosition>();
            position.Alignment = AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ? AspectPosition.EdgeAlignments.Top : AspectPosition.EdgeAlignments.LeftTop;
            __instance.text.text = $"<size=130%><color=#9579ce>Nebula</color></size> v{NebulaPlugin.PluginVisualVersion}\n由<color=#F60000>四个憨批开发组</color>开发\n{__instance.text.text}";
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                position.DistanceFromEdge = new Vector3(1.8f, 0.11f, 0);
            }
            else
            {
                position.DistanceFromEdge = new Vector3(0.5f, 0.11f);
            }
            position.AdjustPosition();
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    private static class LogoPatch
    {
        private static GameObject? creditsPopup = null;
        static void Postfix(MainMenuManager __instance)
        {
            var nebulaLogo = new GameObject("bannerLogo_Nebula");
            nebulaLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            nebulaLogo.transform.localPosition = new Vector3(-0.4f, 0.5f, 5f);

            var renderer = nebulaLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = Helpers.loadSpriteFromResources("Nebula.Resources.Logo.png", 115f);

            var credentialObject = new GameObject("credentials_Nebula");
            var credentials = credentialObject.AddComponent<TextMeshPro>();
            if (Nebula.NebulaPlugin.PluginStage != null)
            {
                credentials.SetText(Nebula.NebulaPlugin.PluginStage + " v" + Nebula.NebulaPlugin.PluginVisualVersion);
            }
            else
            {
                credentials.SetText($"v{Nebula.NebulaPlugin.PluginVisualVersion}");
            }
            credentials.alignment = TMPro.TextAlignmentOptions.Center;
            credentials.fontSize *= 0.05f;

            credentials.transform.SetParent(nebulaLogo.transform);
            credentials.transform.localPosition = Vector3.down + new Vector3(0f, -0.25f, 0f);

            foreach (var obj in GameObject.FindObjectsOfType<GameObject>(true))
            {
                if (obj.name is "FreePlayButton" or "HowToPlayButton") GameObject.Destroy(obj);
            }
        }
    }
}
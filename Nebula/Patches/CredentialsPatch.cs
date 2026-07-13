using TMPro;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

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
            RuntimePrefabs.TextPrefab.gameObject.hideFlags= HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(RuntimePrefabs.TextPrefab.gameObject);
        }
    }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTrackerPatch
    {
        static void Postfix(PingTracker __instance)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
            __instance.text.text = $"<size=130%><color=#9579ce>Nebula</color></size> v{ NebulaPlugin.PluginVisualVersion }\n由<color=#FFFF00>方块の聚会</color>开发\n<size=80%>{ __instance.text.text }</size>";
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(1.2f, 0.8f, 0f);
            }
            else if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.IsDead)
            {
                __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(2.0f, 0.1f, 0f);
            }
            else
            {
                __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(1.2f, 0.1f, 0f);
            }
            __instance.gameObject.GetComponent<AspectPosition>().AdjustPosition();
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    private static class LogoPatch
    {
        private static GameObject? creditsPopup = null;

        private static void CreateCreditsPopup(MainMenuManager manager)
        {
            Transform statsPopup = manager.transform.Find("StatsPopup");
            GameObject popup = GameObject.Instantiate(statsPopup, statsPopup.parent).gameObject;
            creditsPopup = popup;

            GameObject.Destroy(popup.GetComponent<StatsPopup>());
            GameObject.Destroy(popup.transform.Find("StatNumsText_TMP").GetComponent<TextMeshPro>());

            Transform statNumsText = popup.transform.Find("StatsText_TMP");
            statNumsText.GetComponent<TextMeshPro>().text = @$"
<size=80%><size=100%><b>{Language.Language.GetString("mainmenu.creditsMenu.modDev")}</b></size>
<color=#00ffff>FangkuaiYa</color> <color=#FDC509>Imp11</color> <color=#9579ce>凛</color>


<size=90%><b>{Language.Language.GetString("mainmenu.creditsMenu.translator")}</b></color></size>
<color=#EC0C0C>四个憨批汉化组</color> - 简体中文
<color=#FDC509>Imp11</color> - English
<color=#FDC509>Dolly</color> - 日本語
</size>";
            statNumsText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.CaplineLeft;
            statNumsText.localPosition = new Vector3(-1f, 1.7f, -2f);
            statNumsText.localScale = Vector3.one * 1.25f;

            Transform statsText = GameObject.Instantiate(popup.transform.Find("StatsText_TMP"), popup.transform.Find("StatsText_TMP").transform);
            statsText.GetComponent<TextMeshPro>().text = @$"
<size=80%><size=100%><b>{Language.Language.GetString("mainmenu.creditsMenu.designer")}</b></size>
None


<size=90%><b>{Language.Language.GetString("mainmenu.creditsMenu.tester")}</b></color></size>
None
</size>";
            statsText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.CaplineRight;
            statsText.localPosition = new Vector3(1f, 0f, -2f);
            statsText.localScale = Vector3.one * 1.25f;

            Transform titleText = popup.transform.Find("Title_TMP");
            GameObject.Destroy(titleText.GetComponent<TextTranslatorTMP>());
            titleText.GetComponent<TextMeshPro>().text = $"<size=120%><b>{Language.Language.GetString("mainmenu.creditsMenu.titleText")}</b></size>";
            titleText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
            titleText.localScale = new Vector3(1.2f, 1.2f, 1f);
            titleText.localPosition = new Vector3(0f, 2.2f, -2f);

            popup.transform.Find("Background").localScale = new Vector3(1.5f, 1f, 1f);
            popup.transform.Find("CloseButton").localPosition = new Vector3(-3.75f, 2.65f, 0f);
        }

        static void Postfix(MainMenuManager __instance)
        {
            CreateCreditsPopup(__instance);

            var template = GameObject.Find("ExitGameButton");

            var buttonCredits = Object.Instantiate(template, null);
            Object.Destroy(buttonCredits.GetComponent<AspectPosition>());
            buttonCredits.transform.localPosition = new(-0.459f, -1.5f, 0);

            var textCretitsButton = buttonCredits.GetComponentInChildren<TextMeshPro>();
            textCretitsButton.transform.localPosition = new(0, 0.035f, -2);
            textCretitsButton.alignment = TextAlignmentOptions.Right;
            _ = __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
            {
                textCretitsButton?.SetText("Credits");
            })));

            PassiveButton passiveButtonCredits = buttonCredits.GetComponent<PassiveButton>();
            SpriteRenderer buttonSpriteCredits = buttonCredits.transform.FindChild("Inactive").GetComponent<SpriteRenderer>();

            passiveButtonCredits.OnClick = new ButtonClickedEvent();
            passiveButtonCredits.OnClick.AddListener((Action)(() => creditsPopup?.SetActive(true)));

            Color discordColor = Color.cyan;
            buttonSpriteCredits.color = textCretitsButton.color = discordColor;
            passiveButtonCredits.OnMouseOut.AddListener((Action)delegate
            {
                buttonSpriteCredits.color = textCretitsButton.color = discordColor;
            });

            var nebulaLogo = new GameObject("bannerLogo_Nebula");
            nebulaLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            nebulaLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);

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
            credentials.transform.localPosition = Vector3.down + new Vector3(0f, -0.2f, 0f);

            foreach (var obj in GameObject.FindObjectsOfType<GameObject>(true))
            {
                if (obj.name is "FreePlayButton" or "HowToPlayButton") GameObject.Destroy(obj);
            }
        }
    }
}
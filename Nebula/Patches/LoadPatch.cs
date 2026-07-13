using UnityEngine.SceneManagement;

namespace Nebula.Patches;

[HarmonyPriority(Priority.Last)]
[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public class LoadPatch
{
    static Sprite logoSprite = Helpers.loadSpriteFromResources("Nebula.Resources.LoadingLogo.png", 100f);
    static TMPro.TextMeshPro loadText = null!;
    public static string LoadingText { set { loadText.text = value; } }
    static IEnumerator CoLoadNebula(SplashManager __instance)
    {
        var logo = Helpers.CreateObject<SpriteRenderer>("Nebula Logo", null, new Vector3(0, 0.2f, -5f));
        logo.sprite = logoSprite;

        float p = 1f;
        while (p > 0f)
        {
            p -= Time.deltaTime * 2.8f;
            float alpha = 1 - p;
            logo.color = Color.white.AlphaMultiplied(alpha);
            logo.transform.localScale = Vector3.one * (p * p * 0.012f + 1f);
            yield return null;
        }
        logo.color = Color.white;
        logo.transform.localScale = Vector3.one;

        loadText = GameObject.Instantiate(__instance.errorPopup.InfoText, null);
        loadText.transform.localPosition = new(0f, -0.28f, -10f);
        loadText.fontStyle = TMPro.FontStyles.Bold;
        loadText.text = "Loading...";
        loadText.color = Color.white.AlphaMultiplied(0.3f);

        {
            //初期の変更
            //NebulaPlugin.InitialModification(); // 方法为空

            //CPUAffinityEditorを生成
            loadText.text = "Loading CPU Affinity Editor";
            NebulaPlugin.Instance.InstallTools();
            yield return new WaitForSeconds(0.5f);

            //アセットバンドルを読み込む
            loadText.text = "Loading Assets";
            Module.AssetLoader.Load();
            yield return new WaitForSeconds(0.5f);

            //キー入力情報を読み込む
            loadText.text = "Loading Nebula Input Manager";
            Module.NebulaInputManager.Load();
            yield return new WaitForSeconds(0.5f);

            //サーバー情報を読み込む
            //Patches.RegionMenuOpenPatch.Initialize();

            //クライアントオプションを読み込む
            loadText.text = "Building Cilent Options";
            Patches.StartOptionMenuPatch.LoadOption();
            yield return new WaitForSeconds(0.5f);

            //色データを読み込む
            loadText.text = "Building Mod Colors";
            Module.DynamicColors.Load();
            yield return new WaitForSeconds(0.5f);

            //ゲームモードデータを読み込む
            loadText.text = "Building Game Mode";
            Game.GameModeProperty.Load();
            yield return new WaitForSeconds(0.5f);

            //マップ関連のデータを読み込む
            loadText.text = "Editing Map";
            Map.MapEditor.Load();
            Map.MapData.Load();
            yield return new WaitForSeconds(0.5f);

            //オプションを読み込む
            loadText.text = "Building Mod Options";
            CustomOptionHolder.Load();
            yield return new WaitForSeconds(0.5f);

            //GlobalEventデータを読み込む
            loadText.text = "Global Event Loading data";
            Events.Events.Load();
            yield return new WaitForSeconds(0.5f);

            //ヘルプを読み込む
            loadText.text = "Loading Helps";
            Module.HelpContent.Load();
            yield return new WaitForSeconds(0.5f);

            //ゴースト情報を読み込む
            //Ghost.GhostInfo.Load();
            //Ghost.Ghost.Load();

            // Harmonyパッチ全てを適用する
            loadText.text = "Loading Harmony Patch";
            NebulaPlugin.Instance.Harmony.PatchAll();
            yield return new WaitForSeconds(0.5f);

            //RPC情報を読み込む
            loadText.text = "Loading RPC Informations";
            RemoteProcessBase.Load();
            yield return new WaitForSeconds(0.5f);


            loadText.text = "Loading Nebula Manager";
            SceneManager.sceneLoaded += (Action<Scene, LoadSceneMode>)((scene, loadMode) =>
            {
                new GameObject("NebulaManager").AddComponent<NebulaManager>();
            });
            yield return new WaitForSeconds(0.5f);
        }

        loadText.text = "Loading Completed";

        for (int i = 0; i < 3; i++)
        {
            loadText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.03f);
            loadText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.03f);
        }

        GameObject.Destroy(loadText.gameObject);

        p = 1f;
        while (p > 0f)
        {
            p -= Time.deltaTime * 1.2f;
            logo.color = Color.white.AlphaMultiplied(p);
            yield return null;
        }
        logo.color = Color.clear;


        __instance.sceneChanger.AllowFinishLoadingScene();
        __instance.startedSceneLoad = true;
    }

    static bool loadedTheOtherRoles = false;
    public static bool Prefix(SplashManager __instance)
    {
        if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > 4.2f && !loadedTheOtherRoles)
        {
            loadedTheOtherRoles = true;
            __instance.StartCoroutine(CoLoadNebula(__instance).WrapToIl2Cpp());
        }

        return false;
    }
}

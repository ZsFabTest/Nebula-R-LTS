using System.Text.Json;
using System.Text.Json.Serialization;
using AmongUs.Data;
using Assets.InnerNet;
using BepInEx;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using Twitch;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nebula.Module
{
    public class ModUpdater : MonoBehaviour
    {
        public const string RepositoryOwner = "FangkuaiYa";
        public const string RepositoryName = "Nebula-Reactivated";
        public static ModUpdater Instance { get; private set; }

        public ModUpdater(IntPtr ptr) : base(ptr) { }

        private bool _busy;
        private bool showPopUp = true;
        public List<GithubRelease> Releases;
        private List<string> _mirrorUrls;

        public void Awake()
        {
            if (Instance) Destroy(Instance);
            Instance = this;
            foreach (var file in Directory.GetFiles(Paths.PluginPath, "*.old"))
            {
                File.Delete(file);
            }
        }

        private void Start()
        {
            if (_busy) return;
            this.StartCoroutine(CoCheckForUpdate());
            SceneManager.add_sceneLoaded((System.Action<Scene, LoadSceneMode>)(OnSceneLoaded));
        }

        [HideFromIl2Cpp]
        public void StartDownloadRelease(GithubRelease release)
        {
            if (_busy) return;
            this.StartCoroutine(CoDownloadRelease(release));
        }

        [HideFromIl2Cpp]
        private IEnumerator CoFetchMirrorUrls()
        {
            _mirrorUrls = null;
            var www = UnityWebRequest.Get("https://api.amongusclub.cn/Nebula-Reactivated/GitHubURL.json");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                www.Dispose();
                yield break;
            }
            try
            {
                var json = www.downloadHandler.text;
                var data = JsonSerializer.Deserialize<MirrorData>(json);
                if (data?.mirrors != null && data.mirrors.Count > 0)
                {
                    _mirrorUrls = data.mirrors;
                }
            }
            catch { }
            www.Dispose();
        }

        [HideFromIl2Cpp]
        private IEnumerator CoCheckForUpdate()
        {
            _busy = true;
            if (Helpers.isChinese())
            {
                yield return CoFetchMirrorUrls();
            }
            var www = new UnityWebRequest();
            www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
            www.SetUrl($"https://api.github.com/repos/{RepositoryOwner}/{RepositoryName}/releases");
            www.downloadHandler = new DownloadHandlerBuffer();
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (www.isNetworkError || www.isHttpError)
            {
                yield break;
            }

            Releases = JsonSerializer.Deserialize<List<GithubRelease>>(www.downloadHandler.text);
            www.downloadHandler.Dispose();
            www.Dispose();
            Releases?.Sort(SortReleases);
            _busy = false;
        }

        [HideFromIl2Cpp]
        private IEnumerator CoDownloadRelease(GithubRelease release)
        {
            _busy = true;

            var popup = Instantiate(TwitchManager.Instance.TwitchPopup);
            popup.TextAreaTMP.fontSize *= 0.7f;
            popup.TextAreaTMP.enableAutoSizing = false;

            popup.Show();

            var button = popup.transform.GetChild(2).gameObject;
            button.SetActive(false);
            popup.TextAreaTMP.text = $"Updating Nebula-Reactivated\nPlease wait...";

            var asset = release.Assets.Find(FilterPluginAsset);
            string originalUrl = asset.DownloadUrl;

            List<string> urlsToTry = new List<string>();
            if (_mirrorUrls != null && _mirrorUrls.Count > 0)
            {
                foreach (var mirror in _mirrorUrls)
                {
                    urlsToTry.Add(mirror + originalUrl);
                }
            }
            else
            {
                urlsToTry.Add(originalUrl);
            }

            UnityWebRequest downloadRequest = null;
            foreach (var url in urlsToTry)
            {
                var www = new UnityWebRequest();
                www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
                www.SetUrl(url);
                www.downloadHandler = new DownloadHandlerBuffer();
                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    int stars = Mathf.CeilToInt(www.downloadProgress * 10);
                    string progress = $"Updating Nebula-Reactivated\nPlease wait...\nDownloading...\n{new String((char)0x25A0, stars) + new String((char)0x25A1, 10 - stars)}";
                    popup.TextAreaTMP.text = progress;
                    yield return new WaitForEndOfFrame();
                }

                if (!www.isNetworkError && !www.isHttpError)
                {
                    downloadRequest = www;
                    break;
                }

                www.downloadHandler.Dispose();
                www.Dispose();
            }

            if (downloadRequest == null)
            {
                popup.TextAreaTMP.text = "Update wasn't successful\nTry again later,\nor update manually.";
                _busy = false;
                yield break;
            }

            popup.TextAreaTMP.text = $"Updating NoS\nPlease wait...\n\nDownload complete\ncopying file...";

            var filePath = Path.Combine(Paths.PluginPath, asset.Name);

            if (File.Exists(filePath + ".old")) File.Delete(filePath + "old");
            if (File.Exists(filePath)) File.Move(filePath, filePath + ".old");

            var persistTask = File.WriteAllBytesAsync(filePath, downloadRequest.downloadHandler.GetUnstrippedData());
            var hasError = false;
            while (!persistTask.IsCompleted)
            {
                if (persistTask.Exception != null)
                {
                    hasError = true;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            downloadRequest.downloadHandler.Dispose();
            downloadRequest.Dispose();

            if (!hasError)
            {
                popup.TextAreaTMP.text = $"Nebula-Reactivated\nupdated successfully\nPlease restart the game.";
            }
            button.SetActive(true);
            _busy = false;
        }

        [HideFromIl2Cpp]
        private static bool FilterLatestRelease(GithubRelease release)
        {
            return release.IsNewer(Version.Parse(NebulaPlugin.PluginVersionForFetch)) && release.Assets.Any(FilterPluginAsset);
        }

        [HideFromIl2Cpp]
        private static bool FilterPluginAsset(GithubAsset asset)
        {
            return asset.Name == "Nebula.dll";
        }

        [HideFromIl2Cpp]
        private static int SortReleases(GithubRelease a, GithubRelease b)
        {
            if (a.IsNewer(b.Version)) return -1;
            if (b.IsNewer(a.Version)) return 1;
            return 0;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_busy || scene.name != "MainMenu") return;
            var latestRelease = Releases.FirstOrDefault();
            if (latestRelease == null || latestRelease.Version <= Version.Parse(NebulaPlugin.PluginVersionForFetch))
                return;

            var template = GameObject.Find("ExitGameButton");
            if (!template) return;

            var button = Instantiate(template, null);
            var buttonTransform = button.transform;
            button.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.458f, 0.124f);

            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                StartDownloadRelease(latestRelease);
                button.SetActive(false);
            }));

            var text = button.transform.GetComponentInChildren<TMPro.TMP_Text>();
            string t = "Update NoS";
            StartCoroutine(Effects.Lerp(0.1f, (System.Action<float>)(p => text.SetText(t))));
            passiveButton.OnMouseOut.AddListener((Action)(() => text.color = Color.red));
            passiveButton.OnMouseOver.AddListener((Action)(() => text.color = Color.white));
            var announcement = $"<size=150%>A new NEBULA-REACTIVATED update to {latestRelease.Tag} is available</size>\n{latestRelease.Description}";
            var mgr = FindObjectOfType<MainMenuManager>(true);
            if (showPopUp) mgr.StartCoroutine(CoShowAnnouncement(announcement, shortTitle: "NoS Update", date: latestRelease.PublishedAt));
            showPopUp = false;
        }

        [HideFromIl2Cpp]
        public IEnumerator CoShowAnnouncement(string announcement, bool show = true, string shortTitle = "NoS Update", string title = "", string date = "")
        {
            var mgr = FindObjectOfType<MainMenuManager>(true);
            var popUpTemplate = UnityEngine.Object.FindObjectOfType<AnnouncementPopUp>(true);
            if (popUpTemplate == null)
            {
                yield return null;
            }
            var popUp = UnityEngine.Object.Instantiate(popUpTemplate);

            popUp.gameObject.SetActive(true);

            Assets.InnerNet.Announcement creditsAnnouncement = new()
            {
                Id = "nosAnnouncement",
                Language = 0,
                Number = 6969,
                Title = title == "" ? "Nebula-Reactivated Announcement" : title,
                ShortTitle = shortTitle,
                SubTitle = "",
                PinState = false,
                Date = date == "" ? DateTime.Now.Date.ToString() : date,
                Text = announcement,
            };
            mgr.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => {
                if (p == 1)
                {
                    var backup = DataManager.Player.Announcements.allAnnouncements;
                    DataManager.Player.Announcements.allAnnouncements = new();
                    popUp.Init(false);
                    DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                    popUp.CreateAnnouncementList();
                    popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                    popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                    DataManager.Player.Announcements.allAnnouncements = backup;
                }
            })));
        }
    }

    public class MirrorData
    {
        [JsonPropertyName("mirrors")]
        public List<string> mirrors { get; set; }
    }

    public class GithubRelease
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tag_name")]
        public string Tag { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; }

        [JsonPropertyName("body")]
        public string Description { get; set; }

        [JsonPropertyName("assets")]
        public List<GithubAsset> Assets { get; set; }

        public Version Version => Version.Parse(Tag.Replace("v", string.Empty));

        public bool IsNewer(Version version)
        {
            return Version > version;
        }
    }

    public class GithubAsset
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}

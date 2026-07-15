using System.Reflection;
using Hazel;

namespace Nebula.Patches;

public class GameStartManagerPatch
{
    public static Dictionary<int, PlayerVersion> playerVersions = new Dictionary<int, PlayerVersion>();
    private static float kickingTimer = 0f;
    private static bool versionSent = false;

    public class PlayerVersion
    {
        public readonly byte[] version;
        public readonly Guid guid;

        public PlayerVersion(byte[] version, Guid guid)
        {
            this.version = version;
            this.guid = guid;
        }

        public bool Matches()
        {
            if (version.All((b) => b == 0)) return true;

            if (!Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.Equals(this.guid))
            {
                return false;
            }
            if (NebulaPlugin.Instance.PluginVersionData.Length != version.Length)
            {
                return false;
            }
            for (int i = 0; i < version.Length; i++)
            {
                if (version[i] != NebulaPlugin.Instance.PluginVersionData[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics._CoSpawnPlayer_d__42), nameof(PlayerPhysics._CoSpawnPlayer_d__42.MoveNext))]
    public class CoSpawnPlayerPatch
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer != null)
            {
                Helpers.shareGameVersion();
                PlayerControl.LocalPlayer.SetColor(PlayerControl.LocalPlayer.PlayerId);
                RPCEventInvoker.SetMyColor();
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.SetColor(player.PlayerId);
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class GameStartManagerStartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            // Trigger version refresh
            versionSent = false;
            // Reset kicking timer
            kickingTimer = 0f;
            // Copy lobby code
            string code = InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId);
            GUIUtility.systemCopyBuffer = code;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class GameStartManagerUpdatePatch
    {
        private static bool update = false;
        private static string currentText = "";
        private static int lastSetMinPlayers = -1;
        public static float startingTimer = 0;
        private static GameObject copiedStartButton;

        public static void Prefix(GameStartManager __instance)
        {
            try
            {
                if (!GameData.Instance) return;

                GameData.Instance.HandleDisconnect();

                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (player != null && player.PlayerId != player.Data.DefaultOutfit.ColorId)
                    {
                        player.SetColor(player.PlayerId);
                    }
                }
                if (!AmongUsClient.Instance.AmHost) return;
                __instance.MinPlayers = Game.GameModeProperty.GetProperty(CustomOptionHolder.GetCustomGameMode()).MinPlayers;

                if (__instance.MinPlayers != lastSetMinPlayers)
                {
                    lastSetMinPlayers = __instance.MinPlayers;
                    __instance.LastPlayerCount = -1;
                }

                update = GameData.Instance.PlayerCount != __instance.LastPlayerCount;
            }
            catch { }
        }

        public static void Postfix(GameStartManager __instance)
        {
            try
            {
                // Send version as soon as PlayerControl.LocalPlayer exists
                if (PlayerControl.LocalPlayer != null && !versionSent)
                {
                    versionSent = true;
                    Helpers.shareGameVersion();

                    PlayerControl.LocalPlayer.SetColor(PlayerControl.LocalPlayer.PlayerId);
                    AmongUs.Data.DataManager.Player.Customization.Color = PlayerControl.LocalPlayer.PlayerId;
                    RPCEventInvoker.SetMyColor();
                }

                // Host update with version handshake infos
                if (AmongUsClient.Instance.AmHost)
                {
                    bool blockStart = false;
                    string message = "";

                    foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.ToArray())
                    {
                        if (client.Character == null) continue;
                        var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                        if (dummyComponent != null && dummyComponent.enabled)
                            continue;
                        else if (!playerVersions.ContainsKey(client.Id))
                        {
                            blockStart = true;
                            message += $"<color=#FF0000FF>{Language.Language.GetString("lobby.hasNoNebula").Replace("%NAME%", client.Character.Data.PlayerName)}</color>\n";
                        }
                        else if (!NebulaOption.configDontCareMismatchedNoS.Value)
                        {
                            PlayerVersion version = playerVersions[client.Id];
                            if (!version.Matches())
                            {
                                message += $"<color=#FF0000FF>{Language.Language.GetString("lobby.hasDifferentNebula").Replace("%NAME%", client.Character.Data.PlayerName)}</color>\n";
                                blockStart = true;
                            }
                        }
                    }

                    if (blockStart)
                    {
                        __instance.GameStartText.text = message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                        __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                        __instance.GameStartTextParent.SetActive(true);
                    }
                    else
                    {
                        __instance.GameStartText.transform.localPosition = Vector3.zero;
                        __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                        if (!__instance.GameStartText.text.Contains(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")))
                        {
                            __instance.GameStartText.text = String.Empty;
                            __instance.GameStartTextParent.SetActive(false);
                        }

                        // Make starting info available to clients:
                        if (startingTimer <= 0 && __instance.startState == GameStartManager.StartingStates.Countdown)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGameStarting, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCEvents.setGameStarting();

                            // Activate Stop-Button
                            copiedStartButton = GameObject.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.gameObject.transform.parent);
                            copiedStartButton.transform.localPosition = __instance.StartButton.transform.localPosition;
                            copiedStartButton.SetActive(true);
                            var startButtonText = copiedStartButton.GetComponentInChildren<TMPro.TextMeshPro>();
                            startButtonText.text = "";
                            startButtonText.fontSize *= 0.8f;
                            startButtonText.fontSizeMax = startButtonText.fontSize;
                            startButtonText.gameObject.transform.localPosition = Vector3.zero;
                            PassiveButton startButtonPassiveButton = copiedStartButton.GetComponent<PassiveButton>();

                            void StopStartFunc()
                            {
                                __instance.ResetStartState();
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.StopStart, Hazel.SendOption.Reliable, -1);
                                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                copiedStartButton.Destroy();
                                startingTimer = 0;
                                SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
                            }
                            startButtonPassiveButton.OnClick.AddListener((Action)(() => StopStartFunc()));
                            __instance.StartCoroutine(Effects.Lerp(.1f, new System.Action<float>((p) =>
                            {
                                startButtonText.text = "";
                            })));
                        }
                    }
                }

                // Client update with handshake infos
                if (!AmongUsClient.Instance.AmHost)
                {
                    if (!playerVersions.ContainsKey(AmongUsClient.Instance.HostId) || !playerVersions[AmongUsClient.Instance.HostId].Matches())
                    {
                        kickingTimer += Time.deltaTime;
                        if (kickingTimer > 10)
                        {
                            kickingTimer = 0;
                            AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                            SceneChanger.ChangeScene("MainMenu");
                        }

                        __instance.GameStartText.text = $"<color=#FF0000FF>{Language.Language.GetString("lobby.willEliminatedByMismatchVersion").Replace("%STAY%", Math.Round(10 - kickingTimer).ToString())}</color>\n";
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                        __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                        __instance.GameStartTextParent.SetActive(true);
                    }
                    else
                    {
                        __instance.GameStartText.transform.localPosition = Vector3.zero;
                        __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                        if (!__instance.GameStartText.text.Contains(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")))
                        {
                            __instance.GameStartText.text = String.Empty;
                            __instance.GameStartTextParent.SetActive(false);
                        }
                    }
                    if (!__instance.GameStartText.text.Contains(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")) || !CustomOptionHolder.anyPlayerCanStopStart.getBool())
                        copiedStartButton?.Destroy();
                    if (CustomOptionHolder.anyPlayerCanStopStart.getBool() && copiedStartButton == null && __instance.GameStartText.text.Contains(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")))
                    {
                        // Activate Stop-Button
                        copiedStartButton = GameObject.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.gameObject.transform.parent);
                        copiedStartButton.transform.localPosition = __instance.StartButton.transform.localPosition;
                        copiedStartButton.SetActive(true);
                        var startButtonText = copiedStartButton.GetComponentInChildren<TMPro.TextMeshPro>();
                        startButtonText.text = "";
                        startButtonText.fontSize *= 0.8f;
                        startButtonText.fontSizeMax = startButtonText.fontSize;
                        startButtonText.gameObject.transform.localPosition = Vector3.zero;
                        PassiveButton startButtonPassiveButton = copiedStartButton.GetComponent<PassiveButton>();
                        startButtonPassiveButton.SetButtonEnableState(true);

                        void StopStartFunc()
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.StopStart, Hazel.SendOption.Reliable, -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            copiedStartButton.Destroy();
                            __instance.GameStartText.text = String.Empty;
                            startingTimer = 0;
                            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
                            startButtonPassiveButton.gameObject.SetActive(false);
                        }
                        startButtonPassiveButton.OnClick.AddListener((Action)(() => StopStartFunc()));
                        __instance.StartCoroutine(Effects.Lerp(.1f, new System.Action<float>((p) =>
                        {
                            startButtonText.text = "";
                        })));

                    }
                }

                // Start Timer
                if (startingTimer > 0)
                {
                    startingTimer -= Time.deltaTime;
                }

                // Lobby code replacement
                //__instance.GameRoomName.text = TheOtherRolesPlugin.StreamerMode.Value ? $"<color={TheOtherRolesPlugin.StreamerModeReplacementColor.Value}>{TheOtherRolesPlugin.StreamerModeReplacementText.Value}</color>" : lobbyCodeText;

                // Lobby timer
                if (!AmongUsClient.Instance.AmHost || !GameData.Instance) return; // Not host or no instance

                if (update) currentText = __instance.PlayerCounter.text;

                __instance.PlayerCounter.autoSizeTextContainer = true;
            }
            catch (Exception e) { }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.SetStartCounter))]
    public static class SetStartCounterPatch
    {
        public static void Postfix(GameStartManager __instance, sbyte sec)
        {
            if (sec > 0)
            {
                __instance.startState = GameStartManager.StartingStates.Countdown;
                GameStartManagerUpdatePatch.startingTimer = sec;
            }
            else
            {
                __instance.startState = GameStartManager.StartingStates.NotStarting;
                GameStartManagerUpdatePatch.startingTimer = 0;
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public class GameStartManagerBeginGame
    {
        public static int VCModeCount;

        public static bool Prefix(GameStartManager __instance)
        {
            // Block game start if not everyone has the same mod version
            bool continueStart = true;

            if (AmongUsClient.Instance.AmHost)
            {
                // Reset Settings
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ResetVaribles, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCEvents.ResetVaribles();
                //if (PlayerControl.AllPlayerControls.Count > (Game.GameModeProperty.GetProperty(CustomOptionHolder.GetCustomGameMode()).MaxPlayers ?? 15)) continueStart = false;

                foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients)
                {
                    if (client.Character == null) continue;
                    var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                    if (dummyComponent != null && dummyComponent.enabled)
                        continue;

#if !DEBUG
                    if (!playerVersions.ContainsKey(client.Id))
                    {
                        continueStart = false;
                        break;
                    }

                    if (!playerVersions[client.Id].Matches())
                    {
                        continueStart = false;
                        break;
                    }
#endif
                }

                if (CustomOptionHolder.dynamicMap.getBool() && CustomOptionHolder.mapOptions.getBool())
                {
                    // 0 = Skeld
                    // 1 = Mira HQ
                    // 2 = Polus
                    // 3 = Dleks - deactivated
                    // 4 = Airship
                    // 5 = Fungle
                    List<byte> possibleMaps = new List<byte>();
                    if (!CustomOptionHolder.exceptSkeld.getBool()) possibleMaps.Add(0);
                    if (!CustomOptionHolder.exceptMIRA.getBool()) possibleMaps.Add(1);
                    if (!CustomOptionHolder.exceptPolus.getBool()) possibleMaps.Add(2);
                    if (!CustomOptionHolder.exceptAirship.getBool()) possibleMaps.Add(4);
                    if (!CustomOptionHolder.exceptFungle.getBool()) possibleMaps.Add(5);

                    //候補が無い場合はSkeldにする
                    if (possibleMaps.Count == 0) possibleMaps.Add(0);

                    RPCEventInvoker.SetRandomMap(possibleMaps[NebulaPlugin.rnd.Next(possibleMaps.Count)]);
                }

                if (CustomOptionHolder.GetCustomGameMode() is Module.CustomGameMode.FreePlay or Module.CustomGameMode.FreePlayHnS)
                {
                    if (PlayerControl.AllPlayerControls.Count == 1)
                    {
                        int num = 6;
                        if (CustomOptionHolder.GetCustomGameMode() is Module.CustomGameMode.FreePlay)
                            num = (int)CustomOptionHolder.CountOfDummiesOption.getFloat();

                        for (int n = 0; n < num; n++)
                            Helpers.SpawnDummy();
                    }
                }

                //if (CustomOptionHolder.GetCustomGameMode() is Module.CustomGameMode.VirusCrisis)
                //{
                //    if (VCModeCount >= 3) continueStart = false;
                //    else VCModeCount++;
                //}
                //else VCModeCount = 0;
            }

            return continueStart;
        }
    }
}

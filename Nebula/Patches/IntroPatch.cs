namespace Nebula.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
static class HudManagerStartPatch
{
    private static void CleanUp(IEnumerable<Roles.Assignable> roles)
    {
        foreach (var role in roles)
        {
            try
            {
                role.CleanUp();
            }
            catch
            {
                NebulaPlugin.Instance.Logger.Print("An error has occurred in " + role.Name);
            }
        }
    }

    public static void Postfix(HudManager __instance)
    {
        CleanUp(Roles.Roles.AllRoles);
        CleanUp(Roles.Roles.AllExtraRoles);
        CleanUp(Roles.Roles.AllGhostRoles);
    }
}

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
class IntroCutsceneOnDestroyPatch
{
    public static PoolablePlayer PlayerPrefab = null;
    public static void Postfix(IntroCutscene __instance)
    {
        Expansion.GridArrangeExpansion.OnStartGame();
        CloseSpawnGUIPatch.Actions.Clear();

        PlayerPrefab = __instance.PlayerPrefab;

        if (CustomButton.OriginalVentButtonSprite) CustomButton.OriginalVentButtonSprite.hideFlags &= ~HideFlags.DontUnloadUnusedAsset;
        CustomButton.OriginalVentButtonSprite = HudManager.Instance.ImpostorVentButton.GetComponent<SpriteRenderer>().sprite;
        CustomButton.OriginalVentButtonSprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;

        Module.Information.UpperInformationManager.Initialize();
        if (CustomOptionHolder.limiterOptions.getBool())
        {
            Game.GameData.data.Timer = CustomOptionHolder.timeLimitOption.getFloat() * 60 + CustomOptionHolder.timeLimitSecondOption.getFloat();

            new Module.Information.TimeLimit();

            RPCEventInvoker.SynchronizeTimer();
        }

        new Module.Information.TextInformation(Language.Language.GetString("game.message.observerGuide")
            .Replace("%OBSERVER%", Module.NebulaInputManager.allKeyCodes[Module.NebulaInputManager.observerInput.keyCode].displayKey)
            .Replace("%LEFT%", Module.NebulaInputManager.allKeyCodes[Module.NebulaInputManager.changeEyesightLeftInput.keyCode].displayKey)
            .Replace("%RIGHT%", Module.NebulaInputManager.allKeyCodes[Module.NebulaInputManager.changeEyesightRightInput.keyCode].displayKey));

        new Objects.PlayerList(PlayerPrefab);

        Roles.Roles.StaticInitialize();

        //役職予測を初期化
        Game.GameData.data.EstimationAI.Initialize();

        foreach (Game.PlayerData player in Game.GameData.data.AllPlayers.Values)
        {
            Helpers.RoleAction(player, (role) =>
            {
                PlayerControl pc = Helpers.playerById(player.id);
                role.GlobalInitialize(pc);
                role.GlobalIntroInitialize(pc);
            });

            //遍歴に最初の役職を書き込む
            player.AddRoleHistory();
        }

        Helpers.RoleAction(PlayerControl.LocalPlayer, (role) =>
        {
            role.Initialize(PlayerControl.LocalPlayer);
            role.IntroInitialize(PlayerControl.LocalPlayer);
            role.ButtonInitialize(HudManager.Instance);
        });
        Objects.CustomButton.ButtonActivate();

        Game.GameData.data.myData.VentCoolDownTimer = PlayerControl.LocalPlayer.GetModData().role.VentCoolDownMaxTimer;

        if (AmongUsClient.Instance.AmHost)
        {
            if (Game.GameModeProperty.GetProperty(Game.GameData.data.GameMode).RequireStartCountDown)
            {
                byte count = 10;
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(10f, new System.Action<float>((p) =>
                {
                    if ((byte)((1f - p) * 10f) < count)
                    {
                        RPCEventInvoker.CountDownMessage(count);
                        count = (byte)((1f - p) * 10f);
                    }
                    if (p == 1f)
                    {
                        RPCEventInvoker.CountDownMessage(0);
                        Game.GameModeProperty.GetProperty(Game.GameData.data.GameMode).OnCountFinished.Invoke();
                    }
                })));
            }
        }

        //ボタンのガイドを表示
        var keyboardMap = Rewired.ReInput.mapping.GetKeyboardMapInstance(0, 0);
        Il2CppReferenceArray<Rewired.ActionElementMap> actionArray;
        Rewired.ActionElementMap actionMap;

        //マップ
        actionArray = keyboardMap.GetButtonMapsWithAction(4);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            Objects.CustomButton.SetKeyGuideOnSmallButton(HudManager.Instance.MapButton.gameObject, actionMap.keyCode);
            Objects.CustomButton.SetKeyGuide(HudManager.Instance.SabotageButton.gameObject, actionMap.keyCode);
        }

        //使用
        actionArray = keyboardMap.GetButtonMapsWithAction(6);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            Objects.CustomButton.SetKeyGuide(HudManager.Instance.UseButton.gameObject, actionMap.keyCode);
            Objects.CustomButton.SetKeyGuide(HudManager.Instance.PetButton.gameObject, actionMap.keyCode);
        }

        //レポート
        actionArray = keyboardMap.GetButtonMapsWithAction(7);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            Objects.CustomButton.SetKeyGuide(HudManager.Instance.ReportButton.gameObject, actionMap.keyCode);
        }

        //キル
        actionArray = keyboardMap.GetButtonMapsWithAction(8);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            Objects.CustomButton.SetKeyGuide(HudManager.Instance.KillButton.gameObject, actionMap.keyCode);
        }

        //ベント
        actionArray = keyboardMap.GetButtonMapsWithAction(50);
        if (actionArray.Count > 0)
        {
            actionMap = actionArray[0];
            Objects.CustomButton.SetKeyGuide(HudManager.Instance.ImpostorVentButton.gameObject, actionMap.keyCode);
        }

        // 防止介绍动画结束后玩家保持走路状态：清零速度、确保可移动、重置动画
        if (PlayerControl.LocalPlayer != null)
        {
            PlayerControl.LocalPlayer.moveable = true;
            PlayerControl.LocalPlayer.MyPhysics.SetNormalizedVelocity(Vector2.zero);
            PlayerControl.LocalPlayer.NetTransform.Halt();
            PlayerControl.LocalPlayer.MyPhysics.Animations.PlayIdleAnimation();
        }
    }
}

[HarmonyPatch]
class IntroPatch
{
    public static void setupIntroTeamText(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        Roles.Role role = Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId].role;

        __instance.BackgroundBar.material.color = role.introMainDisplaySide.color;
        __instance.TeamTitle.text = Language.Language.GetString("side." + role.introMainDisplaySide.localizeSide + ".name");
        __instance.TeamTitle.color = role.introMainDisplaySide.color;

        __instance.ImpostorText.text = "";
    }

    public static void setupIntroTeamMembers(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {

        Roles.Role role = Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId].role;

        yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
        Roles.Role.ExtractDisplayPlayers(ref yourTeam);
    }

    private static void PreparePlayerAppearances()
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (Game.GameData.data.AllPlayers[player.PlayerId].role.category == Roles.RoleCategory.Impostor)
            {
                DestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Impostor);
            }
            else
            {
                DestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
            }
            Game.GameData.data.AllPlayers[player.PlayerId].role.ReflectRoleEyesight(player.Data.Role);
        }
    }

    private static void SetUpRoleText(IntroCutscene __instance)
    {
        Roles.Role role = Game.GameData.data.AllPlayers[PlayerControl.LocalPlayer.PlayerId].role;

        string roleNames = Language.Language.GetString("role." + role.LocalizeName + ".name");
        Helpers.RoleAction(PlayerControl.LocalPlayer.PlayerId, (role) => { role.EditDisplayRoleName(PlayerControl.LocalPlayer.PlayerId, ref roleNames, true); });

        __instance.RoleText.text = roleNames;
        __instance.RoleText.color = role.Color;
        __instance.RoleBlurbText.text = Language.Language.GetString("role." + role.LocalizeName + ".description");
        __instance.RoleBlurbText.color = role.Color;
        __instance.YouAreText.color = role.side.color;


        //追加ロールの情報を付加
        string description = __instance.RoleBlurbText.text;
        foreach (Roles.ExtraRole exRole in Game.GameData.data.myData.getGlobalData().extraRole)
        {
            exRole.EditDescriptionString(ref description);
        }
        __instance.RoleBlurbText.text = description;

        __instance.YouAreText.gameObject.SetActive(true);
        __instance.RoleText.gameObject.SetActive(true);
        __instance.RoleBlurbText.gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.Data.Role.IntroSound, false, 1f);

        if (__instance.ourCrewmate == null)
        {
            __instance.ourCrewmate = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, false);
            __instance.ourCrewmate.gameObject.SetActive(false);
        }
        __instance.ourCrewmate.gameObject.SetActive(true);
        __instance.ourCrewmate.transform.localPosition = new Vector3(0f, -1.05f, -18f);
        __instance.ourCrewmate.transform.localScale = new Vector3(1f, 1f, 1f);
        __instance.ourCrewmate.ToggleName(false);
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    class CoBeginPatch
    {
        static bool Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
        {
            __result = CustomCoBegin(__instance).WrapToIl2Cpp();
            return false;
        }

        static IEnumerator CustomCoBegin(IntroCutscene __instance)
        {
            IntroCutscene.Instance = __instance;
            HudManager.Instance.HideGameLoader();
            SoundManager.Instance.PlaySound(__instance.IntroStinger, false, 1f, null);

            __instance.HideAndSeekPanels.SetActive(false);
            __instance.CrewmateRules.SetActive(false);
            __instance.ImpostorRules.SetActive(false);
            __instance.ImpostorName.gameObject.SetActive(false);
            __instance.ImpostorTitle.gameObject.SetActive(false);
            __instance.ImpostorText.gameObject.SetActive(false);

            PreparePlayerAppearances();

            var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            setupIntroTeamMembers(__instance, ref yourTeam);
            setupIntroTeamText(__instance, ref yourTeam);

            if (__instance.overlayHandle == null)
            {
                __instance.overlayHandle = DestroyableSingleton<DualshockLightManager>.Instance.AllocateLight();
            }
            yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();

            Roles.Role myRole = Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId].role;
            bool isImpostor = myRole.category == Roles.RoleCategory.Impostor;

            if (!isImpostor)
            {
                Vector3 position = __instance.BackgroundBar.transform.position;
                position.y -= 0.25f;
                __instance.BackgroundBar.transform.position = position;
            }
            __instance.BackgroundBar.material.SetColor("_Color", myRole.introMainDisplaySide.color);
            __instance.TeamTitle.color = myRole.introMainDisplaySide.color;
            __instance.overlayHandle.color = myRole.introMainDisplaySide.color;

            int maxDepth = isImpostor ? 1 : Mathf.CeilToInt(7.5f);
            for (int i = 0; i < yourTeam.Count; i++)
            {
                PlayerControl player = yourTeam[i];
                if (player && player.Data != null)
                {
                    PoolablePlayer p = __instance.CreatePlayer(i, maxDepth, player.Data, isImpostor);
                    if (i == 0 && player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        __instance.ourCrewmate = p;
                }
            }

            Color c = __instance.TeamTitle.color;
            Color fade = Color.black;
            Color impColor = Color.white;
            Vector3 titlePos = __instance.TeamTitle.transform.localPosition;
            float teamDuration = 3f;
            float timer = 0f;
            while (timer < teamDuration)
            {
                timer += Time.deltaTime;
                float num = Mathf.Min(1f, timer / teamDuration);
                __instance.Foreground.material.SetFloat("_Rad", __instance.ForegroundRadius.ExpOutLerp(num * 2f));
                fade.a = Mathf.Lerp(1f, 0f, num * 3f);
                __instance.FrontMost.color = fade;
                c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
                __instance.TeamTitle.color = c;
                __instance.RoleText.color = c;
                impColor.a = Mathf.Lerp(0f, 1f, (num - 0.3f) * 3f);
                __instance.ImpostorText.color = impColor;
                titlePos.y = 2.7f - num * 0.3f;
                __instance.TeamTitle.transform.localPosition = titlePos;
                __instance.overlayHandle.color = new Color(__instance.overlayHandle.color.r, __instance.overlayHandle.color.g, __instance.overlayHandle.color.b, Mathf.Min(1f, timer * 2f));
                yield return null;
            }

            timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float num2 = timer / 1f;
                fade.a = Mathf.Lerp(0f, 1f, num2 * 3f);
                __instance.FrontMost.color = fade;
                __instance.overlayHandle.color = new Color(__instance.overlayHandle.color.r, __instance.overlayHandle.color.g, __instance.overlayHandle.color.b, 1f - fade.a);
                yield return null;
            }

            SetUpRoleText(__instance);
            yield return new WaitForSeconds(2.5f);

            __instance.YouAreText.gameObject.SetActive(false);
            __instance.RoleText.gameObject.SetActive(false);
            __instance.RoleBlurbText.gameObject.SetActive(false);
            __instance.ourCrewmate.gameObject.SetActive(false);

            ShipStatus.Instance.StartSFX();
            GameObject.Destroy(__instance.gameObject);
        }
    }
}

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
class CreatePlayerPatch
{
    public static void Postfix(IntroCutscene __instance, ref PoolablePlayer __result, ref int i, ref int maxDepth, ref NetworkedPlayerInfo pData, ref bool impostorPositioning)
    {
        if (!impostorPositioning) return;

        __result.SetNameColor(Palette.ImpostorRed);
    }
}

[HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
public class CloseSpawnGUIPatch
{
    public static HashSet<System.Action> Actions = new HashSet<System.Action>();
    public static void Postfix(SpawnInMinigame __instance)
    {
        foreach (var action in Actions)
        {
            action.Invoke();
        }
        Actions.Clear();
    }
}
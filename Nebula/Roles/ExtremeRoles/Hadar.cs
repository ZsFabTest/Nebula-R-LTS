namespace Nebula.Roles.ImpostorRoles;

public class Hadar : Role
{
    private SpriteRenderer FS_PlayersSensor = null;

    private void UpdateFullScreen()
    {
        if (!PlayerControl.LocalPlayer) return;
        if (PlayerControl.LocalPlayer.GetModData() == null) return;

        if (FS_PlayersSensor == null)
        {
            FS_PlayersSensor = GameObject.Instantiate(HudManager.Instance.FullScreen, HudManager.Instance.transform);
            FS_PlayersSensor.color = Palette.ImpostorRed.AlphaMultiplied(0f);
            FS_PlayersSensor.enabled = true;
            FS_PlayersSensor.gameObject.SetActive(true);
        }

        if (!PlayerControl.LocalPlayer.GetModData().Property.UnderTheFloor)
            FS_PlayersSensor.color = Palette.ClearWhite;
        else
        {
            float sum = 0f;
            var center = PlayerControl.LocalPlayer.transform.position;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (player.Data.IsDead) continue;
                float dis = player.transform.position.Distance(center);
                if (dis < 8f)
                {
                    sum += 1f - (dis / 8f);
                }
            }
            sum *= 0.25f;
            if (sum > 1f) sum = 1f;

            FS_PlayersSensor.color = Palette.ImpostorRed.AlphaMultiplied(sum * 0.75f);
        }
    }

    static private CustomButton ventButton;
    private float lightRadius = 1f;

    private Sprite ventAppearButtonSprite = null, ventHideButtonSprite = null, auraButtonSprite = null;
    public Sprite GetVentAppearButtonSprite()
    {
        if (ventAppearButtonSprite) return ventAppearButtonSprite;
        ventAppearButtonSprite = Helpers.loadSpriteFromResources("Nebula.Resources.HadarAppearButton.png", 115f);
        return ventAppearButtonSprite;
    }

    public Sprite GetVentHideButtonSprite()
    {
        if (ventHideButtonSprite) return ventHideButtonSprite;
        ventHideButtonSprite = Helpers.loadSpriteFromResources("Nebula.Resources.HadarHideButton.png", 115f);
        return ventHideButtonSprite;
    }

    public Sprite GetAuraButtonSprite()
    {
        if (auraButtonSprite) return auraButtonSprite;
        auraButtonSprite = Helpers.loadSpriteFromResources("Nebula.Resources.ArrestButton.png", 115f);
        return auraButtonSprite;
    }

    private Module.CustomOption DisappearanceOption;
    private Module.CustomOption ReappearanceOption;

    public override void LoadOptionData()
    {
        base.LoadOptionData();

        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        
        DisappearanceOption = CreateOption(Color.white, "disappearance", 20f, 5f, 45f, 2.5f);
        DisappearanceOption.suffix = "second";

        ReappearanceOption = CreateOption(Color.white, "reappearance", 5f, 3f, 15f, 1f);
        ReappearanceOption.suffix = "second";
    }


    public override void Initialize(PlayerControl __instance)
    {
        lightRadius = 1f;
    }

    private CustomButton killButton;

    public override void ButtonInitialize(HudManager __instance)
    {
        if (ventButton != null)
        {
            ventButton.Destroy();
        }
        ventButton = new CustomButton(
            () =>
            {
                var property = PlayerControl.LocalPlayer.GetModData().Property;

                    //ダメージを与える
                    if (property.UnderTheFloor)
                {
                    ventButton.Timer = DisappearanceOption.getFloat();
                }
                else
                {
                    ventButton.Timer = ReappearanceOption.getFloat();
                }

                ventButton.SetLabel(property.UnderTheFloor ?
                    "button.label.hadar.hide" : "button.label.hadar.appear");
                ventButton.Sprite = property.UnderTheFloor ?
                    GetVentHideButtonSprite() : GetVentAppearButtonSprite();
                RPCEventInvoker.UndergroundAction(!property.UnderTheFloor);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () => { ventButton.Timer = ventButton.MaxTimer; },
            GetVentHideButtonSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.hadar.hide"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        ventButton.MaxTimer = ventButton.Timer = 0f;

        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                killButton.Timer = killButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Roles.SchrodingersCat.canUseKillButton.getBool(); },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.GetModData().Property.UnderTheFloor; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
        killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp()
    {
        if (ventButton != null)
        {
            ventButton.Destroy();
            ventButton = null;
        }
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }

        if (FS_PlayersSensor)
        {
            GameObject.Destroy(FS_PlayersSensor);
            FS_PlayersSensor = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        base.MyPlayerControlUpdate();
        Game.GameData.data.myData.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        UpdateFullScreen();
    }

    public override void GetLightRadius(ref float radius)
    {
        if (PlayerControl.LocalPlayer.GetModData().Property.UnderTheFloor)
            lightRadius = 0f;
        else
            lightRadius += (1f - lightRadius) * 0.3f;

        radius *= lightRadius;
    }

    public Hadar()
            : base("Hadar", "hadar", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
                 Impostor.impostorSideSet, Impostor.impostorSideSet, Impostor.impostorEndSet,
                 true, VentPermission.CanNotUse, false, false, true)
    {
        HideKillButtonEvenImpostor = true;
        killButton = null;
        ventButton = null;
    }
}
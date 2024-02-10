namespace Nebula.Roles.ImpostorRoles;

public class Deadbeat : Template.TImpostor
{
    private SpriteLoader AssassinMarkButtonSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 115f);

    public override void Initialize(PlayerControl __instance)
    {
        target = null;
    }

    public override void LoadOptionData()
    {
        base.LoadOptionData();
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
    }

    private PlayerControl target;
    private CustomButton markButton,killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(markButton != null)
        {
            markButton.Destroy();
        }
        markButton = new CustomButton(
            () =>
            {
                target = Game.GameData.data.myData.currentTarget;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
            () => { target = null; },
            AssassinMarkButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.mark"
        ).SetTimer(0);
        markButton.Timer = markButton.MaxTimer = 0;

        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                RPCEventInvoker.FakeKill(PlayerControl.LocalPlayer,Game.GameData.data.myData.currentTarget);
                Helpers.checkMuderAttemptAndKill(target,Game.GameData.data.myData.currentTarget,Game.PlayerData.PlayerStatus.Dead,showAnimation:false);
                Game.GameData.data.myData.currentTarget = null;
                killButton.Timer = killButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
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
        if(markButton != null)
        {
            markButton.Destroy();
            markButton = null;
        }
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
        target = null;
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => !target || p.PlayerId != target.PlayerId);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public Deadbeat() : base("Deadbeat", "deadbeat", false)
    {
        HideKillButtonEvenImpostor = true;
    }
}
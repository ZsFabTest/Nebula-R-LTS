namespace Nebula.Roles.ImpostorRoles;

public class SoulEater : Template.TImpostor
{
    private Module.CustomOption delayOption;
    private Module.CustomOption killCooldownOption;

    private SpriteLoader pseudocideButtonSprite = new SpriteLoader("Nebula.Resources.BuskPseudocideButton.png", 115f);

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        killCooldownOption = CreateOption(Color.white, "killCooldown", 20f, 2.5f, 45f, 2.5f);
        delayOption = CreateOption(Color.white, "delayOption", 10f, 0f, 20f, 0.25f);
    }

    private CustomButton killButton;
    private PlayerControl target = null;
    private bool canKill = false;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                target = Game.GameData.data.myData.currentTarget;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return !target && Game.GameData.data.myData.currentTarget; },
            () => { killButton.Timer = killButton.MaxTimer; canKill = false; },
            pseudocideButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            true,
            delayOption.getFloat(),
            () =>
            {
                if (!target.Data.IsDead && !MeetingHud.Instance)
                {
                    RPCEventInvoker.UpdatePlayerVisibility(target.PlayerId, false);
                    RPCEventInvoker.EmitAttributeFactor(target, new Game.PlayerAttributeFactor(Game.PlayerAttribute.Invisible, 1145141919810f, 5, false));
                    canKill = true;
                }
                else
                {
                    target = null;
                    canKill = false;
                }
            },
            "button.label.eat"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        killButton.MaxTimer = killCooldownOption.getFloat();
    }

    public override void OnMeetingStart()
    {
        if (target && canKill)
        {
            RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,target.PlayerId,Game.PlayerData.PlayerStatus.Dead.Id,false);
            target = null;
            canKill = false;
        }
    }

    public override void CleanUp()
    {
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
        target = null;
        canKill = false;
    }

    public override void Initialize(PlayerControl __instance)
    {
        target = null;
        canKill = false;
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public SoulEater() : base("SoulEater", "soulEater", true)
    {
        target = null;
        canKill = false;
        HideKillButtonEvenImpostor = true;
    }
}
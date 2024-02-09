namespace Nebula.Roles.ImpostorRoles;

public class Hitman : Template.TImpostor{
    private Module.CustomOption killCooldown;
    private Module.CustomOption bleedDuration;

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        killCooldown = CreateOption(Color.white,"killCooldown",27.5f,2.5f,45f,2.5f);
        killCooldown.suffix = "second";
        bleedDuration = CreateOption(Color.white,"bleedDuration",3f,1f,10f,1f);
        bleedDuration.suffix = "second";
    }

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance){
        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                var r = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                if(r == Helpers.MurderAttemptResult.PerformKill){
                    RPCEventInvoker.FixedCleanDeadBody(Game.GameData.data.myData.currentTarget);
                    Events.LocalEvent.Activate(new ExtraRoles.Bloody.BloodyEvent(bleedDuration.getFloat()));
                }
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = killCooldown.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
    }

    public Hitman() : base("Hitman","hitman",true){
        hasRoleUpdate = true;
        HideKillButtonEvenImpostor = true;
        killButton = null;
    }
}
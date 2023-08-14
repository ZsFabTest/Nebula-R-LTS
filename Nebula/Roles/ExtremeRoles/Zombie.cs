namespace Nebula.Roles.ImpostorRoles;
public class Zombie : Template.TImpostor{
    private Module.CustomOption killCooldown;
    private Module.CustomOption ChanceToInfluence;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        killCooldown = CreateOption(Color.white,"killCooldown",25f,5f,45f,2.5f);
        killCooldown.suffix = "second";
        ChanceToInfluence = CreateOption(Color.white,"chanceToInfluence",10f,10f,100f,10f);
        ChanceToInfluence.suffix = "percent";
    }

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null){
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                if(NebulaPlugin.rnd.Next(1,101) <= ChanceToInfluence.getFloat()){
                    RPCEventInvoker.ImmediatelyChangeRole(Game.GameData.data.myData.currentTarget,Roles.ZombieSidekick);
                    PlayerControl target = Game.GameData.data.myData.currentTarget;
                    while(target.GetModData().extraRole.Count > 0){
                        RPCEventInvoker.ImmediatelyUnsetExtraRole(target,target.GetModData().extraRole[0]);
                    }
                    target.ShowFailedMurder();
                }else Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                killButton.Timer = killButton.MaxTimer;
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

    public Zombie() : base("Zombie","zombie",true){
        hasRoleUpdate = true;
        HideKillButtonEvenImpostor = true;
    }
}
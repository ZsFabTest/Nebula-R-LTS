namespace Nebula.Roles.CrewmateRoles;
public class TaoistPriest : Template.TCrewmate{
    public static Color RoleColor = new Color(222f / 255f,220f / 255f,172f / 255f);

    private Module.CustomOption killCooldown;

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        killCooldown = CreateOption(Color.white,"killCooldown",15f,2.5f,45f,2.5f);
        killCooldown.suffix = "second";
    }

    public CustomButton release;
    public PlayerControl target;
    public override void ButtonInitialize(HudManager __instance){
        if(release != null){
            release.Destroy();
        }
        release = new CustomButton(
            () =>
            {
                target = Game.GameData.data.myData.currentTarget;
                Events.Schedule.RegisterPostMeetingAction(() => {
                    if(!PlayerControl.LocalPlayer.Data.IsDead){
                        Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, true);
                        RPCEventInvoker.CleanDeadBody(target.PlayerId);
                    }
                    target = null;
                },16);
                target.ShowFailedMurder();
                release.Timer = release.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove && !target; },
            () => { release.Timer = release.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.releaseSouls"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        release.MaxTimer = killCooldown.getFloat();
        release.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(release != null){
            release.Destroy();
            release = null;
        }
    }

    public TaoistPriest() : base("TaoistPriest","taoistPriest",RoleColor,true){
        hasRoleUpdate = true;
    }
}
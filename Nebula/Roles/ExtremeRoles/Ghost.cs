namespace Nebula.Roles.NeutralRoles;
/*
public class GhostEvent : Events.LocalEvent{
    public GhostEvent() : base(0.1f) {}
    public override void OnTerminal(){
        RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer);
    }
}
*/

public class Ghost : Role,Template.HasWinTrigger{
    public bool WinTrigger { get; set; }
    public byte Winner { get; set; }
    public byte MurderId;
    public short lcnt;

    private Module.CustomOption killCooldown;
    private Module.CustomOption LiveAddition;

    public override void LoadOptionData(){
        killCooldown = CreateOption(Color.white,"killCooldown",25f,5f,45f,2.5f);
        killCooldown.suffix = "second";
        LiveAddition = CreateOption(Color.white,"liveAddition",2f,1f,5f,1f);
    }

    public override void GlobalInitialize(PlayerControl __instance){
        WinTrigger = false;
        Winner = byte.MaxValue;
        MurderId = 16;
        lcnt = -1;
    }

    public override void OnMurdered(byte murderId){
        if(lcnt <= -1){
            /*
			PlayerControl.LocalPlayer.Revive();
			DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
			foreach(var DeadBody in array){
				if(DeadBody.ParentId == PlayerControl.LocalPlayer.PlayerId){
					DeadBody.gameObject.active = false;
				}
			}
            */
            RPCEventInvoker.FixedRevive(PlayerControl.LocalPlayer);
            MurderId = murderId;
            lcnt = (short)LiveAddition.getFloat();
            killButton.Timer = killButton.MaxTimer;
            Helpers.playerById(murderId).ShowFailedMurder();
            //Events.LocalEvent.Activate(new Events.FixCam());
        }
    }

    public CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance){
        if(killButton != null) killButton.Destroy();
        killButton = new CustomButton(
            () => { 
                PlayerControl target = Game.GameData.data.myData.currentTarget;

                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, false, true);
                if (res == Helpers.MurderAttemptResult.PerformKill){
                    killButton.Timer = killButton.MaxTimer;
                    RPCEventInvoker.WinTrigger(this);
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

    public override void MyPlayerControlUpdate(){
        Game.GameData.data.myData.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => {
            if(p.PlayerId == MurderId) return true;
            return false;
        });
        Patches.PlayerControlPatch.SetPlayerOutline(Game.GameData.data.myData.currentTarget,new(0f,0f,0f));
    }

    public override void OnMeetingStart(){
        if(--lcnt == 0) Events.Schedule.RegisterPostMeetingAction(() => {
            RPCEventInvoker.UncheckedMurderPlayer(MurderId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Dead.Id,false);
            RPCEventInvoker.CleanDeadBody(PlayerControl.LocalPlayer.PlayerId);
            Game.GameData.data.myData.CanSeeEveryoneInfo = true;
            MurderId = 16;
            lcnt = -1;
        },0);
    }

    public override void EditOthersDisplayNameColor(byte playerId,ref Color displayColor){
        if(playerId == MurderId) displayColor = new(0f,0f,0f);
    }

    public Ghost() : base("Ghost","ghost",new(1f,1f,1f),RoleCategory.Neutral,Side.Ghost,Side.Ghost,
        new HashSet<Side>() { Side.Ghost },new HashSet<Side>() { Side.Ghost },new HashSet<Patches.EndCondition>(){ Patches.EndCondition.GhostWin },
        true,VentPermission.CanNotUse,false,true,true){
        killButton = null;
    }
}
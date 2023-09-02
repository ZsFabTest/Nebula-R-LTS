namespace Nebula.Roles.VirusCrisisRoles;

public class Survival : Role,Template.HasWinTrigger{
    public bool WinTrigger { get; set; }
    public byte Winner { get; set; }

    private Module.CustomOption taskCount;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        taskCount = CreateOption(Color.white,"taskCount",4f,1f,10f,1f);
    }

    public override void Initialize(PlayerControl __instance){
        RPCEventInvoker.RefreshTasks(PlayerControl.LocalPlayer.PlayerId, (int)taskCount.getFloat(), 0, 0.1f);
    }

    public override void OnTaskComplete(PlayerTask? task){
        if(Game.GameData.data.myData.getGlobalData().Tasks.Completed < taskCount.getFloat()) return;
        //if(PlayerControl.LocalPlayer.GetModData().extraRole.Contains(Roles.Supportee)){
        RPCEventInvoker.ImmediatelyUnsetExtraRole(PlayerControl.LocalPlayer,Roles.Supportee);
        RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Gunner);
    }

    public override void GlobalInitialize(PlayerControl __instance){
        WinTrigger = false;
    }

    public override void OnDied(){
        Game.GameData.data.myData.CanSeeEveryoneInfo = true;
    }

    public Survival() : base("Survival","survival",Palette.CrewmateBlue,RoleCategory.Neutral,Side.Survival,Side.Survival,
         new HashSet<Side>() { Side.Survival },new HashSet<Side>() { Side.Survival },new HashSet<Patches.EndCondition>() { Patches.EndCondition.SurvivalWin },
         false,VentPermission.CanUseUnlimittedVent,true,false,false){
        Allocation = AllocationType.None;
        ValidGamemode = Module.CustomGameMode.VirusCrisis;
        canReport = false;
        CanCallEmergencyMeeting = false;
    }
}
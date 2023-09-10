namespace Nebula.Roles.VirusCrisisRoles;

public class Survival : Role,Template.HasWinTrigger{
    public bool WinTrigger { get; set; }
    public byte Winner { get; set; }

    private Module.CustomOption taskCount;
    private Module.CustomOption chanceToBeDoctor;
    private Module.CustomOption VentCooldown;
    private Module.CustomOption VentDuring;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        taskCount = CreateOption(Color.white,"taskCount",4f,1f,10f,1f);
        chanceToBeDoctor = CreateOption(Color.white,"chanceToBeDoctor",50f,0f,100f,10f);
        chanceToBeDoctor.suffix = "percent";
        VentCooldown = CreateOption(Color.white,"VentCooldown",20f,2.5f,45f,2.5f);
        VentCooldown.suffix = "second";
        VentDuring = CreateOption(Color.white,"VentDuring",10f,2.5f,45f,2.5f);
        VentDuring.suffix = "second";
    }

    public override void Initialize(PlayerControl __instance){
        RPCEventInvoker.RefreshTasks(PlayerControl.LocalPlayer.PlayerId, (int)taskCount.getFloat(), 0, 0.1f);
    }

    public override void OnTaskComplete(PlayerTask? task){
        if(Game.GameData.data.myData.getGlobalData().Tasks.Completed < taskCount.getFloat()) return;
        //if(PlayerControl.LocalPlayer.GetModData().extraRole.Contains(Roles.Supportee)){
        RPCEventInvoker.ImmediatelyUnsetExtraRole(PlayerControl.LocalPlayer,Roles.Supportee);
        if(NebulaPlugin.rnd.Next(0,101) <= chanceToBeDoctor.getFloat()) RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.DoctorV);
        else RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Gunner);
    }

    public override void GlobalInitialize(PlayerControl __instance){
        WinTrigger = false;
        VentCoolDownMaxTimer = VentCooldown.getFloat();
        VentDurationMaxTimer = VentDuring.getFloat();
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
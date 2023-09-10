namespace Nebula.Roles.VirusCrisisRoles;

public class InfectedSidekick : Role{
    private Module.CustomOption tips;

    private Module.CustomOption VentCooldown;
    private Module.CustomOption VentDuring;
    private Module.CustomOption killCooldown;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.ImpostorRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        tips = CreateOption(Color.white,"tips",true);
        killCooldown = CreateOption(Color.white,"killcooldown",22.5f,2.5f,45f,2.5f);
        killCooldown.suffix = "second";
        VentCooldown = CreateOption(Color.white,"VentCooldown",20f,2.5f,45f,2.5f);
        VentCooldown.suffix = "second";
        VentDuring = CreateOption(Color.white,"VentDuring",10f,2.5f,45f,2.5f);
        VentDuring.suffix = "second";
    }

    //public int TotalLives;

/*
    public override void Initialize(PlayerControl __instance){
        RPCEventInvoker.SetInfectLives((byte)(int)lives.getFloat());
    }
    */

/*
    public override Helpers.MurderAttemptResult OnMurdered(byte murderId,byte playerId){
        //Helpers.RoleAction(Helpers.playerById(playerId),(role) => { role.OnMeetingStart(); });
        RPCEventInvoker.SetInfectLives((byte)(Roles.Infected.TotalLives - 1));
        return Helpers.MurderAttemptResult.SuppressKill;
    }
    */

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                if(Game.GameData.data.myData.currentTarget.GetModData().extraRole.Contains(Roles.Supportee)){
                    RPCEventInvoker.ImmediatelyChangeRole(Game.GameData.data.myData.currentTarget, Roles.InfectedSidekick);
                    RPCEventInvoker.ImmediatelyUnsetExtraRole(Game.GameData.data.myData.currentTarget,Roles.Supportee);
                    Game.GameData.data.myData.currentTarget.ShowFailedMurder();
                    RPCEventInvoker.FakeKill(PlayerControl.LocalPlayer,Game.GameData.data.myData.currentTarget);
                } //Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                else{
                    RPCEventInvoker.SetExtraRole(Game.GameData.data.myData.currentTarget,Roles.Supportee,0);
                    Game.GameData.data.myData.currentTarget.ShowFailedMurder();
                    RPCEventInvoker.FakeKill(PlayerControl.LocalPlayer,Game.GameData.data.myData.currentTarget);
                }
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
        ).SetTimer(Roles.Infected.InitKillCooldown.getFloat());
        killButton.MaxTimer = killCooldown.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => { return p.GetModData().role.side != Side.Infected; });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        //if(TotalLives <= 0 && !PlayerControl.LocalPlayer.Data.IsDead) RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Dead.Id,false);
    }

    public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
        displayColor = Color;
    }

/*
    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        displayName += " " + TotalLives.ToString() + "♥";
    }
    */

    public override void OnDied(){
        Game.GameData.data.myData.CanSeeEveryoneInfo = true;
    }

    public override void GlobalInitialize(PlayerControl __instance){
        VentCoolDownMaxTimer = VentCooldown.getFloat();
        VentDurationMaxTimer = VentDuring.getFloat();
    }

    public InfectedSidekick() : base("InfectedSidekick","infectedSidekick",Palette.ImpostorRed,RoleCategory.Neutral,Side.Infected,Side.Infected,
         new HashSet<Side>() { Side.Infected },new HashSet<Side>() { Side.Infected },new HashSet<Patches.EndCondition> { Patches.EndCondition.InfectedWin },
         true,VentPermission.CanUseUnlimittedVent,true,true,true){
        //IsHideRole = true;
        Allocation = AllocationType.None;
        ValidGamemode = Module.CustomGameMode.VirusCrisis;
        canReport = false;
        CanCallEmergencyMeeting = false;
        killButton = null;
    }
}
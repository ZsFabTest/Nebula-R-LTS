namespace Nebula.Roles.VirusCrisisRoles;

public class Infected : Role{
    public Module.CustomOption InitKillCooldown;
    public Module.CustomOption killCooldown;
    private Module.CustomOption lives;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.ImpostorRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        InitKillCooldown = CreateOption(Color.white,"initkillcooldown",15f,2.5f,45f,2.5f);
        InitKillCooldown.suffix = "second";
        killCooldown = CreateOption(Color.white,"killcooldown",15f,10f,20f,2.5f);
        killCooldown.suffix = "second";
        lives = CreateOption(Color.white,"totalLives",5f,1f,10f,1f);
    }

    public int TotalLives;

    public override void GlobalInitialize(PlayerControl __instance){
        TotalLives = (int)lives.getFloat();
    }

    public override Helpers.MurderAttemptResult OnMurdered(byte murderId,byte playerId){
        //Helpers.RoleAction(Helpers.playerById(playerId),(role) => { role.OnMeetingStart(); });
        RPCEventInvoker.SetInfectLives((byte)(Roles.Infected.TotalLives - 1));
        return Helpers.MurderAttemptResult.SuppressKill;
    }

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
        ).SetTimer(InitKillCooldown.getFloat());
        killButton.MaxTimer = killCooldown.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
        TotalLives = 255;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => { return p.GetModData().role.side != Side.Infected; });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);

        if(TotalLives <= 0 && !PlayerControl.LocalPlayer.Data.IsDead) RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Dead.Id,false);
    }

    public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
        displayColor = Color;
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        displayName += " " + TotalLives.ToString() + "♥";
    }

    public override void OnDied(){
        RPCEventInvoker.WinTrigger(Roles.Survival);
    }

    public Infected() : base("Infected","infected",Palette.ImpostorRed,RoleCategory.Neutral,Side.Infected,Side.Infected,
         new HashSet<Side>() { Side.Infected },new HashSet<Side>() { Side.Infected },new HashSet<Patches.EndCondition> { Patches.EndCondition.InfectedWin },
         true,VentPermission.CanNotUse,false,true,true){
        //IsHideRole = true;
        Allocation = AllocationType.None;
        ValidGamemode = Module.CustomGameMode.VirusCrisis;
        canReport = false;
        CanCallEmergencyMeeting = false;
        killButton = null;
        TotalLives = 255;
    }
}
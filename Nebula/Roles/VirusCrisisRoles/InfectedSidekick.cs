namespace Nebula.Roles.VirusCrisisRoles;

public class InfectedSidekick : Role{
    private Module.CustomOption freezeCooldown;
    private Module.CustomOption freezeDuring;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.ImpostorRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        freezeCooldown = CreateOption(Color.white,"freezecooldown",7.5f,2.5f,25f,2.5f);
        freezeCooldown.suffix = "second";
        freezeDuring = CreateOption(Color.white,"freezeduring",2.5f,1f,5f,0.5f);
        freezeDuring.suffix = "second";
    }

    //public int TotalLives;

    private SpriteLoader buttonSprite = new("Nebula.Resources.ChainShiftButton.png",115f);

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

    private CustomButton freeze;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(freeze != null)
        {
            freeze.Destroy();
        }
        freeze = new CustomButton(
            () =>
            {
                RPCEventInvoker.EmitSpeedFactor(Game.GameData.data.myData.currentTarget,new Game.SpeedFactor(255, freezeDuring.getFloat(), 0.1f, true));
                freeze.Timer = freeze.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { freeze.Timer = freeze.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.freeze"
        ).SetTimer(freezeCooldown.getFloat());
        freeze.MaxTimer = freezeCooldown.getFloat();
        //killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(freeze != null){
            freeze.Destroy();
            freeze = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
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

    public InfectedSidekick() : base("InfectedSidekick","infectedSidekick",Palette.ImpostorRed,RoleCategory.Neutral,Side.Infected,Side.Infected,
         new HashSet<Side>() { Side.Infected },new HashSet<Side>() { Side.Infected },new HashSet<Patches.EndCondition> { Patches.EndCondition.InfectedWin },
         true,VentPermission.CanUseUnlimittedVent,true,true,true){
        //IsHideRole = true;
        Allocation = AllocationType.None;
        ValidGamemode = Module.CustomGameMode.VirusCrisis;
        canReport = false;
        CanCallEmergencyMeeting = false;
        freeze = null;
    }
}
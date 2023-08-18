namespace Nebula.Roles.VirusCrisisRoles;

public class Gunner : Role{
    private Module.CustomOption killCooldown;
    public Module.CustomOption neutrallySpawnCount;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        killCooldown = CreateOption(Color.white,"killcooldown",15f,2.5f,45f,2.5f);
        killCooldown.suffix = "second";
        neutrallySpawnCount = CreateOption(Color.white,"neutrallySpawnCount",1f,0f,15f,1f);//
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
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                killButton.Timer = killButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Roles.SchrodingersCat.canUseKillButton.getBool(); },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(10f);
        killButton.MaxTimer = killCooldown.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
        displayColor = Color;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => { return p.GetModData().role == Roles.Infected; });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public override void OnDied(){
        Game.GameData.data.myData.CanSeeEveryoneInfo = true;
    }

    public Gunner() : base("Gunner","gunner",CrewmateRoles.Sheriff.RoleColor,RoleCategory.Neutral,Side.Survival,Side.Survival,
         new HashSet<Side>() { Side.Survival },new HashSet<Side>() { Side.Survival },new HashSet<Patches.EndCondition>() { Patches.EndCondition.SurvivalWin },
         true,VentPermission.CanNotUse,false,false,false){
        Allocation = AllocationType.None;
        ValidGamemode = Module.CustomGameMode.VirusCrisis;
        canReport = false;
        CanCallEmergencyMeeting = false;
    }
}
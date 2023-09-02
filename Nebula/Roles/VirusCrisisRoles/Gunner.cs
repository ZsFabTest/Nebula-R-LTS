namespace Nebula.Roles.VirusCrisisRoles;

public class Gunner : Role{
    private Module.CustomOption killCooldown;
    public Module.CustomOption neutrallySpawnCount;
    public Module.CustomOption leftCount;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    private SpriteLoader killButtonSprite = new SpriteLoader("Nebula.Resources.SheriffKillButton.png", 100f, "ui.button.sheriff.kill");

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        killCooldown = CreateOption(Color.white,"killcooldown",25f,2.5f,45f,2.5f);
        killCooldown.suffix = "second";
        neutrallySpawnCount = CreateOption(Color.white,"neutrallySpawnCount",1f,0f,15f,1f);
        leftCount = CreateOption(Color.white,"leftCount",1f,1f,5f,1f);
    }

    private int left;

    public override void Initialize(PlayerControl __instance){
        left = (int)leftCount.getFloat();
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
                left--;
                killButton.UsesText.text = left.ToString();
                RPCEventInvoker.FakeKill(PlayerControl.LocalPlayer,Game.GameData.data.myData.currentTarget);
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Roles.SchrodingersCat.canUseKillButton.getBool() && left > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            killButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(10f);
        killButton.MaxTimer = killCooldown.getFloat();
        killButton.SetButtonCoolDownOption(true);
        killButton.UsesText.text = left.ToString();
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
        left = 0;
    }

    public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
        displayColor = Color;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => { return p.GetModData().role.side != Side.Survival; });
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
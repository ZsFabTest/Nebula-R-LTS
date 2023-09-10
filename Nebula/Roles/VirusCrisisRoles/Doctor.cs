namespace Nebula.Roles.VirusCrisisRoles;

public class Doctor : Role{
    private Module.CustomOption treatCooldown;

    public override bool IsSpawnable(){
        return CustomOptionHolder.gameModeNormal.getSelection() == 3;
    }

    private SpriteLoader treatSprite = new SpriteLoader("Nebula.Resources.ReviveButton.png", 115f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles;
        TopOption.AddCustomPrerequisite(() => { return CustomOptionHolder.gameModeNormal.getSelection() == 3; });
        treatCooldown = CreateOption(Color.white,"treatcooldown",35f,2.5f,60f,2.5f);
        treatCooldown.suffix = "second";
    }

    private CustomButton treat;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(treat != null)
        {
            treat.Destroy();
        }
        treat = new CustomButton(
            () =>
            {
                RPCEventInvoker.ImmediatelyUnsetExtraRole(Game.GameData.data.myData.currentTarget,Roles.Supportee);
                Game.GameData.data.myData.currentTarget.ShowFailedMurder();
                Game.GameData.data.myData.currentTarget = null;
                treat.Timer = treat.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { treat.Timer = treat.MaxTimer; },
            treatSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.treat"
        ).SetTimer(10f);
        treat.MaxTimer = treatCooldown.getFloat();
    }

    public override void CleanUp(){
        if(treat != null){
            treat.Destroy();
            treat = null;
        }
    }

    public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
        displayColor = Color;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => { return p.GetModData().role.side != Side.Infected && p.GetModData().extraRole.Contains(Roles.Supportee); });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color);
    }

    public override void OnDied(){
        Game.GameData.data.myData.CanSeeEveryoneInfo = true;
    }

    public Doctor() : base("DoctorV","doctorV",CrewmateRoles.Doctor.RoleColor,RoleCategory.Neutral,Side.Survival,Side.Survival,
         new HashSet<Side>() { Side.Survival },new HashSet<Side>() { Side.Survival },new HashSet<Patches.EndCondition>() { Patches.EndCondition.SurvivalWin },
         true,VentPermission.CanNotUse,false,false,false){
        Allocation = AllocationType.None;
        ValidGamemode = Module.CustomGameMode.VirusCrisis;
        canReport = false;
        CanCallEmergencyMeeting = false;
    }
}
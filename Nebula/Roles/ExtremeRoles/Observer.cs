namespace Nebula.Roles.CrewmateRoles;

public class Observer : Role
{
    public static Color RoleColor = new Color(200f / 255f, 190f / 255f, 230 / 255f);

    private static Module.CustomOption hasImpostorLight;
    private static Module.CustomOption hideCoolDown;
    private static Module.CustomOption hideDuringTime;

    private CustomButton hideButton;
    private CustomButton cameraButton;
    private PlayerControl watchingTarget;
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.WarpButton.png", 115f);
    private SpriteLoader MarkButtonSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 115f);
    private SpriteLoader monitorButtonSprite = new SpriteLoader("Nebula.Resources.DecoyMonitorButton.png", 115f);

    public override void LoadOptionData()
    {
        hasImpostorLight = CreateOption(Color.white, "hasImpostorLight", true);
        hideCoolDown = CreateOption(Color.white, "hideCoolDown", 15f, 5f, 30f, 2.5f);
        hideCoolDown.suffix = "second";
        hideDuringTime = CreateOption(Color.white, "hideDuringTime", 15f, 5f, 20f, 2.5f);
        hideDuringTime.suffix = "second";
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        UseImpostorLightRadius = hasImpostorLight.getBool();
        IgnoreBlackout = hasImpostorLight.getBool();
        watchingTarget = null;
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(hideButton != null)
        {
            hideButton.Destroy();
        }
        hideButton = new CustomButton(
            () =>
            {
                RPCEventInvoker.EmitAttributeFactor(PlayerControl.LocalPlayer, new Game.PlayerAttributeFactor(Game.PlayerAttribute.Invisible, hideDuringTime.getFloat(), 0, false));
                RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, false);
                Game.GameData.data.myData.Vision.Register(new Game.VisionFactor(hideDuringTime.getFloat(), 1.5f));
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                hideButton.Timer = hideButton.MaxTimer;
                RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
            },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
           hideDuringTime.getFloat(),
           () =>
           {
               hideButton.Timer = hideButton.MaxTimer;
               RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
           },
            "button.label.hide"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        hideButton.MaxTimer = hideCoolDown.getFloat();

        if (cameraButton != null)
        {
            cameraButton.Destroy();
        }
        cameraButton = new CustomButton(
            () =>
            {
                if(watchingTarget == null){
                    watchingTarget = Game.GameData.data.myData.currentTarget;
                    watchingTarget.ShowFailedMurder();
                    cameraButton.Sprite = monitorButtonSprite.GetSprite();
                    cameraButton.SetLabel("button.label.monitor");
                    return;
                }
                if (HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer){
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);                
                    watchingTarget = null;
                    cameraButton.Sprite = MarkButtonSprite.GetSprite();
                    cameraButton.SetLabel("button.label.mark");
                }
                else HudManager.Instance.PlayerCam.SetTargetWithLight(watchingTarget);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && (watchingTarget || Game.GameData.data.myData.currentTarget); },
            () => { watchingTarget = null; },
            MarkButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.secondaryAbilityInput.keyCode,
            "button.label.mark"
        );
        cameraButton.Timer = cameraButton.MaxTimer = 0f;
    }

    public override void CleanUp()
    {
        if(hideButton != null)
        {
            hideButton.Destroy();
            hideButton = null;
        }
        if(cameraButton != null)
        {
            cameraButton.Destroy();
            cameraButton = null;
        }
    }

    public override void MyPlayerControlUpdate(){
        foreach(var p in PlayerControl.AllPlayerControls){
            if(!p.Data.IsDead) Patches.PlayerControlPatch.SetPlayerOutline(p,RoleColor);
        }
        RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId,true);
        Game.GameData.data.myData.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
    }

    public Observer()
        : base("Observer", "observer", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, true, true)
    {
        watchingTarget = null;
        hideButton = null;
        cameraButton = null;
    }
}

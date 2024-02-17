namespace Nebula.Roles.CrewmateRoles;

public class Officer : Template.TCrewmate{
    static public Color RoleColor = new(210f / 255f,214f / 255f,98f / 255f);

    private Module.CustomOption orderCooldown;
    private Module.CustomOption orderDuration;
    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        orderCooldown = CreateOption(Color.white,"orderCooldown",15f,2.5f,45f,2.5f);
        orderCooldown.suffix = "second";
        orderDuration = CreateOption(Color.white,"orderDuration",7.5f,2.5f,20f,2.5f);
        orderDuration.suffix = "second";
    }

    private SpriteLoader buttonSprite = new("Nebula.Resources.ArrestButton.png",115f);

    private CustomButton order;
    public override void ButtonInitialize(HudManager __instance){
        order?.Destroy();
        order = new CustomButton(
            () => {
                RPCEventInvoker.EmitSpeedFactor(Game.GameData.data.myData.currentTarget,new Game.SpeedFactor(0,3f,0f,false));
                order.Timer = order.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
            () => { order.Timer = order.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.order"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        order.MaxTimer = orderCooldown.getFloat();
    }

    public override void CleanUp(){
        if(order != null){
            order.Destroy();
            order = null;
        }
    }

    public Officer() : base("Officer","officer",RoleColor,false){
        hasRoleUpdate = true;
        order = null;
    }
}
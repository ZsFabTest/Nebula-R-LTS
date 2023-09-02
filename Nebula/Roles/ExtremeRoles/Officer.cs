namespace Nebula.Roles.CrewmateRoles;

public class Officer : Template.TCrewmate{
    static public Color RoleColor = new(210f / 255f,214f / 255f,98f / 255f);

    private Module.CustomOption orderCooldown;
    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        orderCooldown = CreateOption(Color.white,"orderCooldown",15f,2.5f,45f,2.5f);
        orderCooldown.suffix = "second";
    }

    private SpriteLoader buttonSprite = new("Nebula.Resources.ArrestButton.png",115f);

    private PlayerControl target;
    private CustomButton order;
    private Vector3 pos;
    public override void ButtonInitialize(HudManager __instance){
        order?.Destroy();
        order = new CustomButton(
            () => {
                target = Game.GameData.data.myData.currentTarget;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
            () => { order.Timer = order.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            3f,
            () => {
                target = null;
                order.Timer = order.MaxTimer;
            },
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

    public override void MyPlayerControlUpdate(){
        base.MyPlayerControlUpdate();
        if(order.isEffectActive){
            RPCEventInvoker.FakeKill(target,PlayerControl.LocalPlayer);
        }
    }

    public Officer() : base("Officer","officer",RoleColor,false){
        hasRoleUpdate = true;
        order = null;
    }
}
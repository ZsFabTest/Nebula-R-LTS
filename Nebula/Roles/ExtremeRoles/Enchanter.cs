namespace Nebula.Roles.CrewmateRoles;

public class Enchanter : Template.TCrewmate{
    public static Color RoleColor = new(74f / 255f,224f / 255f,80f / 255f);

    public int SetFieldDataId { get; private set; }

    private Module.CustomOption TeleportCooldown;
    private Module.CustomOption setCooldown;
    private Module.CustomOption maxPairNum;
    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        TeleportCooldown = CreateOption(Color.white,"teleportCooldown",5f,0f,25f,2.5f);
        TeleportCooldown.suffix = "second";
        setCooldown = CreateOption(Color.white,"setCooldown",20f,0f,45f,2.5f);
        setCooldown.suffix = "second";
        maxPairNum = CreateOption(Color.white,"maxPairNum",2f,1f,5f,1f);
    }

    public override void GlobalInitialize(PlayerControl __instance){
        __instance.GetModData().SetRoleData(SetFieldDataId,0);
    }

    private SpriteLoader sprite = new("Nebula.Resources.CommTrapButton.png",115f);

    private CustomButton SetField;
    public override void ButtonInitialize(HudManager __instance){
        SetField?.Destroy();
        SetField = new CustomButton(() => {
                RPCEventInvoker.ObjectInstantiate(CustomObject.Type.TeleportField,PlayerControl.LocalPlayer.transform.position);
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,SetFieldDataId,1);
                SetField.Timer = SetField.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.GetModData().GetRoleData(SetFieldDataId) < 2 * maxPairNum.getFloat(); },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () => { SetField.Timer = SetField.MaxTimer; },
            sprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.place"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        SetField.MaxTimer = setCooldown.getFloat();;
    }

    public override void CleanUp(){
        if(SetField != null){
            SetField.Destroy();
            SetField = null;
        }
    }

    public float getCooldown(){
        return TeleportCooldown.getFloat();
    }

    public Enchanter() : base("Enchanter","enchanter",RoleColor,false){
        SetFieldDataId = Game.GameData.RegisterRoleDataId("enchanter.setFieldDataId");
    }
}
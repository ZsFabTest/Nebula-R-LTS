namespace Nebula.Roles.ImpostorRoles;

public class Extortionist : Template.TImpostor{
    private Module.CustomOption abilityCooldown;
    public static Module.CustomOption abilityDuration;
    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        abilityCooldown = CreateOption(Color.white,"abilityCooldown",32.5f,2.5f,60f,2.5f);
        abilityCooldown.suffix = "second";
        abilityDuration = CreateOption(Color.white,"abilityDuration",12.5f,5f,45f,2.5f);
        abilityDuration.suffix = "second";
    }

    private SpriteLoader sprite = new("Nebula.Resources.DecoySwapButton.png",115f);

    private CustomButton extort;
    public override void ButtonInitialize(HudManager __instance){
        extort?.Destroy();
        extort = new CustomButton(() => {
                RPCEventInvoker.Extort(Game.GameData.data.myData.currentTarget,abilityDuration.getFloat());
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
            () => { extort.Timer = extort.MaxTimer; },
            sprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            abilityDuration.getFloat(),
            () => {
                extort.Timer = extort.MaxTimer;
            },
            "button.label.extort"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        extort.MaxTimer = abilityCooldown.getFloat();
    }

    public override void CleanUp(){
        if(extort != null){
            extort.Destroy();
            extort = null;
        }
    }

    public Extortionist() : base("Extortionist","extortionist",true){
        hasRoleUpdate = true;
    }
}
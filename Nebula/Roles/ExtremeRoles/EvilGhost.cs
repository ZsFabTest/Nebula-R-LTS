namespace Nebula.Roles.GhostRoles;

public class EvilGhost : GhostRole{
    public override bool IsAssignableTo(Game.PlayerData player)
    {
        return player.role.side == Side.Crewmate;
    }

    private Module.CustomOption abilityCooldown;
    private Module.CustomOption abilityDuration;
    public override void LoadOptionData(){
        //TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        abilityCooldown = CreateOption(Color.white,"abilityCooldown",15f,2.5f,45f,2.5f);
        abilityCooldown.suffix = "second";
        abilityDuration = CreateOption(Color.white,"abilityDuration",5f,1f,10f,1f);
        abilityDuration.suffix = "second";
    }

    private SpriteLoader sprite = new("Nebula.Resources.DecoySwapButton.png",115f);

    private CustomButton extort;
    public override void ButtonInitialize(HudManager __instance){
        extort?.Destroy();
        extort = new CustomButton(() => {
                RPCEventInvoker.Extort(Game.GameData.data.myData.currentTarget,abilityDuration.getFloat());
            },
            () => { return true; },
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

    public override void MyPlayerControlUpdate(){
        Game.GameData.data.myData.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
    }

    public EvilGhost() : base("EvilGhost","evilGhost",Palette.ImpostorRed){
    }
}
namespace Nebula.Roles.CrewmateRoles;

public class TimeManager : Template.TCrewmate{
    static public Color RoleColor = new(48f / 255f,96f / 255f,158f / 255f);

    private Module.CustomOption abilityCooldown;
    private Module.CustomOption abilityDuration;
    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        abilityCooldown = CreateOption(Color.white,"abilityCooldown",32.5f,2.5f,60f,2.5f);
        abilityCooldown.suffix = "second";
        abilityDuration = CreateOption(Color.white,"abilityDuration",7.5f,2.5f,25f,2.5f);
        abilityDuration.suffix = "second";
    }

    private SpriteLoader sprite = new("Nebula.Resources.TrackNiceButton.png",115f);

    private CustomButton control;
    public override void ButtonInitialize(HudManager __instance){
        control?.Destroy();
        control = new CustomButton(() => {
                foreach(PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
                    if(p.PlayerId != PlayerControl.LocalPlayer.PlayerId){
                        RPCEventInvoker.SetFlash(p,abilityDuration.getFloat(),1f,new(0f,0f,0f));
                        if(!p.Data.IsDead) RPCEventInvoker.EmitSpeedFactor(p,new Game.SpeedFactor(0,abilityDuration.getFloat(),0f,false));
                    }else{
                        RPCEventInvoker.SetFlash(p,abilityDuration.getFloat(),0.2f,new(0f,0f,0f));
                    }
                }
                RPCEventInvoker.SetTimeStatus(true);
                Events.StandardEvent.SetEvent(() => { RPCEventInvoker.SetTimeStatus(false); },abilityDuration.getFloat(),false,false);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !Helpers.SabotageIsActive(); },
            () => { control.Timer = control.MaxTimer; },
            sprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            abilityDuration.getFloat(),
            () => {
                control.Timer = control.MaxTimer;
            },
            "button.label.control"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        control.MaxTimer = abilityCooldown.getFloat();
    }

    public override void CleanUp(){
        if(control != null){
            control.Destroy();
            control = null;
        }
    }

    public override void OnMurdered(byte murderId){
        foreach(PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
            if(p.PlayerId != PlayerControl.LocalPlayer.PlayerId){
                RPCEventInvoker.SetFlash(p,abilityDuration.getFloat(),1f,new(0f,0f,0f));
                if(!p.Data.IsDead) RPCEventInvoker.EmitSpeedFactor(p,new Game.SpeedFactor(0,abilityDuration.getFloat(),0f,false));
            }else{
                RPCEventInvoker.SetFlash(p,abilityDuration.getFloat(),0.2f,new(0f,0f,0f));
            }
        }
        RPCEventInvoker.SetTimeStatus(true);
        Events.StandardEvent.SetEvent(() => { RPCEventInvoker.SetTimeStatus(false); },abilityDuration.getFloat(),false,false);
    }

    public override void MyPlayerControlUpdate(){
        if(control.isEffectActive) control.Timer -= Time.deltaTime;
    }

    public TimeManager() : base("TimeManager","timeManager",RoleColor,false){

    }
}
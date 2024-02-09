namespace Nebula.Roles.ImpostorRoles;

public class FlashEndEvent : Events.LocalEvent{
    public FlashEndEvent(float time) : base(time){}
    public override void OnTerminal(){
        Roles.Grenadier.flashedId.Clear();
        Roles.Grenadier.isFlashing = false;
    }
}

public class Grenadier : Template.TImpostor{
    private Module.CustomOption flashDuration;
    private Module.CustomOption flashRange;
    private Module.CustomOption flashCooldown;

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        flashDuration = CreateOption(Color.white,"flashDuration",8f,3f,30f,1f);
        flashDuration.suffix = "second";
        flashRange = CreateOption(Color.white,"flashRange",6.5f,2.5f,15f,0.5f);
        flashRange.suffix = "cross";
        flashCooldown = CreateOption(Color.white,"flashCooldown",30f,7.5f,60f,2.5f);
        flashCooldown.suffix = "second";
    }

    private Vector3 FlashPos;

    private SpriteLoader Sprite = new("Nebula.Resources.CamoButton.png",115f);

    private CustomButton Flash;
    public override void ButtonInitialize(HudManager __instance){
        Flash?.Destroy();
        Flash = new CustomButton(
            () => {
                FlashPos = PlayerControl.LocalPlayer.transform.position;

                foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
                    if(Vector2.Distance(p.transform.position,FlashPos) <= flashRange.getFloat()){
                        //Debug.LogWarning(p.name + " " + p.GetModData().role.side.localizeSide);
                        if(p.GetModData().role.side == Side.Impostor){
                            //Debug.LogWarning("Impostor pass");
                            RPCEventInvoker.SetFlash(p,flashDuration.getFloat(),0.2f,new(1f,1f,1f));
                        }else{
                            //Debug.LogError(p.name + " " + (p.GetModData().role.side == Side.Impostor).ToString());
                            RPCEventInvoker.SetFlash(p,flashDuration.getFloat(),1f,new(1f,1f,1f));
                        }
                    }
                }
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !Helpers.SabotageIsActive() && (GameOptionsManager.Instance.CurrentGameOptions.MapId != 1 || !GameObject.FindObjectsOfType<PlainDoor>().ToArray().FirstOrDefault(x =>
                    {
                        if (x == null) return false;
                        return !x.Open;
                    }));
            },
            () => { Flash.Timer = Flash.MaxTimer; },
            Sprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            flashDuration.getFloat(),
            () => { 
                //foreach(var pId in impostorList)
                //    Helpers.RoleAction(Helpers.playerById(pId),(role) => { Helpers.PlayQuickFlash(Color.white);Helpers.PlayQuickFlash(Color.white);Helpers.PlayQuickFlash(Color.white); });
                Flash.Timer = Flash.MaxTimer;
            },
            "button.label.flash"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        Flash.MaxTimer = flashCooldown.getFloat();
    }

    public override void CleanUp(){
        if(Flash != null){
            Flash.Destroy();
            Flash = null;
        }
        flashedId.Clear();
    }

    public List<byte> flashedId;
    public bool isFlashing;

    public override void GlobalInitialize(PlayerControl __instance){
        flashedId.Clear();
        isFlashing = false;
    }

    public Grenadier() : base("Grenadier","grenadier",true){
        flashedId = new();
        isFlashing = false;
    }
}
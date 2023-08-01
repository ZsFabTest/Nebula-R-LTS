namespace Nebula.Roles.CrewmateRoles;

public class Programmer : Template.TCrewmate{
    public static Color RoleColor = new Color(180f / 255f,210f / 255f,237f / 255f);
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.CloseVentButton.png", 115f, "ui.button.minekeeper.set");

    string targetVentName;
    Vent targetVent;
    private CustomButton monitorVent;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        targetVentName = "";
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(monitorVent != null){
            monitorVent.Destroy();
        }
        monitorVent = new CustomButton(
            () => {
                targetVentName = targetVent.name;
                targetVent = null;
                Debug.Log(targetVentName);
                monitorVent.Timer = 1f;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && targetVent; },
            () => { },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.change"
        ).SetTimer(0f);
        monitorVent.MaxTimer = monitorVent.Timer = 0;
    }

    public void check(Vent vent,PlayerControl pc){
        //Debug.Log("Programmer check!");
        if(vent.name == targetVentName){
            Helpers.PlayQuickFlash(pc.GetModData().role.Color);
        }
    }

    public override void MyPlayerControlUpdate()
    {
        if (!ShipStatus.Instance) return;

        Vent target = null;
        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        float closestDistance = float.MaxValue;
        //Debug.LogWarning("14 " + ShipStatus.Instance.AllVents[14].name);
        for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++)
        {
            Vent vent = ShipStatus.Instance.AllVents[i];
            //Debug.Log(vent.name);
            if (vent.GetVentData().Sealed) continue;
            float distance = Vector2.Distance(vent.transform.position, truePosition);
            //Debug.Log(vent.name + " " + distance.ToString() + " " + vent.UsableDistance.ToString());
            if (distance <= vent.UsableDistance && distance < closestDistance)
            {
                closestDistance = distance;
                target = vent;
            }
        }
        //try{ Debug.Log(target.name); }catch{ Debug.LogWarning("No Target"); };
        targetVent = target;
    }

    public override void CleanUp()
    {
        if(monitorVent != null){
            monitorVent.Destroy();
            monitorVent = null;
        }
        targetVentName = "";
    }

    public Programmer() : base("Programmer","programmer",RoleColor,false){
    }
}
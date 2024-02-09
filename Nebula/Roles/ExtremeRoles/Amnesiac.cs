namespace Nebula.Roles.NeutralRoles;

public class Amnesiac : Role{
    public class AmnesiacEvent : Events.LocalEvent{
        PlayerControl target;
        public AmnesiacEvent(byte targetId) : base(0.1f) { target = Helpers.playerById(targetId); }
        public override void OnActivate()
        {
            switch (Amnesiac.targetsRoleModeOption.getSelection()){
                case 0:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    break;
                case 1:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    if(target.GetModData().role.side == Side.Crewmate) RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Crewmate);
                    else if(target.GetModData().role.side == Side.Impostor) RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Impostor);
                    else RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Opportunist);
                    break;
                case 2:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Amnesiac);
                    break;
                case 3:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Opportunist);
                    break;
            }
        }
    }

    static public Color RoleColor = new Color(210f / 255f, 220f / 255f, 234f / 255f);

    public byte deadBodyId;
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.PoltergeistButton.png", 115f);
    public static Module.CustomOption targetsRoleModeOption;
    private Module.CustomOption rememberCoolDownOption;
    Dictionary<byte, Arrow> Arrows;
    SpriteLoader arrowSprite = new SpriteLoader("role.spectre.arrow");

    public override void LoadOptionData()
    {
        targetsRoleModeOption = CreateOption(Color.white,"targetsRoleMode",new string[] { "role.amnesiac.targetsRoleMode.dontShift","role.amnesiac.targetsRoleMode.erase","role.amnesiac.targetsRoleMode.toAmnesiac","role.amnesiac.targetsRoleMode.toOpportunist" });
        rememberCoolDownOption = CreateOption(Color.white,"rememberCoolDown",27.5f,15f,40f,2.5f);
        rememberCoolDownOption.suffix = "second";
    }

    public override void Initialize(PlayerControl __instance){
        Arrows = new();
    }

    public override void MyPlayerControlUpdate()
    {
        if (PlayerControl.LocalPlayer.Data.IsDead) return;

        /* 捕食対象の探索 */

        DeadBody b = Patches.PlayerControlPatch.SetMyDeadTarget();
        if (b)
        {
            deadBodyId = b.ParentId;
        }
        else
        {
            deadBodyId = byte.MaxValue;
        }
        Patches.PlayerControlPatch.SetDeadBodyOutline(b, Color.yellow);

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            if (Arrows.Count > 0) ClearArrows();
        }
        else
        {
            DeadBody[] deadBodys = Helpers.AllDeadBodies();
            //削除するキーをリストアップする
            var removeList = Arrows.Where(entry =>
            {
                foreach (DeadBody body in deadBodys)
                {
                    if (body.ParentId == entry.Key) return false;
                }
                return true;
            });
            foreach (var entry in removeList)
            {
                UnityEngine.Object.Destroy(entry.Value.arrow);
                Arrows.Remove(entry.Key);
            }
            //残った矢印を最新の状態へ更新する
            foreach (DeadBody body in deadBodys)
            {
                if (!Arrows.ContainsKey(body.ParentId))
                {
                    Arrows[body.ParentId] = new Arrow(RoleColor,true,arrowSprite.GetSprite());
                    Arrows[body.ParentId].arrow.SetActive(true);
                }
                Arrows[body.ParentId].Update(body.transform.position);
            }
        }
    }

    public override void OnDied()
    {
        ClearArrows();
    }

    public override void OnMeetingEnd()
    {
        ClearArrows();
    }


    private CustomButton remember;
    public override void ButtonInitialize(HudManager __instance){
        if(remember != null){
            remember.Destroy();
        }
        remember = new CustomButton(
            () =>
            {
                byte targetId = deadBodyId;
                Events.LocalEvent.Activate(new AmnesiacEvent(targetId));
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return deadBodyId != Byte.MaxValue && PlayerControl.LocalPlayer.CanMove; },
            () => { remember.Timer = remember.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.remember"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        remember.MaxTimer = rememberCoolDownOption.getFloat();    
    }

    private void ClearArrows()
    {
        //矢印を消す
        foreach (Arrow arrow in Arrows.Values)
        {
            UnityEngine.Object.Destroy(arrow.arrow);
        }
        Arrows.Clear();
    }

    public override void CleanUp(){
        if(remember != null){
            remember.Destroy();
            remember = null;
        }
        ClearArrows();
    }

    public Amnesiac()
        : base("Amnesiac","amnesiac",RoleColor,RoleCategory.Neutral,Side.Amnesiac,Side.Amnesiac,
        new HashSet<Side>() { Side.Amnesiac },new HashSet<Side>() { Side.Amnesiac },
        new HashSet<Patches.EndCondition>() {},
        true,VentPermission.CanNotUse,false,false,false){
        remember = null;
        Arrows = new();
    }
}
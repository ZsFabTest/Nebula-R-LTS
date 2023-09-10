using Nebula.Roles.ComplexRoles;
namespace Nebula.Roles.NeutralRoles;

public class HighRoller : Role,Template.HasWinTrigger{
    public bool WinTrigger { get; set; }
    public byte Winner { get; set; }

    public static Color RoleColor = new(204f / 255f,166f / 255f,86f / 255f);

    private Module.CustomOption targetCnt;
    private Module.CustomOption dieIfGuessFailed;
    private Module.CustomOption canOracleOthers;

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        targetCnt = CreateOption(Color.white,"targetCnt",3f,1f,15f,1f);
        dieIfGuessFailed = CreateOption(Color.white,"dieIfGuessFailed",false);

        canOracleOthers = CreateOption(Color.white,"canOracleOthers",false);

        OracleCooldownOption = CreateOption(Color.white, "divineCoolDown", 30f, 10f, 60f, 2.5f).AddPrerequisite(canOracleOthers);
        OracleCooldownOption.suffix = "second";

        OracleCooldownAdditionOption = CreateOption(Color.white, "divineCoolDownAddition", 5f, 0f, 30f, 2.5f).AddPrerequisite(canOracleOthers);
        OracleCooldownAdditionOption.suffix = "second";

        OracleDurationOption = CreateOption(Color.white, "divineDuration", 1f, 0.5f, 5f, 0.5f).AddPrerequisite(canOracleOthers);
        OracleDurationOption.suffix = "second";

        CandidatesOption = CreateOption(Color.white, "countOfCandidates", 8f, 1f, 15f, 1f).AddPrerequisite(canOracleOthers);
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        GuesserSystem.GlobalInitialize(__instance);
        WinTrigger = false;
    }

    public override void SetupMeetingButton(MeetingHud __instance)
    {
        GuesserSystem.SetupMeetingButton(__instance);
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        GuesserSystem.MeetingUpdate(__instance, meetingInfo);
    }

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Agent);
        RelatedRoles.Add(Roles.EvilAce);
    }

    public override void MyPlayerControlUpdate(){
        if(!dieIfGuessFailed.getBool() && PlayerControl.LocalPlayer.GetModData().GetExtraRoleData(Roles.ProfessionalAssassin.id) <= 0) RPCEventInvoker.UpdateExtraRoleData(PlayerControl.LocalPlayer.PlayerId, Roles.ProfessionalAssassin.id, 1);
        if(PlayerControl.LocalPlayer.GetModData().GetRoleData(GuesserSystem.guessId) >= targetCnt.getFloat()) RPCEventInvoker.WinTrigger(this);
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(1.5f);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    private CustomButton oracleButton;

    private Module.CustomOption OracleCooldownOption;
    private Module.CustomOption OracleCooldownAdditionOption;
    private Module.CustomOption OracleDurationOption;
    private Module.CustomOption CandidatesOption;

    private Dictionary<byte, List<Role>> divineResult = new Dictionary<byte, List<Role>>();

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.OracleButton.png", 115f, "ui.button.oracle.oracle");

    public override void ButtonInitialize(HudManager __instance)
    {
        if(!canOracleOthers.getBool()) return;
        if (oracleButton != null)
        {
            oracleButton.Destroy();
        }
        oracleButton = new CustomButton(
            () =>
            {
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                if (oracleButton.isEffectActive && Game.GameData.data.myData.currentTarget == null)
                {
                    oracleButton.Timer = 0f;
                    oracleButton.isEffectActive = false;
                }
                return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { oracleButton.Timer = oracleButton.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            OracleDurationOption.getFloat(),
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;

                    //まだ占っていなければ占う
                    if (!divineResult.ContainsKey(target.PlayerId))
                {
                    divineResult[target.PlayerId] = Divine(target);
                }

                string message = Language.Language.GetString("role.oracle.message");
                string roles = "";
                int index = 0;
                float rate = 1f;
                foreach (var role in divineResult[target.PlayerId])
                {
                    if (!roles.Equals(""))
                    {
                        roles += ", ";
                    }

                    if (index % 4 == 3) { roles += "\n"; rate *= 1.8f; }

                    roles += Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name"));

                    index++;
                }
                target.GetModData().RoleInfo = roles.Replace("\n", "");
                RPCEventInvoker.SendInfo(target.PlayerId,target.GetModData().RoleInfo);
                message = message.Replace("%ROLES%", roles);
                message = message.Replace("%PLAYER%", target.name);
                CustomMessage customMessage = CustomMessage.Create(target.transform.position, true, message, 5f, 0.5f, 2f, rate, Color.white);
                customMessage.velocity = new Vector3(0f, 0.1f);

                oracleButton.MaxTimer += OracleCooldownAdditionOption.getFloat();
                oracleButton.Timer = oracleButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
            },
            "button.label.oracle"
        ).SetTimer(CustomOptionHolder.InitialModestAbilityCoolDownOption.getFloat());
        oracleButton.MaxTimer = OracleCooldownOption.getFloat();
    }

    public override void CleanUp(){
        if(oracleButton != null){
            oracleButton.Destroy();
            oracleButton = null;
        }
        RPCEventInvoker.UpdateExtraRoleData(PlayerControl.LocalPlayer.PlayerId,Roles.ProfessionalAssassin.id,0);
    }

    private List<Role> Divine(PlayerControl target)
    {
        List<Role> result = new List<Role>();
        Role role = null;

        var data = target.GetModData();
        result.Add(data.role.GetActualRole(data));

        for (int i = 1; i < (int)CandidatesOption.getFloat(); i++)
        {
            do{
                role = Roles.AllRoles[NebulaPlugin.rnd.Next(Roles.AllRoles.Count)];
            }while(role.IsSpawnable() && !result.Contains(role));
        }

        //ランダムに並び替えたものを返す
        return result.OrderBy(a => Guid.NewGuid()).ToList();
    }

    public override bool CheckAdditionalWin(PlayerControl player, Patches.EndCondition condition) => GuesserSystem.CheckAdditionalWin(player,condition);

    public HighRoller() : base("HighRoller","highRoller",RoleColor,RoleCategory.Neutral,Side.HighRoller,Side.HighRoller,
         new HashSet<Side>() { Side.HighRoller },new HashSet<Side>() { Side.HighRoller },new HashSet<Patches.EndCondition>() { Patches.EndCondition.HighRollerWin },
         true,VentPermission.CanNotUse,false,false,false){
        Patches.EndCondition.HighRollerWin.TriggerRole = this;
        oracleButton = null;
    }
}
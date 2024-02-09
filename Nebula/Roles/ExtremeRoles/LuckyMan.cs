namespace Nebula.Roles.CrewmateRoles;

public class LuckyMan : Role{
    public static Color RoleColor = new Color(162f / 255f,222f / 255f,50f / 255f);
    public Module.CustomOption chanceToRevievOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        chanceToRevievOption = CreateOption(Color.white,"chanceToReviev",50f,10f,100f,10f);
        chanceToRevievOption.suffix = "percent";
    }

    public override void OnExiledPost(byte[] voters){
        int r = NebulaPlugin.rnd.Next(0,101);
        if(r <= chanceToRevievOption.getFloat()) RPCEventInvoker.FixedRevive(PlayerControl.LocalPlayer);
    }

    public override Helpers.MurderAttemptResult OnMurdered(byte murderId,byte playerId){
        //Debug.LogError("114514");
        int r = NebulaPlugin.rnd.Next(0,101);
        if(r <= chanceToRevievOption.getFloat()) return Helpers.MurderAttemptResult.SuppressKill;
        return Helpers.MurderAttemptResult.PerformKill;
    }

    public LuckyMan()
        : base("LuckyMan","luckyMan",RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
        Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
        false,VentPermission.CanNotUse,false,false,false){
    }
}
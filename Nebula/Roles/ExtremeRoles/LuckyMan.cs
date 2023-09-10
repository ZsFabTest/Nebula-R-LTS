namespace Nebula.Roles.CrewmateRoles;

public class LuckyMan : Role{
    public static Color RoleColor = new Color(162f / 255f,222f / 255f,50f / 255f);
    public static Module.CustomOption chanceToRevievOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        chanceToRevievOption = CreateOption(Color.white,"chanceToReviev",50f,10f,100f,10f);
        chanceToRevievOption.suffix = "percent";
    }

    public override bool OnExiledPost(byte[] voters, byte playerId)
    {
        if(playerId == PlayerControl.LocalPlayer.PlayerId){
            int r = NebulaPlugin.rnd.Next(1,101);
            if(r <= chanceToRevievOption.getFloat()) return true;
        }
        return false;
    }

    public override Helpers.MurderAttemptResult OnMurdered(byte murderId,byte playerId){
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
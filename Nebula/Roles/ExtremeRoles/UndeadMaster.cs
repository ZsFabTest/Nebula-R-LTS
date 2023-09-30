namespace Nebula.Roles.NeutralRoles;

public class UndeadMaster : Role{
    public static Color RoleColor = new();

    public UndeadMaster() : base("UndeadMaster","undeadMaster",RoleColor,RoleCategory.Neutral,Side.UndeadMaster,Side.UndeadMaster,
         new HashSet<Side>() { Side.UndeadMaster },new HashSet<Side>() { Side.UndeadMaster },new HashSet<Patches.EndCondition>() { Patches.EndCondition.UndeadMasterWin },
         true,VentPermission.CanNotUse,false,false,false){

    }
}
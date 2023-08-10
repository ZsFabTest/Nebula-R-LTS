namespace Nebula.Roles.CrewmateRoles;
public class ZombieSidekick : Role{
    public override bool IsSpawnable()
    {
        return Roles.Zombie.IsSpawnable();
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if(PlayerControl.LocalPlayer.GetModData().role.category == RoleCategory.Impostor || PlayerControl.LocalPlayer.GetModData().role == Roles.ZombieSidekick || Game.GameData.data.myData.CanSeeEveryoneInfo){
            displayColor = Color;
        }
    }

    public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if(Helpers.playerById(playerId).GetModData().role.category == RoleCategory.Impostor || PlayerControl.LocalPlayer.GetModData().role == Roles.ZombieSidekick){
            displayColor = Color;
        }
    }

    public ZombieSidekick() : base("ZombieSidekick","zombieSidekick",Palette.ImpostorRed,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
        Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,ImpostorRoles.Impostor.impostorEndSet,
        true,VentPermission.CanNotUse,false,true,true){
            IsHideRole = true;
    }
}
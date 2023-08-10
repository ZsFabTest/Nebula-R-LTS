namespace Nebula.Roles.CrewmateRoles;

public class WhiteCat : Role
{

    public override bool IsGuessableRole { get => false; protected set => base.IsGuessableRole = value; }

    public override void OnMurdered(byte murderId)
    {
        Roles.SchrodingersCat.OnMurdered(murderId);
    }

    public WhiteCat()
        : base("WhiteCat", "whiteCat", Palette.CrewmateBlue, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             true, VentPermission.CanNotUse, false, false, true)
    {
        IsHideRole = true;
    }
}

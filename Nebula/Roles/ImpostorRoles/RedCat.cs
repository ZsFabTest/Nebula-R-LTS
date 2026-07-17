namespace Nebula.Roles.ImpostorRoles;

public class RedCat : Role
{
    public override bool IsGuessableRole { get => false; protected set => base.IsGuessableRole = value; }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        HideKillButtonEvenImpostor = !Roles.SchrodingersCat.canUseKillButtonI.getBool();
    }

    public override void OnMurdered(byte murderId)
    {
        Roles.SchrodingersCat.OnMurdered(murderId);
    }

    public RedCat()
        : base("RedCat", "redCat", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet,
             Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        IsHideRole = true;
    }
}

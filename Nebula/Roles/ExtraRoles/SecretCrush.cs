using Nebula.Patches;
using Nebula.Roles.NeutralRoles;

namespace Nebula.Roles.ExtraRoles;

public class SecretCrush : ExtraRole
{
    public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
    {
        return condition == EndCondition.YandereWin;
    }
    public SecretCrush() : base("SecretCrush", "secretCrush", Yandere.RoleColor, 0)
    {
        IsHideRole = true;
    }
}
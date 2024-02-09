/*
namespace Nebula.Roles.ExtraRoles;

public class Voter : Template.StandardExtraRole
{
    public static Color RoleColor = new Color(181f / 255f, 230f / 255f, 29f / 255f);

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = false;
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) showFlag = true;

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(
                RoleColor, "!");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "!");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeVoter", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeVoter");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.Voter.IsSpawnable(); });
        return option;
    }

    public Voter() : base("Voter", "voter", RoleColor, 0)
    {
    }
}
*/
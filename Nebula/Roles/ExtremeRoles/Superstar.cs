namespace Nebula.Roles.ExtraRoles;

public class Superstar : ExtraRole{
    public static Color RoleColor = new Color(255f / 255f,242f / 255f,0f / 255f);

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(RoleColor,"☆");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        if (role.category == RoleCategory.Impostor || role.category == RoleCategory.Neutral) return null;
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeSuperstar", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeSuperstar");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.Superstar.IsSpawnable(); });
        return option;
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag) => EditDisplayRoleNameForcely(playerId,ref displayName);

    private void _sub_Assignment(Patches.AssignMap assignMap, List<byte> players, int count)
    {
        int chance = Roles.Superstar.RoleChanceOption.getSelection() + 1;

        byte playerId;
        for (int i = 0; i < count; i++)
        {
            //割り当てられない場合終了
            if (players.Count == 0) return;

            if (chance <= NebulaPlugin.rnd.Next(10)) continue;

            playerId = players[NebulaPlugin.rnd.Next(players.Count)];
            assignMap.AssignExtraRole(playerId, id, 0);
            players.Remove(playerId);
        }
    }

    public override void Assignment(Patches.AssignMap assignMap)
    {
        List<byte> crewmates = new List<byte>();

        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (!player.GetModData()?.role.CanHaveExtraAssignable(this) ?? true) continue;

            switch (player.GetModData()?.role.category)
            {
                case RoleCategory.Crewmate:
                    crewmates.Add(player.PlayerId);
                    break;
            }
        }

        _sub_Assignment(assignMap, crewmates, (int)Roles.Superstar.RoleCountOption.getFloat());
    }

    public override void EditDescriptionString(ref string description)
    {
        description += "\n" + Language.Language.GetString("role.superstar.description");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "☆");
    }

    public Superstar()
         : base("Superstar","superstar",RoleColor,0){
    }
}
namespace Nebula.Roles.ExtraRoles;

public class Wanted : ExtraRole{
    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(Color,"☆");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        if (role.category == RoleCategory.Crewmate || role.category == RoleCategory.Neutral) return null;
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeWanted", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeWanted");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.Wanted.IsSpawnable(); });
        return option;
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag){
        if(PlayerControl.LocalPlayer.GetModData().role.category != RoleCategory.Crewmate) EditDisplayNameForcely(playerId,ref displayName);
    }

    private void _sub_Assignment(Patches.AssignMap assignMap, List<byte> players, int count)
    {
        int chance = Roles.Wanted.RoleChanceOption.getSelection() + 1;

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
        List<byte> impostors = new List<byte>();

        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (!player.GetModData()?.role.CanHaveExtraAssignable(this) ?? true) continue;

            switch (player.GetModData()?.role.category)
            {
                case RoleCategory.Impostor:
                    impostors.Add(player.PlayerId);
                    break;
            }
        }

        _sub_Assignment(assignMap, impostors, (int)Roles.Wanted.RoleCountOption.getFloat());
    }

    public override void EditDescriptionString(ref string description)
    {
        description += "\n" + Language.Language.GetString("role.wanted.description");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "☆");
    }

    public Wanted()
         : base("Wanted","wanted",Palette.ImpostorRed,0){
    }
}
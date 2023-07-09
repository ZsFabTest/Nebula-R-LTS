namespace Nebula.Roles.ExtraRoles;

public class OnlineCelebrity : ExtraRole
{
    public static Color RoleColor = new Color(255f / 255f, 72f / 255f, 72f / 255f);

    public override void OnDied(byte playerId)
    {
        if(PlayerControl.LocalPlayer.GetModData().role.side == Side.Crewmate) Helpers.PlayQuickFlash(RoleColor);
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        if (role.category == RoleCategory.Impostor || role.category == RoleCategory.Neutral) return null;
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeOnlineCelebrity", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeOnlineCelebrity");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.OnlineCelebrity.IsSpawnable(); });
        return option;
    }

    private void _sub_Assignment(Patches.AssignMap assignMap, List<byte> players, int count)
    {
        int chance = Roles.OnlineCelebrity.RoleChanceOption.getSelection() + 1;

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
        description += "\n" + Language.Language.GetString("role.onlinecelebrity.description");
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = false;
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) showFlag = true;

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(
                RoleColor, "々");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "々");
    }

    public OnlineCelebrity()
        : base("OnlineCelebrity", "onlinecelebrity", RoleColor,0){
    }
}

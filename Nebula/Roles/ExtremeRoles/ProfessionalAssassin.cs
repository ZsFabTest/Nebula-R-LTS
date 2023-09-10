namespace Nebula.Roles.ExtraRoles;

public class ProfessionalAssassin : ExtraRole{
    public override void GlobalInitialize(PlayerControl __instance){
        __instance.GetModData().SetExtraRoleData(id,1);
    }

    public override void Assignment(Patches.AssignMap assignMap){
        List<byte> l = new();
        foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
            if(p.GetModData().role.side == Side.Impostor && p.GetModData().extraRole.Contains(Roles.SecondaryGuesser)){
                l.Add(p.PlayerId);
            }
        }
        
        int chance = RoleChanceOption.getSelection() + 1,count = (int)RoleCountOption.getFloat();

        byte playerId;
        for (int i = 0; i < count; i++)
        {
            //割り当てられない場合終了
            if (l.Count == 0) return;

            if (chance <= NebulaPlugin.rnd.Next(10)) continue;

            playerId = l[NebulaPlugin.rnd.Next(l.Count)];
            assignMap.AssignExtraRole(playerId, id, 0);
            l.Remove(playerId);
        }
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this))
            suffix += Helpers.cs(Color, "A");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        if(role.side != Side.Impostor) return null;
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeProfessionalAssassin", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeProfessionalAssassin");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return IsSpawnable(); });
        option.AddCustomPrerequisite(() => { return role.CanHaveExtraAssignable(Roles.SecondaryGuesser); });
        return option;
    }

    public override bool IsSpawnable(){
        return base.IsSpawnable() && Roles.SecondaryGuesser.IsSpawnable();
    }

    public override void CleanUp(){
        RPCEventInvoker.UpdateExtraRoleData(PlayerControl.LocalPlayer.PlayerId,id,0);
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag){
        if(Game.GameData.data.myData.CanSeeEveryoneInfo || PlayerControl.LocalPlayer.PlayerId == playerId) EditDisplayNameForcely(playerId,ref displayName);
    }

    public override void EditDescriptionString(ref string description)
    {
        description += "\n" + Language.Language.GetString("role.superstar.description");
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(Color,"A");
    }

    public ProfessionalAssassin() : base("ProfessionalAssassin","professionalAssassin",ComplexRoles.FGuesser.RoleColor,1){
    }
}
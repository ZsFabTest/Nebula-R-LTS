namespace Nebula.Roles.ExtraRoles{
    public class DeputyMayor : Template.StandardExtraRole{
        public static Color RoleColor = CrewmateRoles.Mayor.RoleColor;

        private Module.CustomOption extraVoteNumOption;

        public override void LoadOptionData()
        {
            base.LoadOptionData();
            extraVoteNumOption = CreateOption(Color.white,"extraVoteNum",1f,1f,5f,1f);
        }

        public override void OnMeetingStart()
        {
            Debug.Log((byte)(1 + (int)extraVoteNumOption.getFloat()));
            RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer,(byte)(1 + (int)extraVoteNumOption.getFloat()));
        }

        public override void OnMeetingEnd()
        {
            RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer,1);
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
                    RoleColor, "M");
        }

        public override void EditSpawnableRoleShower(ref string suffix, Role role)
        {
            if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "M");
        }

        public override Module.CustomOption? RegisterAssignableOption(Role role)
        {
            Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeDeputyMayor", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeDeputyMayor");
            option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
            option.AddCustomPrerequisite(() => { return Roles.DeputyMayor.IsSpawnable(); });
            return option;
        }

        public DeputyMayor() : base("DeputMayor","deputMayor",RoleColor,0){
        }
    }
}
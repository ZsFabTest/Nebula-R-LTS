/*
namespace Nebula.Roles.ExtraRoles{
    public class Singer : Template.StandardExtraRole{
        static public Color RoleColor = new Color(240f / 255f,135f / 255f,132f / 255f);

        private Module.CustomOption showRoleNum;
        public Module.CustomOption jibaiAlwaysBeSinger;

        public override void LoadOptionData()
        {
            base.LoadOptionData();
            showRoleNum = CreateOption(Color.white,"showRoleNum",4f,2f,15f,1f);
            jibaiAlwaysBeSinger = CreateOption(Color.white,"jibaiAlwaysBeSinger",false).AddInvPrerequisite(Roles.Challenger.ChallengerDieIfMeetingStart).AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        }

        public override void MyPlayerControlUpdate()
        {
            Patches.PlayerStatistics statistics = new(new ShipStatus());
            if(statistics.TotalAlive <= showRoleNum.getFloat()) RPCEventInvoker.SetRoleInfo(PlayerControl.LocalPlayer,Language.Language.GetString("role." + Game.GameData.data.myData.getGlobalData().role.LocalizeName + ".name"),false);
        }

        public override void EditDisplayNameForcely(byte playerId, ref string displayName)
        {
            displayName += Helpers.cs(RoleColor,"6");
        }

        public override Module.CustomOption? RegisterAssignableOption(Role role)
        {
            Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeSinger", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeSinger");
            option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
            option.AddCustomPrerequisite(() => { return Roles.Singer.IsSpawnable(); });
            return option;
        }

        public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag){
            if(PlayerControl.LocalPlayer.PlayerId == playerId || Game.GameData.data.myData.CanSeeEveryoneInfo)
                EditDisplayNameForcely(playerId,ref displayName);
        }

        public override void EditSpawnableRoleShower(ref string suffix, Role role)
        {
            if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "6");
        }

        public Singer() : base("Singer","singer",RoleColor,0){
        }
    }
}
*/
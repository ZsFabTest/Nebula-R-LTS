/*
namespace Nebula.Roles.ExtraRoles{
    public class Signaller : Template.StandardExtraRole{
        public static Color RoleColor = new Color(255f / 255f,177f / 255f,41f / 255f);

        private Vector3 pos = new Vector3();

        public override void OnMeetingStart()
        {
            pos = PlayerControl.LocalPlayer.transform.position;
        }

        public override void OnMeetingEnd()
        {
            PlayerControl.LocalPlayer.transform.position = pos;
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
                    RoleColor, "S");
        }

        public override void EditSpawnableRoleShower(ref string suffix, Role role)
        {
            if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "S");
        }

        public override Module.CustomOption? RegisterAssignableOption(Role role)
        {
            Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeSignaller", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeSignaller");
            option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
            option.AddCustomPrerequisite(() => { return Roles.Signaller.IsSpawnable(); });
            return option;
        }

        public Signaller() : base("Signaller","signaller",RoleColor,0){
        }
    }
}
*/
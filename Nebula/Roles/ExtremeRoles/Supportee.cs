namespace Nebula.Roles.ExtraRoles{
    public class Supportee : ExtraRole{
        public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
        {
            if(playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) EditDisplayNameForcely(playerId,ref displayName);
        }

        public override void EditDisplayNameForcely(byte playerId, ref string displayName)
        {
            displayName += Helpers.cs(Color,"+");
        }

        public override void EditDescriptionString(ref string description)
        {
            description += "\n" + Language.Language.GetString("role.supportee.description");
        }

        public Supportee() : base("Supportee","supportee",CrewmateRoles.Supporter.RoleColor,0){
            IsHideRole = true;
        }
    }
}
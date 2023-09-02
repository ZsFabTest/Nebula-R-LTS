namespace Nebula.Roles.CrewmateRoles{
    public class 杀路无法意识 : Template.TCrewmate{
        public override bool IsGuessableRole => false;

        public override void LoadOptionData(){
            TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        }

        public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
            displayColor = Color;
        }

        public 杀路无法意识() : base("杀路无法意识","杀路无法意识",Roles.Singer.Color,true){
        }
    }
}
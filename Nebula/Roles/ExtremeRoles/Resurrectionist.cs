namespace Nebula.Roles.CrewmateRoles{
    public class Resurrectionist : Template.TCrewmate{
        public static Color RoleColor = new Color(63f / 255f,72f / 255f,204f / 255f);

        private bool canRevive = false;
        public bool hasRevived = false;

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
        }

        public override void OnTaskComplete(PlayerTask? task)
        {
            //PlayerControl.LocalPlayer.GetModData().Tasks.Completed
            //Game.GameData.data.myData.getGlobalData().Tasks.AllTasks
            if(PlayerControl.LocalPlayer.GetModData().Tasks.Completed >= Game.GameData.data.myData.getGlobalData().Tasks.AllTasks){
                canRevive = true;
            }
        }

        public override void GlobalInitialize(PlayerControl __instance)
        {
            canRevive = false;
            hasRevived = false;
        }

        public override void OnMeetingEnd()
        {
            if(PlayerControl.LocalPlayer.Data.IsDead && canRevive && !hasRevived){
                RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer);
                hasRevived = true;
            }
        }

        public Resurrectionist() : base("Resurrectionist","resurrectionist",RoleColor,false){
            canRevive = false;
            hasRevived = false;
        }
    }
}
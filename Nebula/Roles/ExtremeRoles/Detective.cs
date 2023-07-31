/*
using TMPro;

namespace Nebula.Roles.CrewmateRoles{
    public class Detective : Template.TCrewmate{
        public static Color RoleColor = new Color(51f / 255f,89f / 255f,242f / 255f);

        public Dictionary<byte,float> reportTime;

        public GameData.PlayerInfo info;
        public bool ismereport = false;

        public Module.CustomOption timeToGetMurder;

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
            timeToGetMurder = CreateOption(Color.white,"timeToGetMurder",5f,0f,45f,2.5f);
            timeToGetMurder.suffix = "second";
        }

        public override void GlobalInitialize(PlayerControl __instance)
        {
            reportTime = new();
            ismereport = false;
        }

        public override void OnAnyoneMurdered(byte murderId, byte targetId)
        {
            reportTime.Add(targetId,Time.deltaTime);
        }

        public override void MeetingUpdate(MeetingHud __instance, TextMeshPro meetingInfo)
        {
            if(Game.GameData.data.Reporter.PlayerId != PlayerControl.LocalPlayer.PlayerId) return;
            string info = Language.Language.GetString("meeting.deadRole") + ":" + Language.Language.GetString("role." + Game.GameData.data.Dead.GetModData().role.LocalizeName + ".name") + "\n";
            info += Language.Language.GetString("meeting.murder") + ":" + (reportTime[Game.GameData.data.Dead.PlayerId] <= timeToGetMurder.getFloat() ? Game.GameData.data.Dead.name : "???");
            meetingInfo.text = info;
        }

        public override void OnMeetingEnd()
        {
            reportTime.Clear();
            ismereport = false;
        }

        public Detective() : base("Detective","detective",RoleColor,false){
            reportTime = new();
            ismereport = false;
        }
    }
}
*/
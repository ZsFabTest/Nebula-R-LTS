namespace Nebula.Roles.CrewmateRoles;
public class WiseMan : Template.TCrewmate{
    public static Color RoleColor = new(108f / 255f,69f / 255f,176f / 255f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        string Info = "";
        short line = 1;
        List<string> allrole = new();
        foreach(PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
            allrole.Add(Language.Language.GetString("role." + p.GetModData().role.LocalizeName + ".name") + ", ");
        }
        allrole.Sort();
        foreach(string s in allrole){
            Info += s;
            if(s.Length >= line * 25){
                Info += "\n";
                line++;
            }
        }
        meetingInfo.text = Info;
        meetingInfo.gameObject.SetActive(true);
    }

    public WiseMan() : base("WiseMan","wiseMan",RoleColor,false){
    }
}
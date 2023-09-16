namespace Nebula.Roles.CrewmateRoles;
public class WiseMan : Template.TCrewmate{
    public static Color RoleColor = new(108f / 255f,69f / 255f,176f / 255f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        string Info = "";
        List<string> infos = new();
        foreach(PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
            infos.Add(Language.Language.GetString("role." + p.GetModData().role.LocalizeName + ".short") + ", ");
        }
        infos.Sort();

        foreach(var s in infos) Info += s;
        
        Info = Info.Insert(Info.Length,"\n");
        
        if(meetingInfo.text != "") meetingInfo.text += "\n";
        meetingInfo.text += Info;
        meetingInfo.gameObject.SetActive(true);
    }

    public WiseMan() : base("WiseMan","wiseMan",RoleColor,false){
    }
}
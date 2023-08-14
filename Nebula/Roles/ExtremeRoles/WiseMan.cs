namespace Nebula.Roles.CrewmateRoles;
public class WiseMan : Template.TCrewmate{
    public static Color RoleColor = new(108f / 255f,69f / 255f,176f / 255f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        string Info = "";
        foreach(PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
            Info += Language.Language.GetString("role." + p.GetModData().role.LocalizeName + ".short") + ", ";
        }
        
        Info = Info.Insert(Info.Length / 2,"\n");
        
        meetingInfo.text = Info;
        meetingInfo.gameObject.SetActive(true);
    }

    public WiseMan() : base("WiseMan","wiseMan",RoleColor,false){
    }
}
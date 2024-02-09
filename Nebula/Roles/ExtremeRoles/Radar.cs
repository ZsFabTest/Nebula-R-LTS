using Nebula.Roles.RoleSystem;

namespace Nebula.Roles.ExtraRoles;

public class Radar : Template.StandardExtraRole{
    public static Color RoleColor = new(163f  /255f,29f / 255f,107f / 255f);

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = false;
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) showFlag = true;

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(
                RoleColor, "Γ");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "Γ");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeRadar", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeRadar");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.Radar.IsSpawnable(); });
        return option;
    }

    private Arrow? arrow;
    SpriteLoader arrowSprite = new SpriteLoader("role.spectre.arrow");

    public override void Initialize(PlayerControl __instance){
        base.Initialize(__instance);
        arrow = null;
    }

    public override void MyPlayerControlUpdate(){
        PlayerControl target = PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((p) => { return p.PlayerId != PlayerControl.LocalPlayer.PlayerId; });
        if(!target) return;
        foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
            if(p.PlayerId != PlayerControl.LocalPlayer.PlayerId){
                target = Vector2.Distance(target.transform.position,PlayerControl.LocalPlayer.transform.position) > Vector2.Distance(p.transform.position,PlayerControl.LocalPlayer.transform.position) ? p : target;
            }
        }
        TrackSystem.PlayerTrack_MyControlUpdate(ref arrow,target,RoleColor,arrowSprite);
    }

    public Radar() : base("Radar","radar",RoleColor,0){
    }
}
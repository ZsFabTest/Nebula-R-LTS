namespace Nebula.Roles.ExtraRoles;

public class Challenged : ExtraRole{
    public override void EditOthersDisplayNameColor(byte playerId,ref Color displayColor){
        if(Helpers.playerById(playerId).GetModData().role == Roles.Challenger) displayColor = Color;
    }

    public override void MyPlayerControlUpdate(){
        if(PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((p) => {
                 return !p.Data.IsDead && p.GetModData().role == Roles.Challenger;
             }) == null){
            RPCEventInvoker.ImmediatelyUnsetExtraRole(PlayerControl.LocalPlayer,this);
        }
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag){
        if(PlayerControl.LocalPlayer.PlayerId == playerId || Game.GameData.data.myData.CanSeeEveryoneInfo)
            EditDisplayNameForcely(playerId,ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(Color,"C");
    }

    public override bool IsSpawnable() => false;

    public Challenged() : base("Challenged","challenged",NeutralRoles.Challenger.RoleColor,0){
        IsHideRole = true;
    }
}
namespace Nebula.Roles.CrewmateRoles{
    public class Supporter : Role{
        public static Color RoleColor = new Color(157f / 255f,242f / 255f,32f / 255f);

        public byte targetId { get; private set; }

        public override void Initialize(PlayerControl __instance){
            List<byte> crewmates = new();
            foreach(var player in PlayerControl.AllPlayerControls.GetFastEnumerator()){
                if(player.GetModData().role.category == RoleCategory.Crewmate && player.GetModData().role != Roles.Madmate && !player.GetModData().IsMadmate() && player.PlayerId != PlayerControl.LocalPlayer.PlayerId && !player.GetModData().extraRole.Contains(Roles.SecondaryJackal) && !player.GetModData().IsMadmate() && player.PlayerId != PlayerControl.LocalPlayer.PlayerId && !player.GetModData().extraRole.Contains(Roles.Lover)){
                    crewmates.Add(player.PlayerId);
                }
            }
            if(crewmates.Count == 0){
                RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Opportunist);
                return;
            }
            targetId = crewmates[NebulaPlugin.rnd.Next(0,crewmates.Count)];
            RPCEventInvoker.SetExtraRole(Helpers.playerById(targetId),Roles.Supportee,PlayerControl.LocalPlayer.PlayerId);
        }

        public override void MyPlayerControlUpdate()
        {
            //Debug.Log(Language.Language.GetString("role." + target.GetModData().role.LocalizeName + ".name"));
            //RPCEventInvoker.SetRoleInfo(Helpers.playerById(targetId),Language.Language.GetString("role." + Helpers.playerById(targetId).GetModData().role.LocalizeName + ".name"),false);
            Helpers.playerById(targetId).GetModData().RoleInfo = Language.Language.GetString("role." + Helpers.playerById(targetId).GetModData().role.LocalizeName + ".name");
        }

        public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
        {
            if(playerId == targetId) displayColor = RoleColor;
        }

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
        }

        public override void CleanUp()
        {
            base.CleanUp();
            RPCEventInvoker.ImmediatelyUnsetExtraRole(Helpers.playerById(targetId),Roles.Supportee);
        }

        public Supporter() : base("Supporter","supporter",RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
             Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
             false,VentPermission.CanNotUse,false,false,true){
            targetId = (byte)Game.GameData.RegisterRoleDataId("supporter.targetid");
        }
    }
}
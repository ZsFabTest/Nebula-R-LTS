namespace Nebula.Roles.NeutralRoles{
    public class Oracle : Role{
        public static Color RoleColor = new Color(127f / 255f,26f / 255f,140f / 255f);

        private Module.CustomOption initKillCooldown;
        private Module.CustomOption reduceKillCooldown;
        private Module.CustomOption leastKillCooldown;

        public override void LoadOptionData()
        {
            initKillCooldown = CreateOption(Color.white,"initKillCooldown",30f,15f,45f,2.5f);
            initKillCooldown.suffix = "second";
            reduceKillCooldown = CreateOption(Color.white,"reduceKillCooldown",5f,1f,10f,1f);
            reduceKillCooldown.suffix = "second";
            leastKillCooldown = CreateOption(Color.white,"leastKillCooldown",0f,0f,15f,2.5f);
            leastKillCooldown.suffix = "second";
        }

        public int killDataId { get; private set; }
        public override void GlobalInitialize(PlayerControl __instance)
        {
            RPCEventInvoker.UpdateRoleData(PlayerControl.LocalPlayer.PlayerId,killDataId,0);
        }

        private CustomButton killButton;
        public override void ButtonInitialize(HudManager __instance)
        {
            if (killButton != null)
            {
                killButton.Destroy();
            }
            killButton = new CustomButton(
                () =>
                {
                    PlayerControl target = Game.GameData.data.myData.currentTarget;
                    var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, false, true);
                    RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,killDataId,1);
                    killButton.Timer = killButton.MaxTimer - reduceKillCooldown.getFloat() * PlayerControl.LocalPlayer.GetModData().GetRoleData(killDataId);
                    Game.GameData.data.myData.currentTarget = null;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null; },
                () => { killButton.Timer = killButton.MaxTimer - reduceKillCooldown.getFloat() * PlayerControl.LocalPlayer.GetModData().GetRoleData(killDataId); },
                __instance.KillButton.graphic.sprite,
                Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
                __instance,
                Module.NebulaInputManager.modKillInput.keyCode,
                "button.label.kill"
            ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
            killButton.MaxTimer = initKillCooldown.getFloat();
            killButton.SetButtonCoolDownOption(true);
        }

        public override void CleanUp()
        {
            if(killButton != null){
                killButton.Destroy();
                killButton = null;
            }
        }

        public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
        {
            if(PlayerControl.LocalPlayer.GetModData().role.side == Side.Oracle){
                displayColor = RoleColor;
            }
        }

        public override void MyPlayerControlUpdate()
        {
            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        }

        public Oracle() : base("OracleN","oracleN",RoleColor,RoleCategory.Neutral,Side.Oracle,Side.Oracle,
         new HashSet<Side>() { Side.Oracle },new HashSet<Side>() { Side.Oracle },new HashSet<Patches.EndCondition>() { Patches.EndCondition.OracleWin },
         true,VentPermission.CanNotUse,false,true,true){
            killDataId = Game.GameData.RegisterRoleDataId("oracleN.killdataid");
        }
    }
}
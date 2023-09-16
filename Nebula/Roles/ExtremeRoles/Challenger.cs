namespace Nebula.Roles.NeutralRoles{
    public class Challenger : Role,Template.HasWinTrigger{
        public bool WinTrigger { get; set; }
        public byte Winner { get; set; }

        public static Color RoleColor = new Color(106f / 255f,107f / 255f,3f / 255f);

        private Module.CustomOption killCooldown;
        private Module.CustomOption targetCnt;
        private int killed = 0;

        public override void GlobalInitialize(PlayerControl __instance)
        {
            WinTrigger = false;
            killed = 0;
        }

        public override void LoadOptionData()
        {
            killCooldown = CreateOption(Color.white,"killCooldown",7.5f,0f,15f,2.5f);
            killCooldown.suffix = "second";
            targetCnt = CreateOption(Color.white,"targetCnt",3f,1f,15f,1f);
        }

        private CustomButton killButton,challenge;
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
                    killButton.Timer = killButton.MaxTimer;
                    Game.GameData.data.myData.currentTarget = null;
                    killed++;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null; },
                () => { killButton.Timer = killButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
                __instance,
                Module.NebulaInputManager.modKillInput.keyCode,
                "button.label.kill"
            ).SetTimer(killCooldown.getFloat());
            killButton.MaxTimer = killCooldown.getFloat();
            killButton.SetButtonCoolDownOption(true);
        }

        public override void MyPlayerControlUpdate()
        {
            if(killed >= targetCnt.getFloat()){
                RPCEventInvoker.WinTrigger(this);
                return;
            }

            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((target) => {
                if(target.GetModData().extraRole.Contains(Roles.Challenged)) return true;
                return false;
            });
            bool flag = true;
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);
            foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
                if(p.PlayerId == PlayerControl.LocalPlayer.PlayerId || p.Data.IsDead) continue;
                if(p.GetModData().extraRole.Contains(Roles.Challenged)){
                    flag = false;
                    break;
                }
            }
            if(flag && !PlayerControl.LocalPlayer.Data.IsDead){
                int idx,hasTryCnt = 0;
                while(PlayerControl.AllPlayerControls[idx = NebulaPlugin.rnd.Next(0,PlayerControl.AllPlayerControls.GetFastEnumerator().Count())].Data.IsDead || PlayerControl.AllPlayerControls[idx].PlayerId == PlayerControl.LocalPlayer.PlayerId)
                     { if(++hasTryCnt >= 1000){ idx = -1; break; } }
                if(idx != -1) RPCEventInvoker.SetExtraRole(PlayerControl.AllPlayerControls[idx],Roles.Challenged,0);
                else Debug.LogWarning("There's no man can be choosed for challenger!");
            }
        }

        public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
        {
            if(Helpers.playerById(playerId).GetModData().extraRole.Contains(Roles.Challenged)) displayColor = RoleColor;
        }

        public override void OnKillPlayer(byte targetId)
        {
            RPCEventInvoker.ImmediatelyUnsetExtraRole(Helpers.playerById(targetId),Roles.Challenged);
            challenge.Timer = challenge.MaxTimer;
        }

        public override void OnExiledPre(byte[] voters){
            PlayerControl target = PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((p) => {
                return !p.Data.IsDead && p.GetModData().extraRole.Contains(Roles.Challenged);
            });
            if(target == null) return;
            RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,target.PlayerId,
                Game.PlayerData.PlayerStatus.Dead.Id,false);
            RPCEventInvoker.CleanDeadBody(target.PlayerId);
        }

        public override void CleanUp()
        {
            if(killButton != null){
                killButton.Destroy();
                killButton = null;
            }
            if(challenge != null){
                challenge.Destroy();
                challenge = null;
            }
        }

        public Challenger() : base("Challenger","challenger",RoleColor,RoleCategory.Neutral,Side.Challenger,Side.Challenger,
             new HashSet<Side>() { Side.Challenger },new HashSet<Side>() { Side.Challenger },new HashSet<Patches.EndCondition>() { Patches.EndCondition.ChallengerWin },
             true,VentPermission.CanNotUse,false,true,true){
            challenge = killButton = null;
        }
    }
}
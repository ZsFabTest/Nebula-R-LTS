namespace Nebula.Roles.NeutralRoles{
    public class Challenger : Role{
        public static Color RoleColor = new Color(106f / 255f,107f / 255f,3f / 255f);

        private SpriteLoader sprite = new SpriteLoader("Nebula.Resources.TrackEvilButton.png",115f);

        private Module.CustomOption killCooldown;
        private Module.CustomOption ChallengerDieIfMeetingStart;
        private Module.CustomOption challengeCooldown;
        private bool canChallenge;

        public override void GlobalInitialize(PlayerControl __instance)
        {
            canChallenge = true;
        }

        public override void LoadOptionData()
        {
            killCooldown = CreateOption(Color.white,"killCooldown",7.5f,0f,15f,2.5f);
            killCooldown.suffix = "second";
            challengeCooldown = CreateOption(Color.white,"challengeCooldown",15f,0f,45f,2.5f);
            challengeCooldown.suffix = "second";
            ChallengerDieIfMeetingStart = CreateOption(Color.white,"challengerDieIfMeetingStart",true);
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
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null && !canChallenge; },
                () => { killButton.Timer = killButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
                __instance,
                Module.NebulaInputManager.modKillInput.keyCode,
                "button.label.kill"
            ).SetTimer(killCooldown.getFloat());
            killButton.MaxTimer = killCooldown.getFloat();
            killButton.SetButtonCoolDownOption(true);

            if (challenge != null)
            {
                challenge.Destroy();
            }
            challenge = new CustomButton(
                () =>
                {
                    RPCEventInvoker.ImmediatelyChangeRole(Game.GameData.data.myData.currentTarget,Roles.Challenger);
                    killButton.Timer = killButton.MaxTimer;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null && canChallenge; },
                () => { challenge.Timer = challenge.MaxTimer; },
                sprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                "button.label.challenge"
            ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
            challenge.MaxTimer = challengeCooldown.getFloat();
        }

        public override void MyPlayerControlUpdate()
        {
            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((target) => {
                if(canChallenge) return true;
                if(target.GetModData().role == Roles.Challenger) return true;
                return false;
            });
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);
            foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator()){
                if(p.PlayerId == PlayerControl.LocalPlayer.PlayerId || p.Data.IsDead) continue;
                if(p.GetModData().role == Roles.Challenger){
                    canChallenge = false;
                    return;
                }
            }
            canChallenge = true;
        }

        public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
        {
            if(Helpers.playerById(playerId).GetModData().role == Roles.Challenger) displayColor = RoleColor;
        }

        public override void OnMeetingStart()
        {
            if(ChallengerDieIfMeetingStart.getBool() && !canChallenge) RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Suicide.Id,false);
        }

        public override void OnKillPlayer(byte targetId)
        {
            RPCEventInvoker.ImmediatelyChangeRole(Helpers.playerById(targetId),Roles.Amnesiac);
            challenge.Timer = challenge.MaxTimer;
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
            canReport = false;
            CanCallEmergencyMeeting = false;
        }
    }
}
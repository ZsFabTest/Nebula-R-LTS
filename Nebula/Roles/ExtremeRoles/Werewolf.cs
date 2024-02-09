namespace Nebula.Roles.NeutralRoles{
    public class Werewolf : Role{
        public static Color RoleColor = new Color(224f / 255f,154f / 255f,54f / 255f);

        private Module.CustomOption killCooldown;
        private Module.CustomOption deliriumCooldown;
        private Module.CustomOption deliriumDuringTime;

        public override void LoadOptionData()
        {
            killCooldown = CreateOption(Color.white,"killCooldown",2.5f,0f,10f,0.5f);
            killCooldown.suffix = "second";
            deliriumCooldown = CreateOption(Color.white,"deliriumCooldown",25f,2.5f,45f,2.5f);
            deliriumCooldown.suffix = "second";
            deliriumDuringTime = CreateOption(Color.white,"deliriumDuring",10f,2.5f,30f,2.5f);
            deliriumDuringTime.suffix = "second";
        }

        private SpriteLoader sprite = new SpriteLoader("Nebula.Resources.BuskReviveButton.png",115f);

        private CustomButton Mad,killButton;
        private bool isMad = false;

        public override void GlobalInitialize(PlayerControl __instance)
        {
            isMad = false;
        }

        public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
        {
            if(PlayerControl.LocalPlayer.GetModData().role.side == Side.Werewolf) displayColor = RoleColor;
        }

        public override void ButtonInitialize(HudManager __instance)
        {
            if(Mad != null){
                Mad.Destroy();
            }
            Mad = new CustomButton(
                () => {
                    isMad = true;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () => { Mad.Timer = Mad.MaxTimer; },
                sprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                true,
                deliriumDuringTime.getFloat(),
                () => {
                    isMad = false;
                    Mad.Timer = Mad.MaxTimer;
                },
                "button.label.mad"
            ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
            Mad.MaxTimer = deliriumCooldown.getFloat();

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
                () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null && isMad; },
                () => { killButton.Timer = killButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
                __instance,
                Module.NebulaInputManager.modKillInput.keyCode,
                "button.label.kill"
            ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
            killButton.MaxTimer = killCooldown.getFloat();
            killButton.SetButtonCoolDownOption(true);
        }

        public override void CleanUp()
        {
            if(Mad != null){
                Mad.Destroy();
                Mad = null;
            }
            if(killButton != null){
                killButton.Destroy();
                killButton = null;
            }
        }

        public override void MyPlayerControlUpdate()
        {
            if(!isMad){
                VentPermission = VentPermission.CanNotUse;
                canMoveInVents = false;
                IgnoreBlackout = false;
                UseImpostorLightRadius = false;
                return;
            }
            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
            VentPermission = VentPermission.CanUseUnlimittedVent;
            canMoveInVents = true;
            IgnoreBlackout = true;
            UseImpostorLightRadius = true;
        }

        public Werewolf() : base("Werewolf","werewolf",RoleColor,RoleCategory.Neutral,Side.Werewolf,Side.Werewolf,
             new HashSet<Side>() { Side.Werewolf },new HashSet<Side>() { Side.Werewolf },new HashSet<Patches.EndCondition> { Patches.EndCondition.WerewolfWin },
             true,VentPermission.CanNotUse,false,false,false){
            Mad = killButton = null;
            isMad = false;
        }
    }
}
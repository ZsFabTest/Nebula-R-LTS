using Nebula.Patches;
namespace Nebula.Roles.NeutralRoles{
    public class Mercenary : Role{
        public static Color RoleColor = new Color(144f / 255f,161f / 255f,74f / 255f);

        public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
        {
            if (condition == EndCondition.NoGame) return false;
            if (condition == EndCondition.NobodySkeldWin) return false;
            if (condition == EndCondition.NobodyMiraWin) return false;
            if (condition == EndCondition.NobodyPolusWin) return false;
            if (condition == EndCondition.NobodyAirshipWin) return false;

            if (player.Data.IsDead && player.GetModData().FinalData?.status != Game.PlayerData.PlayerStatus.Burned) return false;

            EndGameManagerSetUpPatch.AddEndText(Language.Language.GetString("role.mercenary.additionalEndText"));
            return true;
        }

        private Module.CustomOption killCooldown;
        private Module.CustomOption canUseVent;

        public override void LoadOptionData()
        {
            killCooldown = CreateOption(Color.white,"killCooldown",25f,2.5f,45f,2.5f);
            killCooldown.suffix = "second";
            canUseVent = CreateOption(Color.white,"canUseVent",true);
        }

        public override void GlobalInitialize(PlayerControl __instance)
        {
            VentPermission = canUseVent.getBool() ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse;
            canMoveInVents = canUseVent.getBool();
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
                    Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                    killButton.Timer = killButton.MaxTimer;
                    Game.GameData.data.myData.currentTarget = null;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
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
            if(killButton != null){
                killButton.Destroy();
                killButton = null;
            }
        }

        public override void MyPlayerControlUpdate()
        {
            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        }
//
        public Mercenary() : base("Mercenary","mercenary",RoleColor,RoleCategory.Neutral,Side.Opportunist,Side.Opportunist,
        new HashSet<Side>() { Side.Opportunist },new HashSet<Side>() { Side.Opportunist },new HashSet<EndCondition>() {},
        true,VentPermission.CanUseUnlimittedVent,true,false,true){
            killButton = null;
        }
    }
}
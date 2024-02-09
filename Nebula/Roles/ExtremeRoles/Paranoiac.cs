namespace Nebula.Roles.CrewmateRoles{
    public class Paranoiac : Template.TCrewmate{
        public static Color RoleColor = new(255f / 255f,246f / 255f,194f / 255f);

        private Module.CustomOption lockKillCooldown;

        private SpriteLoader buttonSprite = new("Nebula.Resources.BuskReviveButton.png",115f);

        public override void LoadOptionData(){
            TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
            lockKillCooldown = CreateOption(Color.white,"lockKillCooldown",22.5f,2.5f,45f,2.5f);
            lockKillCooldown.suffix = "second";
        }

        private CustomButton lockButton;
        public override void ButtonInitialize(HudManager __instance){
            lockButton?.Destroy();
            lockButton = new CustomButton(
                () => {
                    RPCEventInvoker.LockKillButton(Game.GameData.data.myData.currentTarget);
                    lockButton.Timer = lockButton.MaxTimer;
                    Game.GameData.data.myData.currentTarget.ShowFailedMurder();
                    Game.GameData.data.myData.currentTarget = null;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
                () => { lockButton.Timer = lockButton.MaxTimer; },
                buttonSprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                "button.label.lock"
            ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
            lockButton.MaxTimer = lockKillCooldown.getFloat();
        }

        public override void CleanUp(){
            if(lockButton != null){
                lockButton.Destroy();
                lockButton = null;
            }
        }

        public Paranoiac() : base("Paranoiac","paranoiac",RoleColor,false){
            hasRoleUpdate = true;
        }
    }
}
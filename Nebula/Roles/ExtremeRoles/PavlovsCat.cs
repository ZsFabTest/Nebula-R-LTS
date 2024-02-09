namespace Nebula.Roles.NeutralRoles;

public class PavlovsCat : Role
{
    private static CustomButton killButton;

    public override bool IsGuessableRole { get => false; protected set => base.IsGuessableRole = value; }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;

        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                if (player.GetModData().role.side == Side.Pavlov)
                {
                    return false;
                }
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color);
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null)
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
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Roles.SchrodingersCat.canUseKillButtonP.getBool(); },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(Roles.SchrodingersCat.killCooldownP.getFloat());
        killButton.MaxTimer = Roles.SchrodingersCat.killCooldownP.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void OnMurdered(byte murderId)
    {
        Roles.SchrodingersCat.OnMurdered(murderId);
    }

    public override void CleanUp()
    {
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Pavlov)
        {
            displayColor = Color;
        }
    }

    public PavlovsCat()
        : base("PavlovsCat", "pavlovsCat", Roles.Pavlov.Color, RoleCategory.Neutral, Side.Pavlov, Side.Pavlov,
             new HashSet<Side>() { Side.Pavlov }, new HashSet<Side>() { Side.Pavlov },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.PavlovWin },
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        killButton = null;
        IsHideRole = true;
    }
}

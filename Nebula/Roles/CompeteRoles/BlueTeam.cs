using Reactor.Utilities.Extensions;
using TMPro;

namespace Nebula.Roles.CompeteRoles;

public class BlueTeam : Role
{
    public int Point = 0;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        Point = 0;
    }

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer,Game.GameData.data.myData.currentTarget,Game.PlayerData.PlayerStatus.Alive);
                Game.GameData.data.myData.currentTarget = null;
                killButton.Timer = killButton.MaxTimer;
                RPCEventInvoker.CompeteGetPoint(1);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(15f);
        killButton.MaxTimer = CustomOptionHolder.CompeteKillCooldownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
        //Debug.LogError(killButton.MaxTimer);
    }

    public override void CleanUp()
    {
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((p) => p.GetModData().role.side != Side.BlueTeam);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color);
    }

    public override void OnDied()
    {
        Events.StandardEvent.SetEvent(() => {
            var pos = PlayerControl.LocalPlayer.transform.position;
            var mapData = Map.MapData.GetCurrentMapData();
            do
            {
                pos = PlayerControl.LocalPlayer.transform.position;
                pos += new Vector3(NebulaPlugin.rnd.Next(-10, 10) + (float)NebulaPlugin.rnd.NextDouble(), NebulaPlugin.rnd.Next(-10, 10) + (float)NebulaPlugin.rnd.NextDouble());
            } while (!mapData.isOnTheShip(pos));
            PlayerControl.LocalPlayer.transform.position = pos;
            RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer, false, false);
        }, time: CustomOptionHolder.CompeteReviveDelayOption.getFloat());
    }

    public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (Helpers.playerById(playerId).GetModData().role.side == Side.BlueTeam) displayColor = Color;
        else displayColor = Color.red;
    }

    public BlueTeam() : base("BlueTeam","blueTeam",Color.blue,RoleCategory.Neutral,Side.BlueTeam,Side.BlueTeam,
        new HashSet<Side> { Side.BlueTeam },new HashSet<Side>() { Side.BlueTeam },new HashSet<Patches.EndCondition>() { Patches.EndCondition.BlueTeamWin },
        true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        canReport = false;
        CanCallEmergencyMeeting = false;

        ValidGamemode = Module.CustomGameMode.Compete;
        IsHideRole = true;

        Point = 0;
    }
}
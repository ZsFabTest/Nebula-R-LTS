﻿namespace Nebula.Roles.NeutralRoles;

public class SchrodingersCat : Role
{
    public static Color RoleColor = ChainShifter.RoleColor;

    public class CatEvent : Events.LocalEvent
    {
        byte murderId;
        Role targetRole;
        public CatEvent(byte murderId,Role targetRole) : base(0.05f)
        {
            this.murderId = murderId;
            this.targetRole = targetRole;
        }

        public override void OnActivate()
        {
            RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer);
            RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer, this.targetRole);
        }
    }

    public Module.CustomOption isGuessable;
    public Module.CustomOption canBeCrewmate;
    public Module.CustomOption canBeImpostor;
    public Module.CustomOption canBeJackal;
    public Module.CustomOption canBePavlovsCat;
    public Module.CustomOption canUseKillButtonI;
    public Module.CustomOption canUseKillButton;
    public Module.CustomOption canUseKillButtonP;
    public Module.CustomOption killCooldown;
    public Module.CustomOption killCooldownP;
    public Module.CustomOption canChangeTeam;
    public Module.CustomOption canBeWerewolf;
    public Module.CustomOption canUseKillButtonW;
    public Module.CustomOption killCooldownW;
    public Module.CustomOption canBeOracle;
    public Module.CustomOption canUseKillButtonO;
    public Module.CustomOption killCooldownO;

    public override void LoadOptionData()
    {
        isGuessable = CreateOption(Color.white, "isGuessable", true);
        canBeCrewmate = CreateOption(Palette.CrewmateBlue, "canBeCrewmate", true);
        canBeImpostor = CreateOption(Palette.ImpostorRed, "canBeImpostor", true);
        canBeJackal = CreateOption(Roles.Jackal.Color, "canBeJackal", true);
        canBePavlovsCat = CreateOption(Roles.Pavlov.Color, "canBePavlovs", true);
        canBeWerewolf = CreateOption(Roles.Werewolf.Color, "canBeWerewolf", true);
        canBeOracle = CreateOption(Roles.OracleN.Color, "canBeOracle", true);
        canUseKillButtonI = CreateOption(Palette.ImpostorRed, "canUseKillButtonI", true).AddPrerequisite(canBeImpostor);
        canUseKillButton = CreateOption(Roles.Jackal.Color, "canUseKillButton", false).AddPrerequisite(canBeJackal);
        canUseKillButtonP = CreateOption(Roles.Pavlov.Color, "canUseKillButtonP", false).AddPrerequisite(canBePavlovsCat);
        canUseKillButtonW = CreateOption(Roles.Werewolf.Color, "canUseKillButtonW", false).AddPrerequisite(canBeWerewolf);
        canUseKillButtonO = CreateOption(Roles.OracleN.Color, "canUseKillButtonO", false).AddPrerequisite(canBeOracle);
        killCooldown = CreateOption(Roles.Jackal.Color, "killCooldown", 25f, 10f, 60f, 2.5f).AddPrerequisite(canUseKillButton);
        killCooldown.suffix = "second";
        killCooldownP = CreateOption(Roles.Pavlov.Color, "killCooldownP", 25f, 10f, 60f, 2.5f).AddPrerequisite(canUseKillButtonP);
        killCooldownP.suffix = "second";
        killCooldownW = CreateOption(Roles.Werewolf.Color, "killCooldownW", 25f, 10f, 60f, 2.5f).AddPrerequisite(canUseKillButtonW);
        killCooldownW.suffix = "second";
        killCooldownO = CreateOption(Roles.OracleN.Color, "killCooldownO", 25f, 10f, 60f, 2.5f).AddPrerequisite(canUseKillButtonO);
        killCooldownO.suffix = "second";
        canChangeTeam = CreateOption(Color.white, "canAlwaysChangeTeam", true);
    }

    public override bool IsGuessableRole { get => isGuessable.getBool(); protected set => base.IsGuessableRole = value; }

    public override void OnMurdered(byte murderId)
    {
        Role checkrole = Helpers.playerById(murderId).GetModData().role;
        if(PlayerControl.LocalPlayer.GetModData().role != Roles.SchrodingersCat && !canChangeTeam.getBool()) return;
        if (checkrole.side == Side.Crewmate && canBeCrewmate.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId,Roles.WhiteCat));
        }
        else if (checkrole.side == Side.Impostor && canBeImpostor.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.RedCat));
        }
        else if (checkrole.side == Side.Jackal && canBeJackal.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.BlueCat));
        }
        else if (checkrole.side == Side.Pavlov && Roles.SchrodingersCat.canBePavlovsCat.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.PavlovsCat));
        }
        else if (checkrole.side == Side.Werewolf && Roles.SchrodingersCat.canBeWerewolf.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.WerewolfsCat));
        }
        else if (checkrole.side == Side.Oracle && Roles.SchrodingersCat.canBeOracle.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.OraclesCat));
        }
    }

    public SchrodingersCat()
     : base("SchorodingersCat", "schrodingersCat", RoleColor, RoleCategory.Neutral, Side.SchrodingersCat, Side.SchrodingersCat,
          new HashSet<Side>() { Side.SchrodingersCat }, new HashSet<Side>() { Side.SchrodingersCat },
          new HashSet<Patches.EndCondition>() { },
          true, VentPermission.CanNotUse, false, false, false)
    {
    }
}

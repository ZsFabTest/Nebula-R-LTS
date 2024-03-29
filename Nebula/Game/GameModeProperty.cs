﻿namespace Nebula.Game;

public class GameModeProperty
{
    public Module.CustomGameMode RelatedGameMode { private set; get; }
    public bool RequireImpostors { private set; get; }
    public bool RequireGhosts { private set; get; }
    public bool RequireStartCountDown { private set; get; }
    public int MinPlayers { private set; get; }
    public int? MaxPlayers { private set; get; }
    public bool CountTasks { private set; get; }
    public Module.CustomOptionTab Tabs { private set; get; }

    public Roles.Role DefaultImpostorRole { private set; get; }
    public Roles.Role DefaultCrewmateRole { private set; get; }

    public System.Action OnCountFinished { private set; get; }

    public GameModeProperty(Module.CustomGameMode RelatedMode, IntRange ValidPlayers, bool RequireImpostors, bool RequireGhosts, bool CountTasks, System.Action? RequireStartCountDown, Roles.Role DefaultCrewmate, Roles.Role DefaultImpostor, Module.CustomOptionTab Tabs)
        : this(RelatedMode, ValidPlayers.min, RequireImpostors, RequireGhosts, CountTasks, RequireStartCountDown, DefaultCrewmate, DefaultImpostor, Tabs)
    {
        MaxPlayers = ValidPlayers.max;
    }

    public GameModeProperty(Module.CustomGameMode RelatedMode, int MinPlayers, bool RequireImpostors, bool RequireGhosts, bool CountTasks, System.Action? RequireStartCountDown, Roles.Role DefaultCrewmate, Roles.Role DefaultImpostor, Module.CustomOptionTab Tabs)
    {
        this.RelatedGameMode = RelatedMode;
        this.RequireImpostors = RequireImpostors;
        this.RequireGhosts = RequireGhosts;
        this.CountTasks = CountTasks;
        this.MinPlayers = MinPlayers;
        this.MaxPlayers = null;
        this.Tabs = Tabs;
        if (RequireStartCountDown == null)
        {
            this.OnCountFinished = () => { };
            this.RequireStartCountDown = false;
        }
        else
        {
            this.OnCountFinished = RequireStartCountDown;
            this.RequireStartCountDown = true;
        }
        this.DefaultCrewmateRole = DefaultCrewmate;
        this.DefaultImpostorRole = DefaultImpostor;

        Properties[RelatedMode] = this;
    }

    static public Dictionary<Module.CustomGameMode, GameModeProperty> Properties = new Dictionary<Module.CustomGameMode, GameModeProperty>();

    static public GameModeProperty StandardMode;
    static public GameModeProperty FreePlayMode;
    static public GameModeProperty StandardHnSMode;
    static public GameModeProperty FreePlayHnSMode;
    static public GameModeProperty BattleMode;
    static public GameModeProperty VirusCrisisMode;
    static public GameModeProperty CompeteMode;

    static public void Load()
    {
        StandardMode = new GameModeProperty(Module.CustomGameMode.Standard, 4, true, false, true, null, Roles.Roles.Crewmate, Roles.Roles.Impostor,
            Module.CustomOptionTab.Settings | Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.ImpostorRoles | Module.CustomOptionTab.NeutralRoles | Module.CustomOptionTab.GhostRoles | Module.CustomOptionTab.Modifiers | Module.CustomOptionTab.AdvancedSettings);
        FreePlayMode = new GameModeProperty(Module.CustomGameMode.FreePlay, 0, false, false, false, null, Roles.Roles.Player, Roles.Roles.Impostor,
            Module.CustomOptionTab.Settings | Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.ImpostorRoles | Module.CustomOptionTab.NeutralRoles | Module.CustomOptionTab.GhostRoles | Module.CustomOptionTab.Modifiers | Module.CustomOptionTab.AdvancedSettings | Module.CustomOptionTab.EscapeRoles);
        StandardHnSMode = new GameModeProperty(Module.CustomGameMode.StandardHnS, 4, true, false, false, null, Roles.Roles.HnSCrewmate, Roles.Roles.HnSReaper,
            Module.CustomOptionTab.Settings | Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.ImpostorRoles | Module.CustomOptionTab.AdvancedSettings);
        FreePlayHnSMode = new GameModeProperty(Module.CustomGameMode.FreePlayHnS, new IntRange(0, 1), false, false, false, null, Roles.Roles.HnSCrewmate, Roles.Roles.HnSReaper,
            Module.CustomOptionTab.Settings | Module.CustomOptionTab.AdvancedSettings);
        BattleMode = new GameModeProperty(Module.CustomGameMode.Battle,2,false,false,false,null,Roles.Roles.YellowTeam,Roles.Roles.GreenTeam,Module.CustomOptionTab.Settings | Module.CustomOptionTab.NeutralRoles);
        VirusCrisisMode = new GameModeProperty(Module.CustomGameMode.VirusCrisis,2,false,false,false,null,Roles.Roles.Survival,Roles.Roles.Infected,Module.CustomOptionTab.Settings | Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.ImpostorRoles);
        CompeteMode = new GameModeProperty(Module.CustomGameMode.Compete, 2, false, false, false, null, Roles.Roles.BlueTeam, Roles.Roles.RedTeam, Module.CustomOptionTab.Settings);
    }

    static public GameModeProperty GetProperty(Module.CustomGameMode gameMode)
    {
        return Properties[gameMode];
    }
}
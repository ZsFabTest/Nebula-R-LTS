using Nebula.Patches;
using System.Reflection.Metadata.Ecma335;

namespace Nebula.Roles;

public class Side
{
    public enum IntroDisplayOption
    {
        STANDARD,
        SHOW_ALL,
        SHOW_ONLY_ME,
        Yanderes
    }

    public static Side Crewmate = new Side("Crewmate", "crewmate", IntroDisplayOption.SHOW_ALL, Palette.CrewmateBlue, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal)
        {
            if (statistics.GetAlivePlayers(Impostor) == 0 &&
            statistics.GetAlivePlayers(Jackal) == 0 &&
            statistics.GetAlivePlayers(Pavlov) == 0 &&
            statistics.GetAlivePlayers(Moriarty) == 0 &&
            statistics.AliveWerewolf == 0 && 
            statistics.AliveOracle == 0 &&
            statistics.AliveChallenger == 0 && 
            statistics.AliveMadmate == 0)
            {
                return EndCondition.CrewmateWinByVote;
            }
            if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks && GameData.Instance.CompletedTasks > 0)
            {
                return EndCondition.CrewmateWinByTask;
            }
        }else if (GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek)
        {
            if (GameManager.Instance.Cast<HideAndSeekManager>().LogicFlowHnS.AllTimersExpired())
                return EndCondition.CrewmateWinHnS;
            if (statistics.AliveCrewmates > 0 && statistics.AliveImpostors == 0)
                return EndCondition.CrewmateWinHnS;
        }
        return null;
    });

    public static Side Impostor = new Side("Impostor", "impostor", IntroDisplayOption.STANDARD, Palette.ImpostorRed, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if(statistics.AliveImpostors == 1 && Roles.LastImpostor.TopOption.getBool()){
            foreach(PlayerControl p in PlayerControl.AllPlayerControls){
                if(!p.Data.IsDead && p.GetModData().role.side == Side.Impostor && !p.GetModData().extraRole.Contains(Roles.LastImpostor)){
                    RPCEventInvoker.AddExtraRole(p,Roles.LastImpostor,0);
                    break;
                }
            }
        }
        if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal)
        {
            if (Game.GameData.data != null)
            {
                int aliveUnawakened = 0;
                foreach (var p in Game.GameData.data.AllPlayers.Values)
                {
                    if (p.IsAlive && p.role == Roles.Covert && p.GetRoleData(Roles.Covert.canKillId) == 0) aliveUnawakened++;
                }
                if (aliveUnawakened == statistics.AliveImpostors && aliveUnawakened>0)
                {
                    List<Game.PlayerData> candidate = new List<Game.PlayerData>(Game.GameData.data.AllPlayers.Values.Where((p) => p.role == Roles.Covert));
                    RPCEventInvoker.UpdateRoleData(candidate[NebulaPlugin.rnd.Next(candidate.Count)].id, Roles.Covert.canKillId, 1);
                }
            }

            //Sabotage
            if (status.Systems != null)
            {
                ISystemType systemType = status.Systems.ContainsKey(SystemTypes.LifeSupp) ? status.Systems[SystemTypes.LifeSupp] : null;
                if (systemType != null)
                {
                    LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                    if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
                    {
                        lifeSuppSystemType.Countdown = 10000f;
                        return EndCondition.ImpostorWinBySabotage;
                    }
                }
                ISystemType systemType2 = status.Systems.ContainsKey(SystemTypes.Reactor) ? status.Systems[SystemTypes.Reactor] : null;
                if (systemType2 == null)
                {
                    systemType2 = status.Systems.ContainsKey(SystemTypes.Laboratory) ? status.Systems[SystemTypes.Laboratory] : null;
                }
                if (systemType2 != null)
                {
                    ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                    if (criticalSystem != null && criticalSystem.Countdown < 0f)
                    {
                        criticalSystem.ClearSabotage();
                        return EndCondition.ImpostorWinBySabotage;
                    }
                }
            }

            if (statistics.AliveImpostors - statistics.AliveImpostorsWithSidekick > 0 &&
            statistics.AliveJackals == 0 &&
            statistics.AlivePavlov == 0 &&
            statistics.AliveMoriarty == 0 &&
            statistics.AliveWerewolf == 0 &&
            statistics.AliveChallenger == 0 &&
            statistics.AliveOracle == 0 &&
            (statistics.TotalAlive - statistics.AliveSpectre - statistics.AliveMadmate - statistics.AliveZombieSidekick - statistics.AlivePuppeteer) <= (statistics.AliveImpostors - statistics.AliveInLoveImpostors - statistics.AliveImpostorsWithSidekick + statistics.AliveInLoveImpostorsWithSidekick) * 2 &&
            (statistics.AliveImpostorCouple + statistics.AliveImpostorTrilemma == 0 ||
            statistics.AliveImpostorCouple * 2 + statistics.AliveImpostorTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3))
            {
                if (TempData.LastDeathReason == DeathReason.Kill)
                {
                    return EndCondition.ImpostorWinByKill;
                }
                else
                {
                    return EndCondition.ImpostorWinByVote;
                }
            }
        }
        else if (GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek)
        {
            if (statistics.AliveCrewmates == 0 && statistics.AliveImpostors>0) return EndCondition.ImpostorWinHnS;
        }

        return null;
    });

    public static Side Jackal = new Side("Jackal", "jackal", IntroDisplayOption.STANDARD, NeutralRoles.Jackal.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if ((statistics.AliveJackals - statistics.AliveInLoveJackals - statistics.AliveJackalsWithMadmate + statistics.AliveInLoveJackalsWithMadmate) * 2 >= (statistics.TotalAlive-statistics.AliveSpectre - statistics.AlivePuppeteer) && statistics.GetAlivePlayers(Impostor) - statistics.AliveImpostorsWithSidekick <= 0 && statistics.GetAlivePlayers(Pavlov) - statistics.AlivePavlovWithSidekick <= 0 && statistics.AliveMoriarty - statistics.AliveMoriartyWithSidekick <= 0 && statistics.AliveWerewolf - statistics.AliveWerewolfWithSidekick <= 0 && statistics.AliveChallenger == 0 && statistics.AliveOracle - statistics.AliveOracleWithSidekick == 0 && statistics.AliveMadmate == 0 &&
        (statistics.AliveJackalCouple + statistics.AliveJackalTrilemma == 0 ||
        statistics.AliveJackalCouple * 2 + statistics.AliveJackalTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3))
        {
            return EndCondition.JackalWin;
        }
        return null;
    });

    public static Side Jester = new Side("Jester", "jester", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Jester.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (Roles.Jester.WinTrigger)
        {
            return EndCondition.JesterWin;
        }
        return null;
    });

    public static Side Vulture = new Side("Vulture", "vulture", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Vulture.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (Roles.Vulture.WinTrigger)
        {
            return EndCondition.VultureWin;
        }
        return null;
    });

    public static Side Arsonist = new Side("Arsonist", "arsonist", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Arsonist.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Roles.Arsonist.WinTrigger)
        {
            return EndCondition.ArsonistWin;
        }
        return null;
    });

    public static Side Empiric = new Side("Empiric", "empiric", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Empiric.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Roles.Empiric.WinTrigger)
        {
            return EndCondition.EmpiricWin;
        }
        return null;
    });

    public static Side Paparazzo = new Side("Paparazzo", "paparazzo", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Paparazzo.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Roles.Paparazzo.WinTrigger)
        {
            return EndCondition.PaparazzoWin;
        }
        return null;
    });

    public static Side Opportunist = new Side("Opportunist", "opportunist", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Opportunist.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        return null;
    });

    public static Side Avenger = new Side("Avenger", "avenger", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Avenger.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    }, (EndCondition endCondition, PlayerStatistics statistics, ShipStatus status) =>
    {
        if (endCondition.IsNoBodyWinEnd) return null;
        if (Roles.Avenger.canTakeOverSabotageWinOption.getBool() && endCondition == EndCondition.ImpostorWinBySabotage) return null;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;
            if (player.role != Roles.Avenger) continue;

            if (player.GetRoleData(Roles.Avenger.avengerCheckerId) == 1)
                return EndCondition.AvengerWin;
        }
        return null;
    });

    public static Side Spectre = new Side("Spectre", "spectre", IntroDisplayOption.STANDARD, NeutralRoles.Spectre.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (Roles.Spectre.canTakeOverTaskWinOption.getBool()) return null;

        bool flag = false;
        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;
            if (player.role != Roles.Spectre) continue;
            if(player.Tasks==null || player.Tasks?.Completed==player.Tasks?.AllTasks){ flag = true;  break; }
        }
        if (!flag) return null;

        if(TasksHandler.TotalAlivesTasks > 0 && TasksHandler.CompletedAlivesTasks == TasksHandler.TotalAlivesTasks)return EndCondition.SpectreWin;
        return null;
    }, (EndCondition endCondition, PlayerStatistics statistics, ShipStatus status) =>
    {
        if (endCondition.IsNoBodyWinEnd) return null;

        if (endCondition == EndCondition.CrewmateWinByTask && (Roles.Spectre.canTakeOverTaskWinOption.getBool()||TasksHandler.TotalAlivesTasks==0)) return null;
        if (endCondition == EndCondition.ImpostorWinBySabotage && Roles.Spectre.canTakeOverSabotageWinOption.getBool()) return null;
        if (endCondition == EndCondition.ArsonistWin) return null;
        if (endCondition == EndCondition.EmpiricWin) return null;
        if (endCondition == EndCondition.JesterWin) return null;
        if (endCondition == EndCondition.LoversWin) return null;
        if (endCondition == EndCondition.VultureWin) return null;
        if (endCondition == EndCondition.AvengerWin) return null;
        if (endCondition == EndCondition.SpectreWin) return null;
        if (endCondition == EndCondition.YandereWin) return null;
        if (endCondition == EndCondition.ChallengerWin) return null;
        if (endCondition == EndCondition.GhostWin) return null;
        if (endCondition == EndCondition.PuppeteerWin) return null;
        if (endCondition == EndCondition.HighRollerWin) return null;
        if (endCondition == EndCondition.MoriartyWin || endCondition == EndCondition.MoriartyWinByKillHolmes) return null;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;
            if (player.role != Roles.Spectre) continue;
            if (player.Tasks != null && player.Tasks?.Completed == player.Tasks?.AllTasks) return EndCondition.SpectreWin;
        }
        return null;
    });

    public static Side ChainShifter = new Side("ChainShifter", "chainShifter", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.ChainShifter.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side Madman = new Side("Madman", "madman", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Madman.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side SchrodingersCat = new Side("SchrodingersCat", "schrodingersCat", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.SchrodingersCat.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side Pavlov = new Side("Pavlov", "pavlov", IntroDisplayOption.STANDARD, NeutralRoles.Pavlov.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if ((statistics.AlivePavlov - statistics.AliveInLovePavlov - statistics.AlivePavlovWithSidekick + statistics.AliveInLovePavlovWithSidekick - statistics.AlivePavlovWithMadmate + statistics.AliveInLovePavlovWithMadmate) * 2 >= (statistics.TotalAlive - statistics.AliveSpectre - statistics.AlivePuppeteer) && statistics.AliveImpostors == 0 &&
        (statistics.AlivePavlovCouple + statistics.AlivePavlovTrilemma == 0 ||
        statistics.AlivePavlovCouple * 2 + statistics.AlivePavlovTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3) &&
        statistics.AliveJackals == 0 && statistics.AliveMoriarty == 0 && statistics.AliveWerewolf == 0 && statistics.AliveChallenger == 0 && statistics.AliveOracle == 0 && statistics.AliveMadmate == 0)
        {
            return EndCondition.PavlovWin;
        }
        return null;
    });

    public static Side Moriarty = new Side("Moriarty","moriarty",IntroDisplayOption.STANDARD, NeutralRoles.Moriarty.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if ((statistics.AliveMoriarty - statistics.AliveInLoveMoriarty - statistics.AliveMoriartyWithSidekick - statistics.AliveInLoveMoriartyWithMadmate + statistics.AliveInLoveMoriartyWithMadmate + statistics.AliveInLoveMoriartyWithSidekick) * 2 >= (statistics.TotalAlive - statistics.AlivePuppeteer) && statistics.AliveImpostors == 0 &&
        (statistics.AliveMoriartyCouple + statistics.AliveMoriartyTrilemma == 0 ||
        statistics.AliveMoriartyCouple * 2 + statistics.AliveMoriartyTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3) &&
        statistics.AliveJackals == 0 && statistics.AlivePavlov == 0 && statistics.AliveWerewolf == 0 && statistics.AliveChallenger == 0 && statistics.AliveOracle == 0 && statistics.AliveMadmate == 0)
        {
            return EndCondition.MoriartyWin;
        }
        else if (Roles.Moriarty.WinTrigger) return EndCondition.MoriartyWinByKillHolmes;
        return null;
    });

    public static Side Cascrubinter = new Side("Cascrubinter","cascrubinter",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Cascrubinter.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.Cascrubinter.WinTrigger){
            return EndCondition.CascrubinterWin;
        }
        return null;
    });

    public static Side Amnesiac = new Side("Amnesiac","amnesiac",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Amnesiac.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        return null;
    });

    public static Side Yandere = new Side("Yandere","yandere",IntroDisplayOption.Yanderes,NeutralRoles.Yandere.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveYandere * 2 > statistics.TotalAlive){
            RPCEventInvoker.SetExtraRole(Roles.Yandere.GetLover(),Roles.SecretCrush,0);
            return EndCondition.YandereWin;
        }
        return null;
    });

    public static Side Guesser = new Side("Guesser","guesser",IntroDisplayOption.STANDARD,ComplexRoles.FGuesser.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.F_Guesser.WinTrigger){
            return EndCondition.GuesserWin;
        }
        return null;
    });


    public static Side Werewolf = new Side("Werewolf","werewolf",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Werewolf.RoleColor,(PlayerStatistics statistics,ShipStatus status) => 
    {
        if(statistics.AliveImpostors == 0 && statistics.AliveJackals == 0 && statistics.AlivePavlov == 0 && statistics.AliveMoriarty == 0 && statistics.AliveChallenger == 0 && statistics.AliveOracle == 0 && statistics.AliveMadmate == 0 &&
        (statistics.AliveWerewolf - statistics.AliveInLoveWerewolf - statistics.AliveWerewolfWithSidekick + statistics.AliveInLoveWerewolfWithSidekick - statistics.AliveWerewolfWithMadmate + statistics.AliveInLoveWerewolfWithMadmate) * 2 >= (statistics.TotalAlive - statistics.AliveSpectre - statistics.AlivePuppeteer) && 
        (statistics.AliveWerewolfCouple + statistics.AliveWerewolfTrilemma == 0 ||
        statistics.AliveWerewolfCouple * 2 + statistics.AliveWerewolfTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3)){
            return EndCondition.WerewolfWin;
        }
        return null;
    });

    public static Side Challenger = new Side("Challenger","challenger",IntroDisplayOption.SHOW_ALL,NeutralRoles.Challenger.RoleColor,(PlayerStatistics statistics,ShipStatus status) => 
    {
        if(statistics.AliveChallenger >= statistics.TotalAlive || Roles.Challenger.WinTrigger){
            return EndCondition.ChallengerWin;
        }
        return null;
    });

    public static Side Oracle = new Side("Oracle","oracle",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Oracle.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveImpostors == 0 && statistics.AliveJackals == 0 && statistics.AlivePavlov == 0 && statistics.AliveMoriarty == 0 && statistics.AliveChallenger == 0 && statistics.AliveWerewolf == 0 && statistics.AliveMadmate == 0 &&
        (statistics.AliveOracle - statistics.AliveInLoveOracle - statistics.AliveOracleWithMadmate - statistics.AliveOracleWithSidekick + statistics.AliveInLoveOracleWithMadmate + statistics.AliveInLoveOracleWithSidekick) * 2 >= (statistics.TotalAlive - statistics.AliveSpectre - statistics.AlivePuppeteer) && 
        (statistics.AliveOracleCouple + statistics.AliveOracleTrilemma == 0 ||
        statistics.AliveOracleCouple * 2 + statistics.AliveOracleTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3)){
            return EndCondition.OracleWin;
        }
        return null;
    });

    public static Side Ghost = new Side("Ghost","ghost",IntroDisplayOption.SHOW_ONLY_ME,new(1f,1f,1f),(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.Ghost.WinTrigger){
            return EndCondition.GhostWin;
        }
        return null;
    });

    public static Side SantaClaus = new Side("SantaClaus", "santaClaus", IntroDisplayOption.STANDARD, NeutralRoles.SantaClaus.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (statistics.TotalAlive == 0) return null;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;

            if (player.role.side != Side.SantaClaus && !player.HasExtraRole(Roles.TeamSanta)) return null;

        }

        return EndCondition.SantaWin;
    });

    public static Side Puppeteer = new Side("Puppeteer","puppeteer",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Puppeteer.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.Puppeteer.WinTrigger){
            return EndCondition.PuppeteerWin;
        }
        return null;
    });

    public static Side HighRoller = new Side("HighRoller","highRoller",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.HighRoller.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.HighRoller.WinTrigger){
            return EndCondition.HighRollerWin;
        }
        return null;
    });

    public static Side YellowTeam = new Side("YellowTeam","yellowTeam",IntroDisplayOption.STANDARD,BattleRoles.YellowTeam.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveYellowTeam > 0 && statistics.AliveGreenTeam <= 0) return EndCondition.YellowTeamWin;
        return null;
    });

    public static Side GreenTeam = new Side("GreenTeam","greenTeam",IntroDisplayOption.STANDARD,BattleRoles.GreenTeam.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveGreenTeam > 0 && statistics.AliveYellowTeam <= 0) return EndCondition.GreenTeamWin;
        return null;
    });

    public static Side Infected = new Side("Infected","infected",IntroDisplayOption.SHOW_ONLY_ME,Palette.ImpostorRed,(PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveInfected >= 1 && statistics.AliveSurvivals <= 0) return EndCondition.InfectedWin;
        return null;
    });

    public static Side Survival = new Side("Survival","survival",IntroDisplayOption.STANDARD,Palette.CrewmateBlue,(PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveInfected <= 0 && statistics.AliveSurvivals >= 1) return EndCondition.SurvivalWin;
        return null;
    });

    public static Side UndeadMaster = new Side("UndeadMaster","undeadMaster",IntroDisplayOption.STANDARD,NeutralRoles.UndeadMaster.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        return null;
    },(EndCondition condition,PlayerStatistics statistics,ShipStatus status) => {
        if(statistics.AliveUndeadMaster <= 0) return null;

        if(condition == EndCondition.NobodyWin) return null;
        if(condition == EndCondition.NobodySkeldWin) return null;
        if(condition == EndCondition.NobodyMiraWin) return null;
        if(condition == EndCondition.NobodyPolusWin) return null;
        if(condition == EndCondition.NobodyAirshipWin) return null;

        return EndCondition.UndeadMasterWin;
    });

    public static Side RedTeam = new Side("RedTeam", "redTeam", IntroDisplayOption.STANDARD, Color.red, (PlayerStatistics statistics, ShipStatus status) => {
        return null;
    });

    public static Side BlueTeam = new Side("BlueTeam", "blueTeam", IntroDisplayOption.STANDARD, Color.blue, (PlayerStatistics statistics, ShipStatus status) => {
        return null;
    });

    public static Side GamePlayer = new Side("GamePlayer", "gamePlayer", IntroDisplayOption.SHOW_ONLY_ME, Palette.CrewmateBlue, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side Extra = new Side("Extra", "extra", IntroDisplayOption.STANDARD, new Color(150, 150, 150), (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Game.GameData.data.IsCanceled)
        {
            return EndCondition.NoGame;
        }


        if (CustomOptionHolder.limiterOptions.getBool())
        {
            if (Game.GameData.data.Timer < 1f)
            {
                if(Game.GameData.data.GameMode == Module.CustomGameMode.VirusCrisis){
                    switch(CustomOptionHolder.winnerIfTimesUpOption.getSelection()){
                        case 0:
                            return EndCondition.SurvivalWin;
                        case 1:
                            return EndCondition.InfectedWin;
                        default:
                            return EndCondition.NobodyWin;
                    }
                }
                else if(Game.GameData.data.GameMode == Module.CustomGameMode.Compete)
                {
                    int point1 = Roles.RedTeam.Point;
                    int point2 = Roles.BlueTeam.Point;
                    Roles.RedTeam.Point = Roles.BlueTeam.Point = 0;
                    if (point1 > point2) return EndCondition.RedTeamWin;
                    else if (point1 < point2) return EndCondition.BlueTeamWin;
                    else return EndCondition.Tie;
                }
                switch (GameOptionsManager.Instance.CurrentGameOptions.MapId)
                {
                    case 0:
                    case 3:
                        return EndCondition.NobodySkeldWin;
                    case 1:
                        return EndCondition.NobodyMiraWin;
                    case 2:
                        return EndCondition.NobodyPolusWin;
                    case 4:
                        return EndCondition.NobodyAirshipWin;
                }
            }
        }

            //Lovers単独勝利
            if (statistics.TotalAlive <= 3)
        {
            if (statistics.AliveCouple == 1 && statistics.AliveTrilemma == 0) return EndCondition.LoversWin;
        }


            //Trilemma
            if (statistics.TotalAlive <= 5)
        {
            if (statistics.AliveTrilemma == 1 && statistics.AliveCouple == 0) return EndCondition.TrilemmaWin;
        }

        if (statistics.TotalAlive == 0)
        {
            return EndCondition.NobodyWin;
        }
        return null;
    });

    public static Side RitualCrewmate = new Side("Crewmate", "crewmate", IntroDisplayOption.STANDARD, Palette.CrewmateBlue, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side VOID = new Side("VOID", "void", IntroDisplayOption.SHOW_ONLY_ME, MetaRoles.VOID.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static List<Side> AllSides = new List<Side>()
        {
            Crewmate, Impostor,
            Jackal, Jester, Vulture, Empiric, Arsonist, Paparazzo, Avenger,ChainShifter,Spectre,SantaClaus,
            GamePlayer,
            Extra,VOID,
            RitualCrewmate,
            Madman,SchrodingersCat,Pavlov,Moriarty,Cascrubinter,Amnesiac,Guesser,Yandere,Werewolf,Challenger,Oracle,Ghost/*,SantaClaus*/,Puppeteer,YellowTeam,GreenTeam,Infected,Survival,
            HighRoller,RedTeam,BlueTeam
        };

    public IntroDisplayOption ShowOption { get; }
    public Color color { get; }
    public string side { get; }
    public string localizeSide { get; }

    public EndCriteriaChecker endCriteriaChecker { get; }
    public EndTakeoverChecker endTakeoverChecker { get; }

    private Side(string side, string localizeSide, IntroDisplayOption displayOption, Color color, EndCriteriaChecker endCriteriaChecker, EndTakeoverChecker endTakeoverChecker)
    {
        this.side = side;
        this.localizeSide = localizeSide;
        this.ShowOption = displayOption;
        this.color = color;
        this.endCriteriaChecker = endCriteriaChecker;
        this.endTakeoverChecker = endTakeoverChecker;
    }

    private Side(string side, string localizeSide, IntroDisplayOption displayOption, Color color, EndCriteriaChecker endCriteriaChecker) :
        this(side, localizeSide, displayOption, color, endCriteriaChecker, (a1, a2, a3) => null)
    {
    }
}

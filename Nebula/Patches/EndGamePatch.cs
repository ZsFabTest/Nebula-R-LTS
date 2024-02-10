﻿using System.Text;
using UnityEngine.SceneManagement;

namespace Nebula.Patches;

public delegate void EndAction(FinalPlayerData finalPlayerData);

public class EndCondition
{
    public static EndCondition CrewmateWinByVote = new EndCondition(GameOverReason.HumansByVote, Palette.CrewmateBlue, "crewmate", 16, Module.CustomGameMode.Standard);
    public static EndCondition CrewmateWinByTask = new EndCondition(GameOverReason.HumansByTask, Palette.CrewmateBlue, "crewmate", 16, Module.CustomGameMode.Standard);
    public static EndCondition CrewmateWinDisconnect = new EndCondition(GameOverReason.HumansDisconnect, Palette.CrewmateBlue, "crewmate", 16, Module.CustomGameMode.Standard);
    public static EndCondition ImpostorWinByKill = new EndCondition(GameOverReason.ImpostorByKill, Palette.ImpostorRed, "impostor", 16, Module.CustomGameMode.Standard);
    public static EndCondition ImpostorWinBySabotage = new EndCondition(GameOverReason.ImpostorBySabotage, Palette.ImpostorRed, "impostor", 16, Module.CustomGameMode.Standard);
    public static EndCondition ImpostorWinByVote = new EndCondition(GameOverReason.ImpostorByVote, Palette.ImpostorRed, "impostor", 16, Module.CustomGameMode.Standard);
    public static EndCondition ImpostorWinDisconnect = new EndCondition(GameOverReason.ImpostorDisconnect, Palette.ImpostorRed, "impostor", 16, Module.CustomGameMode.Standard);
    public static EndCondition CrewmateWinHnS = new EndCondition(GameOverReason.HideAndSeek_ByTimer, Palette.CrewmateBlue, "crewmate", 16, Module.CustomGameMode.StandardHnS);
    public static EndCondition ImpostorWinHnS = new EndCondition(GameOverReason.HideAndSeek_ByKills, Palette.ImpostorRed, "lonelyImpostor", 16, Module.CustomGameMode.StandardHnS);
    public static EndCondition JesterWin = new EndCondition(16, Roles.NeutralRoles.Jester.RoleColor, "jester", 1, Module.CustomGameMode.Standard);
    public static EndCondition JackalWin = new EndCondition(17, Roles.NeutralRoles.Jackal.RoleColor, "jackal", 2, Module.CustomGameMode.Standard);
    public static EndCondition ArsonistWin = new EndCondition(18, Roles.NeutralRoles.Arsonist.RoleColor, "arsonist", 1, Module.CustomGameMode.Standard, false, (fpData) => { PlayerControl.AllPlayerControls.ForEach((Action<PlayerControl>)((p) => { if (!p.Data.IsDead && Roles.Roles.Arsonist.Winner != p.PlayerId) { p.MurderPlayer(p); fpData.GetPlayer(p.PlayerId).status = Game.PlayerData.PlayerStatus.Burned; } })); });
    public static EndCondition EmpiricWin = new EndCondition(19, Roles.NeutralRoles.Empiric.RoleColor, "empiric", 1, Module.CustomGameMode.Standard);
    public static EndCondition PaparazzoWin = new EndCondition(20, Roles.NeutralRoles.Paparazzo.RoleColor, "paparazzo", 1, Module.CustomGameMode.Standard);
    public static EndCondition SpectreWin = new EndCondition(21, Roles.NeutralRoles.Spectre.RoleColor, "spectre", 1, Module.CustomGameMode.Standard);
    public static EndCondition VultureWin = new EndCondition(22, Roles.NeutralRoles.Vulture.RoleColor, "vulture", 1, Module.CustomGameMode.Standard);
    public static EndCondition AvengerWin = new EndCondition(23, Roles.NeutralRoles.Avenger.RoleColor, "avenger", 0, Module.CustomGameMode.Standard);
    public static EndCondition LoversWin = new EndCondition(24, Roles.ExtraRoles.Lover.iconColor[0], "lovers", 0, Module.CustomGameMode.Standard);
    public static EndCondition TrilemmaWin = new EndCondition(25, new Color(209f / 255f, 63f / 255f, 138f / 255f), "trilemma", 0, Module.CustomGameMode.Standard);
    public static EndCondition SantaWin = new EndCondition(26, Roles.NeutralRoles.SantaClaus.RoleColor, "santa", 4, Module.CustomGameMode.Standard);

    public static EndCondition NobodyWin = new EndCondition(48, new Color(72f / 255f, 78f / 255f, 84f / 255f), "nobody", 0, Module.CustomGameMode.ExceptCompete).SetNoBodyWin(true);
    public static EndCondition NobodySkeldWin = new EndCondition(49, new Color(72f / 255f, 78f / 255f, 84f / 255f), "nobody.skeld", 32, Module.CustomGameMode.ActuallyAll).SetNoBodyWin(true);
    public static EndCondition NobodyMiraWin = new EndCondition(50, new Color(72f / 255f, 78f / 255f, 84f / 255f), "nobody.mira", 32, Module.CustomGameMode.ActuallyAll).SetNoBodyWin(true);
    public static EndCondition NobodyPolusWin = new EndCondition(51, new Color(72f / 255f, 78f / 255f, 84f / 255f), "nobody.polus", 32, Module.CustomGameMode.ActuallyAll).SetNoBodyWin(true);
    public static EndCondition NobodyAirshipWin = new EndCondition(52, new Color(72f / 255f, 78f / 255f, 84f / 255f), "nobody.airship", 32, Module.CustomGameMode.ActuallyAll).SetNoBodyWin(true);

    public static EndCondition NoGame = new EndCondition(64, new Color(72f / 255f, 78f / 255f, 84f / 255f), "noGame", 0, Module.CustomGameMode.ActuallyAll).SetNoBodyWin(true);

    public static EndCondition PavlovWin = new EndCondition(128, Roles.NeutralRoles.Pavlov.RoleColor, "pavlov", 1, Module.CustomGameMode.Standard);
    public static EndCondition MoriartyWin = new EndCondition(129, Roles.NeutralRoles.Moriarty.RoleColor, "moriarty", 1, Module.CustomGameMode.Standard);
    public static EndCondition MoriartyWinByKillHolmes = new EndCondition(130, Roles.NeutralRoles.Moriarty.RoleColor, "holmesIsKilled", 1, Module.CustomGameMode.Standard);
    public static EndCondition CascrubinterWin = new EndCondition(131,Roles.NeutralRoles.Cascrubinter.RoleColor,"cascrubinter",1,Module.CustomGameMode.Standard);
    public static EndCondition GuesserWin = new EndCondition(132,Roles.ComplexRoles.FGuesser.RoleColor,"guesser",0,Module.CustomGameMode.Standard);
    public static EndCondition YandereWin = new EndCondition(133,Roles.NeutralRoles.Yandere.RoleColor,"yandere",1,Module.CustomGameMode.Standard);
    public static EndCondition WerewolfWin = new EndCondition(134,Roles.NeutralRoles.Werewolf.RoleColor,"werewolf",1,Module.CustomGameMode.Standard);
    public static EndCondition ChallengerWin = new EndCondition(135,Roles.NeutralRoles.Challenger.RoleColor,"challenger",1,Module.CustomGameMode.Standard);
    public static EndCondition OracleWin = new EndCondition(136,Roles.NeutralRoles.Oracle.RoleColor,"oracle",1,Module.CustomGameMode.Standard);
    public static EndCondition GhostWin = new EndCondition(137,new(1f,1f,1f),"ghost",1,Module.CustomGameMode.Standard);
    public static EndCondition PuppeteerWin = new EndCondition(138,Roles.NeutralRoles.Puppeteer.RoleColor,"puppeteer",1,Module.CustomGameMode.Standard);
    public static EndCondition YellowTeamWin = new EndCondition(139,Roles.BattleRoles.YellowTeam.RoleColor,"yellowTeam",1,Module.CustomGameMode.Battle);
    public static EndCondition GreenTeamWin = new EndCondition(140,Roles.BattleRoles.GreenTeam.RoleColor,"greenTeam",1,Module.CustomGameMode.Battle);
    public static EndCondition InfectedWin = new EndCondition(141,Palette.ImpostorRed,"infected",1,Module.CustomGameMode.VirusCrisis);
    public static EndCondition SurvivalWin = new EndCondition(142,Palette.CrewmateBlue,"survival",1,Module.CustomGameMode.VirusCrisis);
    public static EndCondition HighRollerWin = new EndCondition(143,Roles.NeutralRoles.HighRoller.RoleColor,"highRoller",1,Module.CustomGameMode.Standard);
    public static EndCondition UndeadMasterWin = new EndCondition(144,Roles.NeutralRoles.UndeadMaster.RoleColor,"undeadMaster",1,Module.CustomGameMode.Standard);
    public static EndCondition RedTeamWin = new EndCondition(145, Color.red, "redTeam", 1, Module.CustomGameMode.Compete);
    public static EndCondition BlueTeamWin = new EndCondition(146, Color.blue, "blueTeam", 1, Module.CustomGameMode.Compete);
    public static EndCondition Tie = new EndCondition(147, new Color(72f / 255f, 78f / 255f, 84f / 255f), "tie", 32, Module.CustomGameMode.ActuallyAll).SetNoBodyWin(true);

    public static HashSet<EndCondition> AllEnds = new HashSet<EndCondition>() {
            CrewmateWinByVote ,CrewmateWinByTask,CrewmateWinDisconnect,
            ImpostorWinByKill,ImpostorWinBySabotage,ImpostorWinByVote,ImpostorWinDisconnect,
            CrewmateWinHnS,ImpostorWinHnS,
            JesterWin,JackalWin,ArsonistWin,EmpiricWin,PaparazzoWin,VultureWin,SpectreWin,SantaWin,
            LoversWin,TrilemmaWin,AvengerWin,
            NoGame,NobodyWin,NobodySkeldWin,NobodyMiraWin,NobodyPolusWin,NobodyAirshipWin,
            PavlovWin,MoriartyWin,MoriartyWinByKillHolmes,CascrubinterWin,GuesserWin,YandereWin,WerewolfWin,ChallengerWin,OracleWin,GhostWin,PuppeteerWin,YellowTeamWin,GreenTeamWin,InfectedWin,SurvivalWin,
            HighRollerWin,RedTeamWin,BlueTeamWin,Tie
        };

    public static EndCondition GetEndCondition(GameOverReason gameOverReason)
    {
        foreach (EndCondition condition in AllEnds)
        {
            if (condition.Id == gameOverReason)
            {
                return condition;
            }
        }
        return null;
    }

    public GameOverReason Id { get; }
    public Color Color { get; }
    public String Identifier { get; }
    public EndAction EndAction { get; }
    public byte Priority { get; }
    public Roles.Template.HasWinTrigger TriggerRole { get; set; }
    public bool IsPeaceful;
    public bool IsNoBodyWinEnd;

    public Module.CustomGameMode GameMode { get; set; }
    public EndCondition(GameOverReason Id, Color Color, String EndText, byte Priority, Module.CustomGameMode GameMode, bool IsPeaceful = false, EndAction EndAction = null)
    {
        this.Id = Id;
        this.Color = Color;
        this.Identifier = EndText;
        this.EndAction = EndAction == null ? (FinalPlayerData data) => { } : EndAction;
        this.Priority = Priority;
        this.GameMode = GameMode;
        this.TriggerRole = null;
        this.IsPeaceful = IsPeaceful;
        this.IsNoBodyWinEnd = false;
    }

    public EndCondition(int Id, Color Color, String EndText, byte Priority, Module.CustomGameMode GameMode, bool IsPeaceful = false, EndAction EndAction = null) :
        this((GameOverReason)Id, Color, EndText, Priority, GameMode, IsPeaceful, EndAction)
    {
    }

    public EndCondition SetNoBodyWin(bool nobodyWin)
    {
        IsNoBodyWinEnd = nobodyWin;
        return this;
    }
}

public class FinalPlayerData
{
    public const string chars = "αβγδεζηθικλμνξοπρστυφχψ";
    public class FinalPlayer
    {
        public string name { get; private set; }
        public string roleName { get; private set; }
        public string roleDetail { get; private set; }
        public bool hasFakeTask { get; private set; }
        public bool hasExecutableFakeTask { get; private set; }
        public string killer { get; private set; }
        public byte id { get; private set; }
        public int totalTasks { get; private set; }
        public int completedTasks { get; private set; }
        public Game.PlayerData.PlayerStatus status { get; set; }
        public string additionChar { get; private set; }

        public FinalPlayer(byte id, string name, string roleName, string roleDetail, bool hasFakeTask, bool hasExecutableFakeTask, Game.PlayerData.PlayerStatus status, int totalTasks, int completedTasks, string killer = "")
        {
            this.id = id;
            this.name = name;
            this.roleName = roleName;
            this.roleDetail = roleDetail;
            this.hasFakeTask = hasFakeTask;
            this.hasExecutableFakeTask = hasExecutableFakeTask;
            this.totalTasks = totalTasks;
            this.completedTasks = completedTasks;
            this.status = status;
            this.killer = killer;
            this.additionChar = Helpers.cs(Palette.PlayerColors[id],"" + chars[id % 24]) + "-";
        }

        public void SetKiller(string killer)
        {
            this.killer = killer;
        }
    }

    public List<FinalPlayer> players { get; private set; }

    public FinalPlayer? GetPlayer(byte playerId)
    {
        foreach (var p in players)
        {
            if (p.id == playerId) return p;
        }
        return null;
    }

    public FinalPlayerData()
    {
        players = new List<FinalPlayer>();

        string name, roleName, roleDetail;
        bool hasFakeTask, hasExecutableFakeTask;

        foreach (Game.PlayerData player in Game.GameData.data.AllPlayers.Values)
        {
            //名前に表示を追加する
            name = "";
            if (player.ShouldBeGhostRole)
                roleName = Helpers.cs(player.ghostRole.Color, Language.Language.GetString("role." + player.ghostRole.LocalizeName + ".name"));
            else
                roleName = Helpers.cs(player.role.Color, Language.Language.GetString("role." + player.role.LocalizeName + ".name"));

            hasFakeTask = false;
            hasExecutableFakeTask = false;

            Helpers.RoleAction(player.id, (role) =>
            {
                role.EditDisplayNameForcely(player.id, ref roleName);
                role.EditDisplayRoleNameForcely(player.id, ref roleName);
                hasFakeTask |= !role.HasCrewmateTask(player.id);
                hasExecutableFakeTask |= role.HasExecutableFakeTask(player.id);
            });
            roleDetail = roleName;

            if (name.Equals(""))
                name = player.name;
            else
                name = Helpers.cs(Palette.PlayerColors[player.id],player.name) + " " + name;


            string shortHistory = "";
            string history = "";
            for (int i = 0; i < player.roleHistory.Count - 1; i++)
            {
                history += player.roleHistory[i].Item2 + " → ";
                if (i == 0) shortHistory += player.roleHistory[i].Item2 + " → ";
                if (i == 1) shortHistory += " ... → ";
            }
            roleName = shortHistory + roleName;
            roleDetail = history + roleDetail;


            var finalPlayer = new FinalPlayer(player.id, name,
                roleName, roleDetail, hasFakeTask, hasExecutableFakeTask, player.Status, player.Tasks?.Quota ?? 0, player.Tasks?.Completed ?? 0);
            if (Game.GameData.data.deadPlayers.ContainsKey(player.id))
            {
                byte murder = Game.GameData.data.deadPlayers[player.id].MurderId;
                if (murder != byte.MaxValue) finalPlayer.SetKiller(Game.GameData.data.playersArray[murder].name);
            }
            players.Add(finalPlayer);
        }

        players.Sort((p1, p2) => p1.status.Id - p2.status.Id);
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
public class OnGameEndPatch
{
    public static EndCondition EndCondition;
    public static FinalPlayerData FinalData;

    private static System.Collections.IEnumerator GetEnumerator()
    {
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black, 0.5f, false);

        FinalData = new FinalPlayerData();
        //勝利者を消去する
        TempData.winners.Clear();

        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (Game.GameData.data.playersArray[player.PlayerId].role.CheckWin(player, EndCondition))
            {
                TempData.winners.Add(new WinningPlayerData(player.Data));
            }
        }

        //追加勝利
        bool addedFlag = false;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (TempData.winners.FindAll((Il2CppSystem.Predicate<WinningPlayerData>)((data) => { return data.PlayerName == player.name; })).Count > 0) continue;

            addedFlag = false;
            Helpers.RoleAction(player, (role) =>
            {
                if ((!addedFlag) && role.CheckAdditionalWin(player, EndCondition))
                {
                    TempData.winners.Add(new WinningPlayerData(player.Data));
                    addedFlag = true;
                }
            });
        }

        SceneManager.LoadScene("EndGame");
        yield break;
    }

    public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        EndCondition = EndCondition.GetEndCondition(endGameResult.GameOverReason);
        if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = EndCondition.IsPeaceful ? GameOverReason.HumansByTask : GameOverReason.ImpostorByKill;
    }

    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        EndCondition.EndAction.Invoke(FinalData);

        __instance.StopAllCoroutines();
        __instance.StartCoroutine(GetEnumerator().WrapToIl2Cpp());

        //if(Module.AssetLoader.audioSource.isPlaying) Module.AssetLoader.audioSource.Stop();
    }
}

public static class DetailDialog
{
    static EndGameManager endGameManager;
    static GameObject dialog;
    static TMPro.TMP_Text saveText;
    static TMPro.TMP_Text[] text;
    static PassiveButton button;
    static PassiveButton saveButton;
    static SpriteRenderer renderer;

    static Sprite saveButtonSprite;
    static Sprite getSaveButtonSprite()
    {
        if (!saveButtonSprite) saveButtonSprite = Helpers.loadSpriteFromResources("Nebula.Resources.SavePicButton.png", 100f);
        return saveButtonSprite;
    }

    static public void Initialize(EndGameManager endGameManager, ControllerDisconnectHandler handler, TMPro.TMP_Text textTemplate, string[] detail)
    {
        DetailDialog.endGameManager = endGameManager;

        handler.enabled = false;
        handler.name = "DetailDialog";
        handler.gameObject.SetActive(false);
        handler.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        dialog = handler.gameObject;
        renderer = dialog.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        button = dialog.transform.GetChild(2).gameObject.GetComponent<PassiveButton>();
        saveText = dialog.transform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>();

        renderer.transform.localScale = new Vector3(1.6f, 0.85f, 1.0f);

        button.transform.localPosition = new Vector3(0f, -1.95f, 0f);
        button.transform.GetChild(1).GetComponent<TMPro.TextMeshPro>().text = Language.Language.GetString("game.endScreen.close");
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.OnClick.AddListener((System.Action)Close);

        saveText.transform.localPosition = new Vector3(3.45f, -2.3f, 5f);
        saveText.alignment = TMPro.TextAlignmentOptions.TopLeft;
        saveText.color = Color.white;
        saveText.fontSizeMin = 1.25f;
        saveText.fontSizeMax = 1.25f;
        saveText.fontSize = 1.25f;
        saveText.text = "";

        saveButton = GameObject.Instantiate(button);
        saveButton.transform.SetParent(dialog.transform);
        saveButton.transform.localScale = new Vector3(1f, 1f, 1f);
        saveButton.transform.localPosition = new Vector3(1.4f, -1.95f, 0f);
        saveButton.GetComponent<BoxCollider2D>().size = new Vector2(0.39f, 0.39f);
        SpriteRenderer saveRenderer = saveButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        saveRenderer.sprite = getSaveButtonSprite();
        saveRenderer.size = new Vector2(0.45f, 0.45f);
        saveButton.transform.GetChild(1).gameObject.SetActive(false);
        saveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        saveButton.OnClick.AddListener((System.Action)(() =>
        {
                //四隅の座標を算出
                float xl = text[0].transform.position.x;
            float xu = text[text.Length - 1].transform.position.x + text[text.Length - 1].preferredWidth;
            float yl = text[0].transform.position.y - text[0].preferredHeight;
            float yu = text[0].transform.position.y;

            endGameManager.StartCoroutine(CaptureAndSave(xl, xu, yl, yu).WrapToIl2Cpp());
        }));

        text = new TMPro.TMP_Text[detail.Length];
        float width = 0.0f;
        for (int i = 0; i < detail.Length; i++)
        {
            text[i] = UnityEngine.Object.Instantiate(textTemplate);
            text[i].transform.SetParent(dialog.transform);
            text[i].transform.localScale = new Vector3(1f, 1f, 1f);
            text[i].transform.localPosition = new Vector3(width, 2.1f, 0f);
            text[i].alignment = TMPro.TextAlignmentOptions.TopLeft;
            text[i].color = Color.white;
            text[i].fontSizeMin = 1.5f;
            text[i].fontSizeMax = 1.5f;
            text[i].fontSize = 1.5f;
            text[i].text = detail[i];

            text[i].gameObject.SetActive(true);

            RectTransform rectTransform = text[i].gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0f, 0f);

            width += text[i].preferredWidth - 0.05f;
        }

        //中央に移動させる
        for (int i = 0; i < detail.Length; i++)
        {
            text[i].transform.localPosition -= new Vector3(width / 2.0f, 0f, 0f);
        }

        renderer.gameObject.SetActive(true);
        button.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);
        saveText.gameObject.SetActive(true);
    }

    static public void Open()
    {
        dialog.SetActive(true);
        dialog.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
        endGameManager.StartCoroutine(Effects.Lerp(0.12f, (Il2CppSystem.Action<float>)(
            (p) =>
            {
                dialog.transform.localScale = new Vector3(p, p, 1.0f);
            }
            )));
    }

    static public void Close()
    {
        endGameManager.StartCoroutine(Effects.Lerp(0.12f, (Il2CppSystem.Action<float>)(
            (p) =>
            {
                dialog.transform.localScale = new Vector3(1.0f - p, 1.0f - p, 1.0f);
                if (p == 1f) dialog.SetActive(false);
            }
            )));
    }

    static public IEnumerator CaptureAndSave(float xl, float xu, float yl, float yu)
    {
        Vector2Int convertVector(Vector3 vec)
        {
            return new Vector2Int((int)vec.x, (int)vec.y);
        }
        Vector2Int lower = convertVector(Camera.main.WorldToScreenPoint(new Vector2(xl, yl)));
        Vector2Int upper = convertVector(Camera.main.WorldToScreenPoint(new Vector2(xu, yu)));

        yield return new WaitForEndOfFrame();
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

        lower -= new Vector2Int(20, 10);
        upper += new Vector2Int(40, 20);
        Color[] colors = tex.GetPixels(lower.x, lower.y, upper.x - lower.x, upper.y - lower.y);
        Texture2D saveTex = new Texture2D(upper.x - lower.x, upper.y - lower.y, TextureFormat.ARGB32, false);
        saveTex.SetPixels(colors);
        File.WriteAllBytes(NebulaOption.CreateDirAndGetPictureFilePath(out string displayPath), saveTex.EncodeToPNG());
        saveText.text = Language.Language.GetString("game.endScreen.savedResult").Replace("%PATH%", displayPath);
        saveText.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(false);
    }
}

[HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowProgression))]
public class EndGameNavigationShowProgressionPatch
{
    public static void Postfix(EndGameNavigation __instance)
    {
        if (EndGameManagerSetUpPatch.ModDisplay)
        {
            EndGameManagerSetUpPatch.ModDisplay.SetActive(false);
        }
    }
}

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
public class EndGameManagerSetUpPatch
{
    //public const string chars = "αβγδεζηθικλμνξοπρστυφχψ";

    static HashSet<string> AdditionalTextSet = new HashSet<string>();

    public static void AddEndText(string text) { AdditionalTextSet.Add(text); }

    public static GameObject ModDisplay;

    public static void Postfix(EndGameManager __instance)
    {
        //勝利トリガをもどす
        Roles.Roles.ResetWinTrigger();

        //元の勝利チームを削除する
        foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
        {
            UnityEngine.Object.Destroy(pb.gameObject);
        }

        //勝利メンバーを載せる
        int num = Mathf.CeilToInt(7.5f);
        List<WinningPlayerData> list = TempData.winners.ToArray().ToList().OrderBy(delegate (WinningPlayerData b)
        {
            if (!b.IsYou)
            {
                return 0;
            }
            return -1;
        }).ToList<WinningPlayerData>();

        for (int i = 0; i < list.Count; i++)
        {
            WinningPlayerData winningPlayerData2 = list[i];
            int num2 = (i % 2 == 0) ? -1 : 1;
            int num3 = (i + 1) / 2;
            float num4 = (float)num3 / (float)num;
            float num5 = Mathf.Lerp(1f, 0.75f, num4);
            float num6 = (float)((i == 0) ? -8 : -1);
            PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
            float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            Vector3 vector = new Vector3(num7, num7, 1f);
            poolablePlayer.transform.localScale = vector;
            poolablePlayer.UpdateFromPlayerOutfit(winningPlayerData2, PlayerMaterial.MaskType.None, winningPlayerData2.IsDead, true);
            if (winningPlayerData2.IsDead)
            {
                poolablePlayer.SetBodyAsGhost();
                poolablePlayer.SetDeadFlipX(i % 2 == 0);
            }
            else
            {
                poolablePlayer.SetFlipX(i % 2 == 0);
            }

            poolablePlayer.SetName(winningPlayerData2.PlayerName, new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z), Color.white, -15f);
            poolablePlayer.SetNamePosition(new Vector3(0f, -1.31f, -0.5f));
        }

        ModDisplay = new GameObject("ModDisplay");

        // テキストを追加する
        GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        bonusText.transform.SetParent(ModDisplay.transform);
        bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
        bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
        textRenderer.text = Language.Language.GetString("game.endText." + OnGameEndPatch.EndCondition.Identifier);
        foreach (string text in AdditionalTextSet)
        {
            textRenderer.text += text;
        }
        textRenderer.color = OnGameEndPatch.EndCondition.Color;
        AdditionalTextSet.Clear();

        __instance.BackgroundBar.material.SetColor("_Color", OnGameEndPatch.EndCondition.Color);

        var position = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));

        TMPro.TMP_Text[] roleSummaryText = new TMPro.TMP_Text[5];
        for (int i = 0; i < roleSummaryText.Length; i++)
        {
            GameObject obj = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            obj.transform.SetParent(ModDisplay.transform);

            RectTransform roleSummaryTextMeshRectTransform = obj.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.pivot = new Vector2(0f, 1f);
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector3(position.x, position.y - 0.1f, -14f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);

            roleSummaryText[i] = obj.GetComponent<TMPro.TMP_Text>();
            roleSummaryText[i].alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryText[i].color = Color.white;
            roleSummaryText[i].fontSizeMin = 1.25f;
            roleSummaryText[i].fontSizeMax = 1.25f;
            roleSummaryText[i].fontSize = 1.25f;
        }


        //結果表示
        var playerText = new StringBuilder();
        var roleText = new StringBuilder();
        var roleDetailText = new StringBuilder();
        var statusText = new StringBuilder();
        var murdererText = new StringBuilder();
        var taskText = new StringBuilder();

        foreach (FinalPlayerData.FinalPlayer player in OnGameEndPatch.FinalData.players)
        {
            playerText.AppendLine("　" + Helpers.cs(Palette.PlayerColors[player.id],player.name));
            roleText.AppendLine("　" + player.additionChar + player.roleName);
            roleDetailText.AppendLine("　" + player.additionChar + player.roleDetail);
            statusText.AppendLine("　" + Language.Language.GetString("status." + player.status.Status));
            murdererText.AppendLine("　" + (player.killer != "" ? $"<color=#FF5555FF>by " + player.killer + "</color>" : ""));

            if (player.hasFakeTask)
                taskText.AppendLine("　" + (player.totalTasks > 0 ? $"<color=#868686FF>({player.completedTasks}/{player.totalTasks})</color>" : ""));
            else
                taskText.AppendLine("　" + (player.totalTasks > 0 ? $"<color=#FAD934FF>({player.completedTasks}/{player.totalTasks})</color>" : ""));
        }

        roleSummaryText[0].text = playerText.ToString();
        roleSummaryText[1].text = taskText.ToString();
        roleSummaryText[2].text = roleText.ToString();
        roleSummaryText[3].text = statusText.ToString();
        roleSummaryText[4].text = murdererText.ToString();

        float width = 0.0f;
        for (int i = 0; i < 5; i++)
        {
            roleSummaryText[i].transform.position += new Vector3(width, 0f, 0f);
            width += roleSummaryText[i].preferredWidth - 0.05f;
        }

        //ダイアログ呼び出しボタン
        var detailButton = GameObject.Instantiate(__instance.Navigation.ContinueButton.transform.GetChild(0));
        detailButton.transform.SetParent(ModDisplay.transform);
        detailButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        detailButton.localPosition = roleSummaryText[0].transform.localPosition + new Vector3(1.0f, -roleSummaryText[0].preferredHeight - 0.5f);
        PassiveButton detailPassiveButton = detailButton.GetComponent<PassiveButton>();
        detailPassiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        detailPassiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => DetailDialog.Open()));
        TMPro.TMP_Text detailButtonText = detailButton.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>();
        detailButtonText.text = Language.Language.GetString("game.endScreen.detail");
        detailButtonText.gameObject.GetComponent<TextTranslatorTMP>().enabled = false;


        //ダイアログを作成
        var detailDialog = GameObject.Instantiate(GameObject.FindObjectOfType<ControllerDisconnectHandler>(), null);
        DetailDialog.Initialize(__instance, detailDialog, __instance.WinText, new string[] {
                roleSummaryText[0].text,
                roleSummaryText[1].text,
                roleSummaryText[3].text,
                roleSummaryText[4].text,
                roleDetailText.ToString()
            });

        //ゲーム終了後の初期化
        RPCEvents.ResetVaribles();
    }
}


class CheckEndCriteriaPatch
{
    public static void CommonPrefix()
    {
        if (!AmongUsClient.Instance.AmHost) return;

        if (!GameManager.Instance) return;
        if (!GameManager.Instance.ShouldCheckForGameEnd) return;

            if (ExileController.Instance != null)
        {
            if (SpawnInMinigame.Instance == null)
                return;// return false;
        }

        if (!GameData.Instance) return;// return false;

        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

        var statistics = new PlayerStatistics(ShipStatus.Instance);
        if (!statistics.IsValid) return;

        Patches.EndCondition endCondition = null, temp;
        byte priority = Byte.MaxValue;
        
        foreach (Roles.Side side in Roles.Side.AllSides)
        {

            temp = side.endCriteriaChecker(statistics, ShipStatus.Instance);
            if (temp != null && priority >= temp.Priority)
            {
                if ((temp.GameMode & Game.GameData.data.GameMode) == 0) continue;

                endCondition = temp;
                priority = temp.Priority;
            }
        }


        if (endCondition != null)
        {
            //勝利乗っ取り
            foreach (Roles.Side side in Roles.Side.AllSides)
            {
                temp = side.endTakeoverChecker(endCondition, statistics, ShipStatus.Instance);
                if (temp != null) endCondition = temp;
            }

            ShipStatus.Instance.enabled = false;
            GameManager.Instance.RpcEndGame(endCondition.Id, false);
            //return false;
            return;
        }

        return;// return false;
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.FixedUpdate))]
class CheckNormalEndCriteriaPatch
{
    static bool lastShouldCheckedFlag;

    public static void Prefix(GameManager __instance)
    {
        if(GameData.Instance)TasksHandler.RecomputeTasks(GameData.Instance);
        CheckEndCriteriaPatch.CommonPrefix();

        lastShouldCheckedFlag = __instance.ShouldCheckForGameEnd;
        __instance.ShouldCheckForGameEnd = false;
    }

    public static void Postfix(GameManager __instance)
    {
        __instance.ShouldCheckForGameEnd = lastShouldCheckedFlag;
    }
}

public class PlayerStatistics
{
    private Dictionary<Roles.Side, int> alivePlayers;
    //頻繁に使用する値のみ予め抽出
    public int AliveJackals;
    public int AliveImpostors;
    public int AliveCrewmates;

    public int TotalAlive { get; private set; }
    public int AliveCouple;
    public int AliveJackalCouple;
    public int AliveImpostorCouple;
    public int AliveTrilemma;
    public int AliveJackalTrilemma;
    public int AliveImpostorTrilemma;
    public int AliveImpostorsWithSidekick;
    public int AliveSpectre;

    //設定次第で適切な値が入る(独立した陣営として見ない場合常に0)
    public int AliveInLoveImpostors;
    public int AliveInLoveJackals;

    public int AlivePavlov;
    public int AlivePavlovCouple;
    public int AlivePavlovTrilemma;
    public int AliveInLovePavlov;

    public int AliveMoriarty;
    public int AliveMoriartyCouple;
    public int AliveMoriartyTrilemma;
    public int AliveInLoveMoriarty;

    public int AliveYandere;
    public int AliveMadmate;

    public int AliveWerewolf;
    public int AliveWerewolfCouple;
    public int AliveWerewolfTrilemma;
    public int AliveInLoveWerewolf;

    public int AlivePavlovWithSidekick;
    public int AliveMoriartyWithSidekick;
    public int AliveWerewolfWithSidekick;
    public int AliveSecondaryJackal;
    public int AliveSpectreWithSidekick;
    public int AliveInLoveImpostorsWithSidekick;
    public int AliveInLovePavlovWithSidekick;
    public int AliveInLoveMoriartyWithSidekick;
    public int AliveInLoveWerewolfWithSidekick;
    public int AliveJackalsWithMadmate;
    public int AlivePavlovWithMadmate;
    public int AliveMoriartyWithMadmate;
    public int AliveWerewolfWithMadmate;
    public int AliveInLoveJackalsWithMadmate;
    public int AliveInLovePavlovWithMadmate;
    public int AliveInLoveMoriartyWithMadmate;
    public int AliveInLoveWerewolfWithMadmate;

    public int AliveChallenger;

    public int AliveOracle;
    public int AliveInLoveOracle;
    public int AliveOracleCouple;
    public int AliveOracleTrilemma;
    public int AliveOracleWithMadmate;
    public int AliveInLoveOracleWithMadmate;
    public int AliveOracleWithSidekick;
    public int AliveInLoveOracleWithSidekick;

    public int AlivePuppeteer;

    public int AliveZombieSidekick;

    public int AliveYellowTeam;
    public int AliveGreenTeam;

    public int AliveInfected;
    public int AliveSurvivals;

    public int AliveUndeadMaster;

    public int AliveRedTeam;
    public int AliveBlueTeam;

    public bool IsValid;

    //
    public int GetAlivePlayers(Roles.Side side)
    {
        if (alivePlayers.ContainsKey(side))
        {
            return alivePlayers[side];
        }
        return 0;
    }

    public PlayerStatistics(ShipStatus __instance)
    {
        IsValid = false;

        alivePlayers = new Dictionary<Roles.Side, int>();
        TotalAlive = 0;

        AliveCouple = 0;
        AliveJackalCouple = 0;
        AliveImpostorCouple = 0;
        AliveTrilemma = 0;
        AliveJackalTrilemma = 0;
        AliveImpostorTrilemma = 0;
        AliveInLoveJackals = 0;
        AliveInLoveImpostors = 0;
        AliveImpostorsWithSidekick = 0;
        AlivePavlovCouple = 0;
        AlivePavlovTrilemma = 0;
        AliveInLovePavlov = 0;
        AliveMoriarty = 0;
        AliveMoriartyCouple = 0;
        AliveMoriartyTrilemma = 0;
        AliveInLoveMoriarty = 0;
        AliveSpectre = 0;
        AliveMadmate = 0;
        AliveWerewolf = 0;
        AliveWerewolfCouple = 0;
        AliveWerewolfTrilemma = 0;
        AliveInLoveWerewolf = 0;
        AliveSecondaryJackal = 0;
        AliveSpectreWithSidekick = 0;
        AliveInLoveImpostorsWithSidekick = 0;
        AliveInLoveMoriartyWithSidekick = 0;
        AliveInLovePavlovWithSidekick = 0;
        AliveInLoveWerewolfWithSidekick = 0;
        AliveJackalsWithMadmate = 0;
        AlivePavlovWithMadmate = 0;
        AliveMoriartyWithMadmate = 0;
        AliveWerewolfWithMadmate = 0;
        AliveInLoveJackalsWithMadmate = 0;
        AliveInLovePavlovWithMadmate = 0;
        AliveInLoveMoriartyWithMadmate = 0;
        AliveInLoveWerewolfWithMadmate = 0;
        AliveChallenger = 0;

        AliveOracle = 0;
        AliveInLoveOracle = 0;
        AliveOracleCouple = 0;
        AliveOracleTrilemma = 0;
        AliveOracleWithMadmate = 0;
        AliveInLoveOracleWithMadmate = 0;
        AliveOracleWithSidekick = 0;
        AliveInLoveOracleWithSidekick = 0;

        AliveZombieSidekick = 0;
        AlivePuppeteer = 0;
        
        AliveYellowTeam = 0;
        AliveGreenTeam = 0;

        AliveInfected = 0;
        AliveSurvivals = 0;

        AliveUndeadMaster = 0;

        AliveRedTeam = 0;
        AliveBlueTeam = 0;

        Roles.Side side;
        

        foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            try
            {
                if (playerInfo.Disconnected)
                {
                    IsValid = true;
                    continue;
                }
                if (playerInfo.IsDead && !(playerInfo.GetModData().role == Roles.Roles.EvilBusker && Roles.Roles.EvilBusker.pseudocideFlag) && !(playerInfo.GetModData().role == Roles.Roles.Busker && Roles.Roles.Busker.pseudocideFlag))
                {
                    IsValid = true;
                    continue;
                }

                TotalAlive++;

                var data = Game.GameData.data.playersArray[playerInfo.PlayerId];

                if(data.role == Roles.Roles.Madmate || data.HasExtraRole(Roles.Roles.SecondaryMadmate)){
                    AliveMadmate++;
                }

                side = data.role.side;

                if (!alivePlayers.ContainsKey(side))
                {
                    alivePlayers.Add(side, 1);
                }
                else
                {
                    alivePlayers[side] = alivePlayers[side] + 1;
                }

                if (data.HasExtraRole(Roles.Roles.Lover))
                {
                    var lData = Roles.Roles.Lover.GetLoversData(data);
                    if (lData != data && lData.id > data.id && lData.IsAlive)
                    {
                        AliveCouple++;

                        bool flag = false;
                        if (data.role.side == Roles.Side.Jackal || lData.HasExtraRole(Roles.Roles.SecondarySidekick) || lData.HasExtraRole(Roles.Roles.SecondaryJackal))
                        {
                            AliveInLoveJackals++;
                            AliveJackalCouple++;
                            flag = true;
                        }
                        if (lData.role.side == Roles.Side.Jackal || lData.HasExtraRole(Roles.Roles.SecondarySidekick) || lData.HasExtraRole(Roles.Roles.SecondaryJackal))
                        {
                            AliveInLoveJackals++;
                            if (!flag) AliveJackalCouple++;
                        }

                        flag = false;
                        if (data.role.category == Roles.RoleCategory.Impostor)
                        {
                            AliveInLoveImpostors++;
                            AliveImpostorCouple++;
                            flag = true;
                        }

                        if (lData.role.category == Roles.RoleCategory.Impostor)
                        {
                            AliveInLoveImpostors++;
                            if (!flag) AliveImpostorCouple++;
                        }

                        flag = false;
                        if (data.role.side == Roles.Side.Pavlov)
                        {
                            AliveInLovePavlov++;
                            AlivePavlovCouple++;
                            flag = true;
                        }

                        if (data.role.side == Roles.Side.Pavlov)
                        {
                            AliveInLovePavlov++;
                            if (!flag) AlivePavlovCouple++;
                        }

                        flag = false;
                        if (data.role.side == Roles.Side.Moriarty)
                        {
                            AliveInLoveMoriarty++;
                            AliveMoriartyCouple++;
                            flag = true;
                        }

                        if (data.role.side == Roles.Side.Moriarty)
                        {
                            AliveInLoveMoriarty++;
                            if (!flag) AliveMoriartyCouple++;
                        }

                        flag = false;
                        if (data.role.side == Roles.Side.Werewolf)
                        {
                            AliveInLoveWerewolf++;
                            AliveWerewolfCouple++;
                            flag = true;
                        }

                        if (data.role.side == Roles.Side.Werewolf)
                        {
                            AliveInLoveWerewolf++;
                            if (!flag) AliveWerewolfCouple++;
                        }

                        flag = false;
                        if (data.role.side == Roles.Side.Oracle)
                        {
                            AliveInLoveOracle++;
                            AliveOracleCouple++;
                            flag = true;
                        }

                        if (data.role.side == Roles.Side.Oracle)
                        {
                            AliveInLoveOracle++;
                            if (!flag) AliveOracleCouple++;
                        }
                    }
                }

                if (data.HasExtraRole(Roles.Roles.Trilemma))
                {
                    var lData = Roles.Roles.Trilemma.GetLoversData(data);
                    if (lData[2] == data && lData[0].IsAlive && lData[1].IsAlive)
                    {
                        AliveTrilemma++;

                        bool jackalFlag = false, impostorFlag = false, pavlovFlag = false, moriartyFlag = false,werewolfFlag = false,oracleFlag = false;

                        foreach (var d in lData)
                        {
                            if ((d.role.side == Roles.Side.Jackal || d.HasExtraRole(Roles.Roles.SecondarySidekick) || d.HasExtraRole(Roles.Roles.SecondaryJackal)))
                            {
                                jackalFlag = true;
                                AliveInLoveJackals++;
                            }
                            if ((d.role.category == Roles.RoleCategory.Impostor))
                            {
                                impostorFlag = true;
                                AliveInLoveImpostors++;
                            }
                            if ((d.role.side == Roles.Side.Pavlov))
                            {
                                pavlovFlag = true;
                                AliveInLovePavlov++;
                            }
                            if((d.role.side == Roles.Side.Moriarty)){
                                moriartyFlag = true;
                                AliveInLoveMoriarty++;
                            }
                            if((d.role.side == Roles.Side.Werewolf)){
                                werewolfFlag = true;
                                AliveInLoveWerewolf++;
                            }
                            if((d.role.side == Roles.Side.Oracle)){
                                oracleFlag = true;
                                AliveInLoveOracle++;
                            }
                        }
                        if (jackalFlag) AliveJackalTrilemma++;
                        if (impostorFlag) AliveImpostorTrilemma++;
                        if (pavlovFlag) AlivePavlovTrilemma++;
                        if (moriartyFlag) AliveMoriartyTrilemma++;
                        if(werewolfFlag) AliveWerewolfTrilemma++;
                        if(oracleFlag) AliveOracleTrilemma++;
                    }
                }

                if (side == Roles.Side.Impostor)
                {
                    if (data.HasExtraRole(Roles.Roles.SecondarySidekick) || data.HasExtraRole(Roles.Roles.SecondaryJackal))
                    {
                        AliveImpostorsWithSidekick++;
                        AliveSecondaryJackal++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveImpostorsWithSidekick++;
                    }
                }else if (side == Roles.Side.Jackal)
                {
                    if(data.HasExtraRole(Roles.Roles.SecondaryMadmate)){
                        AliveJackalsWithMadmate++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveJackalsWithMadmate++;
                    }
                }else if (side == Roles.Side.Pavlov)
                {
                    if (data.HasExtraRole(Roles.Roles.SecondarySidekick) || data.HasExtraRole(Roles.Roles.SecondaryJackal))
                    {
                        AlivePavlovWithSidekick++;
                        AliveSecondaryJackal++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLovePavlovWithSidekick++;
                    }
                    if(data.HasExtraRole(Roles.Roles.SecondaryMadmate)){
                        AlivePavlovWithMadmate++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLovePavlovWithMadmate++;
                    }
                }else if (side == Roles.Side.Moriarty)
                {
                    if (data.HasExtraRole(Roles.Roles.SecondarySidekick) || data.HasExtraRole(Roles.Roles.SecondaryJackal))
                    {
                        AliveMoriartyWithSidekick++;
                        AliveSecondaryJackal++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveMoriartyWithSidekick++;
                    }
                    if(data.HasExtraRole(Roles.Roles.SecondaryMadmate)){
                        AliveMoriartyWithMadmate++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveMoriartyWithMadmate++;
                    }
                }else if (side == Roles.Side.Werewolf)
                {
                    if (data.HasExtraRole(Roles.Roles.SecondarySidekick) || data.HasExtraRole(Roles.Roles.SecondaryJackal))
                    {
                        AliveWerewolfWithSidekick++;
                        AliveSecondaryJackal++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveWerewolfWithSidekick++;
                    }
                    if(data.HasExtraRole(Roles.Roles.SecondaryMadmate)){
                        AliveWerewolfWithMadmate++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveWerewolfWithMadmate++;
                    }
                }else if (side == Roles.Side.Oracle)
                {
                    if (data.HasExtraRole(Roles.Roles.SecondarySidekick) || data.HasExtraRole(Roles.Roles.SecondaryJackal))
                    {
                        AliveOracleWithSidekick++;
                        AliveSecondaryJackal++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveOracleWithSidekick++;
                    }
                    if(data.HasExtraRole(Roles.Roles.SecondaryMadmate)){
                        AliveOracleWithMadmate++;
                        if(data.HasExtraRole(Roles.Roles.Lover) || data.HasExtraRole(Roles.Roles.Trilemma)) AliveInLoveOracleWithMadmate++;
                    }
                }

                if (data.role == Roles.Roles.Spectre){
                    if(data.extraRole.Contains(Roles.Roles.SecondaryJackal) || data.extraRole.Contains(Roles.Roles.SecondarySidekick)){
                        AliveSpectreWithSidekick++;
                        AliveSecondaryJackal++;
                    }else AliveSpectre++;
                }

                if(data.role == Roles.Roles.ZombieSidekick) AliveZombieSidekick++;

                if(data.role == Roles.Roles.Puppeteer) AlivePuppeteer++;

                if(data.role == Roles.Roles.Infected) AliveInfected++;

                IsValid = true;
            }
            catch(Exception e) 
            {
                Debug.LogError(e.StackTrace);
                continue;
            }
        }

        AliveCrewmates = GetAlivePlayers(Roles.Side.Crewmate);
        AliveImpostors = GetAlivePlayers(Roles.Side.Impostor);
        AliveJackals = GetAlivePlayers(Roles.Side.Jackal) + AliveSecondaryJackal;
        AlivePavlov = GetAlivePlayers(Roles.Side.Pavlov);
        AliveMoriarty = GetAlivePlayers(Roles.Side.Moriarty);
        AliveYandere = GetAlivePlayers(Roles.Side.Yandere);
        AliveWerewolf = GetAlivePlayers(Roles.Side.Werewolf);
        AliveChallenger = GetAlivePlayers(Roles.Side.Challenger);
        AliveOracle = GetAlivePlayers(Roles.Side.Oracle);
        AliveYellowTeam = GetAlivePlayers(Roles.Side.YellowTeam);
        AliveGreenTeam = GetAlivePlayers(Roles.Side.GreenTeam);
        //AliveInfected = GetAlivePlayers(Roles.Side.Infected);
        AliveSurvivals = GetAlivePlayers(Roles.Side.Survival);
        AliveUndeadMaster = GetAlivePlayers(Roles.Side.UndeadMaster);
        AliveRedTeam = GetAlivePlayers(Roles.Side.RedTeam);
        AliveBlueTeam = GetAlivePlayers(Roles.Side.BlueTeam);

        if (!Roles.Roles.Lover.loversAsIndependentSideOption.getBool())
        {
            AliveInLoveImpostors = 0;
            AliveInLoveJackals = 0;
            AliveInLovePavlov = 0;
            AliveInLoveMoriarty = 0;
            AliveInLoveWerewolf = 0;
            AliveInLoveOracle = 0;
        }
        if(!Roles.Roles.Madmate.IgnoringNumOfMadmateOption.getBool()) AliveMadmate = 0;
    }
}
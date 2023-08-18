namespace Nebula.Roles.NeutralRoles;

public class Puppeteer : Role,Template.HasWinTrigger
{
    private List<Objects.Arrow?> impostorArrows;
    private List<Objects.Arrow?> jackalArrows;
    private List<Objects.Arrow?> pavlovArrows;
    private List<Objects.Arrow?> moriartyArrows;
    private List<Objects.Arrow?> sheriffArrows;
    private List<Objects.Arrow?> werewolfArrows;
    private List<Objects.Arrow?> challengerArrows;
    private List<Objects.Arrow?> oracleArrows;
    private List<Objects.Arrow?> santaArrows;

    public bool WinTrigger { get; set; }
    public byte Winner { get; set; }

    public static Color RoleColor = new(114f / 255f,51f / 255f,114f / 255f);

    private CustomButton morphButton;
    private bool isDead = false;
    private int killCount = 0;
    private bool hasKilled;

    private Module.CustomOption morphCoolDownOption;
    private Module.CustomOption morphDurationOption;
    private Module.CustomOption countToWin;
    //private Module.CustomOption canUseVent;

    private PlayerControl? morphTarget;
    private Game.PlayerData.PlayerOutfitData morphOutfit;
    private Objects.Arrow? arrow;
    private Vector3 originPos;

    private SpriteLoader sampleButtonSprite = new SpriteLoader("Nebula.Resources.SampleButton.png", 115f, "ui.button.morphing.sample");
    private SpriteLoader morphButtonSprite = new SpriteLoader("Nebula.Resources.MorphButton.png", 115f, "ui.button.morphing.morph");

    public override HelpSprite[] helpSprite => new HelpSprite[]
    {
            new HelpSprite(sampleButtonSprite,"role.morphing.help.sample",0.3f),
            new HelpSprite(morphButtonSprite,"role.morphing.help.morph",0.3f)
    };

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;

        morphCoolDownOption = CreateOption(Color.white, "morphCoolDown", 25f, 10f, 60f, 5f);
        morphCoolDownOption.suffix = "second";

        morphDurationOption = CreateOption(Color.white, "morphDuration", 15f, 5f, 40f, 2.5f);
        morphDurationOption.suffix = "second";

        countToWin = CreateOption(Color.white, "countToWin", 4f, 1f, 10f, 1f);
    }

    public override void Initialize(PlayerControl __instance){
        killCount = 0;
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        morphTarget = null;

        if (morphButton != null)
        {
            morphButton.Destroy();
        }
        morphButton = new CustomButton(
            () =>
            {
                if (morphTarget == null)
                {
                    morphButton.Timer = 3f;
                    morphButton.isEffectActive = false;
                    morphTarget = Game.GameData.data.myData.currentTarget;
                    Game.GameData.data.myData.currentTarget = null;
                    morphButton.Sprite = morphButtonSprite.GetSprite();
                    morphButton.SetLabel("button.label.morph");
                    morphOutfit = morphTarget.GetModData().GetOutfitData(50).Clone(80);
                }
                else
                {
                    //Game.GameData.data.myData.Vision.Register(new Game.VisionFactor(morphDurationOption.getFloat(), 1.5f));
                    originPos = PlayerControl.LocalPlayer.transform.position;
                    RPCEventInvoker.FixedRevive(PlayerControl.LocalPlayer);
                    RPCEventInvoker.Morph(morphOutfit.Clone(morphOutfit.Priority),morphDurationOption.getFloat());
                }
            },
            () => { return true; },//!PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && (morphTarget != null || Game.GameData.data.myData.currentTarget != null); },
            () =>
            {
                morphButton.Timer = morphButton.MaxTimer;
                morphButton.isEffectActive = false;
                morphButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                RPCEventInvoker.MorphCancel();
                if(isDead){
                    PlayerControl.LocalPlayer.Die(DeathReason.Kill,false);
                    Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId]?.Die();
                }
                //Game.GameData.data.myData.Vision.Factors.Clear();
            },
            sampleButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            morphDurationOption.getFloat(),
            () => { 
                morphButton.Timer = morphButton.MaxTimer;
                RPCEventInvoker.MorphCancel();
                if(isDead){
                    PlayerControl.LocalPlayer.Die(DeathReason.Kill,false);
                    Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId]?.Die();
                }
                PlayerControl.LocalPlayer.transform.position = originPos;
                //morphTarget = null;
            },
            "button.label.sample"
        ).SetTimer(CustomOptionHolder.InitialModestAbilityCoolDownOption.getFloat());
        morphButton.MaxTimer = morphCoolDownOption.getFloat();
        morphButton.EffectDuration = morphDurationOption.getFloat();
        morphButton.SetSuspendAction(() =>
        {
            morphButton.Timer = morphButton.MaxTimer;
            morphButton.isEffectActive = false;
            morphButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            RPCEventInvoker.MorphCancel();
            if(isDead){
                PlayerControl.LocalPlayer.Die(DeathReason.Kill,false);
                Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId]?.Die();
            }
            PlayerControl.LocalPlayer.transform.position = originPos;
            //morphTarget = null;
        });
    }

    public override void onRevived(byte playerId){
        if(PlayerControl.LocalPlayer.PlayerId == playerId) isDead = false;
    }

    public override void OnMurdered(byte murderId){
        if(!morphButton.isEffectActive) isDead = true;
        else{
            morphButton.Timer = morphButton.MaxTimer;
            morphButton.isEffectActive = false;
            morphButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            RPCEventInvoker.MorphCancel();
            if(!morphTarget.Data.IsDead) RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,morphTarget.PlayerId,Game.PlayerData.PlayerStatus.Dead.Id,false);
            hasKilled = true;
            if(!isDead) RPCEventInvoker.FixedRevive(PlayerControl.LocalPlayer);
            else{
                try{
                    DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                    foreach(var DeadBody in array){
                        if(DeadBody.ParentId == PlayerControl.LocalPlayer.PlayerId){
                            DeadBody.gameObject.active = false;
                        }
                    }
                }catch(Exception e) { Debug.LogError(e.StackTrace); }
            }
            /*
            if(isDead){
                PlayerControl.LocalPlayer.Die(DeathReason.Kill,false);
                Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId]?.Die();
            }
            */
            PlayerControl.LocalPlayer.transform.position = originPos;
            if(++killCount >= countToWin.getFloat()) RPCEventInvoker.WinTrigger(this);
        }
    }

    SpriteLoader arrowSprite = new SpriteLoader("role.morphing.arrow");
    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(1f);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);

        RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, morphTarget, RoleColor, arrowSprite);

        int i = 0,i1 = 0,i2 = 0,i3 = 0,i4 = 0,i5 = 0,i6 = 0,i7 = 0,i8 = 0;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if ((p.Data.Role.IsImpostor || p.GetModData().role.DeceiveImpostorInNameDisplay) && !p.Data.IsDead)
            {
                if (impostorArrows.Count >= i) impostorArrows.Add(null);

                var arrow = impostorArrows[i];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Palette.ImpostorRed,arrowSprite);
                impostorArrows[i] = arrow;

                i++;
            }
            else if (p.GetModData().role == Roles.Jackal && !p.Data.IsDead){
                if (jackalArrows.Count >= i1) jackalArrows.Add(null);

                var arrow = jackalArrows[i1];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.Jackal.Color,arrowSprite);
                jackalArrows[i1] = arrow;

                i1++;
            }
            else if ((p.GetModData().role == Roles.Dog) && !p.Data.IsDead){
                if (pavlovArrows.Count >= i2) pavlovArrows.Add(null);

                var arrow = pavlovArrows[i2];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.Pavlov.Color,arrowSprite);
                pavlovArrows[i2] = arrow;

                i2++;
            }
            else if ((p.GetModData().role.side == Side.Moriarty) && !p.Data.IsDead){
                if (moriartyArrows.Count >= i3) moriartyArrows.Add(null);

                var arrow = moriartyArrows[i3];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.Moriarty.Color,arrowSprite);
                moriartyArrows[i3] = arrow;

                i3++;
            }
            else if ((p.GetModData().role == Roles.Sheriff) && !p.Data.IsDead){
                if (sheriffArrows.Count >= i4) sheriffArrows.Add(null);

                var arrow = sheriffArrows[i4];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.Sheriff.Color,arrowSprite);
                sheriffArrows[i4] = arrow;

                i4++;
            }
            else if ((p.GetModData().role == Roles.Werewolf) && !p.Data.IsDead){
                if (werewolfArrows.Count >= i5) werewolfArrows.Add(null);

                var arrow = werewolfArrows[i5];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.Werewolf.Color,arrowSprite);
                werewolfArrows[i5] = arrow;

                i5++;
            }
            else if ((p.GetModData().role == Roles.Challenger) && !p.Data.IsDead){
                if (challengerArrows.Count >= i6) challengerArrows.Add(null);

                var arrow = challengerArrows[i6];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.Challenger.Color,arrowSprite);
                challengerArrows[i6] = arrow;

                i6++;
            }
            else if ((p.GetModData().role == Roles.OracleN) && !p.Data.IsDead){
                if (oracleArrows.Count >= i7) oracleArrows.Add(null);

                var arrow = oracleArrows[i7];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.OracleN.Color,arrowSprite);
                oracleArrows[i7] = arrow;

                i7++;
            }
            else if ((p.GetModData().role == Roles.BlackSanta) && !p.Data.IsDead){
                if (santaArrows.Count >= i8) santaArrows.Add(null);

                var arrow = santaArrows[i8];
                RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, p, Roles.BlackSanta.Color,arrowSprite);
                santaArrows[i8] = arrow;

                i8++;
            }
        }
        int removed = impostorArrows.Count - i;
        for (; i < impostorArrows.Count; i++) if (impostorArrows[i] != null) GameObject.Destroy(impostorArrows[i].arrow);
        impostorArrows.RemoveRange(impostorArrows.Count - removed, removed);
        removed = jackalArrows.Count - i1;
        for (; i1 < jackalArrows.Count; i1++) if (jackalArrows[i1] != null) GameObject.Destroy(jackalArrows[i1].arrow);
        jackalArrows.RemoveRange(jackalArrows.Count - removed, removed);
        removed = pavlovArrows.Count - i2;
        for (; i2 < pavlovArrows.Count; i2++) if (pavlovArrows[i2] != null) GameObject.Destroy(pavlovArrows[i2].arrow);
        pavlovArrows.RemoveRange(pavlovArrows.Count - removed, removed);
        removed = moriartyArrows.Count - i3;
        for (; i3 < moriartyArrows.Count; i3++) if (moriartyArrows[i3] != null) GameObject.Destroy(moriartyArrows[i3].arrow);
        moriartyArrows.RemoveRange(moriartyArrows.Count - removed, removed);
        removed = sheriffArrows.Count - i4;
        for (; i4 < sheriffArrows.Count; i4++) if (sheriffArrows[i4] != null) GameObject.Destroy(sheriffArrows[i4].arrow);
        sheriffArrows.RemoveRange(sheriffArrows.Count - removed, removed);
        removed = werewolfArrows.Count - i5;
        for (; i5 < werewolfArrows.Count; i5++) if (werewolfArrows[i5] != null) GameObject.Destroy(werewolfArrows[i5].arrow);
        werewolfArrows.RemoveRange(werewolfArrows.Count - removed, removed);
        removed = challengerArrows.Count - i6;
        for (; i6 < challengerArrows.Count; i6++) if (challengerArrows[i6] != null) GameObject.Destroy(challengerArrows[i6].arrow);
        challengerArrows.RemoveRange(challengerArrows.Count - removed, removed);
        removed = oracleArrows.Count - i7;
        for (; i7 < oracleArrows.Count; i7++) if (oracleArrows[i7] != null) GameObject.Destroy(oracleArrows[i7].arrow);
        oracleArrows.RemoveRange(oracleArrows.Count - removed, removed);
        removed = santaArrows.Count - i8;
        for (; i8 < santaArrows.Count; i8++) if (santaArrows[i8] != null) GameObject.Destroy(santaArrows[i8].arrow);
        santaArrows.RemoveRange(santaArrows.Count - removed, removed);
    }

    public override void OnMeetingStart(){
        if(isDead){
            PlayerControl.LocalPlayer.Die(DeathReason.Kill,false);
            Game.GameData.data.playersArray[PlayerControl.LocalPlayer.PlayerId]?.Die();
        }
        if(hasKilled){
            Objects.SoundPlayer.PlaySound(Module.AudioAsset.PuppeteerLaugh);
            hasKilled = false;
        }
    }

    public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
    {
        base.EditOthersDisplayNameColor(playerId, ref displayColor);
        PlayerControl player = Helpers.playerById(playerId);
        if (player.GetModData().role == Roles.Jackal)
        {
            displayColor = Jackal.RoleColor;
        }
        else if (player.GetModData().role == Roles.Dog)
        {
            displayColor = Pavlov.RoleColor;
        }
        else if (player.GetModData().role.side == Side.Moriarty)
        {
            displayColor = Moriarty.RoleColor;
        }
        else if (player.GetModData().role == Roles.Sheriff)
        {
            displayColor = CrewmateRoles.Sheriff.RoleColor;
        }
        else if (player.GetModData().role == Roles.Werewolf)
        {
            displayColor = Werewolf.RoleColor;
        }
        else if (player.GetModData().role == Roles.Challenger)
        {
            displayColor = Challenger.RoleColor;
        }
        else if (player.GetModData().role == Roles.OracleN)
        {
            displayColor = Oracle.RoleColor;
        }
        else if (player.GetModData().role == Roles.BlackSanta)
        {
            displayColor = BlackSanta.RoleColor;
        }
    }

    public override void OnMeetingEnd()
    {
        morphTarget = null;
        morphButton.Sprite = sampleButtonSprite.GetSprite();
        morphButton.SetLabel("button.label.sample");
    }

    public override void CleanUp()
    {
        if (morphButton != null)
        {
            morphButton.Destroy();
            morphButton = null;
        }
        if (arrow != null)
        {
            GameObject.Destroy(arrow.arrow);
            arrow = null;
        }
        foreach (var a in impostorArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in jackalArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in pavlovArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in moriartyArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in sheriffArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in werewolfArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in challengerArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in oracleArrows) if (a != null) GameObject.Destroy(a.arrow);
        foreach (var a in santaArrows) if (a != null) GameObject.Destroy(a.arrow);
        impostorArrows.Clear();
        jackalArrows.Clear();
        pavlovArrows.Clear();
        moriartyArrows.Clear();
        sheriffArrows.Clear();
        werewolfArrows.Clear();
        challengerArrows.Clear();
        oracleArrows.Clear();
        santaArrows.Clear();
    }

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Arsonist);
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        isDead = false;
        WinTrigger = false;
    }

    public Puppeteer()
            : base("Puppeteer", "puppeteer", RoleColor, RoleCategory.Neutral, Side.Puppeteer, Side.Puppeteer,
                 new HashSet<Side>() { Side.Puppeteer }, new HashSet<Side>() { Side.Puppeteer }, new HashSet<Patches.EndCondition>() { Patches.EndCondition.PuppeteerWin },
                 true, VentPermission.CanNotUse, false, true, true)
    {
        impostorArrows = new List<Arrow?>();
        jackalArrows = new List<Arrow?>();
        pavlovArrows = new List<Arrow?>();
        moriartyArrows = new List<Arrow?>();
        sheriffArrows = new List<Arrow?>();
        werewolfArrows = new List<Arrow?>();
        challengerArrows = new List<Arrow?>();
        oracleArrows = new List<Arrow?>();
        santaArrows = new List<Arrow?>();
        morphButton = null;
    }
}

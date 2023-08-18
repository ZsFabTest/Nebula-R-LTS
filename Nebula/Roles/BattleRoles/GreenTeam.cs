namespace Nebula.Roles.BattleRoles;

public class GreenTeam : Role
{
    public static Color RoleColor = new(181f / 255f,230f / 255f,29f / 255f);

    private bool equipRifleFlag;

    private PlayerControl GetShootPlayer(float shotSize, float effectiveRange, bool onlyWhiteName = false)
    {
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle = Game.GameData.data.myData.getGlobalData().MouseAngle;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            //自分自身は撃ち抜かれない
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead) continue;

            if (onlyWhiteName)
            {
                if (player.GetModData().role.side == Side.Impostor || player.GetModData().role.DeceiveImpostorInNameDisplay) continue;
            }

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new Vector3(
                pos.x * MathF.Cos(mouseAngle) + pos.y * MathF.Sin(mouseAngle),
                pos.y * MathF.Cos(mouseAngle) - pos.x * MathF.Sin(mouseAngle));
            if (Math.Abs(pos.y) < shotSize && (!(pos.x < 0)) && pos.x < num)
            {
                num = pos.x;
                result = player;
            }
        }
        return result;
    }

    /* ボタン */
    static private CustomButton sniperButton;
    static private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (sniperButton != null)
        {
            sniperButton.Destroy();
        }
        sniperButton = new CustomButton(
            () =>
            {
                if (equipRifleFlag)
                {
                    RPCEventInvoker.SniperSettleRifle();
                }
                else
                {
                    Objects.SoundPlayer.PlaySound(Module.AudioAsset.SniperEquip);
                    RPCEventInvoker.ObjectInstantiate(Objects.ObjectTypes.SniperRifle.Rifle, PlayerControl.LocalPlayer.transform.position);
                }
                equipRifleFlag = !equipRifleFlag;

                sniperButton.SetLabel(equipRifleFlag ? "button.label.unequip" : "button.label.equip");
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                sniperButton.Timer = sniperButton.MaxTimer;
                sniperButton.SetLabel("button.label.equip");
            },
            snipeButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.equip"
        );
        sniperButton.Timer = sniperButton.MaxTimer = 0f;
        sniperButton.actionButton.transform.SetSiblingIndex(HudManager.Instance.ImpostorVentButton.transform.GetSiblingIndex());

        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                Objects.SoundPlayer.PlaySound(Module.AudioAsset.SniperShot);

                PlayerControl target = GetShootPlayer(CustomOptionHolder.BattleShotSizeOption.getFloat() * 0.4f, CustomOptionHolder.BattleShotEffectiveRangeOption.getFloat(), true);
                if (target != null)
                {
                    var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Sniped, false, false);

                    killButton.Timer = killButton.MaxTimer;
                }

                RPCEventInvoker.SniperShot();
                killButton.Timer = killButton.MaxTimer;
                RPCEventInvoker.SniperSettleRifle();
                equipRifleFlag = false;
                sniperButton.SetLabel("button.label.equip");
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && equipRifleFlag; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
             "button.label.snipe"
        ).SetTimer(CustomOptionHolder.BattleInitCooldownOption.getFloat());
        killButton.MaxTimer = CustomOptionHolder.BattleSnipeCoolDownOption.getFloat();
        killButton.FireOnClicked = true;
        killButton.SetButtonCoolDownOption(true);

    }

    public override void OnMeetingStart()
    {
        RPCEventInvoker.SniperSettleRifle();
        equipRifleFlag = false;
    }

    /* 画像 */
    private SpriteLoader snipeButtonSprite = new SpriteLoader("Nebula.Resources.SnipeButton.png", 115f, "ui.button.sniper.equip");

    public override HelpSprite[] helpSprite => new HelpSprite[]
    {
            new HelpSprite(snipeButtonSprite,"role.sniper.help.equip",0.3f)
    };

    public override Tuple<string, Action>[] helpButton => new Tuple<string, Action>[]
    {
        new Tuple<string, Action>("role.sniper.help.shotEffective",()=>{ new Objects.EffectCircle(PlayerControl.LocalPlayer.gameObject.transform.position, Palette.ImpostorRed,CustomOptionHolder.BattleShotEffectiveRangeOption.getFloat(), 16f); }),
        new Tuple<string, Action>("role.sniper.help.soundEffective",()=>{ new Objects.EffectCircle(PlayerControl.LocalPlayer.gameObject.transform.position, Palette.ImpostorRed, CustomOptionHolder.BattleNoticeRangeOption.getFloat(),16f); }),
        new Tuple<string, Action>("role.sniper.help.shotSize",()=>{new Objects.EffectCircle(PlayerControl.LocalPlayer.gameObject.transform.position, Palette.White, CustomOptionHolder.BattleShotSizeOption.getFloat()*0.4f,16f,false,Palette.ImpostorRed);})
    };

    private Sprite snipeArrowSprite = null;
    public Sprite getSnipeArrowSprite()
    {
        if (snipeArrowSprite) return snipeArrowSprite;
        snipeArrowSprite = Helpers.loadSpriteFromResources("Nebula.Resources.SniperRifleArrow.png", 200f);
        return snipeArrowSprite;
    }

    private static Sprite guideSprite = null;
    public static Sprite getGuideSprite()
    {
        if (guideSprite) return guideSprite;
        guideSprite = Helpers.loadSpriteFromResources("Nebula.Resources.SniperGuide.png", 100f);
        return guideSprite;
    }

    public override void MyPlayerControlUpdate()
    {
        if (equipRifleFlag)
        {
            RPCEventInvoker.UpdatePlayerControl();
        }
    }

    public override void OnDied()
    {
        if (equipRifleFlag)
        {
            RPCEventInvoker.SniperSettleRifle();
            equipRifleFlag = false;
        }
    }

    public override void Initialize(PlayerControl __instance)
    {
        equipRifleFlag = false;
    }


    public override void CleanUp()
    {
        if (equipRifleFlag)
        {
            RPCEventInvoker.SniperSettleRifle();
            equipRifleFlag = false;
        }

        if (sniperButton != null)
        {
            sniperButton.Destroy();
            sniperButton = null;
        }

        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void EditOthersDisplayNameColor(byte playerId,ref Color displayColor){
        displayColor = RoleColor;
    }

    public override void OnMurdered(byte murderId){
        Game.GameData.data.myData.CanSeeEveryoneInfo = true;
    }

    public GreenTeam()
        : base("GreenTeam", "greenTeam", RoleColor, RoleCategory.Neutral, Side.GreenTeam, Side.GreenTeam,
             new HashSet<Side>() { Side.GreenTeam }, new HashSet<Side>() { Side.GreenTeam }, new HashSet<Patches.EndCondition>() { Patches.EndCondition.GreenTeamWin },
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        sniperButton = null;
        killButton = null;
        IsHideRole = true;
        ValidGamemode = Module.CustomGameMode.StandardHnS;
        canReport = false;
        CanCallEmergencyMeeting = false;
    }
}

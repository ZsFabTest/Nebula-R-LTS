﻿using Mono.Cecil;

namespace Nebula.Roles.NeutralRoles;

public class Pavlov : Role
{
    public class PavlovEvent : Events.LocalEvent
    {
        PlayerControl target;
        public PavlovEvent(PlayerControl target) : base(0.1f) { this.target = target; }
        public override void OnActivate()
        {
            RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Dog);
        }
    }

    static public Color RoleColor = new Color(236f / 255f, 182f / 255f, 91f / 255f);

    private Module.CustomOption createDogsCooldownOption;
    private Module.CustomOption dogMaxNumOption;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.AppointButton.png", 115f);

    public bool hasDog;
    public static int leftDogDataId;
    public static byte myDog;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        myDog = 255;
        leftDogDataId = (int)dogMaxNumOption.getFloat();
        hasDog = false;
    }

    private CustomButton feed;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (feed != null)
        {
            feed.Destroy();
        }
        feed = new CustomButton(
        () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                Events.LocalEvent.Activate(new PavlovEvent(target));
                myDog = target.PlayerId;
                leftDogDataId--;
                hasDog = true;
                feed.Timer = feed.MaxTimer;
                feed.UsesText.text = leftDogDataId.ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && !hasDog && leftDogDataId > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { feed.Timer = feed.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.feed"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        feed.MaxTimer = createDogsCooldownOption.getFloat();
        feed.UsesText.text = leftDogDataId.ToString();
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) => {
                if (player.Object.inVent) return false;
                if(player.GetModData().role.side == Side.Pavlov) return false;
                return true;
            }
        );
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
        if (hasDog && myDog != 255)
        {
            PlayerControl Dog = Helpers.playerById(myDog);
            if(Dog.Data.IsDead || Dog.GetModData().role != Roles.Dog) hasDog = false;
            feed.Timer = feed.MaxTimer;
        }
    }

    public override void CleanUp()
    {
        hasDog = false;
        myDog = 255;
        if(feed != null)
        {
            feed.Destroy();
            feed = null;
        }
    }

    public override void LoadOptionData()
    {
        createDogsCooldownOption = CreateOption(Color.white, "createDogsCooldown", 30f, 15f, 35f, 5f);
        createDogsCooldownOption.suffix = "second";
        dogMaxNumOption = CreateOption(Color.white,"dogMaxNum",3f,1f,5f,1f);
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Pavlov)
        {
            displayColor = RoleColor;
        }
    }

    public override IEnumerable<Assignable> GetFollowRoles()
    {
        yield return Roles.Dog;
    }

    public Pavlov()
        : base("Pavlov", "pavlov", RoleColor, RoleCategory.Neutral, Side.Pavlov, Side.Pavlov,
             new HashSet<Side>() { Side.Pavlov }, new HashSet<Side>() { Side.Pavlov },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.PavlovWin },
             true, VentPermission.CanNotUse, false, false, false)
    {
        feed = null;
        myDog = 255;
        leftDogDataId = 3;
        HideInExclusiveAssignmentOption = true;
    }
}

public class Dog : Role
{
    public Module.CustomOption dogKillCooldownOption;
    public Module.CustomOption dogCanUseVentOption;
    public Module.CustomOption madDogSuicideMaxTimeOption;
    public Module.CustomOption madDogKillCooldownOption;

    private SpriteLoader SuicideButtonSprite = new SpriteLoader("Nebula.Resources.SuicideButton.png", 115f);

    public override bool IsSpawnable()
    {
        return Roles.Pavlov.IsSpawnable();
    }

    private bool isMadDog = false;
    private double suicideTime;
    private bool isGaming;

    private CustomButton killButton,suicideButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                var r = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                killButton.Timer = killButton.MaxTimer;
                suicideTime = madDogSuicideMaxTimeOption.getFloat();
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = dogKillCooldownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
        VentPermission = dogCanUseVentOption.getBool() ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse;

        if (suicideButton != null)
        {
            suicideButton.Destroy();
        }
        suicideButton = new CustomButton(
            () => { },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && isMadDog; },
            () => { return true; },
            () => { 
                suicideTime = madDogSuicideMaxTimeOption.getFloat();
            },
            SuicideButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            KeyCode.None,
            "button.label.suicideLeft"
        ).SetTimer(114514f);
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Pavlov)
        {
            displayColor = Pavlov.RoleColor;
        }
    }

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

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);

        if(!isMadDog && PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((p) => { return !p.Data.IsDead && p.GetModData().role == Roles.Pavlov; }) == null){
            isMadDog = true;
            suicideTime = madDogSuicideMaxTimeOption.getFloat();
            killButton.MaxTimer = madDogKillCooldownOption.getFloat();
            killButton.Timer = 0f;
        }
        if(isMadDog && suicideTime <= 0 && !PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance)
        {
            RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, Game.PlayerData.PlayerStatus.Suicide.Id,true);
        }
        if(isGaming && !Game.GameData.data.IsTimeStopped){
            suicideTime -= Time.deltaTime;
            suicideButton.Timer = (float)suicideTime;
        }
    }

    public override void Initialize(PlayerControl __instance){
        isGaming = true;
        isMadDog = false;
    }

    public override void OnMeetingStart(){
        isGaming = false;
    }

    public override void OnMeetingEnd(){
        isGaming = true;
    }

    public override void OnRevived(byte playerId){
        if(playerId == PlayerControl.LocalPlayer.PlayerId) suicideTime = madDogSuicideMaxTimeOption.getFloat();
    }

    public override void CleanUp()
    {
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
        if(suicideButton != null){
            suicideButton.Destroy();
            suicideButton = null;
        }
    }

    public override void LoadOptionData()
    {
        TopOption.AddCustomPrerequisite(() => Roles.Pavlov.IsSpawnable());
        dogKillCooldownOption = CreateOption(Color.white, "dogKillCooldown", 25f, 5f, 60f, 5f);
        dogKillCooldownOption.suffix = "second";
        dogCanUseVentOption = CreateOption(Color.white,"dogCanUseVent",true);
        madDogSuicideMaxTimeOption = CreateOption(Color.white,"madDogSuicideMaxTime",25f,2.5f,60f,2.5f);
        madDogSuicideMaxTimeOption.suffix = "second";
        madDogKillCooldownOption = CreateOption(Color.white,"madDogKillCooldown",17.5f,2.5f,45f,2.5f);
        madDogKillCooldownOption.suffix = "second";
    }

    public Dog()
        : base("Dog", "dog", Pavlov.RoleColor, RoleCategory.Neutral, Side.Pavlov, Side.Pavlov,
             new HashSet<Side>() { Side.Pavlov }, new HashSet<Side>() { Side.Pavlov },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.PavlovWin },
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        Allocation = AllocationType.None;
        CreateOptionFollowingRelatedRole = true;
    }
}
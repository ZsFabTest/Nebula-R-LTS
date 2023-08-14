namespace Nebula.Roles.ImpostorRoles;

public class BomberA : Template.TImpostor{
    public static Module.CustomOption bombSetCooldown;
    public static Module.CustomOption bombSetDuringTime;
    public static Module.CustomOption bombExplodeCooldown;

    public static SpriteLoader sprite1 = new("Nebula.Resources.ElecPolePlaceButton.png",115f);
    public static SpriteLoader sprite2 = new("Nebula.Resources.MadmateButton.png",115f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
        bombSetCooldown = CreateOption(Color.white,"bombSetCooldown",15f,2.5f,25f,2.5f);
        bombSetCooldown.suffix = "second";
        bombSetDuringTime = CreateOption(Color.white,"bombSetDuringTime",2f,1f,5f,1f);
        bombSetDuringTime.suffix = "second";
        bombExplodeCooldown = CreateOption(Color.white,"bombExplodeCooldown",20f,2.5f,45f,2.5f);
        bombExplodeCooldown.suffix = "second";
    }

    public override Role[] AssignedRoles => new Role[] { this, Roles.BomberB };
    public override int AssignmentCost => 2;

    public byte target;
    private bool isParternDied;
    private Arrow arrow;

    public override void GlobalInitialize(PlayerControl __instance){
        target = byte.MaxValue;
        HideKillButtonEvenImpostor = true;
        isParternDied = false;
        arrow = new(Palette.ImpostorRed);
    }

    private CustomButton bombButton,explodeButton;
    public override void ButtonInitialize(HudManager __instance){
        bombButton?.Destroy();
        bombButton = new CustomButton(
            () => {  },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && !isParternDied; },
            () => { return PlayerControl.LocalPlayer.CanMove && target == byte.MaxValue && Game.GameData.data.myData.currentTarget; },
            () => { bombButton.Timer = bombButton.MaxTimer; },
            sprite1.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            bombSetDuringTime.getFloat(),
            () => {
                if(Game.GameData.data.myData.currentTarget == null) return;
                target = Game.GameData.data.myData.currentTarget.PlayerId;
                Game.GameData.data.myData.currentTarget = null;
                RPCEventInvoker.SetBombTarget(1,target);
                bombButton.Timer = bombButton.MaxTimer;
            },
            "button.label.set"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        bombButton.MaxTimer = bombSetCooldown.getFloat();

        explodeButton?.Destroy();
        explodeButton = new CustomButton(
            () => {
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer,Helpers.playerById(target),Game.PlayerData.PlayerStatus.Dead,false,false);
                RPCEventInvoker.SetBombTarget(1,byte.MaxValue);
                RPCEventInvoker.SetBombTarget(2,byte.MaxValue);
                explodeButton.Timer = explodeButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && !isParternDied; },
            () => { return PlayerControl.LocalPlayer.CanMove && target != byte.MaxValue && Game.GameData.data.myData.currentTarget && Roles.BomberB.target != byte.MaxValue; },
            () => { 
                target = byte.MaxValue;
                explodeButton.Timer = explodeButton.MaxTimer;
            },
            sprite2.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.secondaryAbilityInput.keyCode,
            "button.label.explode"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        explodeButton.MaxTimer = bombExplodeCooldown.getFloat();
        explodeButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp(){
        if(bombButton != null){
            bombButton.Destroy();
            bombButton = null;
        }
        if(explodeButton != null){
            explodeButton.Destroy();
            explodeButton = null;
        }
        UnityEngine.GameObject.Destroy(arrow?.arrow);
    }

    public override void MyPlayerControlUpdate(){
        Game.MyPlayerData data = Game.GameData.data.myData;
        if(target == byte.MaxValue) data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        else data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((player) => {
            if(player.GetModData().role == Roles.BomberB) return true;
            else return false;
        });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        if(PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((player) => { return !player.Data.IsDead && player.GetModData().role == Roles.BomberB; }) == null){
            target = byte.MaxValue;
            HideKillButtonEvenImpostor = false;
            isParternDied = true;
            GameObject.Destroy(arrow?.arrow);
        }else arrow.Update(PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((player) => { return !player.Data.IsDead && player.GetModData().role == Roles.BomberB; }).transform.position);
    }

    public override void OnDied(){
        target = byte.MaxValue;
        HideKillButtonEvenImpostor = false;
        isParternDied = true;
    }

    public override void EditOthersDisplayNameColor(byte playerId,ref Color displayColor){
        if(playerId == target || playerId == Roles.BomberB.target) displayColor = new(0f,0f,0f);
    }

    public BomberA() : base("Bomber","bomber",true){
        HideKillButtonEvenImpostor = true;
    }
}
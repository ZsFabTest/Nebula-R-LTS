using Rewired.Demos;

namespace Nebula.Roles.ImpostorRoles;

public class BomberB : Template.TImpostor{
    public byte target;
    public bool isParternDied;
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
            BomberA.sprite1.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            BomberA.bombSetDuringTime.getFloat(),
            () => {
                if(Game.GameData.data.myData.currentTarget == null) return;
                target = Game.GameData.data.myData.currentTarget.PlayerId;
                Game.GameData.data.myData.currentTarget = null;
                RPCEventInvoker.SetBombTarget(2,target);
                bombButton.Timer = bombButton.MaxTimer;
            },
            "button.label.set"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        bombButton.MaxTimer = BomberA.bombSetCooldown.getFloat();

        explodeButton?.Destroy();
        explodeButton = new CustomButton(
            () => {
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer,Helpers.playerById(target),Game.PlayerData.PlayerStatus.Dead,false,false);
                RPCEventInvoker.SetBombTarget(1,byte.MaxValue);
                RPCEventInvoker.SetBombTarget(2,byte.MaxValue);
                explodeButton.Timer = explodeButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && !isParternDied; },
            () => { return PlayerControl.LocalPlayer.CanMove && target != byte.MaxValue && Game.GameData.data.myData.currentTarget && Roles.BomberA.target != byte.MaxValue; },
            () => { 
                target = byte.MaxValue;
                explodeButton.Timer = explodeButton.MaxTimer;
            },
            BomberA.sprite2.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.secondaryAbilityInput.keyCode,
            "button.label.explode"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        explodeButton.MaxTimer = BomberA.bombExplodeCooldown.getFloat();
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
            if(player.GetModData().role == Roles.BomberA) return true;
            else return false;
        });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        if(PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((player) => { return !player.Data.IsDead && player.GetModData().role == Roles.BomberA; }) == null){
            target = byte.MaxValue;
            HideKillButtonEvenImpostor = false;
            isParternDied = true;
            GameObject.Destroy(arrow?.arrow);
        }else arrow.Update(PlayerControl.AllPlayerControls.GetFastEnumerator().FirstOrDefault((player) => { return !player.Data.IsDead && player.GetModData().role == Roles.BomberA; }).transform.position);
    }

    public override void OnDied(){
        target = byte.MaxValue;
        HideKillButtonEvenImpostor = false;
        isParternDied = true;
    }

    public override void EditOthersDisplayNameColor(byte playerId,ref Color displayColor){
        if(playerId == target || playerId == Roles.BomberA.target) displayColor = new(0f,0f,0f);
    }

    public BomberB() : base("Bomber","bomber",true){
        IsHideRole = true;
        HideKillButtonEvenImpostor = true;
    }
}
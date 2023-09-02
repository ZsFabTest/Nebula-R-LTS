using Iced.Intel;

namespace Nebula.Roles.NeutralRoles;

public class Follower : Role{
    public static Color RoleColor = new Color(113f / 255f,1f,149f / 255f);

    public byte targetId;

    private SpriteLoader ButtonSprite = new("Nebula.Resources.AssassinMarkButton.png",115f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
    }

    public override void GlobalInitialize(PlayerControl __instance){
        targetId = byte.MaxValue;
    }

    private CustomButton choose;
    public override void ButtonInitialize(HudManager __instance){
        choose?.Destroy();
        choose = new CustomButton(
            () => {
                RPCEventInvoker.UpdateFollowerData(Game.GameData.data.myData.currentTarget.PlayerId);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && targetId == byte.MaxValue; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget; },
            () => {},
            ButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.choose"
        );
        choose.Timer = choose.MaxTimer = 0f;
    }

    public override void CleanUp(){
        if(choose != null){
            choose.Destroy();
            choose = null;
        }
    }

    public override void MyPlayerControlUpdate(){
        var data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget,RoleColor);

        if(targetId != byte.MaxValue){
            try{
                var targetRole = Helpers.playerById(targetId).GetModData().role;
                Helpers.playerById(targetId).GetModData().RoleInfo = Helpers.cs(targetRole.Color,Language.Language.GetString("role." + targetRole.LocalizeName + ".name"));
            }catch(Exception e){
                Debug.LogError(e.StackTrace);
                targetId = byte.MaxValue;
            }
        }
    }

    public override void EditDisplayNameColor(byte playerId,ref Color displayColor){
        if(PlayerControl.LocalPlayer.PlayerId == targetId){
            displayColor = RoleColor;
        }
    }

    public override bool CheckAdditionalWin(PlayerControl player,Patches.EndCondition condition){
        if(targetId == byte.MaxValue) return false;
        var target = Helpers.playerById(targetId);
        if (target.GetModData().role.CheckWin(target, condition))
        {
            Patches.EndGameManagerSetUpPatch.AddEndText(Language.Language.GetString("role.follower.additionalEndText"));
            return true;
        }
        bool winFlag = false;
        Helpers.RoleAction(target, (role) =>
        {
            winFlag |= role.CheckAdditionalWin(target, condition);
        });
        if(winFlag) Patches.EndGameManagerSetUpPatch.AddEndText(Language.Language.GetString("role.follower.additionalEndText"));
        return winFlag;
    }



    public Follower() : base("Follower","follower",RoleColor,RoleCategory.Neutral,Side.Opportunist,Side.Opportunist,
         new HashSet<Side>() { Side.Opportunist },new HashSet<Side>() { Side.Opportunist },new HashSet<Patches.EndCondition>() { },
         true,VentPermission.CanNotUse,false,false,false){
            choose = null;
    }
}
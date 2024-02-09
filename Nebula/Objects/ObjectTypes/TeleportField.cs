using Nebula.Module;

namespace Nebula.Objects.ObjectTypes;

public class TeleportField : TypeWithImage{
    public float[] LastTeleport;
    public static SpriteLoader ActivePortal = new("Nebula.Resources.ActivePortal.png",125f);
    public static SpriteLoader InactivePortal = new("Nebula.Resources.InactivePortal.png",125f);
    public static SpriteLoader UnusablePortal = new("Nebula.Resources.UnusablePortal.png",125f);
    public TeleportField() : base(232, "TeleportField", new SpriteLoader("Nebula.Resources.PerkFrame.png",100f)){
        LastTeleport = new float[100];
    }

    public override bool CanSeeInShadow(CustomObject? obj) => true;
    public override bool IsUsable => true;
    public override bool RequireMonoBehaviour => true;
    public override bool CanUse(CustomObject obj, PlayerControl player)
    {
        if(LastTeleport[obj.Data[0]] > 0f) return false;
        /*
        //int cnt = 0;
        foreach(var oj in CustomObject.Objects){
            if(oj.Value.ObjectType == CustomObject.Type.TeleportField && oj.Value != obj){

            }
        }
        */
        if(CustomObject.Objects.Values.FirstOrDefault((oj) => { return oj.ObjectType == CustomObject.Type.TeleportField && oj != obj && oj.Data[0] == obj.Data[0]; }) == null) return false;
        return true;
    }
    public override void Use(CustomObject obj)
    {
        foreach(var oj in CustomObject.Objects.Values){
            if(oj.ObjectType == CustomObject.Type.TeleportField && oj != obj && oj.Data[0] == obj.Data[0]){
                PlayerControl.LocalPlayer.transform.position = oj.GameObject.transform.position;
                RPCEventInvoker.ResetTeleportField(obj.Data[0]);
            }
        }
    }
    public override void Initialize(CustomObject obj){
        base.Initialize(obj);
        obj.Data = new int[2];
        obj.Data[0] = CustomObject.Objects.Values.Count((oj) => { return oj.ObjectType == CustomObject.Type.TeleportField; }) / 2;
        LastTeleport[obj.Data[0]] = 0f;
        obj.Renderer.sprite = UnusablePortal.GetSprite();
        //obj.Renderer.size = new(2f,2f);
        obj.Renderer.enabled = true;
        obj.UsableBehaviour.hasOutLine = false;
    }

    public override void Update(CustomObject obj)
    {
        base.Update(obj);
        FixZPosition(obj);
        LastTeleport[obj.Data[0]] -= Time.deltaTime / 2;
        if(CustomObject.Objects.Values.FirstOrDefault((oj) => { return oj.ObjectType == CustomObject.Type.TeleportField && oj != obj && oj.Data[0] == obj.Data[0]; }) != null){
            if(LastTeleport[obj.Data[0]] <= 0f) obj.Renderer.sprite = ActivePortal.GetSprite();
            else obj.Renderer.sprite = InactivePortal.GetSprite();
            //base.Initialize(obj);
        }
    }

    public override void Update(CustomObject obj,int command){
        if(command == 1){
            LastTeleport[obj.Data[0]] = Roles.Roles.Enchanter.getCooldown();
        }
    }

    public static void ResetTime(int idx){
        /*
        foreach(var oj in CustomObject.Objects){
            if(oj.Value.ObjectType == CustomObject.Type.TeleportField && oj.Value.Data[0] == idx) oj.Value.Data[1] = Roles.Roles.Enchanter.getCooldown();
        }
        */
        //LastTeleport = Roles.Roles.Enchanter.getCooldown();
        //List<TeleportField> teleportFields;
        foreach(var oj in CustomObject.Objects.Values){
            if(oj.ObjectType == CustomObject.Type.TeleportField && oj.Data[0] == idx) oj.Update(1);
        }
    }
}
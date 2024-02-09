namespace Nebula.Objects.ObjectTypes;

public class Bomb : TypeWithImage
{
    public Bomb() : base(114, "Bomb", new SpriteLoader("Nebula.Resources.BombEffect.png",165f))
    {
    }

    public override bool RequireMonoBehaviour => true;

    public override bool CanSeeInShadow(CustomObject? obj) { return true; }

    public override void OnMeetingEnd(CustomObject obj){
        RPCEvents.ObjectDestroy(obj.Id);
    }
}

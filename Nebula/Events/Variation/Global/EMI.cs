namespace Nebula.Events.Variation.Global;

public class EMI : GlobalEvent
{
    public EMI(float duration, ulong option) : base(GlobalEvent.Type.EMI, duration, option)
    {
    }
}


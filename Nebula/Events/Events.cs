using Nebula.Events.Variation.Global;

namespace Nebula.Events;

class Events
{
    static public void Load()
    {
        GlobalEvent.Register(GlobalEvent.Type.Camouflage, (duration, option) => { return new Camouflage(duration, option); });
        GlobalEvent.Register(GlobalEvent.Type.BlackOut, (duration, option) => { return new BlackOut(duration, option); });
        GlobalEvent.Register(GlobalEvent.Type.EMI, (duration, option) => { return new EMI(duration, option); });
    }

}


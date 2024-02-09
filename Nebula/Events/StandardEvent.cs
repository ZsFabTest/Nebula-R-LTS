namespace Nebula.Events;

public class StandardEvent : LocalEvent{
    Action action;
    public StandardEvent(Action action,float time,bool isCrossMeeting,bool WillStop) : base(time) { this.action = action; SpreadOverMeeting = isCrossMeeting; this.WillStop = WillStop; }
    public override void OnTerminal(){
        action.Invoke();
    }

    public static void SetEvent(Action action,float time = 0f,bool isCrossMeeting = false,bool WillStop = true){
        LocalEvent.Activate(new StandardEvent(action,time,isCrossMeeting,WillStop));
    }
}
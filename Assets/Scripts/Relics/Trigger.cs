using System;
using System.Net.Security;

public abstract class Trigger
{
    public struct TriggerData
    {
        public string description;
        public string type;
        public string amount;
    };

    public string description;
    public string type;
    public string amount;
    protected Action action;
    public Trigger(TriggerData data)
    {
        description = data.description;
        type = data.type;
        amount = data.amount;
    }

    public abstract void LinkEvent(Action action); // ties the action member to an event in EventBus
    public abstract void UnlinkEvent(Action action); // unties the action member to event in EventBus
    
}

public class OnTakeDamageTrigger : Trigger
{
    public OnTakeDamageTrigger(TriggerData data) : 
    base(data)
    {
        
    }
    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerTakeDamage += action; //i think this actually properly links it?
        //normally "action" would be a function, but here it is a variable calling to some function (i think)

        //throw new NotImplementedException();
    }

    public override void UnlinkEvent(Action action)
    {
        EventBus.Instance.OnPlayerTakeDamage -= action; //"unsubscribe" from it (stop doing it)
        //throw new NotImplementedException();
    }
}

public class OnKillTrigger : Trigger
{
    public OnKillTrigger(TriggerData data) : 
    base(data)
    {
        
    }
    public override void LinkEvent(Action action)
    {
        throw new NotImplementedException();
    }
    public override void UnlinkEvent(Action action)
    {
        throw new NotImplementedException();
    }
}

public class OnStandStillTrigger : Trigger
{
    public OnStandStillTrigger(TriggerData data) : 
    base(data)
    {
        
    }
    public override void LinkEvent(Action action)
    {
        throw new NotImplementedException();
    }
    public override void UnlinkEvent(Action action)
    {
        throw new NotImplementedException();
    }
}
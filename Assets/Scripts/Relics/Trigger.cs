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
    public void HandleAction()
    {
        action?.Invoke();
    }
    
}

public class OnTakeDamageTrigger : Trigger
{
    public OnTakeDamageTrigger(TriggerData data) : 
    base(data) {}
    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerTakeDamage += HandleAction; //i think this actually properly links it?
        //normally "action" would be a function, but here it is a variable calling to some function (i think)

        //throw new NotImplementedException();
    }

    public override void UnlinkEvent(Action action)
    {
        EventBus.Instance.OnPlayerTakeDamage -= HandleAction; //"unsubscribe" from it (stop doing it)
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
        this.action = action;
        EventBus.Instance.OnEnemyKilled += HandleAction;
    }
    public override void UnlinkEvent(Action action)
    {
        EventBus.Instance.OnEnemyKilled -= HandleAction;
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
        this.action = action;
        EventBus.Instance.OnPlayerStill += HandleAction;
        
        //is smth like this needed? should this be tracking both movement & non-movement events ??
        //EventBus.Instance.OnPlayerMove -= action;
    }
    public override void UnlinkEvent(Action action)
    {
        EventBus.Instance.OnPlayerStill -= HandleAction;
    }
}
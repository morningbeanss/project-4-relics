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
        EventBus.Instance.OnPlayerTakeDamage += HandleAction; 
    }

    public override void UnlinkEvent(Action action)
    {
        //setting it equal again in case some other action took place in between & set it to smth else
        this.action = action;
        EventBus.Instance.OnPlayerTakeDamage -= HandleAction; //"unsubscribe" from it (stop doing it)
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
        this.action = action;
        EventBus.Instance.OnEnemyKilled -= HandleAction;
    }
}

public class OnMoveTrigger : Trigger
{
    public OnMoveTrigger(TriggerData data) :
    base(data)
    {
        
    }

    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerMove += HandleAction;
    }

    public override void UnlinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerMove -= HandleAction;
    }
}
public class OnStandStillTrigger : Trigger
{
    private float timer;
    public OnStandStillTrigger(TriggerData data) : 
    base(data)
    {
        
    }
    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerStill += HandleAction;
    }
    public override void UnlinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerStill -= HandleAction;
    }
}
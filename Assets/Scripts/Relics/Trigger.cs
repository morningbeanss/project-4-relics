using System;
using System.Collections;
using System.Net.Security;
using UnityEngine;

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
    public abstract void UnlinkEvent(); // unties the action member to event in EventBus
    public virtual void LinkUntil(Action action) {}
    public void HandleAction()
    {
        action?.Invoke();
    }
    
}

public class OnTakeDamageTrigger : Trigger
{
    public OnTakeDamageTrigger(TriggerData data) : base(data) {}
    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerTakeDamage += HandleAction; 
    }

    public override void UnlinkEvent()
    {
        //setting it equal again in case some other action took place in between & set it to smth else
        
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
    public override void UnlinkEvent()
    {
      //  this.action = action;
        EventBus.Instance.OnEnemyKilled -= HandleAction;
    }
}
public class OnStandStillTrigger : Trigger
{
    private Coroutine coroutine;
    public OnStandStillTrigger(TriggerData data) : base(data) {}
    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnPlayerStill += StartTimer;
   //     EventBus.Instance.OnPlayerMove += CancelTimer;
    }
    public override void UnlinkEvent()
    {
       // this.action = action;
        EventBus.Instance.OnPlayerStill -= StartTimer;
   //     EventBus.Instance.OnPlayerMove -= CancelTimer;
    }

    public override void LinkUntil(Action action)
    {
        EventBus.Instance.OnPlayerMove += action;
        EventBus.Instance.OnPlayerMove += CancelTimer;
    }

    private void StartTimer()
    {
        if (coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(coroutine);
        }
        coroutine = CoroutineManager.Instance.StartCoroutine(Timer());
        
    }

    private void CancelTimer()
    {
        if (coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(
            RPNEvaluator.RPNEvaluator.Evaluate(amount, GameManager.Instance.MasterVarDict)
        );
        HandleAction();
        coroutine = null;
    }
}

public class OnSpellCastTrigger : Trigger
{
    public OnSpellCastTrigger(TriggerData data) : base(data) {}

    public override void LinkEvent(Action action)
    {
        this.action = action;
        EventBus.Instance.OnSpellCast += HandleAction;
    }

    public override void UnlinkEvent()
    {
        EventBus.Instance.OnSpellCast -= HandleAction;
    }
}
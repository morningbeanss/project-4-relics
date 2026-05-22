using System;
using System.Diagnostics;

public class Relic
{
    public struct RelicData
    {
      public string name;
      public int sprite;
      public Trigger.TriggerData trigger;
      public Effect.EffectData effect;  
    };

    public string name;
    public int sprite;
    public Trigger trigger;
    public Effect effect;

    //to track whether relic is alr active or not, so functions can just return instead of doing smth twice
    private bool isActive = false; 

    public Relic(RelicData data)
    {

        name = data.name;
        sprite = data.sprite;
        // run the constructor for trigger and effect
        switch (data.trigger.type)
        {
            case "take-damage":
                trigger = new OnTakeDamageTrigger(data.trigger);
            break;

            case "stand-still":
                trigger = new OnStandStillTrigger(data.trigger);
            break;

            case "on-kill":
                trigger = new OnKillTrigger(data.trigger);
            break;

            // probably throw an exception if a type is not found
        }

        switch (data.effect.type)
        {
            case "gain-mana":
                effect = new GainManaEffect(data.effect);
            break;

            case "gain-spellpower":
                effect = new GainSpellPowerEffect(data.effect);
            break;
        }

    }

    //adding functions to Relic class so a relic can be easily activated/deactivated
    //(essentially i'm trying to tie together trigger.cs + effect.cs + eventbus.cs)
    public void Activate()
    {
        if (isActive == true) return;

        //if it made it past the first line, tell bool that relic is active alr
        isActive = true;

        //i (clr) got this line from deepseek cuz i didnt understand how to properly call/use effect here
        Action effectAction = () => effect.ApplyEffect();

        // (Calvin) I was going to do the same thing but with a lambda so same difference 

        //this line tells the trigger to activate the specific effect
        trigger.LinkEvent(effectAction);

        //ok this line below says Debug doesnt contain a definition for log wtv im dealing w this later
        //Debug.Log("Relic activated: " + name);


        if (effect.until != null)
        {
            switch (effect.until)
            {
                case "move" :
                    Action remove = null;
                    remove = () =>
                    {
                        effect.RemoveEffect();
                        EventBus.Instance.OnPlayerMove -= remove;
                    };
                    trigger.LinkUntil(remove);

                break;


                default:
                break;
            }
        }



    }

    public void Deactivate()
    {
        if (isActive == false) return;
        isActive = false;
        Action effectAction = () => effect.ApplyEffect();
        trigger.UnlinkEvent();
    }

    //these functions below r needed for RelicUI.cs
    public string GetLabel()
    {
        return name;
    }

    public bool IsActive() { return isActive; }
}
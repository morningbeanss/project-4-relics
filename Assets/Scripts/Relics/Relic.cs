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

}
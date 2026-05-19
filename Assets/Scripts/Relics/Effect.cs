using System.Collections.Generic;
using UnityEditor.UIElements;
using RPN = RPNEvaluator.RPNEvaluator;

public abstract class Effect
{
    public struct EffectData
    {
        public string description;
        public string type;
        public string amount;
        public string until;  
    };

    public string description;
    public string type;
    public string amount;
    public string until;
    public Effect(EffectData data)
    {
        description = data.description;
        type = data.type;
        amount = data.amount;
        until = data.until;
    }

    public Dictionary<string, int> effectVar = new Dictionary<string, int>(); //need this for RPN evaluations

    public abstract void ApplyEffect();
    public virtual void RemoveEffect() {}
}

public class GainManaEffect : Effect
{
    //2 diff scenarios:
    //1: whenever u take damage, u gain 5 mana
    //2: when u kill an enemy, u gain 10 mana

    public GainManaEffect(EffectData data) :
    base(data) {}

    public override void ApplyEffect()
    {
        //adding this statement for edge case, also makes the 2 lines inside look a lil cleaner
        var player = GameManager.Instance.player.GetComponent<PlayerController>();
        if (player != null)
        {
            //i think we only need ints from RPN.Evaluate (not floats)
            //the dictionary should also only need "wave" variable (so far)
            effectVar["wave"] = player.currentWave;
            player.mana += RPN.Evaluate(amount, effectVar);
        }
    }
}

public class GainSpellPowerEffect : Effect
{
    //2 diff scenarios:
    //1: when you take damage, your next spell gets 100 spellpower
    //2: whenever u don't move for 3 seconds, you gain 10 spellpower (+5/wave). effect ends when u move again

    public GainSpellPowerEffect(EffectData data) :
    base(data) {}

    public override void ApplyEffect()
    {
        var player = GameManager.Instance.player.GetComponent<PlayerController>();
        if (player != null)
        {
            effectVar["wave"] = player.currentWave;
            player.spellPower += RPN.Evaluate(amount, effectVar); 
        }
    }
}

//more effects here if needed ;P
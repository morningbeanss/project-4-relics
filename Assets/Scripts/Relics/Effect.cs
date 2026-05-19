using UnityEditor.UIElements;

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
    public abstract void ApplyEffect();
    public virtual void RemoveEffect() {}
}

public class GainManaEffect : Effect
{

    public GainManaEffect(EffectData data) :
    base(data) {}

    public override void ApplyEffect()
    {
        GameManager.Instance.player.GetComponent<PlayerController>().mana += 5;
    }
}

public class GainSpellPowerEffect : Effect
{
  
    public GainSpellPowerEffect(EffectData data) :
    base(data) {}

    public override void ApplyEffect()
    {
        
    }
}

// More effects to make probably
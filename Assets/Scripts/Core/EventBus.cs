using UnityEngine;
using System;

public class EventBus
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
        if (target == GameManager.Instance.player.GetComponent<PlayerController>().hp)
        {
            OnPlayerTakeDamage?.Invoke();
        }
    }
    // *** OUR EVENTS *** 
    // as far as I understand, none of them need any parameters
    public event Action OnPlayerTakeDamage;
    //public event Action OnPlayerStopped; // float is the amount of time
    public event Action OnPlayerStill;
    public event Action OnPlayerMove;
    public event Action OnEnemyKilled;
    public event Action OnAlterateEnemyKilled;
    public event Action OnSpellCast;
    public event Action<Relic> OnRelicPickup; // ??


    public void PlayerStill()
    {
        OnPlayerStill?.Invoke();
    }
    public void PlayerMove()
    {
        OnPlayerMove?.Invoke();
    }
    public void EnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }
    public void AlternateEnemyKilled()
    {
        OnAlterateEnemyKilled?.Invoke();
    }
    public void SpellCast()
    {
        OnSpellCast?.Invoke();
    }
    public void RelicPickup(Relic relic)
    {
        OnRelicPickup?.Invoke(relic);
    }

}

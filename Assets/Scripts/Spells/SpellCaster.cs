using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SpellCaster
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public Hittable.Team team;

    //keep track of what spell is active rn; can be 0-3 for up to 4 options. player switches between them by hitting 1-4 on keyboard
    public uint highlightedSpell = 0; //0 by default bc thats what ur starting spell is

    public Spell[] spells = new Spell[5]; //u can have max 4 active spells, but the 5th is when u have intermediate one as 1 more is generated each every end-of-round

    //add a variable(s) to keep track of active spells?
    //3+ waves player should constantly have 4 (need to add a way to delete/swap spells)
    //private int active_spells = 1 //player alr has 1 initially 


    PlayerController owner;


    //this file is like the Hittable file
    //(it just sets up a type of thing (sprite (player)) that can cast spells)

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team, PlayerController owner)
    {

        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
   //     Debug.Log("I am supposed to run!!!");
        this.owner = owner;
  //      Debug.Log("owner power = " + this.owner.spellPower);
        spells[0] = SpellBuilder.Build(this, "Arcane Bolt"); 
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {
        if (highlightedSpell > spells.Length - 2)
        {
            yield break;
        }
        if (spells[highlightedSpell] != null)
        {
            if (mana >= spells[highlightedSpell].GetManaCost() && spells[highlightedSpell].IsReady())
            {
                mana -= spells[highlightedSpell].GetManaCost();
                yield return spells[highlightedSpell].Cast(where, target, team);
            }
        }

        yield break;
    }

    public void playSound(AudioClip clip)
    {
        owner.source.PlayOneShot(clip);
    }

}

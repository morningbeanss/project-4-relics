using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;

//this is the file that needs to read the spells json file
//and decide what to slap together to make randomly generated spells

public static class SpellBuilder 
{
 
    static List<SpellData> baseSpells = new List<SpellData>(); //keep track of all the base spell names
    static public List<SpellData> modifierSpells = new List<SpellData>(); //keep track of all modifier spell names

    public struct SpellData
    {

        // shared
        public string name;
        public string description;
         // bal info
        public int icon;
        public Dictionary<string, string> damage;
        public string mana_cost;
        public string cooldown;
        public Dictionary<string, string> projectile;
        public string N;
        public string secondary_damage;
        public Dictionary<string, string> secondary_projectile;
        public string spray;    
         // moinfo
        public string damage_multiplier;
        public string mana_adder;
        public string mana_multiplier;
        public string angle;
        public string cooldown_multiplier;
        public string projectile_trajectory;
        public string delay;
        public string speed_multiplier;
        
    };

    static public Spell Build(SpellCaster owner, string name)
    {
     
        return new Spell(owner, baseSpells.Find( (s) => s.name == name));
        // return new Spell(owner, spelldata)
        

    }

    static public Spell Build(SpellCaster owner)
    {
        //get random base spell
     //   Random.InitState()
        SpellData randomBase = baseSpells[Random.Range(0, baseSpells.Count - 1)];
        //spell variable to hold/modify + return 
        Spell finalSpell = new Spell(owner, randomBase);

        //generating a random # that is how many mods will get added to this base
        int bleh = Random.Range(1, 4); //min:inclusive, max:exclusive
        SpellBuilder.SpellData randomMod; //set up variable that'll get set + called each loop run
        for (int i = 0; i < bleh; i++)
        {
            //randomly pick a modifier spell
            randomMod = modifierSpells[Random.Range(0, modifierSpells.Count)];

            //apply modifier(s)
            finalSpell = ApplyModifier(owner, randomMod, finalSpell);
        }
        return finalSpell; //return final spell
    }

    static SpellBuilder()
    {
        //load in json file
        string spell_json = Resources.Load<TextAsset>("spells").text;
     //   spellsData = JObject.Parse(spell_json);
        
        Dictionary<string, SpellData> spells;

        spells = JsonConvert.DeserializeObject<Dictionary<string, SpellData>>(spell_json);

        List<string> allSpells = spells.Keys.ToList();


        foreach (string spell in allSpells)
        {
            SpellData s = spells[spell];
            if (s.damage != null)
            {
                baseSpells.Add(s);
            } 
            else
            {
                modifierSpells.Add(s);
            }
        
        }

    }

    //json file is loaded, but stuff to actually make spells combine n whatnot needs to be written

    static public Spell ApplyModifier(SpellCaster owner, SpellBuilder.SpellData modifier, Spell baseSpell)
    {
        switch (modifier.name)
        {
            case "Damage-Amplified":
                return new DamageAmp(owner, modifier, baseSpell);

            case "Speed-Amplified":
                return new SpeedAmp(owner, modifier, baseSpell);

            case "Doubled":
                return new Doubler(owner, modifier, baseSpell);

            case "Split":
                return new Splitter(owner, modifier, baseSpell);

            case "Chaotic":
                return new Chaos(owner, modifier, baseSpell);

            case "Homing":
                return new Homing(owner, modifier, baseSpell);

            case "Big & Gassy":
                return new Gassy(owner, modifier, baseSpell);

            case "Bingusified":
                return new Bingus(owner, modifier, baseSpell);
            case "Instakillified":
                return new Instakill(owner, modifier, baseSpell);

            default:
                Debug.Log("Unknown Modifier Name: " + modifier.name);
                return baseSpell;
        }
    }
}

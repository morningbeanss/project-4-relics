using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using RPN = RPNEvaluator.RPNEvaluator;

//--- NOTES SO ALL THE FILES MAKE SENSE: ---
//SpellBuilder is the one that reads JSON n tells which spells to combine
//this file just sets up what a Spell should be
//(generic setup w/ functions, then a bunch of subclasses for each kinda base spell n modifier spell)
//SpellCaster is just a thing that sets up a sprite that can cast spells
public class Spell //: MonoBehaviour //need this monobehaviour (bs english spelling) so i can call it right in spellCaster.cs
{
    //public values: anyone + everyone can access these


    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    public SpellBuilder.SpellData data;

    protected Spell modified; //to hold all the info of a modified spell
    protected Dictionary<string, int> spellPowerDict = new Dictionary<string, int>();
    protected Dictionary<string, float> spellPowerDictf = new Dictionary<string, float>();


    public Spell(SpellCaster owner, SpellBuilder.SpellData data)
    {
        this.owner = owner;
        // probably should include the name and description?
        // i guess your system of having the default values defined at the top works, 
        // although usually you want to always explicity define them in the constructor

        this.team = owner.team;
        this.last_cast = 0;
        this.modified = null;

        this.data = data;
        spellPowerDict.Add("power", this.owner.getPower());
        spellPowerDict.Add("wave", this.owner.getCurrentWave());
        spellPowerDictf.Add("power", (float)this.owner.getPower());
        spellPowerDictf.Add("wave", this.owner.getCurrentWave());
    }

    public void WrapWithModifier(Spell modifiedSpell)
    {
        this.modified = modifiedSpell; //hold values of spell that is going to be modified
    }



    public virtual string GetName()
    {
        //these modified lines r to make sure values r updated correctly if the base spell has alr been thru 1+ modifications
        if (modified != null) return modified.GetName();

        // I am QUITE confident that the line above is completely unnecessary due to how inheritance works

        return data.name;
    }

    public virtual int GetManaCost()
    {
        if (modified != null) return modified.GetManaCost();
        return RPN.Evaluate(data.mana_cost, spellPowerDict);
    }

    public virtual int GetDamage()
    {
        if (modified != null) return modified.GetDamage();
        return RPN.Evaluate(data.damage["amount"], spellPowerDict);
    }

    public virtual float GetCooldown()
    {
        if (modified != null) return modified.GetCooldown();
        return RPN.Evaluatef(data.cooldown, spellPowerDictf);
    }

    public virtual int GetIcon()
    {
        if (modified != null) return modified.GetIcon();
        return data.icon;
    }

    public int GetProjectileSprite()
    {
        if (modified != null) return modified.GetProjectileSprite();
        int sprite;
        int.TryParse(data.projectile["sprite"], out sprite);
        return sprite;
    }

    public virtual string GetTrajectory()
    {
        if (modified != null) return modified.GetTrajectory();
        if (data.projectile.ContainsKey("trajectory"))
        {
            return (string)data.projectile["trajectory"];
        }
        return "straight"; //if it doesnt have that, its set up wrong/weird sp default to straight
    }

    public virtual int GetSpeed()
    {
        if (modified != null) return modified.GetSpeed();
        if (data.projectile.ContainsKey("speed"))
        {
            //try parsing from string -> int, if errorthen input lowk wrong
            if (int.TryParse((string)data.projectile["speed"], out int result))
            {
                return result;
            }
        }
        return 10; //default
    }

    public virtual string GetDescription()
    {
        if (modified != null) return modified.GetDescription();
        return data.description;
    }

    //some of the functions need this
    public virtual int GetAngle()
    {
        if (modified != null) return modified.GetAngle();
        return 0; //default
    }

    //some functions also need this
    public virtual float GetDelay()
    {
        if (modified != null) return modified.GetDelay();
        return 0; //default
    }

    //this is for default spells arcane blast/spray only
    public virtual int GetProjectileCount()
    {
        if (modified != null) return modified.GetProjectileCount();
        if (!string.IsNullOrEmpty(data.N))
        {
            return RPN.Evaluate(data.N, spellPowerDict);
        }
        return 1; //default
    }

    //for arcane spray
    public virtual float GetSprayAngle()
    {
        if (modified != null) return modified.GetSprayAngle();
        if (!string.IsNullOrEmpty(data.spray))
        {
            return RPN.Evaluatef(data.spray, spellPowerDictf);
        }
        return 0; //default
    }

    //for arcane blast
    public virtual int GetSecondaryDamage()
    {
        if (modified != null) return modified.GetSecondaryDamage();
        if (!string.IsNullOrEmpty(data.secondary_damage))
        {
            return RPN.Evaluate(data.secondary_damage, spellPowerDict);
        }
        return 0; //default
    }

    //for arcane blast
    public virtual Dictionary<string, string> GetSecondaryProjectile()
    {
        if (modified != null) return modified.GetSecondaryProjectile();
        return data.secondary_projectile;
    }

    public bool IsReady()
    {
        return last_cast + GetCooldown() < Time.time;
    }

    //i have a feeling this function will need to be edited, but for now im not touching it (clr)
    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (modified != null)
        {
            yield return modified.Cast(where, target, team);
            yield break; //MAKE IT STOOOOOP
        }
        this.team = team;

        Vector2 baseD = (target - where).normalized;

        //if it's got spicy variables (2 cases in base spells)
        int numProjectiles = GetProjectileCount();
        float sprayAngle = GetSprayAngle();
        //handle the spicy ones
        if (numProjectiles > 1 || sprayAngle > 0)
        {
            for (int i = 0; i < numProjectiles; i++)
            {
                float offsetAngle;
                if (sprayAngle > 0) //1 of them doesnt have this parameter
                {
                    //this equation spreads projectiles evenly across spray angle (equation from deepseek)
                    float j = (float)i / (numProjectiles - 1);  //floats to be more accurate
                    offsetAngle = -sprayAngle + (j * sprayAngle * 2);
                }
                else
                {
                    offsetAngle = 0; //no change to angles needed
                }
                //even if no offset angle, some rotations still need to happen
                Quaternion rotation = Quaternion.Euler(0, 0, offsetAngle);
                Vector2 direction = rotation * baseD;
            }
        }
        GameManager.Instance.projectileManager.CreateProjectile(GetProjectileSprite(), GetTrajectory(), where, target - where, GetSpeed(), OnHit);
        yield return new WaitForEndOfFrame();
    }

    public virtual void OnHit(Hittable other, Vector3 impact)
    {

        if (other.team != team)
        {
            if (modified != null)
            {
                other.Damage(new Damage(GetDamage(), Damage.TypeFromString(modified.data.damage["type"])));
            }
            else
            {
                other.Damage(new Damage(GetDamage(), Damage.TypeFromString(this.data.damage["type"])));
            }
        }

        //handle secondary projectiles if they exist
        if (GetSecondaryDamage() > 0 && GetSecondaryProjectile() != null)
        {
            ShootSecondaryProjectiles(impact);
        }
    }

    //next 2 functions r helper functions for the spicy base spells
    //only arcane blast has secondary_projectile field
    void ShootSecondaryProjectiles(Vector3 impactPos)
    {
        int numProjectiles = GetProjectileCount();
        Dictionary<string, string> secondaryProj = GetSecondaryProjectile();
        if (secondaryProj == null) return; //wrong base spell here, get outta here
        //giving these variables default values bc there's error messages w/o
        string sTrajectory = "straight";
        int sSpeed = 10;
        float sLifetime = 0.5f;
        int sSprite = 0;
        if (secondaryProj.ContainsKey("trajectory"))
        {
            sTrajectory = (string)secondaryProj["trajectory"];
        }
        if (secondaryProj.ContainsKey("speed")) sSpeed = int.Parse((string)secondaryProj["speed"]);
        if (secondaryProj.ContainsKey("lifetime")) sLifetime = float.Parse((string)secondaryProj["lifetime"]);
        if (secondaryProj.ContainsKey("sprite")) sSprite = int.Parse((string)secondaryProj["sprite"]);

        for (int i = 0; i < numProjectiles; i++)
        {
            float offsetAngle = (360 / numProjectiles) * i;
            Quaternion rotation = Quaternion.Euler(0, 0, offsetAngle);
            Vector2 direction = rotation * Vector2.up;
            //make a projectile for each angle
            GameManager.Instance.projectileManager.CreateProjectile(sSprite, sTrajectory, impactPos, direction, sSpeed, onSecondaryHit);
        }
    }

    void onSecondaryHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetSecondaryDamage(), Damage.TypeFromString(data.damage["type"])));
        }
    }
}

//the rest of the stuff below is modifier spells
//the modifier spells are 1 class for each kind of modifier spells, they're basically all 
//wrappers that each do diff tings to the base spell

// !!!! TO CALVIN: we need to make 3 original modifier spells of our own. i havent invented any yet,
//these r just rough drafts of all the modifier spells he alr gave us !!

public class DamageAmp : Spell //increases damage + mana cost
{
    private float damageMultiplier;
    private float manaMultiplier;

    public DamageAmp(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data) //need to figure out how inheritance works to make this right
    {
        this.WrapWithModifier(baseSpell);
    }

    //so spell name is displayed correctly
    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }

    //need to write new GetDamage + GetManaCost functions to override, the rest can just be the default ones from Spell class
    public override int GetManaCost()
    {
        if (float.TryParse(this.data.mana_multiplier, out manaMultiplier))
        {
            float evaluationRes = modified.GetManaCost() * manaMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override int GetDamage()
    {
        if (float.TryParse(this.data.damage_multiplier, out damageMultiplier))
        {
            float evaluationRes = modified.GetDamage() * damageMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            this.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }
}

public class SpeedAmp : Spell //increased projectile speed
{
    private float speedMultiplier;

    public SpeedAmp(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data)
    {
        this.WrapWithModifier(baseSpell);
    }

    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }

    public override int GetSpeed()
    {
        if (float.TryParse(this.data.speed_multiplier, out speedMultiplier))
        {
            float evaluationRes = modified.GetSpeed() * speedMultiplier;
            return (int)evaluationRes;
        }
        return 0;
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            this.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }
}

public class Doubler : Spell //spell is cast 2nd time after small delay; increased mana cost & cooldown
{
    private float delay;
    private float manaMultiplier;
    private float cooldownMultiplier;

    public Doubler(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data)
    {
        this.WrapWithModifier(baseSpell);
    }

    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }

    public override int GetManaCost()
    {
        if (float.TryParse(this.data.mana_multiplier, out manaMultiplier))
        {
            float evaluationRes = modified.GetManaCost() * manaMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override float GetCooldown()
    {
        if (float.TryParse(this.data.cooldown_multiplier, out cooldownMultiplier))
        {
            float evaluationRes = modified.GetCooldown() * cooldownMultiplier;
            return evaluationRes;
        }
        return 0; //default
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    //bc of the special delay stuff, the IEnumerator Cast thingy needs to be rewritten to fire at diff time
    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(), modified.GetTrajectory(),
            where, target - where, modified.GetSpeed(), this.OnHit);
        yield return new WaitForSeconds(delay);
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(), modified.GetTrajectory(),
            where, target - where, modified.GetSpeed(), this.OnHit);
    }
}


// !!!! THE ONES BELOW ARE NOT DONE ALL OF THE WAY !!!
// (the ones above should work as expected (hopefully))

public class Splitter : Spell //cast twice in slightly diff directions; increased mana cost
{
    private float manaMultiplier;
    private int angle;

    public Splitter(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data)
    {
        this.WrapWithModifier(baseSpell);

    }
    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }
    public override int GetManaCost()
    {
        if (float.TryParse(this.data.mana_multiplier, out manaMultiplier))
        {
            float evaluationRes = modified.GetManaCost() * manaMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    //smth else needs to be done here w angle !!
    public override int GetAngle()
    {
        if (int.TryParse(this.data.angle, out angle))
        {
            return angle;
        }
        return 0; //default
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        Vector2 baseD = (target - where).normalized;
        Quaternion rotationA = Quaternion.Euler(0, 0, angle);
        Quaternion rotationB = Quaternion.Euler(0, 0, -angle);
        Vector2 directionA = rotationA * baseD;
        Vector2 directionB = rotationB * baseD;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(), modified.GetTrajectory(),
            where, directionA, modified.GetSpeed(), this.OnHit);
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(), modified.GetTrajectory(),
            where, directionB, modified.GetSpeed(), this.OnHit);
        yield return new WaitForEndOfFrame();
    }
}

public class Chaos : Spell //significantly increased damage, but projectile has wacky spiraling pattern
{
    private float damageMultiplier;

    public Chaos(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data)
    {
        this.WrapWithModifier(baseSpell);

    }
    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }
    public override int GetDamage() //cant just use other GetDamage(), this one is an RPN expression
    {
        if (!string.IsNullOrEmpty(data.damage_multiplier))
        {
            damageMultiplier = RPN.Evaluatef(data.damage_multiplier, spellPowerDictf);
        }
        else
        {
            damageMultiplier = 2f; //default fall-back
        }
        return (int)(modified.GetDamage() * damageMultiplier);
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    //smth else needs to be done w trajectoryMod !!!
    public override string GetTrajectory()
    {
        if (!string.IsNullOrEmpty(data.projectile_trajectory)) //adding extra crash catches
        {
            return this.data.projectile_trajectory;
        }
        else
        {
            return "straight"; //default fall-back
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            modified.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }
}

public class Homing : Spell //homing projectile; decreased damage & increased mana cost
{
    private float damageMultiplier;
    private int manaAddition;

    public Homing(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data)
    {
        this.WrapWithModifier(baseSpell);

    }
    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }
    public override int GetDamage()
    {
        if (float.TryParse(this.data.damage_multiplier, out damageMultiplier))
        {
            float evaluationRes = modified.GetDamage() * damageMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override int GetManaCost() //this one is special, its not multiplier but just a simple addition (i think)
    {
        if (int.TryParse(this.data.mana_adder, out manaAddition))
        {
            int evaluationRes = modified.GetManaCost() + manaAddition;
            return evaluationRes;
        }
        return 0; //default
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    public override string GetTrajectory()
    {
        return this.data.projectile_trajectory;
    }


    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            modified.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }
}

//----- OUR HOMEMADE MODIFIER SPELLS ---
//sry to the grader for the first one it spawned from 7+ hours straight of coding </3 
//it also fulfills our requirement of a mod that does something beyond just editing stats; it plays a sound
public class Gassy : Spell //plays sound, increased damage + cooldown + mana cost, spiraling trajectory
{
    private float damageMultiplier;
    private int manaAddition;
    private float cooldownMultiplier;

    public Gassy(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
        base(owner, data)
    {
        this.WrapWithModifier(baseSpell);

    }

    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }

    public override int GetDamage()
    {
        if (float.TryParse(this.data.damage_multiplier, out damageMultiplier))
        {
            float evaluationRes = modified.GetDamage() * damageMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override int GetManaCost()
    {
        if (!string.IsNullOrEmpty(data.mana_adder))
        {
            manaAddition = RPN.Evaluate(data.mana_adder, spellPowerDict);
        }
        else
        {
            manaAddition = 7; //default fall-back
        }
        return modified.GetManaCost() + manaAddition;
    }

    public override float GetCooldown()
    {
        if (float.TryParse(this.data.cooldown_multiplier, out cooldownMultiplier))
        {
            float evaluationRes = modified.GetCooldown() * cooldownMultiplier;
            return evaluationRes;
        }
        return 0; //default
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    public override string GetTrajectory()
    {
        return this.data.projectile_trajectory;
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        AudioClip clip = Resources.Load<AudioClip>("bowser_gassy_1");
        owner.playSound(clip);
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            modified.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }
}

public class Bingus : Spell //slight increased damage, decreased mana cost & cooldown time
{
    private float damageMultiplier;
    private int manaAddition;
    private float cooldownMultiplier;
    public Bingus(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
        base(owner, data)
    {
        this.WrapWithModifier(baseSpell);
    }

    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }

    public override int GetDamage()
    {
        if (float.TryParse(this.data.damage_multiplier, out damageMultiplier))
        {
            float evaluationRes = modified.GetDamage() * damageMultiplier;
            return (int)evaluationRes;
        }
        return 0; //default
    }

    public override int GetManaCost()
    {
        if (!string.IsNullOrEmpty(data.mana_adder))
        {
            manaAddition = RPN.Evaluate(data.mana_adder, spellPowerDict);
        }
        else
        {
            manaAddition = 7; //default fall-back
        }
        return modified.GetManaCost() - manaAddition; //minus cuz i want mana to be cheaper
    }

    public override float GetCooldown()
    {
        if (float.TryParse(this.data.cooldown_multiplier, out cooldownMultiplier))
        {
            float evaluationRes = modified.GetCooldown() * cooldownMultiplier;
            return evaluationRes;
        }
        return 0; //default
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            this.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }
}

public class Instakill : Spell // very high damage
{
    private float damageMultiplier;
    private float cooldownMultiplier;
    public Instakill(SpellCaster owner, SpellBuilder.SpellData data, Spell baseSpell) :
    base(owner, data)
    {
        this.WrapWithModifier(baseSpell);
    }

    public override string GetName()
    {
        return data.name + " " + modified.GetName();
    }

    public override string GetDescription()
    {
        return this.data.description + " " + modified.GetDescription();
    }

    public override int GetDamage()
{
    if (!string.IsNullOrEmpty(data.damage_multiplier))
    {
        damageMultiplier = RPN.Evaluatef(data.damage_multiplier, spellPowerDictf);
    }
    else
    {
        damageMultiplier = 2f; //default fallback
    }
    return (int)(modified.GetDamage() * damageMultiplier);
}

    public override float GetCooldown()
    {
        if (float.TryParse(this.data.cooldown_multiplier, out cooldownMultiplier))
        {
            float evaluationRes = modified.GetCooldown() * cooldownMultiplier;
            return evaluationRes;
        }
        return 0; //default
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        //AudioClip clip = Resources.Load<AudioClip>("bowser_gassy_1");
        //owner.playSound(clip);
        //AudioClip clip = Resources.Load<AudioClip>("children-yaysound-effect");

       // owner.playSound(clip);
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(
            modified.GetProjectileSprite(),
            modified.GetTrajectory(),
            where, target - where,
            modified.GetSpeed(),
            this.OnHit  // this is the key — registers the wrapper's OnHit
        );
        yield return new WaitForEndOfFrame();
    }

    public override void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            AudioClip clip = Resources.Load<AudioClip>("children-yaysound-effect");
            owner.playSound(clip);
            base.OnHit(other, impact);
        }

        
    }




}
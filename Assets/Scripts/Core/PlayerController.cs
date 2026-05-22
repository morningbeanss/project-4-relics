using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using RPN = RPNEvaluator.RPNEvaluator;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;
    public SpellCaster spellcaster;
    public SpellUI spellui;
    public float speed; //changed from int (instructions said so)
    public Unit unit;
    public Player player;
    public Dictionary<string, Player> classes;
    
  
    public int sprite;
    public int health;
    public int mana;
    public int manaRegen;
    public int spellPower; //i need this in Spell.c
    public List<Relic> relics = new List<Relic>(); //this replaces ownedRelics in RelicBuilder (replacing it cuz the line "player.relics.Count-1" in RelicUIManager)

    public AudioSource source;
    public AudioClip clip;
    private bool IsMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;

        //load player class json shi
        string player_json = Resources.Load<TextAsset>("classes").text;
        classes = JsonConvert.DeserializeObject<Dictionary<string, Player>>(player_json);
    }

    public void StartLevel(string playerClassName)
    {
        //Debug.Log("PlayerController.StartLevel()");
        player = classes[playerClassName]; //set player variable to correct player class
        Debug.Log("player manaRegen = " + player.mana_regeneration);
      //  PlayerUpdate();

    
        sprite = player.sprite;

        health = RPN.Evaluate(player.health, GameManager.Instance.MasterVarDict);
        mana = RPN.Evaluate(player.mana, GameManager.Instance.MasterVarDict);
        manaRegen = RPN.Evaluate(player.mana_regeneration, GameManager.Instance.MasterVarDict);

        UpdatePower(null); // this also updates spellpower
        
        speed = RPN.Evaluatef(player.speed, GameManager.Instance.MasterVarDict);


        //SpellCaster(int mana, int mana_reg, Hittable.Team team)
        spellcaster = new SpellCaster(mana, manaRegen, Hittable.Team.PLAYER, this);
        StartCoroutine(spellcaster.ManaRegeneration());

        spellcaster.max_mana = mana;
        spellcaster.mana = mana;

        spellcaster.mana_reg = manaRegen;


        //Hittable(int hp, Team team, GameObject owner)
        hp = new Hittable(health, Hittable.Team.PLAYER, gameObject);
        hp.max_hp = health;
        hp.hp = health;
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellui.SetSpell(spellcaster.spells[0], 0); //call in a loop if multiple spells?
    }

    public void PlayerUpdate()
    {
    //    Debug.Log("made it to PlayerUpdate");
        //update all tha stuff stuff (bc wave changes values of a lot of stuff its gotta update all the time)
      
        sprite = player.sprite;

        health = RPN.Evaluate(player.health, GameManager.Instance.MasterVarDict);
        mana = RPN.Evaluate(player.mana, GameManager.Instance.MasterVarDict);
        manaRegen = RPN.Evaluate(player.mana_regeneration, GameManager.Instance.MasterVarDict);

        UpdatePower(null);

        speed = RPN.Evaluatef(player.speed, GameManager.Instance.MasterVarDict);


        hp.max_hp = health;
        hp.hp = health;

        spellcaster.max_mana = mana;
        spellcaster.mana = mana;

        spellcaster.mana_reg = manaRegen;
     
    }

    private void Update()
    {

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            if (spellcaster.spells[0] != null)
            {
                spellcaster.highlightedSpell = 0;
            }
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            if (spellcaster.spells[1] != null)
            {
                spellcaster.highlightedSpell = 1;
            }
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            if (spellcaster.spells[2] != null)
            {
               // Debug.Log("switching to 3");
                spellcaster.highlightedSpell = 2;
            }
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            if (spellcaster.spells[3] != null)
            {

                spellcaster.highlightedSpell = 3;
            }
        }


        if (!IsMoving && unit.movement != new Vector2(0,0)) // if the player is moving 
        {
            IsMoving = true;
            EventBus.Instance.PlayerMove();
        }
        else if (IsMoving && unit.movement == new Vector2(0,0))
        {
            IsMoving = false;
            EventBus.Instance.PlayerStill();
        }

        

    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

     //   source.PlayOneShot(clip);
        // switch to Play() and make it a coroutine or smth and then only let spellcaster cast when it's done

        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>()*speed;

        
    }

    

    public void Die()
    {
        Debug.Log("You Lost");
        GameManager.Instance.state = GameManager.GameState.GAMEOVER; //set the state so game knows its over
    }

    public void dropSpell()
    {
        spellcaster.spells[spellcaster.highlightedSpell] = null;
    }

    public void UpdatePower(string power)
    {
        if (power != null)
        {
            player.spellpower = power;
        }
        spellPower = RPN.Evaluate(player.spellpower, GameManager.Instance.MasterVarDict);

        GameManager.Instance.MasterVarDict["power"] = spellPower;
        GameManager.Instance.MasterVarDictF["power"] = spellPower;
    }

}

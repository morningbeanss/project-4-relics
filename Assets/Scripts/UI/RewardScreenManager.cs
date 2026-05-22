using UnityEngine;
using Unity.VisualScripting;

using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    public SpellUI spellUI;
    public PlayerController player;
    bool isSpellCreated = false;
    Spell spell;
    public TextMeshProUGUI spellDesc;
    
    public List <Relic> tempRelics = new List<Relic>();
    bool allRelicsCreated = false;
    public List <GameObject> relicChoices = new List<GameObject>();
    public List <Image> relicIcon = new List<Image>();
    public List <TextMeshProUGUI> relicName = new List<TextMeshProUGUI>();
    public List <TextMeshProUGUI> relicDescription = new List<TextMeshProUGUI>();
    public List<Button> relicButton = new List<Button>();
    public bool alreadyTookARelic = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            rewardUI.SetActive(true); // already is setting spellUI active
            
        }
        else
        {
            rewardUI.SetActive(false);
        }

        switch (GameManager.Instance.state)
        {

            case GameManager.GameState.COUNTDOWN:
                if (isSpellCreated)
                {
                    isSpellCreated = false;
                }
                if (allRelicsCreated)
                {
                    allRelicsCreated = false;
                }
                if (alreadyTookARelic)
                {
                    alreadyTookARelic = false;
                }
                tempRelics.Clear();
                rewardUI.SetActive(false);
            break;

            case GameManager.GameState.WAVEEND:
                
                if (!isSpellCreated)
                {
                    spell = SpellBuilder.Build(player.spellcaster);
                    //spell = SpellBuilder.Build(player.spellcaster, "Arcane Bolt");
                    //SpellBuilder.SpellData bigAndGassy = SpellBuilder.modifierSpells[6];
                    //spell = SpellBuilder.ApplyModifier(player.spellcaster, bigAndGassy, spell);
                    Debug.Log("new spell created");
                    isSpellCreated = true;

                    // update spellUI
                    spellUI.SetSpell(spell, 0);
                    spellDesc.text = spell.GetDescription();
                }
                
                //make smth like if wave != 0 && wave % 3 == 0 (so this stuff only shows up every 3 rounds)
                //add smth where game wont break if there aren't three diff relics to display (like it can display two or one as we begin to run out of options)
                if (!allRelicsCreated)
                {
                    RandomlySelectThreeRelics();
                    //after it all runs, update variable
                    allRelicsCreated = true; //might need a condition check before setting dis?
                    Debug.Log("num relics created: " + tempRelics.Count);
                }
                
                rewardUI.SetActive(true);
            break;

            default:
                rewardUI.SetActive(false);
            break;
        }
    }

    public void AcceptSpell()
    {
        
        int i;
        bool spellAvailable = false;
        for (i = 0; i < player.spellcaster.spells.Length - 1; i++)
        {
            if (player.spellcaster.spells[i] == null)
            {
                spellAvailable = true;
                break;
            }
        }
        if (spell != null && spellAvailable)
        {
            Debug.Log("accepting spell");
            player.spellcaster.spells[i] = spell; // make this a new spell
            spell = null;
            //spellUI.gameObject.SetActive(false);
        }
        
    }

    //teacher example of reward screen relics didn't seem to use relicUI, i liked how that looked so i set it up here (clr)
    public void RandomlySelectThreeRelics()
    {
        tempRelics.Clear(); //double making sure list is empty b4 refilling
        for (int i = 0; i < 3; i++) //3 relics need to be displayed to player for them to choose from
        {
            Relic tempRelic = RelicBuilder.Build();
            if (tempRelic == null)
            {
                Debug.Log("no more original relics available -rewardscreenmanager");
                break; //Build() will return null if no available/unused relics
            }

            //accidentally made the infinity loop from hell w my previous while loop
            int maxAttempts = 15; //adding a limiter + counter to try to avoid infinite hell death loop
            int attempts = 0;
            while (tempRelics.Contains(tempRelic) && attempts < maxAttempts)
            {
                //retry if tempRelic alr in tempRelics (no dupes!)
                tempRelic = RelicBuilder.Build();
                if (tempRelic == null) //had a Contains() thing before, forgot to check for null (prolly where evil loop hell came from)
                {
                    break;
                }
                attempts++;
            }

            Debug.Log("new relic created");

            if (tempRelic != null)
            {
                tempRelics.Add(tempRelic); //add completed relic to temporary holder
                //set up icon, name, & description texts on Reward Screen (i think buttons don't need to be set)
                if (relicIcon[i] != null)
                {
                    GameManager.Instance.relicIconManager.PlaceSprite(tempRelic.sprite, relicIcon[i].GetComponent<Image>());
                }
                if (relicName[i] != null)
                {
                    relicName[i].text = tempRelic.name;
                }
                if (relicDescription[i] != null)
                {
                    
                    relicDescription[i].text = tempRelic.trigger.description + " " + tempRelic.effect.description;
                }
            }
            else
            {
                Debug.Log("ran out of relics they null ash");
                break;
            }
        }
    }

    public void TakeRelic(int chosenRelicIndex)
    {
        if (alreadyTookARelic == true)
        {
            Debug.Log("You have already chosen a relic! You only may pick 1 at a time.");
        }
        else
        {
            if (chosenRelicIndex < tempRelics.Count && tempRelics[chosenRelicIndex] != null)
            {
                Relic selectedRelic = tempRelics[chosenRelicIndex];
                //double checking player doesn't have it b4 it's added
                if (!player.relics.Contains(selectedRelic))
                {
                    player.relics.Add(selectedRelic);
                    selectedRelic.Activate();
                    alreadyTookARelic = true;
                    Debug.Log("player selected: " + selectedRelic.name);
                }
            }
        }
    }
}

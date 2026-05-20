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
    public List <Relic> tempRelics = new List<Relic>();
    bool allRelicsCreated = false;
    public List <RelicUI> relicUI = new List<RelicUI>();

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
                rewardUI.SetActive(false);
            break;

            case GameManager.GameState.WAVEEND:
                
                if (!isSpellCreated)
                {
                    spell = SpellBuilder.Build(player.spellcaster);
                    Debug.Log("new spell created");
                    isSpellCreated = true;

                    // update spellUI
                    spellUI.SetSpell(spell, 0);

                }

                //make smth like if wave != 0 && wave % 3 == 0 (so this stuff only shows up every 3 rounds)
                //add smth where game wont break if there aren't three diff relics to display (like it can display two or one as we begin to run out of options)
                if (!allRelicsCreated)
                {
                    tempRelics.Clear(); //make sure list is empty
                    for (int i = 0; i < 3 && i < relicUI.Count; i++) //3 relics need to be displayed to player for them to choose from
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
                        
                        if (tempRelic != null)
                        {
                            tempRelics.Add(tempRelic);
                            if (relicUI[i] != null)
                            {
                                relicUI[i].SetRelic(tempRelic);
                                relicUI[i].player = player;
                                relicUI[i].index = i;
                            }
                        }
                        else
                        {
                            Debug.Log("ran out of relics they null ash");
                            break;
                        }
                    }
                    //after it all runs, update variable
                    allRelicsCreated = true;
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
        for (i = 0; i < player.spellcaster.spells.Length - 1; i++)
        {
            if (player.spellcaster.spells[i] == null)
            {
                break;
            }
        }
        if (spell != null)
        {
            Debug.Log("accepting spell");
            player.spellcaster.spells[i] = spell; // make this a new spell
            spell = null;
            //spellUI.gameObject.SetActive(false);
        }
        
    }
}

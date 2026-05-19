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
    List <Relic> tempRelics;
    bool allRelicsCreated = false;
    public List <RelicUI> relicUI;

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
                    tempRelics.Clear(); //empty list for next round 
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
                if (!allRelicsCreated)
                {
                    for (int i = 0; i < 3; i++) //3 relics need to be displayed to player for them to choose from
                    {
                        Relic tempRelic = RelicBuilder.Build();
                        if (tempRelics.Contains(tempRelic))
                        {
                            while (tempRelics.Contains(tempRelic))
                            {
                                //retry if tempRelic alr in tempRelics (no dupes!)
                                tempRelic = RelicBuilder.Build();
                                if (!tempRelics.Contains(tempRelic))
                                {
                                    break;
                                }
                            }
                        }
                        tempRelics[i] = tempRelic;
                        relicUI[i].
                    }
                    //after it all runs, update variable
                    allRelicsCreated = true;
                    Debug.Log("all relics created");
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

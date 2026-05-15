using UnityEngine;
using UnityEngine.UI;

public class SpellUIContainer : MonoBehaviour
{
    public GameObject [] spellUIs;
    public PlayerController player;
  

    //variables i'm (clr) adding :P //these might be unnecessary idk
    //private const int MAX_ACTIVE_SPELLS = 4; //can only have 4 active spells at once according to directions
    //private int active_spells = 0; //will fluctuate, player alr has one initially (0 for indicies' sake)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spellUIs[0].SetActive(true); //player has 1 spell initially, so show it
        
        
        for(int i = 1; i < spellUIs.Length; ++i) //rest of the slots shouldn't be showing yet
        {
            spellUIs[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.spellcaster != null && player.spellcaster.spells != null) 
        {
            for (int i = 0; i < player.spellcaster.spells.Length; i++)
            {
                if (player.spellcaster.spells[i] != null)
                {
                    if (spellUIs[i] != null)
                    {
                        spellUIs[i].SetActive(true);
                        if (i == player.spellcaster.highlightedSpell)
                        {
                            spellUIs[i].GetComponent<SpellUI>().highlight.SetActive(true);
                        }
                        else
                        {
                            spellUIs[i].GetComponent<SpellUI>().highlight.SetActive(false);
                        }
                        SpellUI spellUI = spellUIs[i].GetComponent<SpellUI>();
                        if (spellUI != null)
                        {
                            spellUI.SetSpell(player.spellcaster.spells[i], i);
                        }
                    }
                    
                }
                else
                {
                    if (i < spellUIs.Length && spellUIs[i] != null)
                    {
                        spellUIs[i].SetActive(false);  
                    }
                    
                }
            }
        }

        
    }

}

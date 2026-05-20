using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicUI : MonoBehaviour
{
    public PlayerController player;
    public int index;

    public Image icon;
    public GameObject highlight;
    public TextMeshProUGUI label;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if a player has relics, this is how you *could* show them
        Relic r = player.relics[index]; //this line wont work if the relics are on rewardScreenManager, cuz then they havent been added to the player's relics yet (but idk what to do abt that rn)
        GameManager.Instance.relicIconManager.PlaceSprite(r.sprite, icon);
        
    }

    // Update is called once per frame
    void Update()
    {
        // Relics could have labels and/or an active-status
     
        Relic r = player.relics[index];
        label.text = r.GetLabel();
        highlight.SetActive(r.IsActive());
    }

    public void SetRelic(Relic relic)
    {

    }
}

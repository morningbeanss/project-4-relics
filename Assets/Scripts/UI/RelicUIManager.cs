using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicUIManager : MonoBehaviour
{
    public GameObject relicUIPrefab;
    public PlayerController player;

    //trying to make a similar setup as my setup in rewardscreenmanager (but just images) (clr)
    //list length = 7 (set up in unity editor) (7 bc we only have 7 relics so youll never need more slots than that <3)
    public List<Image> relicIcon = new List<Image>();
    public int relicIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.Instance.OnRelicPickup += OnRelicPickup;
        for (int i = 0; i < relicIcon.Count; i++)
        {
            relicIcon[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRelicPickup(Relic r)
    {
        // make a new Relic UI representation
        /*
        GameObject rui = Instantiate(relicUIPrefab, transform);
        rui.transform.localPosition = new Vector3(-450 + 40 * (player.relics.Count - 1), 0, 0);
        RelicUI ruic = rui.GetComponent<RelicUI>();
        ruic.player = player;
        ruic.index = player.relics.Count - 1;
        */
        //i dont like this code above it challenges me grrr 

        if (relicIcon[relicIndex] != null)
        {
            GameManager.Instance.relicIconManager.PlaceSprite(r.sprite, relicIcon[relicIndex].GetComponent<Image>());
            relicIcon[relicIndex].gameObject.SetActive(true);
            relicIndex++;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

//after every 3rd wave (starting w/ wave 3) 3 relics drop (show up on endWave screen)
//player chooses ONE out of the three to keep
//relics r cumulative, there's no upper limit, but NO duplicates

public static class RelicBuilder
{
    static List<Relic.RelicData> relics; //list to hold all possible relics from json
    static List<Relic.RelicData> ownedRelics = new List<Relic.RelicData>(); //list to hold all the relics player owns (so no dupes r presented)
    //make a list that temporary holds relics while they're being generated??
    //(build will be called 3 times in a row, ownedRelics shouldnt be updated during those 3 times yet
    //bc player hasnt actually selected the three. after player selects, temp one should be wiped, and selected
    //one should be officially added to ownedRelics)
    static List<Relic.RelicData> tempRandomSelected = new List<Relic.RelicData>();

    static RelicBuilder()
    {
        string relic_json = Resources.Load<TextAsset>("relics").text; //read json file
        relics = JsonConvert.DeserializeObject<List<Relic.RelicData>>(relic_json);
    }

    public static Relic Build() //returns randomly selected relic from relics list; no dupes
    {
        //this generates a list of all possible relics that the player DOESN'T own
        List<Relic.RelicData> availableRelics = relics.FindAll(r => !tempRandomSelected.Contains(r));
        if (availableRelics.Count == 0)
        {
            Debug.Log("out of relics! (player owns all of them!)");
            return null;
        }

        //randomly choose a relic from the available ones
        int randomIndex = Random.Range(0, availableRelics.Count);
        Relic.RelicData selected = availableRelics[randomIndex];
        //add it to temp holding variable
        tempRandomSelected.Add(selected);

        //return new relic
        return new Relic(selected);
    }

    //this is similar to the one above, but in case we want to test specific relics n not a randomly selected one
    public static Relic BuildSpecific(int index)
    {
        if (index >= 0 && index < relics.Count)
        {
            return new Relic(relics[index]);
        }
        else
        {
            Debug.Log("invalid relic loc index");
            return null; //return nothing if int invalid
        }
    }

    //this to add relic to owned relic, and to wipe temporary holder (void bc i dont think a return is necessary)
    // !!!! call this wherever the button selection logic is !!!!
    public static void PlayerSelectedARelic(Relic selectedRelic)
    {
        //get the RelicData from the Relic
        Relic.RelicData selectedData = relics.Find(r => r.name == selectedRelic.name);

        //add to owned relics (w/ if-statement for safety)
        if (selectedData.name != null)
        {
            ownedRelics.Add(selectedData);
            Debug.Log("Relic " + selectedData.name + " successfully added to player's ownedRelics");
        }

        //clear random selection
        tempRandomSelected.Clear();
    }
}
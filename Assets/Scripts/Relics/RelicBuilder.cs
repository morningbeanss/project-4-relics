using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
public static class RelicBuilder
{
    static List<Relic.RelicData> relics;
    static RelicBuilder()
    {
        string relic_json = Resources.Load<TextAsset>("relics").text; //read json file
        relics = JsonConvert.DeserializeObject<List<Relic.RelicData>>(relic_json);
    }

    public static Relic Build()
    {
        return new Relic(relics[0]);
    }
}
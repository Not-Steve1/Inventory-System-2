using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemClass {

    public string itemType; //make array of type names later
    public int level;
    [Range(0, 3)]
    public int quality;
    public IntVector2 size;

    public string QualityIntToString()
    {
        switch (quality)
        {
            case 0: return "Broken";
            case 1: return "Normal";
            case 2: return "Modded";
            case 3: return "Rare";
            default: return null;
        }
    }
}

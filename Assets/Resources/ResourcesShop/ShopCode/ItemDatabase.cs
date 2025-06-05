using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    [Header("Assign your ItemData assets here")]
    public List<ItemData> weapons = new List<ItemData>();
    public List<ItemData> potions = new List<ItemData>();
    public List<ItemData> armors = new List<ItemData>();
    public List<ItemData> trinkets = new List<ItemData>();
    public List<ItemData> materials = new List<ItemData>();

    // Optional: get all items combined
    public List<ItemData> GetAllItems()
    {
        List<ItemData> all = new List<ItemData>();
        all.AddRange(weapons);
        all.AddRange(potions);
        all.AddRange(armors);
        all.AddRange(trinkets);
        all.AddRange(materials);
        return all;
    }
}

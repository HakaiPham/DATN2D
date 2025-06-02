using System.Collections.Generic;

[System.Serializable]
public class InventorySaveData
{
    public string itemName;
    public int amount;
    public ItemType type;
}

[System.Serializable]
public class InventorySaveWrapper
{
    public List<InventorySaveData> items;
}

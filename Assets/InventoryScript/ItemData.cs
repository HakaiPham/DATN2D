using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum ItemType { Weapon, Potion, Armor, Trinket, Material }

public enum Rarity
{
    Common = 1,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public bool isStackable;
    public Rarity rarity; // Applies to Weapon, Armor, Trinket only

    [Header("Extra Info")]
    public string category; // Used for legacy or display logic
    public int dropRate = 0;

    [Header("Attributes")]
    public List<ItemAttribute> attributes = new List<ItemAttribute>();

    public string GetAttributesDescription()
    {
        if (attributes == null || attributes.Count == 0)
            return "No attributes";

        StringBuilder sb = new StringBuilder();

        foreach (var attr in attributes)
        {
            sb.AppendLine(attr.GetDescription());
        }

        return sb.ToString();
    }
}

[System.Serializable]
public class ItemAttribute
{
    public enum AttributeType
    {
        Attack,
        Defense,
        Speed,
        HP,
        CriticalRate
    }

    public AttributeType type;
    public float value;
    public bool isPercent = false;

    public string GetDescription()
    {
        string percentSign = isPercent ? "%" : "";
        return $"+{value}{percentSign} {type}";
    }
}

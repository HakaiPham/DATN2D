using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public ItemType type;
}

public enum ItemType
{
    Weapon,
    Potion,
    Armor,
    Trinket,
    Material
}

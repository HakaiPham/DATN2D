using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public ItemType acceptedType;
    public InventoryItem currentItem;

    public bool CanAcceptItem(ItemData item)
    {
        return item.type == acceptedType;
    }

    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        // Update UI here if needed
    }

    public void ClearSlot()
    {
        currentItem = null;
        // Clear UI visuals
    }
}

[System.Serializable]
public class InventoryItem
{
    public ItemData data;
    public int amount;

    public InventoryItem(ItemData data, int amount = 1)
    {
        this.data = data;
        this.amount = amount;
    }
}

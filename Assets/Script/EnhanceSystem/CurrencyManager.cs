using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int gold = 500; // Số vàng ban đầu

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"Đã trừ {amount} vàng. Vàng còn lại: {gold}");
            return true;
        }
        Debug.Log("Không đủ vàng!");
        return false;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Đã cộng {amount} vàng. Vàng hiện tại: {gold}");
    }
}

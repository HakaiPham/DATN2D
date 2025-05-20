using System.Collections.Generic;
using UnityEngine;

public enum UpgradeResult
{
    Success,
    MaxLevel,
    NotEnoughGold,
    NotEnoughMaterials
}

public class UpgradeManager : MonoBehaviour
{
    // Tham chiếu tới 2 manager tài nguyên
    public CurrencyManager currencyManager;
    public MaterialManager materialManager;

    // Lưu trữ cấp độ của các vật phẩm theo tên
    public Dictionary<string, int> itemLevels = new Dictionary<string, int>();

    // Cấp tối đa
    public int maxLevel = 5;

    // Chi phí nâng cấp
    public int goldCostPerLevel = 100;
    public Dictionary<string, int> requiredMaterialsPerLevel = new Dictionary<string, int>()
    {
        {"IronOre", 2}
    };

    public UpgradeResult UpgradeItem(string itemName)
    {
        if (!itemLevels.ContainsKey(itemName))
            itemLevels[itemName] = 0; // Khởi tạo cấp 0

        int currentLevel = itemLevels[itemName];

        if (currentLevel >= maxLevel)
        {
            Debug.Log($"{itemName} đã đạt cấp tối đa!");
            return UpgradeResult.MaxLevel;
        }

        // Kiểm tra tiền
        if (!currencyManager || !currencyManager.SpendGold(goldCostPerLevel))
        {
            Debug.Log("Không đủ vàng để nâng cấp");
            return UpgradeResult.NotEnoughGold;
        }

        // Kiểm tra vật liệu
        foreach (var mat in requiredMaterialsPerLevel)
        {
            if (!materialManager || !materialManager.HasMaterials(mat.Key, mat.Value))
            {
                Debug.Log($"Không đủ vật liệu {mat.Key} để nâng cấp");
                // Hoàn lại tiền vàng nếu không đủ vật liệu
                if (currencyManager)
                    currencyManager.AddGold(goldCostPerLevel);
                return UpgradeResult.NotEnoughMaterials;
            }
        }

        // Trừ vật liệu
        foreach (var mat in requiredMaterialsPerLevel)
        {
            materialManager.SpendMaterial(mat.Key, mat.Value);
        }

        // Nâng cấp
        itemLevels[itemName] = currentLevel + 1;
        Debug.Log($"{itemName} được nâng lên cấp {itemLevels[itemName]}");
        return UpgradeResult.Success;
    }

    public int GetItemLevel(string itemName)
    {
        return itemLevels.ContainsKey(itemName) ? itemLevels[itemName] : 0;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private List<Item> inventory = new List<Item>();

    public InventoryDisplay inventoryDisplay;
    public GameObject inventoryCanvas; // ← Thêm Canvas vào đây

    private bool isInventoryOpen = false;

    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Không thể thêm item null.");
            return false;
        }

        string categoryLower = item.category.ToLower();

        if (categoryLower.Contains("vu khi"))
        {
            inventory.Add(item);
            Debug.Log("Đã thêm vũ khí (cho phép trùng): " + item.itemName);
            UpdateDisplay();
            return true;
        }
        else if (categoryLower.Contains("thuc pham"))
        {
            Item existing = inventory.Find(i => i.itemName == item.itemName && i.category == item.category);
            if (existing != null)
            {
                existing.quantity += 1;
                Debug.Log("Đã cộng số lượng thức phẩm: " + existing.itemName + ", số lượng: " + existing.quantity);
            }
            else
            {
                Item newItem = item.Clone();
                newItem.quantity = 1;
                inventory.Add(newItem);
                Debug.Log("Đã thêm thức phẩm mới: " + newItem.itemName);
            }
            UpdateDisplay();
            return true;
        }
        else
        {
            bool exists = inventory.Exists(i => i.itemName == item.itemName && i.category == item.category);
            if (!exists)
            {
                inventory.Add(item);
                Debug.Log("Đã thêm item: " + item.itemName);
                UpdateDisplay();
                return true;
            }
            else
            {
                Debug.Log("Item đã tồn tại: " + item.itemName);
                return false;
            }
        }
    }

    public int GetQuantity(string itemName)
    {
        Item existingItem = inventory.FirstOrDefault(i => i.itemName == itemName);
        return existingItem != null ? existingItem.quantity : 0;
    }

    private void UpdateDisplay()
    {
        if (inventoryDisplay != null)
        {
            inventoryDisplay.ShowInventory(inventory);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isInventoryOpen = !isInventoryOpen;

            if (inventoryCanvas != null)
            {
                inventoryCanvas.SetActive(isInventoryOpen);
                if (isInventoryOpen)
                {
                    UpdateDisplay(); // Cập nhật khi mở
                }
            }
        }
    }
}

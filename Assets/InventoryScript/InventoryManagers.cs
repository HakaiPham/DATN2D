using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManagers : MonoBehaviour
{
    public List<InventorySlot> slots;
    public List<InventorySlotUI> inventorySlotUIs;
    public GameObject inventoryCanvas;

    private bool isInventoryOpen = false;

    public void AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Không thể thêm item null.");
            return;
        }

        foreach (var slot in slots)
        {
            if (slot.CanAcceptItem(itemData) && slot.currentItem != null && slot.currentItem.data == itemData && itemData.isStackable)
            {
                slot.currentItem.amount += amount;
                UpdateDisplay();
                return;
            }
        }

        foreach (var slot in slots)
        {
            if (slot.CanAcceptItem(itemData) && slot.currentItem == null)
            {
                slot.SetItem(new InventoryItem(itemData, amount));
                UpdateDisplay();
                return;
            }
        }

        Debug.Log("Inventory full or no valid slot.");
    }

    public void MoveItem(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (fromSlot == toSlot || fromSlot.currentItem == null) return;

        if (!toSlot.CanAcceptItem(fromSlot.currentItem.data)) return;

        if (toSlot.currentItem != null && toSlot.currentItem.data == fromSlot.currentItem.data && fromSlot.currentItem.data.isStackable)
        {
            toSlot.currentItem.amount += fromSlot.currentItem.amount;
            fromSlot.ClearSlot();
        }
        else if (toSlot.currentItem == null)
        {
            toSlot.SetItem(fromSlot.currentItem);
            fromSlot.ClearSlot();
        }
        UpdateDisplay();
    }

    public string SaveInventory()
    {
        List<InventorySaveData> saveData = new();
        foreach (var slot in slots)
        {
            if (slot.currentItem != null)
            {
                saveData.Add(new InventorySaveData
                {
                    itemName = slot.currentItem.data.itemName,
                    amount = slot.currentItem.amount,
                    type = slot.currentItem.data.type
                });
            }
            else
            {
                saveData.Add(null);
            }
        }

        string json = JsonUtility.ToJson(new InventorySaveWrapper { items = saveData });
        PlayerPrefs.SetString("inventory", json);
        return json;
    }

    public void LoadInventory(List<ItemData> allItems)
    {
        string json = PlayerPrefs.GetString("inventory", "");
        if (string.IsNullOrEmpty(json)) return;

        var loadedData = JsonUtility.FromJson<InventorySaveWrapper>(json);
        for (int i = 0; i < loadedData.items.Count; i++)
        {
            var data = loadedData.items[i];
            if (data != null)
            {
                var itemAsset = allItems.Find(x => x.itemName == data.itemName);
                if (itemAsset != null)
                    slots[i].SetItem(new InventoryItem(itemAsset, data.amount));
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
        UpdateDisplay();
    }

    public int GetQuantity(string itemName)
    {
        foreach (var slot in slots)
        {
            if (slot.currentItem != null && slot.currentItem.data.itemName == itemName)
            {
                return slot.currentItem.amount;
            }
        }
        return 0;
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventorySlotUIs.Count)
            {
                inventorySlotUIs[i].SetItem(slots[i].currentItem);
            }
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
                    UpdateDisplay();
                }
            }
        }
    }
}
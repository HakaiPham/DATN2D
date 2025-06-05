using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrefightInventoryManager : MonoBehaviour
{
    public GameObject prefightSlotPrefab;
    public Transform prefightPanel;
    public Text warningText;

    private List<InventorySlotUI> prefightSlots = new();

    public void TryAddItem(InventorySlotUI draggedItemUI)
    {
        InventoryItem item = draggedItemUI.currentItem;
        if (item == null || item.data.type != ItemType.Potion)
        {
            ShowMessage("Only poison items can be added to prefight inventory!");
            draggedItemUI.transform.SetParent(draggedItemUI.originalParent);
            draggedItemUI.transform.localPosition = Vector3.zero;
            return;
        }

        // Check total count
        int totalItems = 0;
        foreach (var slot in prefightSlots)
            if (slot.currentItem != null)
                totalItems += slot.currentItem.amount;

        if (totalItems >= 100 || prefightSlots.Count >= 10)
        {
            ShowMessage("Prefight inventory is full (Max 10 slots, 10 items each)!");
            draggedItemUI.transform.SetParent(draggedItemUI.originalParent);
            draggedItemUI.transform.localPosition = Vector3.zero;
            return;
        }

        // Look for a slot with same item
        foreach (var slot in prefightSlots)
        {
            if (slot.currentItem != null && slot.currentItem.data == item.data)
            {
                int roomLeft = 10 - slot.currentItem.amount;
                if (roomLeft > 0)
                {
                    int toMove = Mathf.Min(item.amount, roomLeft);
                    slot.currentItem.amount += toMove;
                    item.amount -= toMove;

                    if (item.amount <= 0)
                    {
                        Destroy(draggedItemUI.gameObject);
                        return;
                    }

                    ShowMessage("Partial transfer. Some items left in inventory.");
                    draggedItemUI.SetItem(item);
                    draggedItemUI.transform.SetParent(draggedItemUI.originalParent);
                    draggedItemUI.transform.localPosition = Vector3.zero;
                    return;
                }
            }
        }

        // Else, add new slot if room
        if (prefightSlots.Count < 10)
        {
            GameObject slotGO = Instantiate(prefightSlotPrefab, prefightPanel);
            InventorySlotUI newSlot = slotGO.GetComponent<InventorySlotUI>();
            newSlot.SetItem(new InventoryItem(item.data, Mathf.Min(10, item.amount)));
            item.amount -= 10;
            prefightSlots.Add(newSlot);

            if (item.amount <= 0)
                Destroy(draggedItemUI.gameObject);
            else
            {
                draggedItemUI.SetItem(item);
                draggedItemUI.transform.SetParent(draggedItemUI.originalParent);
                draggedItemUI.transform.localPosition = Vector3.zero;
            }

            return;
        }

        // Inventory full
        ShowMessage("No space for more items!");
        draggedItemUI.transform.SetParent(draggedItemUI.originalParent);
        draggedItemUI.transform.localPosition = Vector3.zero;
    }

    private void ShowMessage(string msg)
    {
        warningText.text = msg;
        warningText.gameObject.SetActive(true);
        CancelInvoke("HideMessage");
        Invoke("HideMessage", 2f);
    }

    private void HideMessage()
    {
        warningText.gameObject.SetActive(false);
    }
}

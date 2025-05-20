using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IDropHandler
{
    public string currentItemName;
    public UpgradeUI upgradeUI; // Tham chiếu tới UpgradeUI để cập nhật UI

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem droppedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (droppedItem != null)
        {
            droppedItem.parentToReturnTo = transform;
            droppedItem.transform.SetParent(transform);
            droppedItem.transform.localPosition = Vector3.zero;

            currentItemName = droppedItem.itemName;
            Debug.Log("Item đã được đặt vào slot nâng cấp: " + currentItemName);

            // Cập nhật tên item cho UI
            if (upgradeUI != null)
            {
                upgradeUI.SetItemName(currentItemName);
            }
        }
    }

    public bool HasItem()
    {
        return !string.IsNullOrEmpty(currentItemName);
    }

    public void ClearSlot()
    {
        currentItemName = null;
    }
}

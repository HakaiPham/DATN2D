using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeItemSlot : MonoBehaviour, IDropHandler
{
    public string currentItemName;
    public UpgradeUI upgradeUI;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem droppedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (droppedItem != null)
        {
            // Cập nhật nơi item sẽ quay lại nếu không thả đúng
            droppedItem.parentToReturnTo = transform;

            // Gán lại parent của item
            droppedItem.transform.SetParent(transform);
            droppedItem.transform.localPosition = Vector3.zero;

            // Lưu tên item
            currentItemName = droppedItem.itemName;

            Debug.Log("Item đã được đặt vào slot nâng cấp: " + currentItemName);

            // Cập nhật UI
            upgradeUI?.SetItemName(currentItemName);
        }
    }

    public void ClearSlot()
    {
        currentItemName = null;
        upgradeUI?.SetItemName("");

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out DraggableItem item))
            {
                if (item.originalParent != null)
                {
                    child.SetParent(item.originalParent);
                    child.localPosition = Vector3.zero;
                    item.parentToReturnTo = item.originalParent; // Cập nhật lại điểm về
                }
                else
                {
                    child.SetParent(transform.root);
                    child.localPosition = Vector3.zero;
                }
            }
        }
    }

}

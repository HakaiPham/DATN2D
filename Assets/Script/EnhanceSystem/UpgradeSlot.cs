using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IDropHandler
{
    public string currentItemName;
    public UpgradeUI upgradeUI;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem droppedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (droppedItem != null)
        {
            droppedItem.parentToReturnTo = transform;
            droppedItem.transform.SetParent(transform);
            droppedItem.transform.localPosition = Vector3.zero;

            currentItemName = droppedItem.itemName;
            Debug.Log("Item đã được đặt vào slot nâng cấp: " + currentItemName);

            if (upgradeUI != null)
                upgradeUI.SetItemName(currentItemName);
        }
    }

    public void ClearSlot()
    {
        currentItemName = null;

        // Xóa UI liên quan
        if (upgradeUI != null)
        {
            upgradeUI.SetItemName(""); // Reset tên trong UI
        }

        // Xóa item game object khỏi slot nếu nó còn nằm trong slot
        foreach (Transform child in transform)
        {
            if (child.GetComponent<DraggableItem>() != null)
            {
                // Nếu item này hiện vẫn là con của slot thì mới destroy
                if (child.parent == transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }

}

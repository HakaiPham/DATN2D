using UnityEngine;
using UnityEngine.EventSystems;

public class MergeSlot : MonoBehaviour, IDropHandler
{
    public string currentItemName;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem item = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (item != null)
        {
            currentItemName = item.itemName;
            item.parentToReturnTo = transform;
            item.transform.SetParent(transform);
            item.transform.localPosition = Vector3.zero;
        }
    }

    public void ClearSlot()
    {
        currentItemName = "";
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}

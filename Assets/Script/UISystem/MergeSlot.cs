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
            item.parentToReturnTo = transform;

            item.transform.SetParent(transform);
            item.transform.localPosition = Vector3.zero;

            currentItemName = item.itemName;
        }
    }

    public void ClearSlot()
    {
        currentItemName = "";

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out DraggableItem item))
            {
                if (item.originalParent != null)
                {
                    child.SetParent(item.originalParent);
                    child.localPosition = Vector3.zero;
                    item.parentToReturnTo = item.originalParent;
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

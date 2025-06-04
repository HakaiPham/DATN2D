using UnityEngine;
using UnityEngine.EventSystems;

public class Dragslot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragableItems dragableItem = dropped.GetComponent<DragableItems>();
        dragableItem.parentAfterDrag = transform;
    }

    
}

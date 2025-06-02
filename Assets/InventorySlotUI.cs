using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public InventoryItem currentItem;
    public Transform originalParent;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        icon.sprite = item.data.icon;
        icon.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (eventData.pointerEnter == null || !eventData.pointerEnter.GetComponent<InventorySlotUI>())
        {
            // Dragged outside UI or invalid target
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            // Optional: handle valid drop between slots
        }
    }
}

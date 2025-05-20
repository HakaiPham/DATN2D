using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemName;

    public Transform parentToReturnTo = null;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = transform.parent;
        transform.SetParent(canvas.transform); // Đưa lên top canvas
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentToReturnTo); // Trả về vị trí ban đầu nếu không thả đúng
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}

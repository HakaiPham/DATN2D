using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemName;

    public Transform parentToReturnTo = null;
    public Transform originalParent = null; // Lưu chỗ gốc ban đầu (ItemHolder)

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (originalParent == null)
            originalParent = transform.parent; // Lưu nơi item được spawn ban đầu (ItemHolder)

        parentToReturnTo = transform.parent;

        transform.SetParent(canvas.transform); // Đưa lên top
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentToReturnTo);
        transform.localPosition = Vector3.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Nếu không drop vào slot mới → trả lại nơi ban đầu
        if (transform.parent == canvas.transform)
        {
            if (originalParent != null)
            {
                transform.SetParent(originalParent);
                parentToReturnTo = originalParent;
                transform.localPosition = Vector3.zero;
            }
        }
    }
}

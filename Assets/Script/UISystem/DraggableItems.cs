using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemName;

    public Transform parentToReturnTo = null;
    private Canvas canvas;
    private Transform originalParent;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        parentToReturnTo = transform.parent; // Lưu lại nơi bắt đầu kéo
        transform.SetParent(canvas.transform); // Đưa lên top canvas
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentToReturnTo); // Trả về nếu không thả đúng chỗ
        transform.localPosition = Vector3.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Nếu thả ra ngoài slot (parent không thay đổi → chưa bị xử lý bởi OnDrop)
        if (transform.parent == originalParent)
        {
            // Nếu original parent là UpgradeSlot, Clear nó
            UpgradeSlot upgradeSlot = originalParent.GetComponent<UpgradeSlot>();
            if (upgradeSlot != null)
            {
                upgradeSlot.ClearSlot();
            }

            // Nếu là MergeSlot
            MergeSlot mergeSlot = originalParent.GetComponent<MergeSlot>();
            if (mergeSlot != null)
            {
                mergeSlot.ClearSlot();
            }
        }
    }
}

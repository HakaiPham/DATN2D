using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText;

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

        if (item == null)
        {
            icon.enabled = false;
            nameText.text = "";
            countText.text = "";
            return;
        }

        icon.sprite = item.data.icon;
        icon.enabled = true;

        nameText.text = item.data.itemName;
        countText.text = item.amount > 1 ? $"x{item.amount}" : "";

        nameText.color = GetColorByRarity(item.data);
    }

    private Color GetColorByRarity(ItemData data)
    {
        if (data.type != ItemType.Weapon && data.type != ItemType.Armor && data.type != ItemType.Trinket)
            return Color.white;

        switch (data.rarity)
        {
            case Rarity.Common: return Color.white;
            case Rarity.Uncommon: return new Color(0.2f, 1f, 0.2f);
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return new Color(0.6f, 0.2f, 1f);
            case Rarity.Legendary: return Color.yellow;
            default: return Color.gray;
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.enabled = false;
        nameText.text = "";
        countText.text = "";
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
            // Drop fail: return to original slot
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            // Optional: successful drop, move item
        }
    }
}

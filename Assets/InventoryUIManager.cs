using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;
    public GameObject weaponPage, poisonPage, armorPage, trinketPage, materialPage;
    public Button weaponBtn, poisonBtn, armorBtn, trinketBtn, materialBtn, sortBtn;

    private Dictionary<ItemType, GameObject> pages = new();
    private Dictionary<ItemType, List<InventorySlotUI>> slotLists = new();
    private ItemType currentPage;

    private void Start()
    {
        // Link pages
        pages[ItemType.Weapon] = weaponPage;
        pages[ItemType.Potion] = poisonPage;
        pages[ItemType.Armor] = armorPage;
        pages[ItemType.Trinket] = trinketPage;
        pages[ItemType.Material] = materialPage;

        // Setup buttons
        weaponBtn.onClick.AddListener(() => SwitchPage(ItemType.Weapon));
        poisonBtn.onClick.AddListener(() => SwitchPage(ItemType.Potion));
        armorBtn.onClick.AddListener(() => SwitchPage(ItemType.Armor));
        trinketBtn.onClick.AddListener(() => SwitchPage(ItemType.Trinket));
        materialBtn.onClick.AddListener(() => SwitchPage(ItemType.Material));
        sortBtn.onClick.AddListener(SortCurrentPage);

        foreach (var key in pages.Keys)
            slotLists[key] = new List<InventorySlotUI>();

        SwitchPage(ItemType.Weapon); // default
    }

    public void AddItemToUI(InventoryItem item)
    {
        GameObject page = pages[item.data.type];
        GameObject newSlot = Instantiate(slotPrefab, page.transform);
        InventorySlotUI ui = newSlot.GetComponent<InventorySlotUI>();
        ui.SetItem(item);

        slotLists[item.data.type].Add(ui);
    }

    private void SwitchPage(ItemType pageType)
    {
        foreach (var kvp in pages)
            kvp.Value.SetActive(kvp.Key == pageType);

        currentPage = pageType;
    }

    private void SortCurrentPage()
    {
        var slots = slotLists[currentPage];

        // Clean nulls
        slots = slots.Where(s => s != null && s.currentItem != null).ToList();

        // Sort items alphabetically by itemName
        slots = slots.OrderBy(s => s.currentItem.data.itemName).ToList();

        // Clear children and re-add
        foreach (Transform child in pages[currentPage].transform)
            Destroy(child.gameObject);

        slotLists[currentPage].Clear();

        foreach (var slot in slots)
        {
            GameObject newSlot = Instantiate(slotPrefab, pages[currentPage].transform);
            InventorySlotUI ui = newSlot.GetComponent<InventorySlotUI>();
            ui.SetItem(slot.currentItem);
            slotLists[currentPage].Add(ui);
        }
    }
}

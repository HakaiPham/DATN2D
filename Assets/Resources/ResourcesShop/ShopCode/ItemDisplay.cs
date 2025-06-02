using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

[System.Serializable]
public class ItemSlotUI
{
    public Button button;                   // Nút hiển thị icon item
    public TextMeshProUGUI nameText;        // TextMeshPro hiển thị tên item
    public TextMeshProUGUI quantityText;    // TextMeshPro hiển thị số lượng
    public Image iconImage;                 // Image để hiển thị sprite item
}

public class ItemDisplay : MonoBehaviour
{
    [Header("Data Source")]
    public ItemDatabase itemDatabase;            // Tham chiếu đến ItemDatabase (chứa list<Item>)

    [Header("Slot UI List")]
    public List<ItemSlotUI> itemSlots;           // Danh sách slot UI (kéo thả từ Inspector)
    public Button refreshButton;                 // Button "Làm mới"
    public Button buyButton;                     // Button "Mua"

    [Header("Inventory Manager (để lấy số lượng)")]
    public InventoryManager inventoryManager;    // Script quản lý túi đồ (bạn gán trong Inspector)

    private List<Item> currentItems = new List<Item>();
    private Item selectedItem = null;

    // Lưu trạng thái đã mua cho từng slot
    private Dictionary<int, bool> itemPurchased = new Dictionary<int, bool>();

    [Header("Item Detail UI (Panel)")]
    public GameObject itemDetailPanel;               // Panel chứa chi tiết item (mặc định SetActive(false))
    public Image itemDetailImage;                    // Ảnh item trong panel chi tiết
    public TextMeshProUGUI itemDetailNameText;       // Tên item (có đổi màu tùy rate)
    public TextMeshProUGUI itemDetailCategoryText;   // Thể loại
    public TextMeshProUGUI itemDetailAttributesText; // Chuỗi thuộc tính chi tiết
    public Button closeDetailButton;                 // Nút đóng panel chi tiết
    public Button backgroundCloseButton;             // Nút/Vùng mờ phía sau panel để bấm ngoài panel cũng đóng được

    private void Start()
    {
        // Gán sự kiện cho các Button ở trên
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RandomizeItems);

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuySelectedItem);
            buyButton.interactable = false;
        }

        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(HideItemDetail);

        if (backgroundCloseButton != null)
            backgroundCloseButton.onClick.AddListener(HideItemDetail);

        // Ẩn panel chi tiết item lúc đầu
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);

        // Khởi tạo trạng thái đã mua
        ResetPurchaseStates();

        // Khởi tạo hiển thị
        RandomizeItems();
    }

    /// <summary>
    /// Reset trạng thái mua của tất cả slot
    /// </summary>
    private void ResetPurchaseStates()
    {
        itemPurchased.Clear();
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemPurchased[i] = false;
        }
    }

    /// <summary>
    /// Lấy ngẫu nhiên một số item (theo rate) từ itemDatabase và hiển thị ra itemSlots
    /// </summary>
    public void RandomizeItems()
    {
        // Reset trạng thái mua để có thể mua lại
        ResetPurchaseStates();

        // Lấy danh sách nguồn ban đầu từ ItemDatabase
        List<Item> sourceItems = new List<Item>(itemDatabase.weapons);

        currentItems.Clear();

        // Số lượng item sẽ hiển thị = số lượng slot
        int numberOfItemsToShow = itemSlots.Count;

        // Lặp chọn random theo weight dựa trên rate
        for (int i = 0; i < numberOfItemsToShow; i++)
        {
            Item selected = GetRandomItemByRate(sourceItems);
            if (selected != null)
                currentItems.Add(selected);
        }

        selectedItem = null;
        UpdateBuyButton();
        DisplayItems();
    }

    /// <summary>
    /// Chọn ngẫu nhiên 1 item dựa trên trường rate (1..5) và weight
    /// </summary>
    private Item GetRandomItemByRate(List<Item> items)
    {
        // Tạo danh sách (item, weight) dựa trên rate
        List<(Item item, float weight)> weightedList = new List<(Item, float)>();

        foreach (var item in items)
        {
            float weight;
            switch (item.rate)
            {
                case 1: weight = 0.7f; break;   // 70%
                case 2: weight = 0.5f; break;   // 50%
                case 3: weight = 0.25f; break;  // 25%
                case 4: weight = 0.1f; break;   // 10%
                case 5: weight = 0.05f; break;  // 5%
                default: weight = 0f; break;
            }

            if (weight > 0)
                weightedList.Add((item, weight));
        }

        if (weightedList.Count == 0)
            return null;

        // Tính tổng trọng số
        float totalWeight = weightedList.Sum(w => w.weight);
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var (item, weight) in weightedList)
        {
            cumulative += weight;
            if (randomValue <= cumulative)
                return item.Clone(); // Clone để không sửa chính đối tượng gốc
        }

        return null;
    }

    /// <summary>
    /// Hiển thị danh sách currentItems lên từng slot UI
    /// </summary>
    private void DisplayItems()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < currentItems.Count)
            {
                Item item = currentItems[i];

                // 1. Hiển thị hình ảnh (nếu bạn đã thêm iconImage vào ItemSlotUI)
                if (itemSlots[i].iconImage != null && item.image != null)
                {
                    itemSlots[i].iconImage.sprite = item.image;
                    itemSlots[i].iconImage.gameObject.SetActive(true);
                }

                // 2. Hiển thị tên và đổi màu theo rate
                if (itemSlots[i].nameText != null)
                {
                    itemSlots[i].nameText.text = item.itemName;
                    itemSlots[i].nameText.color = GetColorByRate(item.rate);
                    itemSlots[i].nameText.gameObject.SetActive(true);
                }

                // 3. Hiển thị số lượng
                if (itemSlots[i].quantityText != null)
                {
                    bool purchased = itemPurchased.ContainsKey(i) && itemPurchased[i];
                    int quantityToShow = purchased ? 0 : 1; // Nếu đã mua thì = 0, chưa mua thì = 1
                    //itemSlots[i].quantityText.text = quantityToShow > 0 ? $"x{quantityToShow}" : ""; // Sau khi mua sẽ ẩn số lượng
                    itemSlots[i].quantityText.text = $"x{quantityToShow}"; // Sau khi mua số lượng sẽ hiển thị 0

                    itemSlots[i].quantityText.gameObject.SetActive(true);
                }

                // 5. Bật nút và gán sự kiện
                itemSlots[i].button.gameObject.SetActive(true);
                int index = i; // cần lưu lại index cho closure
                itemSlots[i].button.onClick.RemoveAllListeners();
                itemSlots[i].button.onClick.AddListener(() => OnItemClicked(index));
            }
            else
            {
                // Nếu không có item ở vị trí này thì ẩn slot
                itemSlots[i].button.gameObject.SetActive(false);

                if (itemSlots[i].nameText != null)
                    itemSlots[i].nameText.gameObject.SetActive(false);

                if (itemSlots[i].quantityText != null)
                    itemSlots[i].quantityText.gameObject.SetActive(false);

                if (itemSlots[i].iconImage != null)
                    itemSlots[i].iconImage.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Khi click vào một item slot, chọn item đó và hiển thị menu chi tiết
    /// </summary>
    private void OnItemClicked(int index)
    {
        if (index >= 0 && index < currentItems.Count)
        {
            selectedItem = currentItems[index];
            Debug.Log("Selected: " + selectedItem.itemName);
            UpdateBuyButton();

            // Nếu item đã mua thì disable nút mua
            if (itemPurchased.ContainsKey(index) && itemPurchased[index])
                buyButton.interactable = false;
            else
                buyButton.interactable = true;

            ShowItemDetail(selectedItem);
        }
    }

    /// <summary>
    /// Cập nhật trạng thái bật/tắt nút Mua
    /// </summary>
    private void UpdateBuyButton()
    {
        if (buyButton != null)
            buyButton.interactable = (selectedItem != null);
    }

    /// <summary>
    /// Khi bấm nút "Mua", sẽ thêm selectedItem vào InventoryManager rồi tắt menu chi tiết
    /// </summary>
    private void BuySelectedItem()
    {
        if (selectedItem != null && inventoryManager != null)
        {
            // Tìm index của item trong currentItems
            int index = currentItems.IndexOf(selectedItem);
            if (index < 0)
                return;

            // Nếu chưa mua thì mua
            if (!itemPurchased.ContainsKey(index) || !itemPurchased[index])
            {
                bool added = inventoryManager.AddItem(selectedItem.Clone());
                if (added)
                {
                    Debug.Log($"Đã mua: {selectedItem.itemName}");
                    itemPurchased[index] = true;
                    buyButton.interactable = false;

                    // Cập nhật số lượng hiển thị
                    DisplayItems();
                }
                else
                {
                    Debug.Log($"Vật phẩm đã có trong túi: {selectedItem.itemName}");
                }
            }
            else
            {
                Debug.Log("Vật phẩm này đã được mua rồi!");
            }
        }
        HideItemDetail();
    }

    /// <summary>
    /// Hiển thị panel chi tiết item (ảnh, tên, màu, thể loại, thuộc tính)
    /// </summary>
    private void ShowItemDetail(Item item)
    {
        if (itemDetailPanel == null) return;

        itemDetailPanel.SetActive(true);

        if (itemDetailImage != null)
            itemDetailImage.sprite = item.image;

        if (itemDetailNameText != null)
        {
            itemDetailNameText.text = item.itemName;
            itemDetailNameText.color = GetColorByRate(item.rate);
        }

        if (itemDetailCategoryText != null)
            itemDetailCategoryText.text = $"Thể loại: {item.category}";

        if (itemDetailAttributesText != null)
            itemDetailAttributesText.text = item.GetAttributesDescription();
    }

    /// <summary>
    /// Ẩn panel chi tiết
    /// </summary>
    private void HideItemDetail()
    {
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);
    }

    /// <summary>
    /// Trả về màu tùy theo độ hiếm (rate)
    /// </summary>
    private Color GetColorByRate(int rate)
    {
        switch (rate)
        {
            case 1: return Color.white;
            case 2: return Color.green;
            case 3: return Color.cyan;
            case 4: return new Color(0.6f, 0.2f, 0.8f); // Tím
            case 5: return Color.yellow;
            default: return Color.gray;
        }
    }
}

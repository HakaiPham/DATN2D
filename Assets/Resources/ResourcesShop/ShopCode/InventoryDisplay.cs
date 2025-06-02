using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    public Transform contentParent;
    public GameObject itemSlotPrefab; // Prefab mặc định có 2 dòng: tên + số lượng
    public GameObject itemSlotPrefabWeapon; // Prefab chỉ hiển thị tên (dành cho vũ khí)

    private List<GameObject> spawnedSlots = new List<GameObject>();

    public void ShowInventory(List<Item> inventoryItems)
    {
        // Xóa các ô cũ
        foreach (GameObject go in spawnedSlots)
        {
            Destroy(go);
        }
        spawnedSlots.Clear();

        foreach (Item item in inventoryItems)
        {
            // Dùng prefab khác nếu là vũ khí
            bool isWeapon = item.category.ToLower() == "vu khi";
            GameObject prefabToUse = isWeapon ? itemSlotPrefabWeapon : itemSlotPrefab;

            GameObject newSlot = Instantiate(prefabToUse, contentParent);

            // Cập nhật hình ảnh
            Image img = newSlot.GetComponent<Image>();
            if (img != null)
                img.sprite = item.image;

            // Cập nhật text
            TextMeshProUGUI[] texts = newSlot.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 1)
            {
                texts[0].text = item.itemName;
                texts[0].color = GetColorByRate(item.rate); // ← Thêm dòng này để đổi màu theo rate

                // Nếu không phải vũ khí thì hiển thị số lượng
                if (!isWeapon && texts.Length >= 2)
                {
                    texts[1].text = $"x{item.quantity}";
                }
            }

            spawnedSlots.Add(newSlot);
        }
    }

    /// <summary>
    /// Trả về màu theo độ hiếm (rate)
    /// </summary>
    private Color GetColorByRate(int rate)
    {
        switch (rate)
        {
            case 1: return Color.white;                              // Bình thường
            case 2: return new Color(0.2f, 1f, 0.2f);                 // Lục (hiếm)
            case 3: return Color.blue;                               // Lam (cực hiếm)
            case 4: return new Color(0.6f, 0.2f, 1f);                 // Tím (siêu hiếm)
            case 5: return Color.yellow;                             // Vàng (huyền thoại)
            default: return Color.gray;
        }
    }
}

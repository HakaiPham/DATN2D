using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MergeManager : MonoBehaviour
{
    public MergeSlot[] mergeSlots; // 3 slots ghép
    public Transform resultSlot;   // slot hiển thị kết quả
    public GameObject resultItemPrefab; // prefab item kết quả

    public Button mergeButton;
    public TextMeshProUGUI mergeStatusText;

    public UpgradeManager upgradeManager; // xử lý cấp đồ

    private void Start()
    {
        mergeButton.onClick.AddListener(OnMergeClicked);
        mergeStatusText.text = "";
        ClearResultSlot(); // Đảm bảo rỗng ban đầu
    }

    void OnMergeClicked()
    {
        ClearResultSlot(); // Xoá item cũ nếu có

        List<string> itemNames = new List<string>();
        foreach (var slot in mergeSlots)
        {
            if (!string.IsNullOrEmpty(slot.currentItemName))
                itemNames.Add(slot.currentItemName);
        }

        if (itemNames.Count < 3)
        {
            mergeStatusText.text = "Cần 3 món giống nhau để ghép!";
            return;
        }

        // Kiểm tra tất cả giống nhau
        if (itemNames.TrueForAll(name => name == itemNames[0]))
        {
            string baseItem = itemNames[0];
            bool success = upgradeManager.MergeItems(baseItem);

            if (success)
            {
                foreach (var slot in mergeSlots)
                    slot.ClearSlot();

                mergeStatusText.text = "Ghép thành công!";

                // Spawn item kết quả
                GameObject newItem = Instantiate(resultItemPrefab, resultSlot);
                DraggableItem dragItem = newItem.GetComponent<DraggableItem>();
                dragItem.itemName = baseItem;
                dragItem.parentToReturnTo = resultSlot; // ✅ Cho phép kéo về đúng slot nếu không thả
                newItem.transform.localPosition = Vector3.zero;
            }
            else
            {
                mergeStatusText.text = "Không thể ghép (đã đạt cấp tối đa)!";
            }
        }
        else
        {
            mergeStatusText.text = "3 món phải giống nhau!";
        }
    }

    void ClearResultSlot()
    {
        foreach (Transform child in resultSlot)
        {
            Destroy(child.gameObject);
        }
    }
}

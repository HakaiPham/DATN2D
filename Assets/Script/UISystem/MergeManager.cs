using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MergeManager : MonoBehaviour
{
    public MergeSlot[] mergeSlots; // 3 slots ghép
    public Transform resultSlot;   // slot hiển thị kết quả
    public GameObject resultItemPrefab; // prefab item kết quả

    public Button mergeButton;
    public TextMeshProUGUI mergeStatusText;

    public UpgradeManager upgradeManager; // xử lý cấp đồ

    private Coroutine hideTextCoroutine;

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
            ShowStatus("Cần 3 món giống nhau để ghép!");
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

                ShowStatus("Ghép thành công!");

                // Spawn item kết quả
                GameObject newItem = Instantiate(resultItemPrefab, resultSlot);
                DraggableItem dragItem = newItem.GetComponent<DraggableItem>();
                dragItem.itemName = baseItem;
                dragItem.parentToReturnTo = resultSlot;
                newItem.transform.localPosition = Vector3.zero;
            }
            else
            {
                ShowStatus("Không thể ghép (đã đạt cấp tối đa)!");
            }
        }
        else
        {
            ShowStatus("3 món phải giống nhau!");
        }
    }

    void ClearResultSlot()
    {
        foreach (Transform child in resultSlot)
        {
            Destroy(child.gameObject);
        }
    }

    void ShowStatus(string message)
    {
        mergeStatusText.text = message;

        if (hideTextCoroutine != null)
            StopCoroutine(hideTextCoroutine);

        hideTextCoroutine = StartCoroutine(HideTextAfterSeconds(2f)); // ẩn sau 2 giây
    }

    IEnumerator HideTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        mergeStatusText.text = "";
    }
}

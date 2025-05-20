using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    public UpgradeManager upgradeManager;
    public CurrencyManager currencyManager;
    public MaterialManager materialManager;

    public Button upgradeButton;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI materialsText;
    public TextMeshProUGUI upgradeStatusText;

    public UpgradeSlot upgradeSlot; // <- lấy item từ slot

    private string currentItemName = "";

    void Start()
    {
        gameObject.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeClick);
        UpdateUI();
    }

    public void SetItemName(string newItemName)
    {
        currentItemName = newItemName;
        upgradeButton.interactable = true;
        UpdateUIWithResult(currentItemName, UpgradeResult.Success); // chỉ để hiển thị level, không nâng cấp
    }

    void OnUpgradeClick()
    {
        if (string.IsNullOrEmpty(currentItemName))
        {
            upgradeStatusText.text = "Vui lòng thả một trang bị vào trước!";
            return;
        }

        UpgradeResult result = upgradeManager.UpgradeItem(currentItemName);
        UpdateUIWithResult(currentItemName, result);
    }

    public void UpdateUIWithResult(string itemName, UpgradeResult result)
    {
        int level = upgradeManager.GetItemLevel(itemName);
        levelText.text = $"{itemName} Level: {level}";

        if (currencyManager != null)
            goldText.text = $"Gold: {currencyManager.gold}";

        if (materialManager != null)
        {
            materialsText.text = "";
            foreach (var mat in materialManager.materials)
                materialsText.text += $"{mat.Key}: {mat.Value}\n";
        }

        switch (result)
        {
            case UpgradeResult.Success:
                upgradeStatusText.text = "Nâng cấp thành công!";
                break;
            case UpgradeResult.MaxLevel:
                upgradeStatusText.text = "Đã đạt cấp tối đa!";
                break;
            case UpgradeResult.NotEnoughGold:
                upgradeStatusText.text = "Không đủ vàng!";
                break;
            case UpgradeResult.NotEnoughMaterials:
                upgradeStatusText.text = "Không đủ vật liệu!";
                break;
        }
    }

    void UpdateUI()
    {
        if (currencyManager != null)
            goldText.text = $"Gold: {currencyManager.gold}";

        if (materialManager != null)
        {
            materialsText.text = "";
            foreach (var mat in materialManager.materials)
                materialsText.text += $"{mat.Key}: {mat.Value}\n";
        }
    }

}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    public UpgradeManager upgradeManager;

    // Thêm 2 manager tham chiếu
    public CurrencyManager currencyManager;
    public MaterialManager materialManager;

    public Button upgradeButton;
    public TextMeshProUGUI levelText;

    // Thêm 2 TextMeshPro để hiển thị vàng và vật liệu
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI materialsText;
    public TextMeshProUGUI upgradeStatusText; // <-- thêm dòng này để hiện trạng thái nâng cấp


    private string itemName = "Sword";

    void Start()
    {
        upgradeButton.onClick.AddListener(OnUpgradeClick);
        UpdateUI();
    }

    void OnUpgradeClick()
    {
        UpgradeResult result = upgradeManager.UpgradeItem(itemName);
        UpdateUI(); // Cập nhật vàng + vật liệu sau mỗi thao tác

        int currentLevel = upgradeManager.GetItemLevel(itemName);
        levelText.text = $"{itemName} Level: {currentLevel}";

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
        // Cập nhật vàng
        if (currencyManager != null)
            goldText.text = $"Gold: {currencyManager.gold}";

        // Cập nhật vật liệu
        if (materialManager != null)
        {
            materialsText.text = "";
            foreach (var mat in materialManager.materials)
            {
                materialsText.text += $"{mat.Key}: {mat.Value}\n";
            }
        }
    }

}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UpgradeUI : MonoBehaviour
{
    public UpgradeManager upgradeManager;
    public CurrencyManager currencyManager;
    public MaterialManager materialManager;

    public UITextFader statusTextFader;

    public Button upgradeButton;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI materialsText;
    public TextMeshProUGUI upgradeStatusText;

    public UpgradeSlot upgradeSlot;

    private string currentItemName = "";
    private Coroutine clearStatusCoroutine;

    void Start()
    {
        gameObject.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeClick);

        upgradeButton.interactable = false;
        upgradeStatusText.text = "";
        levelText.text = "";
        materialsText.text = "";

        UpdateCurrencyAndMaterialUI();
    }

    public void SetItemName(string newItemName)
    {
        currentItemName = newItemName;
        upgradeButton.interactable = !string.IsNullOrEmpty(newItemName);

        ClearUpgradeStatusImmediate();

        if (string.IsNullOrEmpty(newItemName))
        {
            levelText.text = "";
            return;
        }

        UpdateUIWithResult(currentItemName, UpgradeResult.Success, false); // Chỉ hiển thị level
    }

    void OnUpgradeClick()
    {
        if (string.IsNullOrEmpty(currentItemName))
        {
            SetUpgradeStatusText("Vui lòng thả một trang bị vào trước!");
            return;
        }

        UpgradeResult result = upgradeManager.UpgradeItem(currentItemName);
        UpdateUIWithResult(currentItemName, result, true);
    }

    public void UpdateUIWithResult(string itemName, UpgradeResult result, bool showStatus)
    {
        int level = upgradeManager.GetItemLevel(itemName);
        levelText.text = $"{itemName} Level: {level}";

        UpdateCurrencyAndMaterialUI();

        if (showStatus)
        {
            string message = result switch
            {
                UpgradeResult.Success => "Nâng cấp thành công!",
                UpgradeResult.MaxLevel => "Đã đạt cấp tối đa!",
                UpgradeResult.NotEnoughGold => "Không đủ vàng!",
                UpgradeResult.NotEnoughMaterials => "Không đủ vật liệu!",
                _ => ""
            };

            SetUpgradeStatusText(message);
        }
    }

    void UpdateCurrencyAndMaterialUI()
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

    void SetUpgradeStatusText(string message)
    {
        statusTextFader?.ShowText(message, 2.5f);
    }

    void ClearUpgradeStatusImmediate()
    {
        statusTextFader?.ClearNow();
    }

}

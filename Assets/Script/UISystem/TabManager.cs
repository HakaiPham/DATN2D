using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject panelUpgrade;
    public GameObject panelMerge;

    public Button buttonUpgrade;
    public Button buttonMerge;

    public Color selectedColor = Color.white;
    public Color normalColor = Color.gray;

    public void ShowUpgradeTab()
    {
        panelUpgrade.SetActive(true);
        panelMerge.SetActive(false);
        UpdateTabButtonColors(true);
    }

    public void ShowMergeTab()
    {
        panelUpgrade.SetActive(false);
        panelMerge.SetActive(true);
        UpdateTabButtonColors(false);
    }

    private void Start()
    {
        ShowUpgradeTab(); // Mặc định mở tab Upgrade

        // Gắn sự kiện click nếu chưa gắn trong Unity
        buttonUpgrade.onClick.AddListener(ShowUpgradeTab);
        buttonMerge.onClick.AddListener(ShowMergeTab);
    }

    private void UpdateTabButtonColors(bool isUpgradeSelected)
    {
        if (buttonUpgrade != null && buttonMerge != null)
        {
            var upgradeColors = buttonUpgrade.colors;
            var mergeColors = buttonMerge.colors;

            ColorBlock upgradeBlock = buttonUpgrade.colors;
            ColorBlock mergeBlock = buttonMerge.colors;

            upgradeBlock.normalColor = isUpgradeSelected ? selectedColor : normalColor;
            mergeBlock.normalColor = isUpgradeSelected ? normalColor : selectedColor;

            buttonUpgrade.colors = upgradeBlock;
            buttonMerge.colors = mergeBlock;
        }
    }
}

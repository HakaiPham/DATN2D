using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject detailPanel;
    public GameObject selection;

    void Awake()
    {
        detailPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Over");
        selection.SetActive(true);
        detailPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit");
        detailPanel.SetActive(false);
        selection.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonShop : MonoBehaviour, IPointerClickHandler
{
    public GameObject menuToShow;

    private void Start()
    {
        if (menuToShow != null)
        {
            menuToShow.SetActive(false); // Ẩn menu lúc bắt đầu
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (menuToShow != null)
        {
            menuToShow.SetActive(!menuToShow.activeSelf); // Bật / tắt khi click
        }
    }

    public void CloseShop()
    {
        if (menuToShow != null)
        {
            menuToShow.SetActive(false);
        }
    }
}

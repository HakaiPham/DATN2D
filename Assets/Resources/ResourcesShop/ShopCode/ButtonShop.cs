using UnityEngine;
using UnityEngine.EventSystems; // Để dùng IPointerClickHandler

public class ButtonShop : MonoBehaviour, IPointerClickHandler
{
    public GameObject menuToShow; // Kéo menu UI vào đây trong Inspector

    // Hàm sẽ tự động được gọi khi ảnh được click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (menuToShow != null)
        {
            // Bật / tắt menu mỗi lần click
            menuToShow.SetActive(!menuToShow.activeSelf);
        }
    }
    public void CloseShop()
    {
        if (menuToShow != null)
        {
            menuToShow.SetActive(false); // Tắt menu
        }
    }

}

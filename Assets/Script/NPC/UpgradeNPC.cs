using UnityEngine;

public class UpgradeNPC : MonoBehaviour
{
    public GameObject upgradeUI; // Giao diện nâng cấp cần bật/tắt
    public KeyCode interactKey = KeyCode.E; // Phím tương tác

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (upgradeUI != null)
            {
                bool isActive = upgradeUI.activeSelf;
                upgradeUI.SetActive(!isActive);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Bạn có thể hiển thị "Nhấn E để nâng cấp" ở đây nếu muốn
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Tự động tắt UI khi rời khỏi vùng NPC
            if (upgradeUI != null && upgradeUI.activeSelf)
            {
                upgradeUI.SetActive(false);
            }
        }
    }
}

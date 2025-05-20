using UnityEngine;

public class UpgradeNPC : MonoBehaviour
{
    public GameObject upgradeUI; // Gán vào UI nâng cấp trong Inspector
    public KeyCode interactKey = KeyCode.E;

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
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (upgradeUI != null && upgradeUI.activeSelf)
                upgradeUI.SetActive(false);
        }
    }
}

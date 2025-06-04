using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FighterStats : MonoBehaviour, IComparable<FighterStats>
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string animatorHit;

    [Header("Character Info UI")]
    public GameObject characterInfoPanel;

    public GameObject characterIconFace;

    [SerializeField]
    private Image healthFill;

    [SerializeField]
    private Image magicFill;


    //private GameObject GameControllerObj;

    [Header("Stats")]
    public float health;
    public float magic;
    public float melee;
    public float defense;
    public float magicRange;
    public float speed;
    public float experience;

    public float startHealth;
    public float startMagic;

    [HideInInspector]
    public int nextActTurn;

    private bool dead = false;

    private GameController gameController;

    //Resize health and Magic Bar
    private Transform healthTransform;
    private Transform magicTransform;

    private Vector2 healthScale;
    private Vector2 magicScale;

    //private float xNewHealthScale;
    //private float xNewMagicScale;

    private void Awake()
    {
        if (healthFill != null)
        {
            healthTransform = healthFill.GetComponent<RectTransform>();
            healthScale = healthFill.transform.localScale;
        }

        if (magicFill != null)
        {
            magicTransform = magicFill.GetComponent<RectTransform>();
            magicScale = magicFill.transform.localScale;
        }

        startHealth = health;
        startMagic = magic;

        animator = GetComponent<Animator>();

        // Đảm bảo panel thông tin ban đầu ẩn
        ////if (characterInfoPanel != null)
        ////{
        ////    characterInfoPanel.SetActive(false);
        ////    characterIconFace.SetActive(false);
        ////}
    }

    //Hàm này để bật panelUi thông tin của nhân vật này
    public void ShowCharacterInfoPanel()
    {
        if (characterInfoPanel != null)
        {
            characterInfoPanel.SetActive(true);
            characterIconFace.SetActive(true);
            UpdateManaBarUI();
            // Có thể gọi hàm cập nhật UI ở đây nếu bạn muốn đảm bảo nó luôn mới nhất
        }
    }

    // Hàm để tắt toàn bộ panel UI thông tin của nhân vật này
    public void HideCharacterInfoPanel()
    {
        if (characterInfoPanel != null)
        {
            characterInfoPanel.SetActive(false);
            characterIconFace.SetActive(false);
        }
    }

    //Hàm này sẽ được GameController gọi để truyền reference
    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }
    public void ReceiveDamage(float damage)
    {
        health -= damage;
        animator.Play(animatorHit);
        //animator.Play("Damage"); (Khong co animation dùng tạm)
        if (health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            //Destroy(healthFill);
            //Destroy(gameObject);
            gameObject.SetActive(false); // Ẩn tạm thời
            HideCharacterInfoPanel();
            Debug.Log(gameObject.name + " has been defeated!");

            //Gọi GameController để kiểm tra trạng thái trận đấu
            if (gameController != null)
            {
                gameController.CheckBattelEnd();
            }
        }
        else if (damage > 0)
        {
            float xNewHealthScale = healthScale.x * (health / startHealth);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }
        if (damage > 0)
        {
            if (gameController != null && gameController.battelText != null)
            {
                //Hiển thị số lượng sát thương gây ra trên mục tiêu
                gameController.battelText.gameObject.SetActive(true);
                gameController.battelText.text = damage.ToString();
                //Sau khi hiện xong đợi xíu rồi tắt
                Invoke("HideDamageText", 0.5f);
            }
            else
            {
                Debug.Log("gameController Null");
            }
        }
    }

    private void HideDamageText()
    {
        if (gameController != null && gameController.battelText != null)
        {
            gameController.battelText.gameObject.SetActive(false);
        }
    }
    public void updateMagicFill(float cost)
    {
        if (cost > 0)
        {
            magic -= cost;
            Debug.Log($"[updateMagicFill] Đang cập nhật mana cho: " +
                $"{gameObject.name}. Mana hiện tại: {magic}");
            magic = Mathf.Max(0, magic);
            //gameController.mpText.text = "Mana: " + magic + "/" + startMagic;
            //Sửa lỗi NaN nếu startMagic = 0
            if (startMagic <= 0)
            {
                magicFill.transform.localScale = new Vector2(0, magicScale.y);
                Debug.LogWarning(gameObject.name + ": " +
                    "startMagic hiện tại là 0, và thanh năng magicFill sẽ về 0");
            }
            else
            {
                UpdateManaBarUI();
            }
        }
    }
    public bool GetDead()
    {
        return dead;
    }
    public void CalculateNextTurn(int currentTurn)
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }
    public int CompareTo(FighterStats other)
    {
        if (other == null) return 1;
        return this.nextActTurn.CompareTo(other.nextActTurn);
    }

    private void UpdateManaBarUI()
    {
        if (magicFill != null)
        {
            float xNewMagicScale = magicScale.x * (magic / startMagic);
            magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
            // Debug.Log($"[UpdateManaBarUI] {gameObject.name}'s Mana bar updated. Scale: {magicFill.transform.localScale.x}");
        }
        else Debug.LogWarning($"{gameObject.name}: magicFill is NULL! Cannot update mana bar.");
    }
}

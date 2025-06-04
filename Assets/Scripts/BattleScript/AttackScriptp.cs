using UnityEngine;

public class AttackScriptp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject owner; //Ai tấn công
    public GameObject target; // Ai bị tấn công

    [SerializeField]
    private string animationNormalAttack;
    [SerializeField]
    private string animationSkill;

    [SerializeField]
    private bool magicAttack;

    public float magicCost; //Chỉnh thành public để fighterAction có thẻ lấy cost

    [SerializeField]
    private float minAttackMultiplier;

    [SerializeField]
    private float maxAttackMultiplier;

    [SerializeField]
    private float minDefenseMultiplier;

    [SerializeField]
    private float maxDefenseMultiplier;

    private FighterStats attackerStats;
    private FighterStats targetStats;
    //public float damage = 0.0f;

    public GameController gameController;

    //Hàm Attack không nhận victim nữa, mà dùng biến target đã được gán
    public void Attack()
    {
        if (owner == null || target == null)
        {
            Debug.LogError("AttackScriptp: Owner or Target is null. Cannot attack.");
            if (gameController != null && owner != null && owner.CompareTag("Hero"))
            {
                gameController.EndPlayerFighterTurn(); //Đảm bảo lượt kết thúc
            }
            else if (gameController != null && owner != null && owner.CompareTag("Enemy"))
            {
                gameController.AdvanceTurnState(); //Đảm bảo lượt Ai kết thúc
            }
            Destroy(gameObject); //Hủy đối tượng tấn công này
            return;
        }
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = target.GetComponent<FighterStats>();

        if (attackerStats == null || targetStats == null)
        {
            Debug.LogError("AttackScriptp: FighterStats not found on owner or target.");
            if (gameController != null && owner != null && owner.CompareTag("Hero"))
            {
                gameController.EndPlayerFighterTurn();
            }
            else if (gameController != null && owner != null && owner.CompareTag("Enemy"))
            {
                gameController.AdvanceTurnState();
            }
            Destroy(gameObject);
            return;
        }

        //Kiểm tra maigc phải đủ trước khi tấn công
        if (magicAttack && attackerStats.magic < magicCost)
        {
            Debug.Log(owner.name + " Không đủ mana đến tấn công!");
            //Không đủ mana, lượt này coi như bỏ qua, chuyển lượt
            if (owner.CompareTag("Hero") && gameController != null)
            {
                gameController.EndPlayerFighterTurn();
            }
            else if (owner.CompareTag("Enemy") && gameController != null)
            {
                gameController.AdvanceTurnState();
            }
            Destroy(gameObject); // Hủy đối tượng tấn công này
            return;
        }

        float damage = 0.0f;
        float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);

        if (magicAttack)
        {
            damage = multiplier * attackerStats.magicRange;
            attackerStats.updateMagicFill(magicCost); //Trừ mana khi xác nhận tấn công
            owner.GetComponent<Animator>().Play(animationSkill);
        }
        else
        {
            damage = multiplier * attackerStats.melee;
            owner.GetComponent<Animator>().Play(animationNormalAttack);
        }

        float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
        damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

        targetStats.ReceiveDamage(Mathf.CeilToInt(damage));

        //Sau khi tấn công và xử lý sát thương thì kết thúc lượt
        //Nếu có animation thì ghi vào đây
        Invoke("OnAttackFinished", 0.5f);
    }
    private void OnAttackFinished()
    {
        //Kiểm tra xem hiện tại ai là người đang tấn công để xử lý cho phù hợp
        if (gameController != null)
        {
            if (owner != null && owner.CompareTag("Hero"))
            {
                gameController.EndPlayerFighterTurn(); // Hero đã hành động xong
            }
            else if (owner != null && owner.CompareTag("Enemy"))
            {
                gameController.AdvanceTurnState(); // Enemy đã hành động xong, chuyển lượt AI tiếp theo
            }
        }
        Destroy(gameObject);
    }
}

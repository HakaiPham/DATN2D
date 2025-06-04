using UnityEngine;
using UnityEngine.UI;

public class FighterAction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //Không cần tìm vì GameController sẽ tự động truyền vào
    //private GameObject enemy;
    //private GameObject hero;

    [SerializeField] private GameObject meleePrefab;

    [SerializeField] private GameObject rangePrefab;

    [SerializeField] private Image faceIcon;

    private AttackScriptp currentAttackScript;

    //Biến tham chiếu đến GameController
    private GameController gameController;

    // Hàm khởi tạo để GameController truyền reference
    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }
    //Hàm được gọi bởi MakeButton khi người chơi chọn tấn công
    public void SelectAttackType(string attackType)
    {
        //Khi người chơi chọn tấn công bắt đầu chuyển sang chọn mục tiêu
        gameController.SetTargetSelectionMode(this.GetComponent<FighterStats>(), attackType);
    }

    //Hàm này được gọi bởi GameController khi người chơi đã chọn được mục tiêu tấn công
    public void PerformAttack(GameObject victim, string attackType)
    {
        FighterStats attackerStats = GetComponent<FighterStats>();

        //Kiểm tra liệu có đủ mana (Nếu tấn công phép)
        float magicCost = 0;
        if (attackType == "range")
        {
            AttackScriptp tempAttack = rangePrefab.GetComponent<AttackScriptp>();
            if (tempAttack != null)
            {
                magicCost = tempAttack.magicCost;
            }
        }
        if (magicCost > 0 && attackerStats.magic < magicCost)
        {
            Debug.Log(gameObject.name + " không đủ năng lượng để tấn công!");
            //Thông báo người chơi và có thể cho phép chọn lại
            //Sẽ kết thúc lượt của người chơi nếu chưa chọn được gì
            gameController.EndPlayerFighterTurn();
            return;
        }

        GameObject attackPrefabToInstantiate = null;
        if (attackType.CompareTo("melee") == 0)
        {
            attackPrefabToInstantiate = meleePrefab;
        }
        else if (attackType.CompareTo("range") == 0)
        {
            attackPrefabToInstantiate = rangePrefab;
        }

        if (attackPrefabToInstantiate != null)
        {
            //Tạo ra thiết lập tấn công và thiết lập owner
            GameObject attackInstantiate = Instantiate(attackPrefabToInstantiate,
                transform.position, Quaternion.identity);
            currentAttackScript = attackInstantiate.GetComponent<AttackScriptp>();

            if (currentAttackScript != null)
            {
                currentAttackScript.owner = this.gameObject;
                Debug.Log("OwnerName: " + currentAttackScript.owner.name);
                currentAttackScript.target = victim;// truyền mục tiêu đã chọn
                Debug.Log("TargetName: " + currentAttackScript.target.name);
                currentAttackScript.gameController = gameController; //Truyền GameController reference

                currentAttackScript.Attack(); //Gọi hàm tấn công
            }
            else
            {
                Debug.LogError("AttackScriptp Script này " +
                    "không tìm thấytại đối tượng: " + attackPrefabToInstantiate.name);
                //Nếu có lỗi, kết thúc lượt để tránh kẹt game
                gameController.EndPlayerFighterTurn();
            }
        }
        else
        {
            Debug.LogError("AttackType Prefab không tìm thấy: " + attackType);
            gameController.EndPlayerFighterTurn();
        }
    }

    //Hàm này sẽ được GameController gọi khi đến lượt Enemy (Ai) thực hiện lượt
    public void AISelectAttack(GameObject targetEnemy)
    {
        string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
        PerformAttack(targetEnemy, attackType);
    }
}

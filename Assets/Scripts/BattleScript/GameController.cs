using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;

public class GameController : MonoBehaviour
{
    //List để quản lý nhân vật
    private List<FighterStats> allFighters; //Chứa tất cả các nhân vật trong game, sắp xếp ban đầu
    private List<FighterStats> playerFightersAvailable; //Hero còn lượt đi trong lượt hiện tại
    private List<FighterStats> enemyFightersAvailable; //Enemy còn lượt đi trong lượt hiện tại

    //Ui Reference
    [SerializeField] public GameObject battleMenu; //Ui Menu cho người chơi
    public TextMeshProUGUI battelText; //Text hiển thị sát thương thông báo
    public TextMeshProUGUI battelCurrentTurn; //Hiển thị tên nhân vật đang được chọn


    [Header("Turn Visual")]
    [SerializeField] private GameObject turnIndicatorPrefab; // Biểu tượng dưới chân của các nhân vật
                                                             // giúp nhận biết đang ở chr
    private GameObject currentTurnIndicatorInstance;

    [SerializeField]
    private List<Transform> characterSpawnPoint; //List chứa vị trí đã được sắp xếp trước

    private Dictionary<string, Transform> spawnPointMap = new Dictionary<string, Transform>();

    //Layer của các nhân vật có thể click được
    public LayerMask clickableLayer;

    //Trạng thái
    public enum TurnState
    {
        START_ROUND,// Bắt đầu vòng đấu mới (reset lượt tất cả nv)
        PLAYER_SELECTION_PHASE,//người chơi đang chọn hero để hành động
        PLAYER_TARGET_SELECTION,//Người chơi đã chọn Hero, chuẩn bị sang chọn mục tiêu
        PLAYER_ACTION, //Trạng thái đợi
        ENEMY_TURN,//Lượt của Enemy
        CHECK_BATTEL_END,//Kiểm tra điều kiện thắng thua
        BATTEL_ENDED //Trận đấu kết thúc
    }
    public TurnState currentTurnState;

    [HideInInspector] public FighterStats selectedPlayerFighter; //Hero mà người chơi chọn để hành động
    private string selectedAttackType; //Loại tấn công mà người chơi chọn
    private FighterStats currentEnemyFighter; //Enemy đang hành động

    private FighterStats currentlyDisplayedInfoFighter; // biến này để theo dõi nhân vật
                                                        // đang hiển thị info
    [Header("CharacterInfoText")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    private TextMeshProUGUI meleeText;
    private TextMeshProUGUI magicRangeText;
    private TextMeshProUGUI defenseText;
    private TextMeshProUGUI dodgeText;
    private TextMeshProUGUI accuracyText;

    private void Awake()
    {
        //Khởi tạo map điểm spawn
        foreach (Transform spawnPoint in characterSpawnPoint)
        {
            if (!spawnPointMap.ContainsKey(spawnPoint.name))
            {
                spawnPointMap.Add(spawnPoint.name, spawnPoint);
            }
        }
    }

    void Start()
    {
        IntializeBattleField();
        IntializeFighter();
        SetupUI();

        currentTurnState = TurnState.START_ROUND; //Bắt đầu trận đấu
        AdvanceTurnState();
    }
    private void Update()
    {
        //Xử lý Input chỉ khi ở trạng thái cần thiết (người chơi)
        if (currentTurnState == TurnState.PLAYER_SELECTION_PHASE)
        {
            HandlePlayerSelectionInput();
        }
        else if (currentTurnState == TurnState.PLAYER_TARGET_SELECTION)
        {
            HandlePlayerTargetSelectionInput();
        }
    }
    //Khởi tạo nhân vật và UI

    private void IntializeBattleField()
    {
        if (TeamManager.Instance == null)
        {
            Debug.LogError("TeamManager Instance không tìm thấy !");
            return;
        }
        Debug.Log("Set Up Nv");

        foreach (TeamManager.AssignedBattleCharacter assignedChar in TeamManager.Instance.finalBattleTeam)
        {
            Transform spawnTransform = null;
            //Tìm điểm spawn dựa trên vị trí (Tên) Ui đã lưu
            if (spawnPointMap.ContainsKey(assignedChar.assignedViTriName))
            {
                spawnTransform = spawnPointMap[assignedChar.assignedViTriName];
            }
            else
            {
                spawnTransform = transform; //Sử dụng Transform của GameController làm mặc định
            }

            //Sinh nv
            GameObject character = Instantiate(assignedChar.characterData.battlePrefab, spawnTransform.position, Quaternion.identity);

            character.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            Canvas[] canvas = character.GetComponentsInChildren<Canvas>();
            foreach (Canvas i in canvas)
            {
                i.GetComponent<Canvas>().worldCamera = Camera.main;
                character.GetComponentInChildren<Canvas>().worldCamera = i.worldCamera;
            }
            character.GetComponent<FighterStats>().characterInfoPanel = GameObject.Find("ThuocTinhHero");
            // Gán các stats từ CharacterData vào các script Fighter của nhân vật
            FighterStats fighterStats = character.GetComponent<FighterStats>();
            if (fighterStats != null)
            {
                //Gán thuộc tính
                fighterStats.health = assignedChar.characterData.health;
                //fighterStats.startHealth = assignedChar.characterData.health;
                fighterStats.magic = assignedChar.characterData.magic;
                fighterStats.melee = assignedChar.characterData.melee;
                fighterStats.defense = assignedChar.characterData.defense;
                fighterStats.magicRange = assignedChar.characterData.magicRange;
                fighterStats.speed = assignedChar.characterData.speed;
            }
        }
    }

    void IntializeFighter()
    {
        allFighters = new List<FighterStats>();
        playerFightersAvailable = new List<FighterStats>();
        enemyFightersAvailable = new List<FighterStats>();

        GameObject[] heroObjects = GameObject.FindGameObjectsWithTag("Hero");
        heroObjects = heroObjects.OrderBy(go => go.name).ToArray();
        foreach (GameObject heroObj in heroObjects)
        {
            FighterStats fs = heroObj.GetComponent<FighterStats>();
            FighterAction fa = heroObj.GetComponent<FighterAction>();
            if (fs != null && fa != null)
            {
                fs.SetGameController(this); //Truyển reference của GameController vào FighterStats
                fa.SetGameController(this); //truyền reference của GameController vào FighterAction
                allFighters.Add(fs);
            }
        }

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemyObjects = enemyObjects.OrderBy(go => go.name).ToArray();
        foreach (GameObject enemyObj in enemyObjects)
        {
            FighterStats fs = enemyObj.GetComponent<FighterStats>();
            FighterAction fa = enemyObj.GetComponent<FighterAction>();
            if (fs != null && fa != null)
            {
                fs.SetGameController(this); //Truyển reference của GameController
                fa.SetGameController(this);
                allFighters.Add(fs);
            }
        }

        //Sắp xếp ban đầu cho mục đích Debug hoặc khởi tạo
        allFighters.Sort();

        Debug.Log("Initial fighter order: ");
        foreach (FighterStats fs in allFighters)
        {
            Debug.Log("- " + fs.gameObject.name);
        }
    }

    public void SetupUI()
    {
        hpText = GameObject.Find("HP").GetComponent<TextMeshProUGUI>();
        mpText = GameObject.Find("Mana").GetComponent<TextMeshProUGUI>();
        meleeText = GameObject.Find("Melee").GetComponent<TextMeshProUGUI>();
        magicRangeText = GameObject.Find("MagicRange").GetComponent<TextMeshProUGUI>();
        defenseText = GameObject.Find("Defense").GetComponent<TextMeshProUGUI>();
        dodgeText = GameObject.Find("Dodge").GetComponent<TextMeshProUGUI>();
        accuracyText = GameObject.Find("Accuracy").GetComponent<TextMeshProUGUI>();

        if (battleMenu != null) battleMenu.SetActive(false);
        if (battelText != null) battelText.gameObject.SetActive(false);

        if (battelCurrentTurn != null) battelCurrentTurn.gameObject.SetActive(false);
        // Ẩn tất cả các panel thông tin cá nhân của nhân vật khi bắt đầu game
        foreach (FighterStats fs in allFighters)
        {
            fs.HideCharacterInfoPanel();
        }

        if (battleMenu == null) Debug.LogError("Battle Menu is not assigned in GameController!");
    }

    //Xử lý Input của người chơi
    void HandlePlayerSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Lấy vị trí chuột trong không gian 2D
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Physic2D.Raycast(điểm bắt đầu, hướng, khoảng cách, layer);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, clickableLayer);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                FighterStats clickedFighter = hitObject.GetComponent<FighterStats>();

                if (clickedFighter != null)
                {
                    Debug.Log("clickedFighter: " + clickedFighter.name);
                    // Hiển thị thông tin của nhân vật được click
                    ShowFighterSpecificInfo(clickedFighter);

                    if (clickedFighter.gameObject.CompareTag("Hero")
                        && playerFightersAvailable.Contains(clickedFighter)
                        && !clickedFighter.GetDead())
                    {
                        // Chọn Hero hợp lệ để hành động
                        SelectedPlayerFighter(clickedFighter);
                    }
                    else if (clickedFighter.gameObject.CompareTag("Enemy") ||
                        (clickedFighter.gameObject.CompareTag("Hero") &&
                        !playerFightersAvailable.Contains(clickedFighter)))
                    {
                        // Click vào Enemy hoặc Hero đã hết lượt/đã chết trong giai đoạn chọn Hero
                        // Chỉ hiển thị info, không chuyển lượt/hành động.
                        battleMenu.SetActive(false);
                        ShowFighterSpecificInfo(clickedFighter);

                    }
                }

            }
        }
    }


    void HandlePlayerTargetSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousPos, Vector2.zero, 0f, clickableLayer);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                FighterStats clickedFighter = hitObject.GetComponent<FighterStats>();

                if (clickedFighter != null)
                {
                    // Hiển thị thông tin của mục tiêu được click
                    // Người chơi phải chọn một Enemy còn sống làm mục tiêu
                    if (clickedFighter.gameObject.CompareTag("Enemy") && !clickedFighter.GetDead())
                    {
                        // Người chơi đã chọn mục tiêu!
                        PerformPlayerAttack(clickedFighter.gameObject);
                    }
                }
            }
        }
    }

    void SelectedPlayerFighter(FighterStats fighter)
    {
        selectedPlayerFighter = fighter;
        battelCurrentTurn.gameObject.SetActive(true);
        battelCurrentTurn.text = fighter.gameObject.name.ToString(); //Hiển thị tên Hero được chọn

        //Hiển thị biểu tượng dưới chân Hero
        if (currentTurnIndicatorInstance != null)
        {
            Destroy(currentTurnIndicatorInstance);
        }
        if (turnIndicatorPrefab != null)
        {
            currentTurnIndicatorInstance = Instantiate(turnIndicatorPrefab,
                selectedPlayerFighter.transform.position + Vector3.up * 2f,
                Quaternion.identity, selectedPlayerFighter.transform);
            if (selectedPlayerFighter.gameObject.CompareTag("Enemy") ||
                        (selectedPlayerFighter.gameObject.CompareTag("Hero") &&
                        !playerFightersAvailable.Contains(selectedPlayerFighter)))
            {
                battleMenu.SetActive(false); //Hiển thị Menu hành động (Melee/Range)
            }
            else
            {
                battleMenu.SetActive(true);
            }
            //currentTurnState = TurnState.PLAYER_ACTION; //Tạm thời chuyển trạng thái đợi
            //để chọn kiểu tấn công
            Debug.Log("Player selected: " + selectedPlayerFighter.gameObject.name
                + ". State: PLAYER_ACTION.");

        }
    }

    // Hàm này được MakeButton gọi sau khi người chơi chọn loại tấn công (Melee/Range)
    public void SetTargetSelectionMode(FighterStats attackingFighter, string attackType)
    {
        Debug.Log(">>>>");
        selectedPlayerFighter = attackingFighter; //Xác nhận lại ai đang tấn công
        selectedAttackType = attackType; // Lưu lại loại tấn công

        currentTurnState = TurnState.PLAYER_TARGET_SELECTION; //Chuyển trạng thái chọn mục tiêu

        // Có thể highlight các Enemy để người chơi dễ chọn hơn
    }

    //Hàm này được chọn khi người chơi chọn mục tiêu hợp lệ
    void PerformPlayerAttack(GameObject target)
    {
        if (selectedPlayerFighter == null)
        {
            return;
        }

        FighterAction playerAction = selectedPlayerFighter.GetComponent<FighterAction>();
        if (playerAction != null)
        {
            //Gọi PerformAttack với mục tiêu và loại tấn công
            playerAction.PerformAttack(target, selectedAttackType);
            // Note: EndPlayerFighterTurn sẽ được gọi từ AttackScriptp sau khi animation xong.
            //Làm mở màu của nhân vật đã tấn công
            StartCoroutine(CoolDownTurn(playerAction));
        }
        else
        {
            Debug.LogError("FighterAction not found on selected player fighter!");
            EndPlayerFighterTurn(); // Đảm bảo lượt kết thúc nếu có lỗi
        }

        //Sau khi tấn công ẩn Ui liên quan đến lựa chọn tấn công
        battleMenu.SetActive(false);
        battelCurrentTurn.gameObject.SetActive(false);
        if (currentTurnIndicatorInstance != null) Destroy(currentTurnIndicatorInstance);
        HideCurrentlyDisplayedFighterInfo(); // Ẩn panel thông tin sau khi chọn mục tiêu

    }

    IEnumerator CoolDownTurn(FighterAction fighterAction)
    {
        yield return new WaitForSeconds(0.5f);
        Color currentColorFighter = fighterAction.GetComponent<SpriteRenderer>().color;
        currentColorFighter.a = 0.5f;
        fighterAction.GetComponent<SpriteRenderer>().color = currentColorFighter;
    }

    // Hàm này được AttackScriptp gọi sau khi Hero đã hoàn tất hành động của mình
    public void EndPlayerFighterTurn()
    {
        if (selectedPlayerFighter != null)
        {
            playerFightersAvailable.Remove(selectedPlayerFighter); //Đánh dấu Hero đã hành động
            Debug.Log(selectedPlayerFighter.gameObject.name +
                " has finished their turn. Remaining Player Fighters: "
                + playerFightersAvailable.Count);
            selectedPlayerFighter = null; //Reset selection

            HideCurrentlyDisplayedFighterInfo(); // Ẩn panel info của mọi người khi kết thúc lượt

            //Quay về trạng thái lựa chọn hoặc chuyển lượt
            currentTurnState = TurnState.PLAYER_SELECTION_PHASE; //Để người chơi chọn Hero khác
            AdvanceTurnState(); //Kiểm tra Hero nào chưa hành động
        }
    }
    // Quản lý luồng lượt đi
    public void AdvanceTurnState()
    {
        switch (currentTurnState)
        {
            case TurnState.START_ROUND:
                //Reset danh sách các nv có thể hành động
                playerFightersAvailable.Clear();
                enemyFightersAvailable.Clear();
                Color colorFighter;
                foreach (FighterStats fs in allFighters)
                {
                    if (!fs.GetDead())
                    {
                        if (fs.gameObject.CompareTag("Hero"))
                        {
                            playerFightersAvailable.Add(fs);
                            if (playerFightersAvailable[0])
                            {
                                battelCurrentTurn.gameObject.SetActive(true);
                                battelCurrentTurn.text = "" + playerFightersAvailable[0].name;
                                ShowFighterSpecificInfo(playerFightersAvailable[0]);
                                SelectedPlayerFighter(playerFightersAvailable[0]);
                            }
                            colorFighter = fs.GetComponent<SpriteRenderer>().color;
                            colorFighter.a = 1f;
                            fs.GetComponent<SpriteRenderer>().color = colorFighter;

                        }
                        else if (fs.gameObject.CompareTag("Enemy"))
                        {
                            enemyFightersAvailable.Add(fs);
                        }
                    }
                }

                // Sắp xếp lại danh sách
                playerFightersAvailable = playerFightersAvailable.OrderBy(fs => fs.gameObject.name).ToList();
                enemyFightersAvailable = enemyFightersAvailable.OrderBy(fs => fs.gameObject.name).ToList(); // Hoặc theo speed/initiative cho AI


                currentTurnState = TurnState.PLAYER_SELECTION_PHASE;
                //Bật Ui cần thiết cho người chơi lựa chọn
                break;

            case TurnState.PLAYER_SELECTION_PHASE:
                //Nếu người chơi vẫn còn hero chưa hành động
                Debug.Log("playerFightersAvailable: " + playerFightersAvailable.Count);
                if (playerFightersAvailable.Count > 0)
                {
                    // Tiếp tục ở trạng thái này để người chơi chọn Hero khác
                    Debug.Log("State: PLAYER_SELECTION_PHASE. Choose another Hero to act.");
                    battelCurrentTurn.gameObject.SetActive(true);
                    battelCurrentTurn.text = "" + playerFightersAvailable[0].name;
                    ShowFighterSpecificInfo(playerFightersAvailable[0]);
                    SelectedPlayerFighter(playerFightersAvailable[0]);
                }
                else
                {
                    //Tất cả Hero đã hành động, chuyển sang lượt của Enemy
                    currentTurnState = TurnState.ENEMY_TURN;
                    Debug.Log("All Player Heroes have acted. State: ENEMY_TURN.");
                    AdvanceTurnState(); // Gọi lại để bắt đầu lượt Enemy
                }
                break;

            case TurnState.PLAYER_TARGET_SELECTION:
                // Trạng thái này chỉ tồn tại trong thời gian ngắn để chọn mục tiêu.
                // Sau khi chọn xong, nó sẽ chuyển sang trạng thái hành động và sau đó kết thúc lượt.
                // Nếu người chơi không chọn mục tiêu, bạn cần một timeout hoặc nút "Cancel"
                Debug.Log("Still waiting for target selection...");
                HandlePlayerTargetSelectionInput();
                break;

            case TurnState.ENEMY_TURN:
                //Ẩn Ui người chơi
                battleMenu.SetActive(false);
                if (currentTurnIndicatorInstance != null) Destroy(currentTurnIndicatorInstance);
                //HideCurrentlyDisplayedFighterInfo(); // Đảm bảo ẩn tất cả info panel

                //Nếu còn Enemy chưa hành động
                if (enemyFightersAvailable.Count > 0)
                {
                    //Lấy Enemy đầu tiên chưa hành động
                    currentEnemyFighter = enemyFightersAvailable[0];
                    enemyFightersAvailable.RemoveAt(0); //Đánh dấu Enemy đã hành động

                    if (currentEnemyFighter.GetDead()) //Nếu Enemy đã chết thì bỏ qua
                    {
                        Debug.Log(currentEnemyFighter.gameObject.name + " is dead, skipping turn.");
                        AdvanceTurnState(); // Chuyển ngay sang Enemy tiếp theo
                        return;
                    }

                    battelCurrentTurn.gameObject.SetActive(true);
                    battelCurrentTurn.text = currentEnemyFighter.gameObject.name + " Turn";

                    //Hiển thị biểu tượng lượt đi của Enemy
                    if (turnIndicatorPrefab != null)
                    {
                        currentTurnIndicatorInstance = Instantiate(turnIndicatorPrefab,
                            currentEnemyFighter.transform.position + Vector3.up * 2f,
                            Quaternion.identity, currentEnemyFighter.transform);
                    }
                    //AI(Enemy) chọn mục tiêu để tấn công
                    FighterStats targetHero = GetRandomAliveHero();
                    if (targetHero != null)
                    {
                        // Gọi Ai thực hiện hành động
                        currentEnemyFighter.GetComponent<FighterAction>()
                            .AISelectAttack(targetHero.gameObject);
                        // Note: AdvanceTurnState sẽ được gọi từ AttackScriptp sau khi animation xong.
                    }
                    else
                    {
                        Debug.Log("No alive heroes for enemy to attack. Skipping AI turn.");
                        AdvanceTurnState(); // Nếu không có mục tiêu, chuyển lượt luôn
                    }
                }
                else
                {
                    //Tất cả Enemy đã hành động, kết thúc lượt
                    currentTurnState = TurnState.CHECK_BATTEL_END;
                    AdvanceTurnState(); // Gọi lại để kiểm tra điều kiện kết thúc
                }
                break;

            case TurnState.CHECK_BATTEL_END:
                CheckBattelEnd();
                break;

            case TurnState.BATTEL_ENDED:
                Debug.Log("Battle Ended! Game Over.");
                // Xử lý thắng thua, hiển thị kết quả cuối cùng...
                break;
        }

    }

    //Hàm hỗ trợ: Lấy một Hero ngẫu nhiên còn sống
    FighterStats GetRandomAliveHero()
    {
        List<FighterStats> aliveHeroes = new List<FighterStats>();
        foreach (FighterStats fs in allFighters)
        {
            if (fs.gameObject.CompareTag("Hero") && !fs.GetDead())
            {
                aliveHeroes.Add(fs);
            }
        }
        if (aliveHeroes.Count > 0)
        {
            return aliveHeroes[Random.Range(0, aliveHeroes.Count)];
        }
        return null;
    }

    //Hàm kiểm tra điều kiện thắng thua
    public void CheckBattelEnd()
    {
        int aliveHeroes = allFighters.Count(fs => fs.gameObject.CompareTag("Hero") && !fs.GetDead());
        int aliveEnemies = allFighters.Count(fs => fs.gameObject.CompareTag("Enemy") && !fs.GetDead());

        if (aliveHeroes == 0)
        {
            Debug.Log("All heroes defeated! You Lose!");
            currentTurnState = TurnState.BATTEL_ENDED;
        }
        else if (aliveEnemies == 0)
        {
            Debug.Log("All enemies defeated! You Win!");
            currentTurnState = TurnState.BATTEL_ENDED;
        }
        else
        {
            //Nếu trận đấu chưa kết thúc bắt đầu vòng mới
            Debug.Log("Battle continues. Starting new round...");
            currentTurnState = TurnState.START_ROUND;
        }
        AdvanceTurnState(); // Chuyển trạng thái game sau khi kiểm tra
    }
    // Hàm hiển thị thông tin nhân vật (dùng Raycast)
    // --- Điều khiển hiển thị thông tin nhân vật cụ thể ---
    void ShowFighterSpecificInfo(FighterStats fighterToShow)
    {
        // Ẩn panel thông tin của nhân vật đang hiển thị (nếu có và không phải nhân vật hiện tại)
        if (currentlyDisplayedInfoFighter != null && currentlyDisplayedInfoFighter != fighterToShow)
        {
            currentlyDisplayedInfoFighter.HideCharacterInfoPanel();
        }

        // Hiển thị panel thông tin của nhân vật mới được click
        hpText.text = "HP: " + Mathf.CeilToInt(fighterToShow.health) + "/" + Mathf.CeilToInt(fighterToShow.startHealth);
        mpText.text = "Mana: " + Mathf.CeilToInt(fighterToShow.magic) + "/" + Mathf.CeilToInt(fighterToShow.startMagic);
        meleeText.text = "Melee: " + Mathf.CeilToInt(fighterToShow.melee);
        magicRangeText.text = "MagicRange: " + Mathf.CeilToInt(fighterToShow.magicRange);
        defenseText.text = "Defense: " + Mathf.CeilToInt(fighterToShow.defense);
        fighterToShow.ShowCharacterInfoPanel();
        currentlyDisplayedInfoFighter = fighterToShow; // Cập nhật nhân vật đang được hiển thị info
    }

    // Hàm này được gọi khi click ra ngoài hoặc khi chuyển lượt để ẩn info
    void HideCurrentlyDisplayedFighterInfo()
    {
        if (currentlyDisplayedInfoFighter != null)
        {
            currentlyDisplayedInfoFighter.HideCharacterInfoPanel();
            currentlyDisplayedInfoFighter = null; // Đặt lại để không còn nhân vật nào đang hiển thị info
        }
    }
}

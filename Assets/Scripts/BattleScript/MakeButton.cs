using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;

public class MakeButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //[SerializeField] private bool psysical;

    private GameController gameController;
    void Start()
    {
        gameController = GameObject.Find("GameControllerObject").GetComponent<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameControllerObject not found or GameController script not attached!");
            return;
        }

        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallBack(temp));
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void AttachCallBack(string btn)
    {
        if (gameController == null || gameController.selectedPlayerFighter == null)
        {
            Debug.LogWarning("No player fighter selected or GameController not ready.");
            return;
        }

        //Lấy FighterAction của Hero đang được chọn bởi người chơi
        FighterAction currentHeroAction = gameController.selectedPlayerFighter
            .GetComponent<FighterAction>();
        Debug.Log("currentHeroAction: " + currentHeroAction.name);

        if (currentHeroAction != null)
        {
            if (btn.CompareTo("MeleeButton") == 0)
            {
                currentHeroAction.SelectAttackType("melee");
            }
            else if (btn.CompareTo("RangeButton") == 0)
            {
                currentHeroAction.SelectAttackType("range");
            }
        }
        else
        {
            Debug.LogError("FighterAction not found on selected player fighter!");
        }
        //Ẩn Menu sau khi người chơi chọn loại hình tấn công
        //gameController.battleMenu.SetActive(false);
    }
}

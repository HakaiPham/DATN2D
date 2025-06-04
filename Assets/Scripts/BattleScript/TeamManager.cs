using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TeamManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static TeamManager Instance { get; private set; }
    List<ViTri> viTriManager;
    List<CharacterIconData> allCharacterIcons;
    GameObject[] vitri;
    public LayerMask clickable;

    [System.Serializable] // Nó có tác dụng để cho 1 class ko kế thừa từ bất kỳ tk nào có thể được hiển thị và chỉnh sửa trong Inspector
    public class AssignedBattleCharacter
    {
        public CharacterData characterData; // Dữ liệu của nhân vật
        public string assignedViTriName; // Tên GameObject của vị trí
    }

    public List<AssignedBattleCharacter> finalBattleTeam = new List<AssignedBattleCharacter>(); // <-- Đội hình cuối cùng

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Nếu bạn muốn TeamManager tồn tại giữa các scene:
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một instance khác, hủy instance này để chỉ có một TeamManager duy nhất
            Destroy(gameObject);
        }
    }
    void Start()
    {
        viTriManager = new List<ViTri>();

        vitri = GameObject.FindGameObjectsWithTag("ViTri");

        foreach (GameObject i in vitri)
        {
            if (i != null)
            {
                viTriManager.Add(i.GetComponent<ViTri>());
            }
        }
        allCharacterIcons = new List<CharacterIconData>(FindObjectsOfType<CharacterIconData>());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PrepareAndStartBattle() //Hàm sự kiện của Nút Button Fighter
    {
        finalBattleTeam.Clear(); //Xóa toàn bộ đội hình cũ

        //Duyệt qua tất cả các vị trí có nv trên bàn
        foreach (ViTri viTri in viTriManager)
        {
            if (viTri.isCharacterIsHere)
            {
                Debug.Log("Name vị trí: " + viTri.name);

                CharacterIconData assignedIcon = null; //Tìm nv nào đang chiếm vị trí này

                foreach (CharacterIconData icon in allCharacterIcons)
                {
                    if (icon.isAssignedToViTri && icon.currentAssignedViTri == viTri)
                    {
                        Debug.Log("iconName: " + icon.name);
                        assignedIcon = icon;
                        break;
                    }
                }

                if (assignedIcon != null && assignedIcon.characterDataRef != null)
                {
                    AssignedBattleCharacter newAssignedChar = new AssignedBattleCharacter();
                    newAssignedChar.characterData = assignedIcon.characterDataRef;
                    newAssignedChar.assignedViTriName = viTri.gameObject.name; //Dùng tên Object của vị trí để tìm
                    finalBattleTeam.Add(newAssignedChar);
                }
            }
        }
        SceneManager.LoadScene("BattleScene");
    }

    public void SelectCharacterIcon(CharacterIconData selectedIconData) // Hàm này nhấn vào icon nv để chọn nhân vật tự động tìm kiếm vị trí còn trống
    {
        if (selectedIconData.isDragging) return;

        RectTransform rectTransform = selectedIconData.GetComponent<RectTransform>();
        Debug.Log("isAssignedToViTri: " + selectedIconData.isAssignedToViTri);
        if (selectedIconData.isAssignedToViTri)
        {
            Debug.Log("Nhân vật rời khỏi vị trí");
            ReturnIconToOriginalPosition(selectedIconData);
        }
        else
        {
            Debug.Log(">>>>");
            foreach (ViTri viTri in viTriManager)
            {
                if (!viTri.isCharacterIsHere)
                {
                    AssignIconToViTri(selectedIconData, viTri);
                    //Đánh dấu vị trí có người
                    break;
                }
            }
        }
    }

    //Hàm này xử lý khi thả icon vào vị trí
    public void HandleIconDropToSpecificViTri(CharacterIconData droppedIcon, ViTri targetViTri)
    {
        // Nếu icon đã ở vị trí này rồi, thì không làm gì cả
        if (droppedIcon.isAssignedToViTri && droppedIcon.currentAssignedViTri == targetViTri)
        {
            Debug.Log("Icon đã ở vị trí này");
            droppedIcon.currentAssignedViTri.isCharacterIsHere = false;
            droppedIcon.isDroppedSuccessfully = true; // Đánh dấu xử lý thành công
            return;

        }

        // Nếu icon đã được gán vào một ViTri khác, giải phóng ViTri cũ trước
        if (droppedIcon.isAssignedToViTri && droppedIcon.currentAssignedViTri != null)
        {
            Debug.Log("Icon đã không ở vị trí này");
            droppedIcon.currentAssignedViTri.isCharacterIsHere = false;

        }
        AssignIconToViTri(droppedIcon, targetViTri); //Gán Icon vào vị trí mới
        droppedIcon.isDroppedSuccessfully = true;
    }

    //Xử lý khi kết thúc việc kéo Icon (Thả sai vị trí)
    public void HandleIconDrop(CharacterIconData droppedIcon, PointerEventData eventData)
    {
        // Cờ isDroppedSuccessfully đã được đặt trong HandleIconDropToSpecificViTri nếu icon được thả vào một ViTri hợp lệ.
        if (!droppedIcon.isDroppedSuccessfully)
        {
            ReturnIconToOriginalPosition(droppedIcon);

        }
    }

    //Hàm này dành cho việc kéo icon vào vị trí (Khác với hàm Click)
    private void AssignIconToViTri(CharacterIconData icon, ViTri vitri)
    {
        Debug.Log("IconName: " + icon.name);
        Debug.Log("ViTriName: " + vitri.name);
        RectTransform iconRect = icon.GetComponent<RectTransform>();

        iconRect.localPosition = vitri.GetLocalPosition();
        vitri.isCharacterIsHere = true;
        icon.SetAssignedStatus(true);
        icon.currentAssignedViTri = vitri; //Lưu vị trí của icon hiện tại
    }

    //Hàm này đưa icon vào vị trí cũ khi mà thả icon không đúng
    public void ReturnIconToOriginalPosition(CharacterIconData icon)
    {
        icon.GetComponent<RectTransform>().localPosition = icon.ReturnOriginalTransform();
        icon.SetAssignedStatus(false);

        if (icon.currentAssignedViTri != null)
        {
            icon.currentAssignedViTri.isCharacterIsHere = false;
            icon.currentAssignedViTri = null;
        }

    }
}

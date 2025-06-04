using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterIconData : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3 originalPosition;
    public bool isAssignedToViTri = false; //Đã được gán vào vị trí trống hay chưa
    public RectTransform iconNVRectTransform;
    public ViTri currentAssignedViTri = null;

    private Canvas canvas; //Lấy Canvas chứa Icon nv này để thay đổi tọa độ
    public bool isDragging = false; // Xác nhận hiện tại đang kéo hay chỉ click
    public bool isDroppedSuccessfully = false;

    [Header("Data")]
    public CharacterData characterDataRef; //Tham chiếu đến ScriptAble
    private void Awake()
    {
        if (iconNVRectTransform == null)
        {
            iconNVRectTransform = GetComponent<RectTransform>();
        }

        originalPosition = iconNVRectTransform.localPosition;

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas is Null");
        }
    }

    // Hàm public này để TeamManager có thể gọi khi cần thiết

    public Vector3 ReturnOriginalTransform()
    {
        return originalPosition;
    }
    public void SetAssignedStatus(bool assigned)
    {
        isAssignedToViTri = assigned;
    }
    public void CharacterIconSelected()
    {
        if (isDragging) return; //Nếu đang kéo thì bỏ qua

        TeamManager.Instance.SelectCharacterIcon(this);
    }

    public void OnBeginDrag(PointerEventData eventData) //Bắt đầu kéo
    {
        //Chỉ khi nhấn chuột trái
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Đang kéo");
            isDragging = true;
            isDroppedSuccessfully = false; //Đặt về false khi kết thúc kéo


            //iconNVRectTransform.SetParent(canvas.transform);
            //Đảm bảo icon không bị chặn bởi các Element khác khi kéo
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {

            //Di chuyển icon theo con trỏ chuột
            //Sử dụng eventData.delta để chi chuyển theo độ lệch của chuột
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
                canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Vector3 worldPosition;
                // Với Screen Space Canvas, di chuyển bằng cách cộng delta vào anchoredPosition
                //iconNVRectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
                Vector3 screenPoint = eventData.position;
                // Sử dụng Z hiện tại của icon so với camera
                screenPoint.z = Mathf.Abs(canvas.worldCamera.transform.position.z - transform.position.z);

                worldPosition = canvas.worldCamera.ScreenToWorldPoint(screenPoint);

                worldPosition.z = iconNVRectTransform.position.z;

                iconNVRectTransform.position = worldPosition;
            }
            else if (canvas.renderMode != RenderMode.WorldSpace)
            {
                //Nếu là WorldSpace di chuyển teo ScreenToWorldPoint
                Vector3 worldPosition = canvas.worldCamera.ScreenToWorldPoint(eventData.position);
                worldPosition.z = iconNVRectTransform.position.z; // Giữ nguyên Z (Dành cho mode này)
                iconNVRectTransform.position = worldPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Debug.Log("Kếts thúc kéo");
            isDragging = false;

            //isDroppedSuccessfully = true; //Đặt về false khi kết thúc kéo

            //Truyền icon và event để TeamManager kiểm tra vị trí thả
            TeamManager.Instance.HandleIconDrop(this, eventData);

            // Cho phép icon chặn raycast trở lại
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }
        }
    }

    //Hàm xử lí Click (Phân biệt rõ giữa Click và kéo)
    public void OnPointerClick(PointerEventData eventData)
    {
        //if(!isDragging) //Xử lí click nếu không phải kéo
        //{
        //    Debug.Log("Nhấn không kéo");
        //    CharacterIconSelected();
        //}
    }
}

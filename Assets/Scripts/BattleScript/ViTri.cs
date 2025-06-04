using UnityEngine;
using UnityEngine.EventSystems;

public class ViTri : MonoBehaviour, IDropHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isCharacterIsHere = false;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Kiểm tra xem đối tượng được thả có phải là icon nv mà ta đang muốn kéo hay không
        // eventData.pointerDrag là GameObject đang được kéo
        CharacterIconData droppedIcon = eventData.pointerDrag?.GetComponent<CharacterIconData>();
        Debug.Log("Kiểm tra vị trí");
        if (droppedIcon != null)
        {
            //Kiểm tra vị trí này có trống không
            if (!isCharacterIsHere)
            {
                Debug.Log("Vị trí " + gameObject.name + "này trống");
                TeamManager.Instance.HandleIconDropToSpecificViTri(droppedIcon, this);
            }
            //else
            //{
            //    // Quay icon về vị trí ban đầu nếu vị trí đó có người
            //    TeamManager.Instance.ReturnIconToOriginalPosition(droppedIcon);
            //}
        }

    }
    public Vector3 GetLocalPosition()
    {
        return rectTransform.localPosition;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.tag == "IconCharacter")
    //    {
    //        isCharacterIsHere = true;
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "IconCharacter")
    //    {
    //        isCharacterIsHere = false;
    //    }
    //}
}

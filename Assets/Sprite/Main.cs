using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Main : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;

    public AudioClip VideoClip;
    public AudioSource ButtonSoure;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = new Color(1, 1, 1, 100);
        ButtonSoure.PlayOneShot(VideoClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = new Color(1, 1, 1, 0);

    }

    void Start()
    {
        ButtonSoure = GetComponent<AudioSource>();
        buttonImage = GetComponent<Image>();
        buttonImage.color = new Color(1, 1, 1, 0);
    }

    
}

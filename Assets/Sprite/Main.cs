using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class Main : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image buttonImage;

    public AudioClip VideoClip;
    public AudioSource ButtonSoure;
    public GameObject activePanel;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!activePanel.activeInHierarchy)
        {
            activePanel.SetActive(true);
        }
        else
        {
            activePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (activePanel.activeInHierarchy && Input.GetMouseButtonDown(1))
        {
            activePanel.SetActive(false);
        }
    }

}

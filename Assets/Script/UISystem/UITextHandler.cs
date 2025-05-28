using UnityEngine;
using TMPro;
using System.Collections;

public class UITextFader : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    private Coroutine currentCoroutine;

    public void ShowText(string message, float duration = 2.5f)
    {
        if (targetText == null) return;

        targetText.text = message;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ClearAfterDelay(duration));
    }

    IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetText.text = "";
        currentCoroutine = null;
    }

    public void ClearNow()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (targetText != null)
            targetText.text = "";
    }
}

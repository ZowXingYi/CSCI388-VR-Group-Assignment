using UnityEngine;
using TMPro;
using System.Collections;

public class MirrorTextDisplay : MonoBehaviour
{
    public TextMeshPro mirrorText;

    void Start()
    {
        mirrorText.text = "";
    }

    public void ShowMessage(string message, float duration = 4f)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayText(message, duration));
    }

    IEnumerator DisplayText(string message, float duration)
    {
        mirrorText.text = message;
        yield return new WaitForSeconds(duration);
        mirrorText.text = "";
    }
}
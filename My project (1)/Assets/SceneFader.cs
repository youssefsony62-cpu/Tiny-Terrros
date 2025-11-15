using UnityEngine;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup fadePanel; // Assign in Inspector
    public float fadeDuration = 1.5f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0;
    }
}

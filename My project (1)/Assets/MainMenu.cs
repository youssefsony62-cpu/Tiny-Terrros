using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public AudioSource menuMusic;
    public AudioSource clickSound;
    public CanvasGroup fadePanel; // Reference to black panel's CanvasGroup
    public float fadeDuration = 1.5f;
    public string nextSceneName = "cut seen";

    public void NewGame()
    {
        clickSound.Play();
        StartCoroutine(FadeOutMusicAndScreen());
    }

    private IEnumerator FadeOutMusicAndScreen()
    {
        float startVolume = menuMusic.volume;
        float fadeTime = 0f;

        // Fade both music and screen together
        while (fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;
            float t = fadeTime / fadeDuration;

            // Fade music
            menuMusic.volume = Mathf.Lerp(startVolume, 0, t);

            // Fade screen
            fadePanel.alpha = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        menuMusic.Stop();
        menuMusic.volume = startVolume;

        // Wait for click sound to finish
        yield return new WaitForSeconds(clickSound.clip.length);

        // Load scene
        SceneManager.LoadScene(nextSceneName);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public TMP_Dropdown livesDropdown;
    AudioSource audioSource;
    public AudioClip buttonClick;
    public AudioSource backgroundMusic;
public void StartGame()
{
    // Start fading out the background music
    if (backgroundMusic != null)
    {
        StartCoroutine(FadeOutMusic(backgroundMusic, 2.0f));  // Adjust fade duration if needed
    }

    // Play button click sound
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.volume = 1.0f;
    audioSource.PlayOneShot(buttonClick);

    // Delay scene loading
    Invoke("LoadGameScene", 2.0f);
}

private IEnumerator FadeOutMusic(AudioSource audioSource, float fadeDuration)
{
    float startVolume = audioSource.volume;
    float elapsedTime = 0f;

    while (elapsedTime < fadeDuration)
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("Time.deltaTime: " + Time.deltaTime + ", Elapsed Time: " + elapsedTime);

        audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
        yield return null;
    }

    audioSource.volume = 0f;  // Ensure volume is exactly zero
    audioSource.Stop();
    audioSource.volume = startVolume;  // Reset for future use
}

private void LoadGameScene()
{
    int selectedLives = GetSelectedLives();
    PlayerPrefs.SetInt("PlayerLives", selectedLives);
    SceneManager.LoadScene("GameScene");
}

     public int GetSelectedLives()
    {
        // Match the dropdown values to lives (assumes you set up the dropdown options in this order)
        switch (livesDropdown.value)
        {
            case 0: return 5; // First option is "5 Lives"
            case 1: return 3; // Second option is "3 Lives"
            case 2: return 1; // Third option is "1 Life"
            default: return 3; // Default to 3 lives if something goes wrong
        }
    }
}
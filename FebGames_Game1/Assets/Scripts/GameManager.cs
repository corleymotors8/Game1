using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public AudioClip respawnSound;
    public AudioClip gameOver;
    public AudioClip winSound;
    AudioSource audioSource;
    public Vector3 respawnPosition;
    public int playerLives;
    private bool isRespawning = false;
    public GameObject gameOverText;  // Assign your "GAME OVER" text GameObject here
    public GameObject youWinText;
    public float gameOverDelay = 5f;  // Time to wait before returning to the menu

    void Awake()
    {
    playerLives = PlayerPrefs.GetInt("PlayerLives", 3); // Default to 3 if not set
    Debug.Log("Player starts with " + playerLives + " lives.");
    }
    
    void Start()
    {
        
        // InitializeLives(playerLives);

         audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        gameOverText.SetActive(false);
        youWinText.SetActive(false);
    }
    
    public void PlayerDied()
    {
        if (isRespawning) return;  // Prevent multiple respawn calls
        isRespawning = true;
        if (playerLives > 0)
        {
            playerLives--;
            Debug.Log("Player died. Remaining lives: " + playerLives);
            Invoke("RespawnPlayer", 1.0f);
        }
        else
        {
        // Stop background music    
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();

        // Play Game Over music
        audioSource.PlayOneShot(gameOver, .4f);

        // Hide all objects
        HideAllGameObjects();  

        
        // Show Game Over text
        gameOverText.SetActive(true);



        // Start coroutine to handle delay and scene change
        StartCoroutine(GameOverSequence());
        }
    }

    // Coroutine to handle delay and return to menu
    private IEnumerator GameOverSequence()
    {
        // Wait for a few seconds
        yield return new WaitForSeconds(gameOverDelay);

        // Return to the main menu scene
        SceneManager.LoadScene("MainMenu");  // Replace with your actual menu scene name
    }

    void RespawnPlayer()
    {
        Debug.Log("Player respawned");
        audioSource.PlayOneShot(respawnSound, 0.3f);
         GameObject player = GameObject.FindWithTag("Player");  // Find player by tag
        player.transform.position = new Vector3(-12.5f, -6.7f, 0);  // Move player to respawn location
        player.GetComponent<SpriteRenderer>().enabled = true;  // Make the player visible
        isRespawning = false;  // Reset respawn flag

    }

    void HideAllGameObjects()
{
    // Find all game objects with a Renderer component
    Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);

    // Loop through each renderer and disable it
    foreach (Renderer renderer in renderers)
    {
        renderer.enabled = false;
    }
}

    public void WinGame()
    {
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();
        audioSource.PlayOneShot(winSound);
        youWinText.SetActive(true);
        HideAllGameObjects();
        StartCoroutine(GameOverSequence());


    }

}

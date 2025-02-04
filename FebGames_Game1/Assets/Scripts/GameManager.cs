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

    public int enemiesKilled = 0;
    public GameObject gameOverText;  // Assign your "GAME OVER" text GameObject here
    public GameObject youWinText;
    private Timer timerUI;
    public float gameOverDelay = 7f;  // Time to wait before returning to the menu

    void Awake()
    {
    playerLives = PlayerPrefs.GetInt("PlayerLives", 3); // Default to 3 if not set
    // Debug.Log("Player starts with " + playerLives + " lives.");
    }
    
    void Start()
    {
        
        //Find the timer
        timerUI = FindFirstObjectByType<Timer>();
        // reset isGameWonOrLost
        timerUI.isGameWonOrLost = false;
        
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
            Invoke("RespawnPlayer", 0.1f);
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
        //Stop timer
        timerUI.isGameWonOrLost = true;
        
        // Wait for a few seconds
        yield return new WaitForSeconds(gameOverDelay);

        // Return to the main menu scene
        SceneManager.LoadScene("MainMenu");  // Replace with your actual menu scene name
    }

    public void WinGame()
    {
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();
        audioSource.PlayOneShot(winSound, 0.4f);
        youWinText.SetActive(true);
          // Stop the timer
    
        HideAllGameObjects();
        StartCoroutine(GameOverSequence());
    }

    public void IncrementEnemiesKilled()
    {
        enemiesKilled++;
        Debug.Log("Enemies killed: " + enemiesKilled);
    }

    void RespawnPlayer()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.PlayOneShot(respawnSound, 0.2f);
        GameObject player = GameObject.FindWithTag("Player");  // Find player by tag
        player.transform.position = respawnPosition;  // Move player to respawn location
        player.GetComponent<SpriteRenderer>().enabled = true;  // Make the player visible
        player.GetComponent<PlayerScript>().isImmune = true;  // Activate immunity
        StartCoroutine(HandleImmunity(player));  // Start flashing effect and immunity timer

        isRespawning = false;  // Reset respawn flag
    }

    private IEnumerator HandleImmunity(GameObject player)
{
    SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
    float immunityDuration = 5f;
    float flashInterval = 0.2f;

    for (float timer = 0; timer < immunityDuration; timer += flashInterval)
    {
        playerRenderer.enabled = !playerRenderer.enabled;  // Toggle visibility
        yield return new WaitForSeconds(flashInterval);
    }

    playerRenderer.enabled = true;  // Ensure player is visible
    player.GetComponent<PlayerScript>().isImmune = false;  // End immunity
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
    

}

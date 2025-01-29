using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public AudioClip respawnSound;
   AudioSource audioSource;
    public Vector3 respawnPosition;
    public int playerLives = 3;

    void Start()
    {
         audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    
    public void PlayerDied()
    {
        if (playerLives > 0)
        {
            playerLives--;
            Debug.Log("Player died. Remaining lives: " + playerLives);
            Invoke("RespawnPlayer", 1.0f);
        }
        else
        {
            Debug.Log("Game Over!");
            // Handle game over logic (e.g., restart game, show menu, etc.)
        }
    }

    void RespawnPlayer()
    {
        Debug.Log("Player respawned");
        audioSource.PlayOneShot(respawnSound, 0.3f);
        Instantiate(playerPrefab, respawnPosition, Quaternion.identity);
    }
}
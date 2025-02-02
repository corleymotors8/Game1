using UnityEngine;

public class StarScript : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip starSound;
    public AudioClip getStar;
    public AudioClip winJump;

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    public GameObject player;
    private Timer timerUI;

    void Start()
    {
        timerUI = FindFirstObjectByType<Timer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerRb = other.GetComponent<Rigidbody2D>();

            audioSource.PlayOneShot(starSound, 0.3f);
            Invoke("triggerWinGame", 0.05f);
        }
    }

    void triggerWinGame()
    {
        // Freeze the player's position
        if (playerRb != null)
        {
            playerRb.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        // Stop the timerscript
            if (timerUI != null)
    {
        timerUI.isGameWonOrLost = true;
    }

        // Step 2: After 2 seconds, destroy star and play getStar sound
        Invoke("DestroyStarAndPlayGetStar", 2);

        // Step 3: After 3 seconds, play winJump sound and move player
        Invoke("PlayWinJumpAndMovePlayer", 3);

        // Step 4: After 4 seconds, call win game function in Game GameManager
        Invoke("playLastText", 4f);
    }

    void playLastText()
    {
        FindFirstObjectByType<GameManager>().WinGame();
    }

    void DestroyStarAndPlayGetStar()
    {
        audioSource.PlayOneShot(getStar, 0.3f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false; // Hide the star
        gameObject.GetComponent<Collider2D>().enabled = false; // Disable collisions
    }

    void PlayWinJumpAndMovePlayer()
    {
        audioSource.PlayOneShot(winJump, 0.3f);
        if (playerRb != null)
        {
            playerRb.constraints = RigidbodyConstraints2D.None; // Unfreeze movement
            playerRb.linearVelocity = new Vector2(0, 40); // Adjust speed if needed
            Invoke("DisappearPlayer", 0.5f);
        }
    }
    void DisappearPlayer()
    {
        Destroy(player);

    }
}
using UnityEngine;
using System.Collections;

public class BatsNestScript : MonoBehaviour
{
    public int maxSpawns = 1;
    public int spawnCount = 0;
    public int totalKilledCount = 0;
    private int bigBatThreshold = 1;
    public bool shouldSpawnBigBats = false;
    private bool bigBatExists = false;
    private bool isGameOver = false;
    public GameObject batPrefab;
    public GameObject bigBatPrefab;
    private Rigidbody2D playerRB;
    public GameObject player;

    AudioSource audioSource;
    public AudioClip spawnSound;
    public AudioClip nestBreak;
    public AudioClip winJump;
    private bool isSpawningSmall = false;
    private bool isSpawningBig = false;
    private float spawnTimer = 6.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource = gameObject.AddComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        playerRB = player?.GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (spawnCount < maxSpawns && !isSpawningSmall && !isGameOver) 
        {
            spawnTimer = Random.Range(1.0f, 5.0f);
            StartCoroutine(SpawnBatWithDelay(spawnTimer));  // Start coroutine with a 2-second delay
        }
        
        if (!isGameOver && totalKilledCount >= bigBatThreshold && !bigBatExists & shouldSpawnBigBats) // Spawn big bat after set number of small bats killed
        {
            
            StartCoroutine(SpawnBigBatWithDelay(5.0f));
            bigBatExists = true;
        }
    }

    private IEnumerator SpawnBatWithDelay(float delay) // takes the argument passed in StartCoroutine
    {
        isSpawningSmall = true;          // Prevent multiple coroutines from starting
        yield return new WaitForSeconds(delay);  // Wait for the delay
        SpawnBat();
        isSpawningSmall = false;         // Allow the next spawn
    }

    void SpawnBat()
    {
        GameObject spawnedBat = Instantiate(batPrefab, transform.position, Quaternion.identity);
        spawnCount++;
        Debug.Log("Playing spawn sound");
        audioSource.PlayOneShot(spawnSound, 0.08f);
        
        BatFlapScript batScript = spawnedBat.GetComponent<BatFlapScript>();
        if (batScript != null)
        {
            batScript.batSpeed = Random.Range(2f, 6f);
            batScript.isHovering = false;
            StartCoroutine(EnableHoveringAfterDelay(batScript));
        }
    }

    private IEnumerator SpawnBigBatWithDelay(float delay)
    {
        isSpawningBig = true;          // Prevent multiple coroutines from starting
        yield return new WaitForSeconds(delay);  // Wait for the delay
        SpawnBigBat();
        isSpawningBig = false;         // Allow the next spawn
    }

    private IEnumerator EnableHoveringAfterDelay(BatFlapScript batScript)
    {
        yield return new WaitForSeconds(0.2f);
        batScript.isHovering = true;
    }

    void SpawnBigBat()
    {
        GameObject spawnedBigBat = Instantiate(bigBatPrefab, transform.position, Quaternion.identity);
        
        BigBatScript bigBatScript = spawnedBigBat.GetComponent<BigBatScript>();
        if (bigBatScript != null)
        {
            bigBatScript.batSpeed = Random.Range(2f, 4f);
            bigBatScript.isHovering = false;
            StartCoroutine(EnableBigBatHoveringAfterDelay(bigBatScript));
        }

    }
    private IEnumerator EnableBigBatHoveringAfterDelay(BigBatScript bigBatScript)
    {
        yield return new WaitForSeconds(0.2f);
        bigBatScript.isHovering = true;
    }

    // Destroy nest if collided with by rideable enemy
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("RideableEnemy"))
        {
            // Make bat nest invisible
            isGameOver = true;
            GetComponent<SpriteRenderer>().enabled = false;
            audioSource.PlayOneShot(nestBreak, 1.0f);
            Invoke("triggerWinGame", 2.0f);
        }
    }

    void triggerWinGame()
    {
        // Access timerscript and stop timer
        Timer timerUI = FindFirstObjectByType<Timer>();
        if (timerUI != null)
    {
       // stops timer
         timerUI.isGameWonOrLost = true;
    }

        // Freeze the player
        if (playerRB != null)
        {
            playerRB.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        // After 2 seconds, play winJump sound and move player
        Invoke("PlayWinJumpAndMovePlayer", 2);

        // After 3 seconds, call win game function in Game GameManager
        Invoke("playLastText", 3f);
    }

     void PlayWinJumpAndMovePlayer()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        audioSource.PlayOneShot(winJump, 0.3f);
        if (playerRB != null)
        {
            playerRB.constraints = RigidbodyConstraints2D.None; // Unfreeze movement
            playerRB.linearVelocity = new Vector2(0, 40); // Adjust speed if needed
            Invoke("DisappearPlayer", 0.5f);
        }
    }

     void DisappearPlayer()
    {
        //Make player invisible
        player.GetComponent<SpriteRenderer>().enabled = false;

    }

    void playLastText()
    {
        FindFirstObjectByType<GameManager>().WinGame();
    }

}

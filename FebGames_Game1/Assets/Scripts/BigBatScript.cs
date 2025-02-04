using System.Collections;
using UnityEngine;

public class BigBatScript : MonoBehaviour
{
    public float flapStrength = 5.0f;
    public float flapInterval = 1.0f;
    public bool isHovering = true;
    public bool isFlapping = false;
    public bool isStationaryTarget = true;
    private bool isDiveBombing = false; 
    public float batSpeed = 2.0f;
    public float pushDownForce = 10f;
    public bool isChasingPlayer = false;
    public bool isRidable = false; // Check if the bat is ridable
    public bool isBigBat = false; // use to change parameters for big bats
    AudioSource audioSource;
    private Transform batsNestScript;
    public AudioClip deathSound;
    public AudioClip PlayerRiding;
    public AudioClip spawn;
    public AudioClip flap;
    private Transform playerLocation;
    private GameObject player;
    private BatFlapScript batFlapScript;
    private PlayerScript playerScript;
    private Rigidbody2D rb;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
       Animator animator = GetComponent<Animator>();
       
       // Disable animation at first for stationary target
       if (isStationaryTarget)
       {
           animator.enabled = false;  // Disable animation at the start
       }
       animator.enabled = false;  // Disable animation at the start
       
       
       batFlapScript = GameObject.FindFirstObjectByType<BatFlapScript>();
       GameObject player = GameObject.Find("Player");
       playerScript = player.GetComponent<PlayerScript>();
       batsNestScript = GameObject.FindFirstObjectByType<BatsNestScript>()?.transform;
       rb = GetComponent<Rigidbody2D>(); 
       playerLocation = FindFirstObjectByType<PlayerScript>()?.transform;
         audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
    }

    // Update is called once per frame
    void Update()
    {
    if (!isStationaryTarget) // If the bat spawns with AI, or just sits there waiting for player
        {
        if (!isRidable)
        {
            if (isHovering && !isFlapping && !isDiveBombing && !isRidable)
            {
            StartCoroutine(FlapAndFall());
            }
            // Chase behavior
            // Debug.Log($"isChasingPlayer: {isChasingPlayer}, Player position: {playerLocation.position}, Bat position: {transform.position}");
    
            if (isChasingPlayer & !isDiveBombing && !isRidable)
            {
            // Move the bat toward the player's x position
            Vector3 targetPosition = new Vector3(playerLocation.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, batSpeed * Time.deltaTime);
            // Dive bomb if close to the player and above
                if (transform.position.x == playerLocation.position.x && transform.position.y > playerLocation.position.y)
                {
                // Debug.Log("Dive bombing");
                isDiveBombing = true;
                }
            }
        }
        }
    }

    private IEnumerator FlapAndFall() // Only happens if bat has AI (not is stationary target)
    {
        isFlapping = true;
        isDiveBombing = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, flapStrength);
        
        // Push down if at top of screen
        if (transform.position.y > batsNestScript.position.y)
        {
            // Debug.Log("Pushing down");
            PushDown();
        }
         // Stop any currently playing sound to prevent overlapping
    
    if (audioSource.isPlaying)
    {
        audioSource.Stop();
    }

    // Play the flap sound
    if (flap != null)
    {
        audioSource.clip = flap;  // Assign the clip to control it with Play()
        audioSource.volume = 0.025f;  // Set the volume to 50%

        audioSource.Play();
    }
        yield return new WaitForSeconds(flapInterval);
        isFlapping = false;
    }

    private void PushDown()
{
    rb.AddForce(Vector2.down * pushDownForce, ForceMode2D.Impulse);
}
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
       //// IF BAT STARTS AS STATIONARY TARGET
       // Kill enemies collided with (already assumes player on bat)
        if (playerScript.onBat && collision.gameObject.CompareTag("Enemy"))
        {
            batFlapScript = collision.gameObject.GetComponent<BatFlapScript>();
            if (batFlapScript != null)
                {
                batFlapScript.Die();
                GameManager gameManager = GameObject.FindFirstObjectByType<GameManager>();
                gameManager.IncrementEnemiesKilled();
                }
            else
                {
                Debug.LogWarning($"BatFlapScript not found on object {collision.gameObject.name}");
                }
        }
        
        // Turn on animator and play sound when player lands on bat 
        if (collision.gameObject.CompareTag("Player") && isStationaryTarget)
        {
            Animator animator = GetComponent<Animator>();
            animator.enabled = true;
            audioSource.PlayOneShot(PlayerRiding, 0.2f);

        }

       //// IF BAT STARTS AS MOVING ENEMY
        if (!isStationaryTarget)
        {
         // If player is falling make bat rideable
        if (playerScript.isFalling && collision.gameObject.CompareTag("Player"))
        {
        MakeBatRidable();
        }
        // If player is not falling, trigger player death
        if (!playerScript.isFalling && collision.gameObject.CompareTag("Player"))
        {
            GameObject.FindFirstObjectByType<LifeCountScript>().LoseLife();
            // Debug.Log("Lose Life called");
            playerScript.DestroyPlayer();
        }
        // Land on ground and stay for 5 seconds
        if (collision.gameObject.CompareTag("Ground") && !isRidable)
        {
            // Debug.Log("Big Bat Landed on the ground");
            isDiveBombing = false;
            isHovering = true;
            isChasingPlayer = false;
            flapStrength = 0.0f;
            Invoke("StrongFlapStrength", 3.0f);
        }
        // Add force collision to enemy bats
       if (!playerScript.onBat && collision.gameObject.CompareTag("Enemy"))
            {
            BatFlapScript batFlapScriptLocal = collision.gameObject.GetComponent<BatFlapScript>();
            if (batFlapScriptLocal != null)
                {
                Rigidbody2D batRb = batFlapScriptLocal.GetComponent<Rigidbody2D>();
                    if (batRb != null)
                    {
                    batRb.AddForce(Vector2.left * 5.0f, ForceMode2D.Impulse);
                    }       
                    else
                    {
                    Debug.LogWarning("Rigidbody2D not found on bat object!");
                    }
                }
            else
                {
                Debug.LogWarning($"BatFlapScript not found on object {collision.gameObject.name}");
                }
            }
        }
    }

    private void StrongFlapStrength()
    {
        // Debug.Log("Big Bat Invoking Strong flap strength");
        flapStrength = 35.0f;
        isFlapping = true;
        isChasingPlayer = true;
        Invoke("ResetFlapStrength", 4.0f);
    }

    private void ResetFlapStrength()
    {
        // Debug.Log("Resetting flap strength");
        flapStrength = 12.0f;
        isFlapping = false;
    }

    // Script to make the bat rideable
    void MakeBatRidable()
    {
        isRidable = true;
        isChasingPlayer = false;
        isFlapping = false;
        isHovering = false;
        // Debug.Log("Bat is ridable. No longer chasing player");
    }

//     {
//         // Play the death sound on the existing audio source
//     audioSource.PlayOneShot(deathSound);
//     // Debug.Log("Playing death sound");

//     // Disable enemy visuals (prevent further interactions)
//     GetComponent<Collider2D>().enabled = false;
//     GetComponent<SpriteRenderer>().enabled = false;
//     this.enabled = false;  // Disable enemy script to stop further behavior

//     // Destroy the enemy after the sound finishes
//     Destroy(gameObject, deathSound.length);

//     //Decrement spawnCounter
//     GameObject.FindFirstObjectByType<BatsNestScript>().spawnCount--;

//     }
}

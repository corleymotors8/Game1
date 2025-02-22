using System.Collections;
using UnityEngine;

public class BatFlapScript : MonoBehaviour
{
    public float flapStrength = 5.0f;
    public float flapInterval = 1.0f;
    public bool isHovering = true;
    public bool isFlapping = false;
    private bool isDiveBombing = false; 
    public float batSpeed = 2.0f;
    public float pushDownForce = 5f;
    public bool isChasingPlayer = false;
    public bool isRidable = false; // Check if the bat is ridable
    public bool isBigBat = false; // use to change parameters for big bats
    private float batDestroyDelay = 0.1f;
    AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip diveBombSound;
    public AudioClip spawn;
    public AudioClip flap;
    private Transform playerLocation;
    private Transform batsNestScript;
    private Rigidbody2D rb;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
       batsNestScript = FindFirstObjectByType<BatsNestScript>()?.transform;
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
    // audioSource.PlayOneShot(flap);
    // Hover behavior; send to FlapAndFall Coroutine
    if (isHovering && !isFlapping && !isDiveBombing)
    {
    StartCoroutine(FlapAndFall());
    }
    // Chase behavior
    // Debug.Log($"isChasingPlayer: {isChasingPlayer}, Player position: {playerLocation.position}, Bat position: {transform.position}");
    if (isChasingPlayer & !isDiveBombing)
    {
    // Move the bat toward the player's x position
    Vector3 targetPosition = new Vector3(playerLocation.position.x, transform.position.y, transform.position.z);
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, batSpeed * Time.deltaTime);
        // Dive bomb if close to the player and above
        if (transform.position.x == playerLocation.position.x && transform.position.y > playerLocation.position.y)
        {
            // Freeze bat preparation for divebomb
            isChasingPlayer = false;
            // Debug.Log("Dive bombing");
            Invoke("TimeToDiveBomb", 0.5f);
        }
    }
    }

    private void TimeToDiveBomb()
    {
        isDiveBombing = true;
        audioSource.PlayOneShot(diveBombSound, 0.3f);
    }

    private IEnumerator FlapAndFall()
    {
        // Debug.Log("Flapping and falling");
        isFlapping = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, flapStrength);
        
        // Push down if at top of screen
        if (transform.position.y > batsNestScript.position.y)
        {
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
    // Push down if at top of screen
    rb.AddForce(Vector2.down * pushDownForce, ForceMode2D.Impulse);
}

  
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
         // Reset hover state when the bat collides with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Debug.Log("Collided with ground");
            isDiveBombing = false;
            isHovering = true;
            isChasingPlayer = true;
            // Debug.Log("Resetting bat to chase player");
            flapStrength = 0.0f;
            StartCoroutine(PushLeftPushRight());
            Invoke("ResetFlapStrength", 3.0f);
        }
        // Have bats bounce off each other
        if (collision.gameObject.CompareTag("Enemy"))
        {
            rb.AddForce(Vector2.left * 1.0f, ForceMode2D.Impulse);
        }
    }

    private IEnumerator PushLeftPushRight()
    {
        rb.AddForce(Vector2.left * 3.0f, ForceMode2D.Impulse);
        // Debug.Log("Pushing left");
        yield return new WaitForSeconds(0.2f);
        rb.AddForce(Vector2.right * 3.0f, ForceMode2D.Impulse);
        // Debug.Log("Pushing right");
    }

    private void ResetFlapStrength()
    {
        // Debug.Log("Resetting flap strength");
        flapStrength = 15.5f;
    }

   public void Die ()
    {
        // Play the death sound on the existing audio source
    audioSource.PlayOneShot(deathSound, 0.8f);
    // Debug.Log("Playing death sound");

    // Increment the total killed count in BatsNestScript
    BatsNestScript nestScript = FindFirstObjectByType<BatsNestScript>();
    if (nestScript != null)
    {
        nestScript.totalKilledCount++;
        nestScript.spawnCount--;
    }
    
    StartCoroutine("WaitBeforeDestroy");
    }

    private IEnumerator WaitBeforeDestroy()
    {

    yield return new WaitForSeconds(batDestroyDelay);  // Wait for the sound to finish

    // Disable enemy visuals (prevent further interactions)
    GetComponent<Collider2D>().enabled = false;
    GetComponent<SpriteRenderer>().enabled = false;
    this.enabled = false;  // Disable enemy script to stop further behavior

    // Destroy the enemy after the sound finishes
    Destroy(gameObject, deathSound.length);

    //Decrement spawnCounter
    GameObject.FindFirstObjectByType<BatsNestScript>().spawnCount--;

    yield break;
    }
}

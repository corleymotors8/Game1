using System.Collections;
using UnityEngine;

public class Enemy2Script : MonoBehaviour
{
    // Public variables to adjust timing and movement in the inspector
    public AudioClip landSound;
    public AudioClip killSound;
    AudioSource audioSource;
    private float landVolume = 0.1f;
    public float fallSpeed = 4f;  // How fast the block falls
    public float riseSpeed = 2f; // How fast the block rises
    public float waitTime = 2f;  // How long the block waits on the ground

    // Public variables to track landing position
    public Transform landingPlatform;  // Assign the platform in the Inspector
    private float fallingPosition;      // The vertical position where the block stops falling

    // Private variables to track state
    private Vector3 initialPosition;  // Starting position of the block
    public bool isFalling = false;    // Whether the block is currently falling
    public bool isRising = false;    // Whether the block is currently rising

    // Blood prefab

    public GameObject bloodPrefab;

    private Rigidbody2D rb;

    void Start()
    {
        isFalling = false;
        // Debug.Log("Is falling? " + isFalling);
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        audioSource = gameObject.AddComponent<AudioSource>();

         // Ensure a platform is assigned
        if (landingPlatform != null)
    {
        Collider2D platformCollider = landingPlatform.GetComponent<Collider2D>();
        if (platformCollider != null)
        {
            // Set the falling position to the top of the platform
            fallingPosition = platformCollider.bounds.max.y;
        }

    }
    }
void Update()
{
    if (isFalling)
    {
        // Move the block downward
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Stop falling when it reaches the falling position
        if (GetComponent<Collider2D>().bounds.min.y <= fallingPosition)
        {
            isFalling = false;
            audioSource.PlayOneShot(landSound, landVolume);
            StartCoroutine(WaitAndRise());
        }
    }

    if (isRising)
    {
        // Move the block upward
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Stop rising when the block reaches just below its initial position
        if (transform.position.y >= initialPosition.y - 0.5f)
        {
            isRising = false;
            isFalling = true; // Prepare for the next cycle
        }
    }
}

 IEnumerator WaitAndRise()
    {
        yield return new WaitForSeconds(waitTime); // Wait on the ground
        isRising = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Triggering is falling");
        if (collision.CompareTag("Player"))
        {
            isFalling = true;
        }
    }

    /// OPTIONAL - Uncomment this method to stop the block from falling when the player leaves the trigger
    // void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Player"))
    //     {
    //        Debug.Log("Triggering is not falling");
    //         isFalling = false;
    //     }
    // }
}
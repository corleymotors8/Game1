using System.Collections;
using UnityEngine;

public class Enemy2Script : MonoBehaviour
{
    // Public variables to adjust timing and movement in the inspector
    public AudioClip landSound;
    public AudioClip killSound;
    AudioSource audioSource;
    public float landVolume = 1.0f;
    public float fallSpeed = 4f;  // How fast the block falls
    public float riseSpeed = 2f; // How fast the block rises
    public float waitTime = 2f;  // How long the block waits on the ground

    // Public variables to track landing position
    public Transform landingPlatform;  // Assign the platform in the Inspector
    private float fallingPosition;      // The vertical position where the block stops falling

    // Private variables to track state
    private Vector3 initialPosition;  // Starting position of the block
    public bool isFalling = true;    // Whether the block is currently falling
    public bool isRising = false;    // Whether the block is currently rising

    // Blood prefab

    public GameObject bloodPrefab;

    private Rigidbody2D rb;

    void Start()
    {
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

    IEnumerator WaitAndRise()
    {
        yield return new WaitForSeconds(waitTime); // Wait on the ground
        isRising = true;
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
            //transform.position = new Vector3(transform.position.x, fallingPosition, transform.position.z); // Snap to the target position
            isFalling = false;
            audioSource.PlayOneShot(landSound, landVolume);
            StartCoroutine(WaitAndRise());
        }
    }

    if (isRising)
    {
        // Move the block upward
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Stop rising when the block reaches its initial position
        if (transform.position.y >= initialPosition.y)
        {
            isRising = false;
            isFalling = true; // Prepare for the next cycle
        }
    }
}


}
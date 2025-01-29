using UnityEngine;

public class Cannon : MonoBehaviour
{
    public AudioClip cannonSound;
    AudioSource audioSource;
    public Enemy2Script enemy;
    public GameObject cannonballPrefab; // The cannonball prefab
    public Transform spawnPoint;       // The spawn point
    public float fireForce = 10f;      // How fast the cannonball is shot
    public float fireRate = 1f;        // Time between cannonball shots

    private bool isPlayerInRange = false; // Check if player is in range
    private float nextFireTime = 0f;      // Time to control firing rate

    private Animator animator;           // Reference to the Animator

    // control fire direction
    public bool fireLeft = true;

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        bool enemyAboveThreshold = (enemy != null && enemy.isRising && enemy.transform.position.y > -2.5f);

        // Fire only if the player is in range and the cooldown has passed
        if (isPlayerInRange && enemyAboveThreshold && Time.time >= nextFireTime)
        {
            FireCannonball();
            nextFireTime = Time.time + fireRate; // Reset fire cooldown
        }
    }

    void FireCannonball()
    {
        // Play the cannon fire animation
        if (animator != null)
        {
            animator.SetTrigger("Fire");
        }

        // Instantiate the cannonball at the spawn point
        GameObject cannonball = Instantiate(cannonballPrefab, spawnPoint.position, spawnPoint.rotation);

        // Apply force to the cannonball
        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        if (rb != null && fireLeft)
        {
            rb.AddForce(-spawnPoint.right * fireForce, ForceMode2D.Impulse);
        }
        else if (rb != null && !fireLeft)
        {
            rb.AddForce(spawnPoint.right * fireForce, ForceMode2D.Impulse);
        }
        audioSource.PlayOneShot(cannonSound, .2f);
    }

    // Detect when the player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the object is the player
        {
            isPlayerInRange = true;
        }
    }

    // Detect when the player leaves the trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the object is the player
        {
            isPlayerInRange = false;

            // Optional: Reset animation to idle
            if (animator != null)
            {
                animator.ResetTrigger("Fire");
            }
        }
    }
}
using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


public class PlayerScript : MonoBehaviour
{
    public AudioClip landSound;
    public AudioClip[] jumpSounds;
    public AudioClip deathSound;
    public AudioClip stompSound;
    private bool isJumping = false;
    public bool isFalling = false;
    public bool isImmune = false;
    public bool onBat = false;
    public float batSpeed = 5.0f;
    private bool previousFallingState;
    public float fallThreshold = 0.0f;  // Threshold for detecting falling

    public AudioClip[] footstepSounds; // Assign your footstep sounds in the Inspector


    private Vector3 respawnPosition;
    public float footstepVolume = 1.0f; // Public variable to control volume (default is max: 1.0)
    public float jumpVolume = 1.0f; // Public variable to control volume (default is max: 1.0)
    public float landVolume = 1.0f; // Public variable to control volume (default is max: 1.0)


   private AudioSource audioSource; // Reference to the AudioSource
   private bool isFacingRight = true;
   private int jumpCount = 0;
   float horizontalInput;
   public float speed = 10.0f;
   public float jumpForce = 5.0f;
   public float doubleJumpForce = 5.0f;
   bool isGrounded = false;
   Animator animator;
   private Rigidbody2D rb;
   private Rigidbody2D currentBatRb;
  
   // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
   {
       animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
       audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
   }

   // Update is called once per frame
   void Update()
 {
    // Get player input
    horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");  // For bat's vertical movement

    // If the player is riding a bat, move the bat instead
    if (onBat && currentBatRb != null)
    {
        currentBatRb.linearVelocity = new Vector2(horizontalInput * batSpeed, verticalInput * batSpeed);
    
    // Handle jump while on the bat
    if (Input.GetButtonDown("Jump"))
    {
        StopRidingBat();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        isJumping = true;
        ++ jumpCount;

        int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Length);
        audioSource.PlayOneShot(jumpSounds[randomIndex], jumpVolume);
        return;  // Skip the rest of Update for this frame
    }  
    }
    else
    {
        // Normal player movement
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        
        // Flip character animation
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }
    // Single jump
    if (Input.GetButtonDown("Jump") && isGrounded && jumpCount == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            isJumping = true;
            ++ jumpCount;
            //animator.SetBool("isJumping", !isGrounded);
           int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Length); // Pick a random sound
           audioSource.PlayOneShot(jumpSounds[randomIndex], jumpVolume); // Play the sound
        }
    // Double jump
    else if (Input.GetButtonDown("Jump") && !isGrounded && jumpCount == 1)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
        int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Length); // Pick a random sound
        audioSource.PlayOneShot(jumpSounds[randomIndex], jumpVolume); // Play the sound

        ++ jumpCount;

    }
     // Check if the player is falling based on velocity
        if (rb.linearVelocity.y < fallThreshold)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
}

private void Flip()
{
    isFacingRight = !isFacingRight;
    Vector3 scale = transform.localScale;
    scale.x *= -1; // Flip the x-axis
    transform.localScale = scale;
}
   private void FixedUpdate()
   {
      if (!onBat)
      {
      // Move character left/right
      rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
      
      // Trigger move animation (xVelocity) and jump animation (yVelocity)
      animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
      animator.SetFloat("yVelocity", rb.linearVelocity.y);
      }
       else
    {
        // Reset xVelocity animation to 0 when on the bat
        animator.SetFloat("xVelocity", 0f);
    }
   }
   
   
   private void OnCollisionEnter2D(Collision2D collision)
{
    Vector2 relativePosition = transform.position - collision.transform.position; // check if player is above the enemy

        // Jump Detection
       if (collision.gameObject.CompareTag("Ground"))
       {
           isGrounded = true;
           jumpCount = 0;
           isJumping = false;
           isFalling = false;
           //animator.SetBool("isJumping", !isGrounded);
           audioSource.PlayOneShot(landSound, landVolume);
       }
         
         // Handle enemy detection when jumping onto (i.e. falling)
 
        if (collision.gameObject.CompareTag("Enemy") && isFalling && relativePosition.y > 0)
                 // Kill bats
                { 
                BatFlapScript batFlapScript = collision.gameObject.GetComponent<BatFlapScript>();
                if (batFlapScript != null)
                    {
                    batFlapScript.Die();  // Call Die() on the correct bat instance
                    audioSource.PlayOneShot(stompSound, 0.05f);
                    GameManager gameManager = GameObject.FindFirstObjectByType<GameManager>();
                    gameManager.IncrementEnemiesKilled();

                    }
                {
                // Kill regular enemies
                Destroy(collision.gameObject);
                audioSource.PlayOneShot(stompSound, 0.05f);
                GameManager gameManager = GameObject.FindFirstObjectByType<GameManager>();
                gameManager.IncrementEnemiesKilled();
                }
                }

        // Handle enemy detection when colliding in air while falling

        if (!isImmune && collision.gameObject.CompareTag("Enemy") && isFalling && relativePosition.y < 0)
        {
            // Trigger player death
            GameObject.FindFirstObjectByType<LifeCountScript>().LoseLife();
            Debug.Log("Lose Life called");
            audioSource.PlayOneShot(deathSound, 0.025f);
            Invoke("DestroyPlayer", 0.1f);
        }
        
        // Handle enemy detection when on ground 

        if (!isImmune && collision.gameObject.CompareTag("Enemy") && !isFalling && !onBat) 
            {   
            GameObject.FindFirstObjectByType<LifeCountScript>().LoseLife();
            Debug.Log("Lose Life called");
            audioSource.PlayOneShot(deathSound, 0.025f);
            Invoke("DestroyPlayer", 0.1f);
            }

        // Handle bat riding
        if (isFalling && collision.gameObject.CompareTag("RideableEnemy"))
        {
            RidingBatFunction(collision.collider);
        }
    }
    
    void RidingBatFunction(Collider2D collision)
    {
            Debug.Log("Riding bat function triggered");
            onBat = true;
            currentBatRb = collision.gameObject.GetComponent<Rigidbody2D>();
           // Set player on top of middle of bat
           transform.position = new Vector3(collision.transform.position.x + 0.3f, collision.transform.position.y + .5f, collision.transform.position.z);
            StartCoroutine(SetParentDelayed(collision.gameObject.transform));
            
            //Adjust friction
            PhysicsMaterial2D FrictionMaterial = new PhysicsMaterial2D("FrictionMaterial");
            FrictionMaterial.friction = 100.0f;
            GetComponent<Collider2D>().sharedMaterial = FrictionMaterial;
    }

private IEnumerator SetParentDelayed(Transform parentTransform)
{
    yield return null;  // Wait for one frame
    transform.SetParent(parentTransform);
}

    private void StopRidingBat()
{
    onBat = false;
    transform.SetParent(null);
    currentBatRb = null;
     // Reset friction to 0 by using a new material
    PhysicsMaterial2D normalMaterial = new PhysicsMaterial2D("NormalMaterial");
    normalMaterial.friction = 0.0f;
    GetComponent<Collider2D>().sharedMaterial = normalMaterial;
}

public void DestroyPlayer()
{
    GameManager gameManager = GameObject.FindFirstObjectByType<GameManager>();
    if (gameManager != null)
    {
        // Notify the Game Manager to handle player death before destroying the object
        gameManager.PlayerDied();
    }

    // Now make player invisible
    GetComponent<SpriteRenderer>().enabled = false;
}

   public void PlayFootstepSound()
{
    if (isGrounded && footstepSounds.Length > 0)
    {
        int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Length); // Pick a random sound
        audioSource.PlayOneShot(footstepSounds[randomIndex], footstepVolume); // Play the sound
    }
}
}

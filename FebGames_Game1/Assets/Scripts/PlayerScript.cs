using System;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    public AudioClip landSound;
    public AudioClip[] jumpSounds;
    public AudioClip deathSound;
    public AudioClip stompSound;
    private bool isJumping = false;
    public bool isFalling = false;
    private bool onBat = false;
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

       // If rideable bat change player friction and control bat
        var enemy = collision.gameObject.GetComponent<BatFlapScript>(); // Access bat script
         if (collision.gameObject.CompareTag("RideableEnemy") && enemy.isRidable)
         {
            Debug.Log("Collided with rideable bat");
            onBat = true;
            currentBatRb = collision.gameObject.GetComponent<Rigidbody2D>();
            transform.SetParent(collision.transform);

            //change player's material to be high friction
             // Clone the material and set friction
            PhysicsMaterial2D tempMaterial = new PhysicsMaterial2D("TemporaryMaterial");
            tempMaterial.friction = 100.0f;
            GetComponent<Collider2D>().sharedMaterial = tempMaterial;
            //Trasnform enemy position with inputs for player

         }
        
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Check if StompCheck exists and enemy has been stomped
            var stompCheck = collision.gameObject.GetComponentInChildren<StompCheck>();
            
            // If StompCheck exists and enemy has been stomped
            if (stompCheck != null && stompCheck.isStomped)
            {
                audioSource.PlayOneShot(stompSound);
                stompCheck.isStomped = false;
            }
            // Trigger player death if not stomped
            else if (stompCheck != null && !stompCheck.isStomped)
            {
            GameObject.FindFirstObjectByType<LifeCountScript>().LoseLife();
            Debug.Log("Lose Life called");
            audioSource.PlayOneShot(deathSound, 0.1f);
            Invoke("DestroyPlayer", 0.1f);
            }
            
            // Trigger player death if no StompCheck (i.e. bats)
            else if (stompCheck == null & !isJumping & !isFalling)
            {
                audioSource.PlayOneShot(deathSound, 0.025f);
                Invoke("DestroyPlayer", 0.1f);
            }
            else if (stompCheck == null & isJumping & !isFalling)
            {
                // Trigger bat enemy's death function
                GameObject.FindFirstObjectByType<BatFlapScript>().Die();
            }
        }
    }

void DestroyPlayer()
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

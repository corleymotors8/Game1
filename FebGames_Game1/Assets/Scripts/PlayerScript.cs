using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    public AudioClip landSound;
    public AudioClip[] jumpSounds;
    public AudioClip deathSound;
    public AudioClip stompSound;

    private Vector3 respawnPosition;
    public float footstepVolume = 1.0f; // Public variable to control volume (default is max: 1.0)
    public float jumpVolume = 1.0f; // Public variable to control volume (default is max: 1.0)
    public float landVolume = 1.0f; // Public variable to control volume (default is max: 1.0)


    public AudioClip[] footstepSounds; // Assign your footstep sounds in the Inspector
    private AudioSource audioSource; // Reference to the AudioSource
   private bool isFacingRight = true;
   float horizontalInput;
   public float speed = 10.0f;
   public float jumpForce = 5.0f;
   bool isGrounded = false;
   Animator animator;
   private Rigidbody2D rb;
  
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
    // Left-right movement
    horizontalInput = Input.GetAxis("Horizontal");
    if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
    else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    // Jumping
    if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            //animator.SetBool("isJumping", !isGrounded);
           int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Length); // Pick a random sound
           audioSource.PlayOneShot(jumpSounds[randomIndex], jumpVolume); // Play the sound
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
      rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocityY);
      animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
      animator.SetFloat("yVelocity", rb.linearVelocity.y);


      if (rb.linearVelocity.y < 0)
   {
   }
   }

  
   private void OnCollisionEnter2D(Collision2D collision)
   {
        // Jump Detection
       if (collision.gameObject.CompareTag("Ground"))
       {
           isGrounded = true;
           //animator.SetBool("isJumping", !isGrounded);
            audioSource.PlayOneShot(landSound, landVolume);
       }
       // Enemyy Detection
        if (collision.gameObject.CompareTag("Enemy"))
    {
        // Check if StompCheck exists and enemy has been stomped
        var stompCheck = collision.gameObject.GetComponentInChildren<StompCheck>();
        if (stompCheck != null && stompCheck.isStomped)
        {
            audioSource.PlayOneShot(stompSound);
            Debug.Log("Player avoided death by stomping the enemy!");
            stompCheck.isStomped = false;
            Debug.Log("Resetting stomp check to false");
            return;  // Skip death logic if enemy was stomped
        }

        // Trigger player death if not stomped
        audioSource.PlayOneShot(deathSound, 0.1f);
        Invoke("DestroyPlayer", 0.1f);
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

    // Now destroy the object
    Destroy(this.gameObject, 0.1f);
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

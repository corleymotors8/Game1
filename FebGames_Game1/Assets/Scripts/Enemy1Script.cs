  using UnityEngine;
  
   public class Enemy1Script : MonoBehaviour
    {
        public float speed;
        private bool shouldMove = false;
        private Rigidbody2D rb;
        public bool chasePlayer = false;
        private bool moveLeft = true;

        public bool destroyAfter = false;
        
        // Store platform boundaries for enemy movement
        private float leftBound;
        private float rightBound;
    
    
    public void Start()
    {
        Debug.Log("Should move? " + shouldMove);
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Enemy gravity scale: " + rb.gravityScale);
    }
    
    public void FixedUpdate()
    {
        Debug.Log("During fixed update: Should move? " + shouldMove);
        if (!chasePlayer)
        {
        //check if should go left or right
        if (transform.position.x >= rightBound)
        {
            moveLeft = true;
        }
        else if (transform.position.x <= leftBound)
        {
            moveLeft = false;
        }
    
        // assign left-right movement
        if (shouldMove && moveLeft)
        {
            Debug.Log("Moving left");
            rb.linearVelocity = new Vector2(-speed, 0); // Constant movement to the left
        }
        else if (shouldMove && !moveLeft)
        {
        rb.linearVelocity = new Vector2(speed, 0); // Constant movement to the right
        }
        }
        else
        {
        rb.linearVelocity = new Vector2(speed, 0); // Constant movement to the right
        }
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && destroyAfter)
        {
        FindFirstObjectByType<PlatformEnemySpawn>().DestroyAllEnemies();
        }
    }
    
    public void StartMoving()
{
    Debug.Log("Time for enemy to move");
    shouldMove = true;
}

public void SetBounds(float left, float right)
{
    leftBound = left + 0.5f; // Add a buffer to the left bound
    rightBound = right - 0.5f; // Add a buffer to the right bound
}
    }

    
    
    
    
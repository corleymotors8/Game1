  using UnityEngine;
  
   public class Enemy1Script : MonoBehaviour
    {
        public float speed;
       public bool shouldMove
{
    get { return _shouldMove; }
    set
    {
        Debug.Log($"{gameObject.name}: shouldMove changed to {value}");
        _shouldMove = value;
    }
}
private bool _shouldMove = false;
        private Rigidbody2D rb;
        public bool chasePlayer = false;
        public bool groundEnemy = false;
        private bool moveLeft = true;
        private int direction = 1;
        private float moved = 0f;
        public float moveAmount = 3f;

        public bool destroyAfter = false;
        
        // Store platform boundaries for enemy movement
        private float leftBound;
        private float rightBound;
    
    
    public void Start()
    {
        Debug.Log("Should move? " + shouldMove);
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Enemy gravity scale: " + rb.gravityScale);
        Vector3 startPosition = transform.position;
    }
    
    public void FixedUpdate()
    {
        // Left-right movement for base layer enemy
        if (groundEnemy)
        {

        // Move enemy in the current direction
        transform.position += Vector3.right * speed * direction * Time.deltaTime;
        moved += speed * Time.deltaTime;

        // Switch direction after moving the set amount
        if (moved >= moveAmount)
        {
            direction *= -1;  // Reverse direction
            moved = 0f;       // Reset moved distance
        }
        }


        // Platform-bound movement for platform enemies
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

        if (shouldMove)
{
    Debug.Log($"Enemy Debug Log - " +
              $"ShouldMove: {shouldMove}, " +
              $"Speed: {speed}, " +
              $"Velocity: {rb.linearVelocity}, " +
              $"Position: {transform.position}, " +
              $"LeftBound: {leftBound}, " +
              $"RightBound: {rightBound}, " +
              $"MoveLeft: {moveLeft}, " +
              $"Moved: {moved}, " +
              $"Direction: {direction}, " +
              $"GroundEnemy: {groundEnemy}, " +
              $"ChasePlayer: {chasePlayer}, " +
              $"GravityScale: {rb.gravityScale}");
}
    }

    // Destroy platform enemies on player death
    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && destroyAfter)
        {
        FindFirstObjectByType<PlatformEnemySpawn>().DestroyAllEnemies();
        }
    }
    
    public void StartMoving()
{
    Debug.Log("Start moving = true");
    shouldMove = true;
    Debug.Log("Should move = " + shouldMove);
}

public void SetBounds(float left, float right)
{
    leftBound = left + 0.5f; // Add a buffer to the left bound
    rightBound = right - 0.5f; // Add a buffer to the right bound
}

    }

    
    
    
    
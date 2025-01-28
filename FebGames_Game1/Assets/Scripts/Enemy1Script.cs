  using UnityEngine;
  
   public class Enemy1Script : MonoBehaviour
    {
        public float speed;
        public bool shouldMove = false;
        private Rigidbody2D rb;
    
    
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void FixedUpdate()
    {
        if (shouldMove)
    {
    rb.linearVelocity = new Vector2(-speed, 0); // Constant movement to the left
    Debug.Log("Current Velocity: " + rb.linearVelocity); // Check if velocity changes

    }
    }
    
    public void StartMovingLeft()
{
    Debug.Log("Time for enemy to move");
    shouldMove = true;
}
    }

    
    
    
    
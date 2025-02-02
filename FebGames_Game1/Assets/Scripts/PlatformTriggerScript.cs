using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public Enemy1Script enemy; // Assign the Enemy1 script in the Inspector

    private float leftBound;
    private float rightBound;
    
    void Start()
    {
        leftBound = transform.parent.GetComponent<Collider2D>().bounds.min.x; // Set the left bound of the platform
        rightBound = transform.parent.GetComponent<Collider2D>().bounds.max.x; // Set the right bound of the platform
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Check if the player lands
        {
            Debug.Log("Platform entered");
            enemy.StartMoving(); // Call a method in the enemy script to start moving
            enemy.SetBounds(leftBound, rightBound); // Set the bounds of the enemy's movement
            Debug.Log(leftBound + rightBound);
        }
    }
}
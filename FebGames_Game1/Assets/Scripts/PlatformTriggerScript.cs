using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public Enemy1Script enemy; // Assign the enemy script in the Inspector

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Check if the player lands
        {
            enemy.StartMovingLeft(); // Call a method in the enemy script to start moving
        }
    }
}
using UnityEngine;

public class StompCheck : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with" + collision.tag);
        if (collision.CompareTag("PlayerFeet")) // Check if PlayerFeet collides
        {
            Debug.Log("Player landed on the enemy!");
            Destroy(transform.parent.gameObject); // Destroy the enemy's parent
        }
    }
}
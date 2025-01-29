using UnityEngine;

public class StompCheck : MonoBehaviour
{
    public bool isStomped = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with" + collision.tag);
        if (collision.CompareTag("Player")) // Check if PlayerFeet collides
        {
            Debug.Log("Player landed on the enemy!");
            isStomped = true;
            Destroy(transform.parent.gameObject, .01f); // Destroy the enemy's parent
        }
    }
}
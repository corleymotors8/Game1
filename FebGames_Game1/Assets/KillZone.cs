using Unity.VisualScripting;
using UnityEngine;

public class KillZone : MonoBehaviour
{
   AudioSource audioSource;
   public AudioClip killSound;
   public GameObject landingPlatform;
   public GameObject bloodPrefab;
   private GameManager gameManager;
   

   void Start()
   {
         audioSource = GetComponent<AudioSource>();
         audioSource = gameObject.AddComponent<AudioSource>();
         gameManager = GameObject.FindFirstObjectByType<GameManager>();

   }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") & GetComponentInParent<Enemy2Script>().isFalling)
        {
            audioSource.PlayOneShot(killSound, .2f);
            TriggerPlayerDeath(collision.gameObject);
        }
    }

    void TriggerPlayerDeath(GameObject player)
    {
    // Get the player's current X position
    Vector3 playerPosition = player.transform.position;

    // Get the platform's top Y position
    float platformTopY = landingPlatform.GetComponent<Collider2D>().bounds.max.y;

    // Spawn the blood sprite at the correct position
    if (bloodPrefab != null)
    {
        Instantiate(bloodPrefab, new Vector3(playerPosition.x, platformTopY, playerPosition.z), Quaternion.identity);
    }
    else
    {
        Debug.LogWarning("Blood Prefab is not assigned!");
    }

    // Call the lose-life script
    GameObject.FindFirstObjectByType<LifeCountScript>().LoseLife();
    
    // Call the Game Manager PlayerDied function and "destroy" the player 
    gameManager?.PlayerDied();
    GetComponent<SpriteRenderer>().enabled = false;;
    
}
}

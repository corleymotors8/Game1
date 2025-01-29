using UnityEngine;

public class PlatformEnemySpawn : MonoBehaviour
{
   
    private int numberSpawns = 0; // Enemy spawn counter
    public int maxSpawns = 1; // Maximum number of enemy spawns
    public GameObject enemyPrefab;
    private float lastSpawnTime = 0f;
    public float spawnCooldown = 2f; // Set cooldown time (e.g., 2 seconds)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource component on this GameObject
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not present
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && numberSpawns < maxSpawns)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if (Time.time - lastSpawnTime < spawnCooldown)
        return; // Prevent spawning if cooldown hasn't passed

        lastSpawnTime = Time.time; // Update the last spawn time
        
        numberSpawns++;
        // Spawn enemy at the right edge
        Vector3 spawnPosition = new Vector3(-10.7f, 5.8f, 0f);

        // Instantiate the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        enemyRb.gravityScale = 20;

        Enemy1Script enemyScript = enemy.GetComponent<Enemy1Script>();

    if (enemyScript != null)
    {
        enemyScript.StartMoving();  
    }
    }
}

    



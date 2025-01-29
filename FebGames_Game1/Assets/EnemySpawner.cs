using UnityEngine;
using System.Collections.Generic;

public class PlatformEnemySpawn : MonoBehaviour
{
   
    private int numberSpawns = 0; // Enemy spawn counter
    public int maxSpawns = 1; // Maximum number of enemy spawns
    public GameObject enemyPrefab;
    private float lastSpawnTime = 0f;
    public float spawnCooldown = 2f; // Set cooldown time (e.g., 2 seconds)

    private AudioSource audioSource;
    public AudioClip quackSpawn;
    private List<GameObject> spawnedEnemies = new List<GameObject>();


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
        audioSource.PlayOneShot(quackSpawn);
        spawnedEnemies.Add(enemy); // add spawned enemy to List
        Debug.Log("Spawned Enemy added to list" + spawnedEnemies.Count);

        Enemy1Script enemyScript = enemy.GetComponent<Enemy1Script>();
        enemyScript.destroyAfter = true;  // ✅ Set the destroyAfter variable to true


    if (enemyScript != null)
    {
        enemyScript.StartMoving();  
    }
    }

    public void DestroyAllEnemies()
    {
    foreach (GameObject enemy in spawnedEnemies)
    {
        Destroy(enemy, 0.5f);
    }
    spawnedEnemies.Clear();  // Clear the list after destroying
}

}

    



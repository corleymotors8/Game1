using UnityEngine;
using System.Collections;

public class BatsNestScript : MonoBehaviour
{
    public int maxSpawns = 1;
    public int spawnCount = 0;
    public GameObject batPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    
    void Update()
    {
        if (spawnCount < maxSpawns)
        {
            Invoke("SpawnBat", 2.0f);
            spawnCount++;
        }
    }

    void SpawnBat()
{
    GameObject spawnedBat = Instantiate(batPrefab, transform.position, Quaternion.identity);
     // Access the BatFlapScript on the spawned bat and disable hovering
    BatFlapScript batScript = spawnedBat.GetComponent<BatFlapScript>();
    if (batScript != null)
    {
        batScript.batSpeed = Random.Range(2f, 4f);
        batScript.isHovering = false;  // Disable hovering initially
        StartCoroutine(EnableHoveringAfterDelay(batScript));  // Wait before enabling hovering
    }
}

private IEnumerator EnableHoveringAfterDelay(BatFlapScript batScript)
{
    yield return new WaitForSeconds(0.2f);  // Wait for 0.2 seconds
    batScript.isHovering = true;  // Enable hovering
}


}
    


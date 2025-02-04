using UnityEngine;
using TMPro;

public class EnemiesKilled : MonoBehaviour

{
public TextMeshProUGUI enemiesKilled;
public bool isGameWonOrLost = false;
private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.enemiesKilled = 0;
        Debug.Log(enemiesKilled == null ? "Text reference not assigned!" : "Text reference assigned.");
    }

    // Update is called once per frame
    void Update()
    {
    if (!isGameWonOrLost)
    {
        enemiesKilled.text = $"Enemies Killed: {gameManager.enemiesKilled}";
    }   
    }
}

using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
public TextMeshProUGUI timerText;
public bool isGameWonOrLost = false;
private float elapsedTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (!isGameWonOrLost)
    {
    elapsedTime += Time.deltaTime;
    timerText.text = $"Time: {elapsedTime:F2}"; 
    }   
    }
}

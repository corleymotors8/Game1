using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class LifeCountScript : MonoBehaviour

{
    public Image[] lives;
    private int numberPlayerLives;


 private void Start()
{
    GameManager gameManager = FindFirstObjectByType<GameManager>();
    
    if (gameManager == null)
    {
        Debug.LogError("GameManager not found!");
        return;
    }

    numberPlayerLives = gameManager.playerLives;
    // Debug.Log("Player lives from GameManager: " + numberPlayerLives);
    // Debug.Log("Total life icons: " + lives.Length);

    for (int i = numberPlayerLives; i < lives.Length; i++)
    {
        lives[i].enabled = false;
        Debug.Log("Disabling life icon at index: " + i);
    }
}
    
    public void LoseLife()
    {
        if (numberPlayerLives > 0)
        {
        //Decrease the value of LivesRemaining -- delete from the end
        numberPlayerLives --;
        //Hide one of the life images
        lives[numberPlayerLives].enabled = false;
        //If we run out of lives we lose the game
        if(numberPlayerLives==0)
        {
            Debug.Log("Out of lives");
        }
        }
    }

}

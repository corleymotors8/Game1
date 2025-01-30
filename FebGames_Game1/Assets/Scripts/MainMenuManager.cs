using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public TMP_Dropdown livesDropdown;
    public void StartGame()
    {
        int selectedLives = GetSelectedLives();
        PlayerPrefs.SetInt("PlayerLives", selectedLives);
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with the name of your game scene.
    }

     public int GetSelectedLives()
    {
        // Match the dropdown values to lives (assumes you set up the dropdown options in this order)
        switch (livesDropdown.value)
        {
            case 0: return 5; // First option is "5 Lives"
            case 1: return 3; // Second option is "3 Lives"
            case 2: return 1; // Third option is "1 Life"
            default: return 3; // Default to 3 lives if something goes wrong
        }
    }
}
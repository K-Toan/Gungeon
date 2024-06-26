using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayGame()
    {
        SceneManager.LoadScene("DungeonScene");
    }
    public void GoToSettingsMenu()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

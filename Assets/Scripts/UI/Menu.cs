using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionMenu;
    public GameObject PauseMenu;
    public GameObject SelectionMenu;

    public Toggle Char1;
    public Toggle Char2;
    public Toggle Char3;

    public Toggle GameModeSurvival;
    public Toggle GameModeArena;

    private int SelectedChar = 1;
    private string GameMode = "SurvivalScene";

    private void Awake()
    {
        MainMenu.SetActive(true);
        OptionMenu.SetActive(false);
        PauseMenu.SetActive(false);
        SelectionMenu.SetActive(false);
    }

    private void Update()
    {

    }

    public void PlayButton_OnClick()
    {
        Debug.Log("Play Button Hit");
        MainMenu.SetActive(false);
        SelectionMenu.SetActive(true);
    }

    public void StartButton_OnClick()
    {
        SetCharIndex();
        SetGameMode();
        Debug.Log("Char: " + SelectedChar);
        Debug.Log("GameMode: " + GameMode);
        PlayerPrefs.SetInt("CharIndex", SelectedChar);
        SceneManager.LoadScene(GameMode);
    }

    public void OptionButton_OnClick()
    {
        Debug.Log("Option Button Hit");
        MainMenu.SetActive(false);
        OptionMenu.SetActive(true);
    }

    public void ResumeButton_OnClick()
    {

    }

    public void BackButton_OnClick()
    {

    }

    public void QuitButton_OnClick()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void ExitButton_OnClick()
    {
        Debug.Log("Exit to menu");
        MainMenu.SetActive(true);
        OptionMenu.SetActive(false);
        PauseMenu.SetActive(false);
        SelectionMenu.SetActive(false);
    }

    public void SetVolume(float Volume)
    {

    }

    public void SetCharIndex()
    {
        if (Char1.isOn)
        {
            SelectedChar = 1;
        }
        else if (Char2.isOn)
        {
            SelectedChar = 2;
        }
        else if (Char3.isOn)
        {
            SelectedChar = 3;
        }
    }

    public void SetGameMode()
    {
        if (GameModeSurvival.isOn)
        {
            GameMode = "SurvivalScene";
        }
        else if (GameModeArena.isOn)
        {
            GameMode = "ArenaScene";
        }
    }
}

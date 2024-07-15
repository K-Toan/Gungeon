using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionMenu;
    public GameObject PauseMenu;
    public GameObject SelectionMenu;

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
}

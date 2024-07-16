using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject MainMenu;
    public GameObject OptionMenu;
    public AudioSource backgroundMusic; // Thêm biến này
    private bool isPaused = false;
    private bool escPressed = false;

    void Awake()
    {
        OptionMenu.SetActive(false);
        PauseMenu.SetActive(false);
        MainMenu.SetActive(false);

        // Đặt ignoreListenerPause cho AudioSource của bạn
        if (backgroundMusic != null)
        {
            backgroundMusic.ignoreListenerPause = true;
        }
    }

    void Update()
    {
        if (!escPressed && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton_OnClick();
            escPressed = true;
        }
    }

    public void ResumeButton_OnClick()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        escPressed = false;
    }

    public void PlayButton_OnClick()
    {
        MainMenu.SetActive(false);
    }

    public void ExitButton_OnClick()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OptionButton_OnClick()
    {
        MainMenu.SetActive(false);
        OptionMenu.SetActive(true);
    }

    public void PauseButton_OnClick()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitButton_OnClick()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {

    }

    public void BackButton_OnClick()
    {
        PauseMenu.SetActive(true);
        OptionMenu.SetActive(false);
    }
}

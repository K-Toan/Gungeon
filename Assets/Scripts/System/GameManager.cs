using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Objects")]
    public GameObject MainCamera;
    public GameObject Player;
    public GameObject[] Playables;

    [Header("UI")]
    public Canvas Menu;

    [Header("State")]
    public float timeScale = 1f;
    public bool gameIsPaused = false;
    public bool isSlowedDown = false;

    [Header("Setting")]
    [Range(0, 100)]
    public int Volumn = 100;

    [Header("Scripts")]
    public InputSystem _input;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FindGameObjects();
        FindUIs();
    }

    private void Start()
    {
        _input = GetComponent<InputSystem>();
    }

    private void Update()
    {
        if (_input.menu)
        {
            if (!gameIsPaused)
            {
                PauseGame();
            }
            else
            {
                ContinueGame();
            }
        }
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     if (isSlowedDown)
        //     {
        //         ResetTimeScale();
        //         isSlowedDown = false;
        //     }
        //     else
        //     {
        //         SlowDownGame(0.5f);
        //         isSlowedDown = true;
        //     }
        // }
    }

    private void FindGameObjects()
    {
        MainCamera = Camera.main.gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FindUIs()
    {

    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    private void ContinueGame()
    {
        Time.timeScale = timeScale;
        AudioListener.pause = false;
    }

    public IEnumerator SlowDownGameRoutine(float time, float factor)
    {
        SlowDownGame(factor);
        yield return new WaitForSecondsRealtime(time);
        ResetTimeScale();
    }

    private void SlowDownGame(float factor)
    {
        Time.timeScale = factor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        isSlowedDown = true;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        isSlowedDown = false;
    }
}

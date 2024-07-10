using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Objects")]
    public GameObject MainCamera;
    public GameObject Player;
    public GameObject[] Enemies;

    [Header("UI")]
    public Canvas Menu;

    [Header("State")]
    public float timeScale = 1f;
    public bool gameIsPaused = false;

    [Header("Scripts")]
    public InputSystem _input;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Keep the across scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate 
            Destroy(gameObject);
        }
        
        FindGameObjects();
        FindUIs();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void FindGameObjects()
    {
        MainCamera = Camera.main.gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
}

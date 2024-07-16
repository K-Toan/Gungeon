using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Objects")]
    public GameObject MainCamera;
    public GameObject Player;

    [Header("State")]
    public float timeScale = 1f;
    public bool gameIsPaused = false;
    public bool isSlowedDown = false;

    [Header("Setting")]
    [Range(0, 100)]
    public int Volumn = 100;

    [Header("Count")]
    public int killCount = 0;
    public float totalPlayTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindGameObjects();
    }

    private void Update()
    {
        totalPlayTime += Time.deltaTime; // Update total playtime each frame
    }

    private void FindGameObjects()
    {
        MainCamera = Camera.main.gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
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

    public void IncrementKillCount()
    {
        killCount++;
        Debug.Log("Kill Count: " + killCount);
    }

    public int KillCount => killCount;
    public float TotalPlayTime => totalPlayTime;
}

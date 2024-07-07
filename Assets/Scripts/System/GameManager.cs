using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Objects")]
    public GameObject Player;
    public GameObject[] Enemies;

    [Header("UI")]
    public Canvas Menu;

    [Header("Input")]
    public InputSystem _input;

    
    private void Awake()
    {
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
        Player = GameObject.FindGameObjectWithTag("Player");
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void FindUIs()
    {

    }

    private void PauseGame()
    {

    }
}

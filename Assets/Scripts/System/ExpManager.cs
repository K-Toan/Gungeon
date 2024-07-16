using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    public static ExpManager Instance;

    public delegate void ExpChangeHandler(int amount);
    public event ExpChangeHandler OnExpChange;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void AddExp(int amount)
    {
        OnExpChange?.Invoke(amount);
    }
}

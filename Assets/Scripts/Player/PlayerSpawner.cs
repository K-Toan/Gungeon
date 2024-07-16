using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Characters")]
    public GameObject[] Characters;

    private void Awake()
    {
        Instantiate(Characters[PlayerPrefs.GetInt("CharIndex", 1) - 1], new Vector3(0, 0, 0), transform.rotation);
    }
}

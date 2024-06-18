using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Stats")]
    public int currentIndex = 0;
    private int index = 0;

    [Header("Game Objects")]
    public List<GameObject> WeaponPrefabs;
    public List<GameObject> WeaponInstances;

    private void Awake()
    {
        foreach(GameObject weapon in WeaponPrefabs)
        {
            WeaponInstances.Add(Instantiate(weapon));
        }
    }

    private void Update()
    {
        
    }

    public void AddWeapon()
    {

    }

    public GameObject GetWeapon()
    {
        return WeaponInstances[currentIndex];
    }
}

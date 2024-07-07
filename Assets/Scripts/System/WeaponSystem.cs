using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Stats")]
    public int WeaponCount = 0;

    [Header("Game Objects")]
    public GameObject GunRoot;
    public List<GameObject> Weapons;

    private void Start()
    {
        WeaponCount = GunRoot.transform.childCount;
        for(int i = 0; i < WeaponCount; i++)
        {
            GameObject gun = GunRoot.transform.GetChild(i).gameObject;
            gun.SetActive(false);
            Weapons.Add(gun);
        }
    }

    private void Update()
    {
        
    }

    public void AddWeapon(GameObject weapon)
    {
        Weapons.Add(Instantiate(weapon));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [Header("Index")]
    public int CurrentIndex = 0;

    [Header("Stats")]
    public int WeaponCount = 0;

    [Header("Game Objects")]
    public GameObject GunRoot;
    public List<GameObject> Guns;

    private void Awake()
    {
        GunRoot = transform.Find("GunRoot").gameObject;
        List<GameObject> Guns = new();
        AssignGun();
    }

    private void Update()
    {

    }

    private void AssignGun()
    {
        WeaponCount = GunRoot.transform.childCount;
        for (int i = 0; i < WeaponCount; i++)
        {
            GameObject gun = GunRoot.transform.GetChild(i).gameObject;
            Guns.Add(gun);
            // Active current index gun 
            gun.SetActive(i == CurrentIndex);
        }
    }

    public void AddGun(GameObject gun)
    {
        Guns.Add(gun);
        AssignGun();
    }

    public GameObject GetGun(int index)
    {
        if (index <= WeaponCount && WeaponCount > 0)
        {
            for (int i = 0; i < WeaponCount; i++)
            {
                Guns[i].SetActive(false);
            }
        }
        Guns[index - 1].SetActive(true);
        return Guns[index - 1];
    }

    public GameObject GetFirstGun()
    {
        if (WeaponCount > 0)
        {
            for (int i = 0; i < WeaponCount; i++)
            {
                Guns[i].SetActive(false);
            }
            Guns[0].SetActive(true);
            return Guns[0];
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public WeaponStatSO[] weaponStat;

    private void Start()
    {
        InitializeWeapon();
    }

    public void InitializeWeapon()
    {
        for (int i = 0; i < weaponStat.Length; i++)
        {
            weaponStat[i].Initialise();
        }
    }

}

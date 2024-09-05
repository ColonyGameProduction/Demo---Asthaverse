using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : WeaponLogic
{
    [SerializeField] private string weaponType;
    public string Weapon()
    {
        return weaponType;
    }

    public void Shooting()
    {
        ExecuteShooting(weaponType);
    }
}

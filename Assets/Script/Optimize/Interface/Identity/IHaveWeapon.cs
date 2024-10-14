using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public WeaponData(WeaponStatSO stat)
    {
        weaponStatSO = stat;
    }
    public WeaponStatSO weaponStatSO;

    public float totalBullet;
    public float currBullet;
}
public interface IHaveWeapon
{
    List<WeaponData> WeaponLists{ get; }
    WeaponData CurrWeapon{get;}
    void ReloadWeapon();

}

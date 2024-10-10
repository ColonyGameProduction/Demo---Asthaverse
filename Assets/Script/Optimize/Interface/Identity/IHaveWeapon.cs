using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public WeaponStatSO weaponStatSO;

    public int totalBullet;
    public int currBullet;
}
public interface IHaveWeapon
{
    List<WeaponData> WeaponLists{ get; }
    WeaponData CurrWeapon{get;}
    void ReloadWeapon();

}

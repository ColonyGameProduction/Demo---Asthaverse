using System;

using System.Collections.Generic;


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

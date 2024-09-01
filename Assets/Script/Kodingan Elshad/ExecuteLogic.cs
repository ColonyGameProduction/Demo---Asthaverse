using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* PERHATIAN!!!
 * Kalo mau akses logic di skrip ini
 * Public class nya extend ke class ini
 * Jangan ke MonoBehaviour
 */
public class ExecuteLogic : WeaponLogic
{

    //setelah di extend, klean bisa make function ini tanpa perlu refrence

    //logic 'Shoot'
    public void Shoot()
    {
        GameObject weaponType = GetComponentInChildren<WeaponType>().gameObject;
        ExecuteShooting(weaponType);
    }    

    public void ChangingWeapon()
    {
        ChangeWeapon();
    }

    //logic 'SilentKill'
    public void SilentKill()
    {
        SilentKill silentKill = GetComponentInChildren<SilentKill>();
        silentKill.canKill = true;
    }

}

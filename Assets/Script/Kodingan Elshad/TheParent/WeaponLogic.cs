using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogicHandler : MonoBehaviour
{
    //Untuk menembak tergantung dari tag weapon nya    
    public void ExecuteShooting(weaponType weaponType, WeaponStatSO weaponStat)
    {
        switch(weaponType)
        {
            case weaponType.Pistol:
                Pistol(weaponStat);
                break;
            
            case weaponType.AssaultRifle:
                AsaultRifle(weaponStat);
                break;
            
            case weaponType.Shotgun:
                Shotgun(weaponStat);
                break;
            
            case weaponType.Sniper:
                Sniper(weaponStat);
                break;
            
            case weaponType.SMG:
                SMG(weaponStat);
                break;
            
            case weaponType.Rifle:
                Rifle(weaponStat);
                break;
            
            case weaponType.MachineGun:
                MachineGun(weaponStat);
                break;
        }

    }

    //logic Pistol
    public void Pistol(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //logic Asault Rifle
    public void AsaultRifle(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //logic Shotgun
    public void Shotgun(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //logic Sniper
    public void Sniper(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //logic SMG
    public void SMG(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //Logic Rifle
    public void Rifle(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //Logic Machine Gun
    public void MachineGun(WeaponStatSO weaponStat)
    {
        ShootingPerformed(weaponStat);
    }

    //Logic Shooting
    public void ShootingPerformed(WeaponStatSO weaponStat)
    {
        float x = Random.Range(-weaponStat.recoil, weaponStat.recoil);
        float y = Random.Range(-weaponStat.recoil, weaponStat.recoil);

        Vector3 bulletDirection = Camera.main.transform.forward + new Vector3(x, y, 0); 

        weaponStat.currBullet--;
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, bulletDirection, out hit, weaponStat.range, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(Camera.main.transform.position, hit.point, Color.red);
            Debug.Log(hit.point);
        }

    }

    

}

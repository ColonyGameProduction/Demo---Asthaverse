using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    
    public void ChangeWeapon()
    {

    }


    //Untuk menembak tergantung dari tag weapon nya    

    //logic Asault Rifle
    public void AsaultRifle(float fireRate, float bulletSpawn, float recoil, float damage)
    {
        SpawnBullet bullet = GetComponentInChildren<SpawnBullet>();
        
    }

    //logic Shotgun
    public void Shotgun(float fireRate, float bulletSpawn, float recoil, float damage)
    {
        SpawnBullet bullet = GetComponentInChildren<SpawnBullet>();
    }

    //logic Pistol
    public void Pistol(float fireRate, float bulletSpawn, float recoil, float damage)
    {
        SpawnBullet bullet = GetComponentInChildren<SpawnBullet>();
        bullet.SpawningBullet();



    }

}

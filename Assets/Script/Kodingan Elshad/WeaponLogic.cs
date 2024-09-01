using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{

    public void ChangeWeapon()
    {

    }


    //Untuk menembak tergantung dari tag weapon nya
    public void ExecuteShooting(GameObject gameObject)
    {
        string weaponType = gameObject.GetComponent<WeaponType>().Weapon();
        float fireRate;
        float spawnRate;

        switch (weaponType)
        {
            case "AssaultRifle":
                fireRate = 10;
                spawnRate = 1;
                AsaultRifle(fireRate, spawnRate);
                break;
            case "Pistol":
                fireRate = 1;
                spawnRate = 1;
                Pistol(fireRate, spawnRate);
                break;
            case "Shotgun":
                fireRate = 1;
                spawnRate = 3;
                Shotgun(fireRate, spawnRate);
                break;
        };        
    }

    //logic Asault Rifle
    public void AsaultRifle(float fireRate, float spawnRate)
    {
        SpawnBullet bullet = GetComponentInChildren<SpawnBullet>();
        bullet.SpawningBullet();
        bullet.SpawningBullet();
    }

    //logic Shotgun
    public void Shotgun(float fireRate, float spawnRate)
    {
        SpawnBullet bullet = GetComponentInChildren<SpawnBullet>();
        bullet.SpawningBullet();
        bullet.SpawningBullet();
        bullet.SpawningBullet();
    }

    //logic Pistol
    public void Pistol(float fireRate, float spawnRate)
    {
        SpawnBullet bullet = GetComponentInChildren<SpawnBullet>();
        bullet.SpawningBullet();
    }

}

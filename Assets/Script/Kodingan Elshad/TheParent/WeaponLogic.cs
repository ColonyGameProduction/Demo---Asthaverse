using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class WeaponLogicHandler
{    

    //Logic Shooting
    public void ShootingPerformed(Vector3 origin, Vector3 direction, EntityStatSO entityStat, WeaponStatSO weaponStat, LayerMask entityMask)
    {       
        weaponStat.currBullet -= weaponStat.bulletPerTap;
        if (weaponStat.bulletPerTap > 1)
        {
            for(int i = 0; i < weaponStat.bulletPerTap; i++)
            {
                BulletShoot(origin, direction, entityStat, weaponStat, entityMask);
            }

        }   
        else
        {
            BulletShoot(origin, direction, entityStat, weaponStat, entityMask);
        }

    }
    
    public void BulletShoot(Vector3 origin, Vector3 direction, EntityStatSO entityStat, WeaponStatSO weaponStat, LayerMask entityMask)
    {
        float recoilMod = weaponStat.recoil + ((100 - entityStat.acuracy) * weaponStat.recoil / 100);

        float x = Random.Range(-recoilMod, recoilMod);
        float y = Random.Range(-recoilMod, recoilMod);

        Vector3 recoil = new Vector3(x, y, 0);
        Vector3 bulletDirection = (direction + recoil).normalized;  

        // Debug.Log(origin + " " + direction + " " + weaponStat + " " + entityMask);

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, weaponStat.range, entityMask))
        {
            Debug.DrawRay(origin, bulletDirection * weaponStat.range, Color.black);

            GameObject entityGameObject = hit.collider.gameObject;

            CalculateDamage(weaponStat, entityGameObject);
            // Debug.Log(hit.point);

        }
        else
        {
            hit.point = origin + bulletDirection * weaponStat.range;
            Debug.DrawRay(origin, hit.point, Color.black);
            //Debug.Log(hit.point);
        }
    }

    public void CalculateDamage(WeaponStatSO weapon, GameObject entityGameObject)
    {
        Debug.Log("???");
        IHealth _getHealthFunction;
        _getHealthFunction =  entityGameObject.transform.GetComponent<IHealth>();
        if(_getHealthFunction == null) _getHealthFunction = entityGameObject.transform.GetComponentInParent<IHealth>();
        if(_getHealthFunction != null)
        {
            _getHealthFunction.Hurt(weapon.baseDamage);
            Debug.Log(entityGameObject.name + " Hit!" + " HP:" + _getHealthFunction.HealthNow);
        }
        // if(entityGameObject.CompareTag("Enemy"))
        // {
        //     EnemyAI enemy = entityGameObject.GetComponentInParent<EnemyAI>(); 
        //     float enemyHP = enemy.GetEnemyHP();

        //     enemyHP -= weapon.baseDamage;

        //     enemy.SetEnemyHP(enemyHP);
        //     Debug.Log("Enemy Hit!");
        // }
        // else if(entityGameObject.CompareTag("Player"))
        // {
        //     PlayerAction player = entityGameObject.GetComponentInParent<PlayerAction>();
            
        // }
    }

}

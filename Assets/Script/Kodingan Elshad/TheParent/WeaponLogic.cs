using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogicHandler : MonoBehaviour
{    

    //Logic Shooting
    public void ShootingPerformed(Vector3 origin, Vector3 direction, WeaponStatSO weaponStat, LayerMask entityMask)
    {
        float x = Random.Range(-weaponStat.recoil, weaponStat.recoil);
        float y = Random.Range(-weaponStat.recoil, weaponStat.recoil);

        Vector3 bulletDirection = (direction - origin).normalized + new Vector3(x, y, 0); 

        weaponStat.currBullet--;
        RaycastHit hit;
        if(Physics.Raycast(origin, bulletDirection, out hit, weaponStat.range, entityMask))
        {
            Debug.DrawRay(origin, bulletDirection * weaponStat.range, Color.black);

            GameObject entityGameObject = hit.collider.gameObject;

            CalculateDamage(weaponStat, entityGameObject);
            //Debug.Log(hit.point);

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
        
        if(entityGameObject.CompareTag("Enemy"))
        {
            EnemyAI enemy = entityGameObject.GetComponentInParent<EnemyAI>(); 
            float enemyHP = enemy.GetEnemyHP();

            enemyHP -= weapon.baseDamage;

            enemy.SetEnemyHP(enemyHP);
            Debug.Log("Enemy Hit!");
        }
        else if(entityGameObject.CompareTag("Player"))
        {
            PlayerAction player = entityGameObject.GetComponentInParent<PlayerAction>();
            
        }
    }

}
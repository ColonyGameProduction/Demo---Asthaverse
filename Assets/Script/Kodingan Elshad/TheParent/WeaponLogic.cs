using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class WeaponLogicHandler
{    
    
    //Logic Shooting
    public void ShootingPerformed(GameObject whoShoot, Vector3 origin, Vector3 direction, EntityStatSO entityStat, WeaponStatSO weaponStat, LayerMask entityMask, float recoil)
    {       
        weaponStat.currBullet -= weaponStat.bulletPerTap;
        if (weaponStat.bulletPerTap > 1)
        {
            for(int i = 0; i < weaponStat.bulletPerTap; i++)
            {
                BulletShoot(whoShoot, origin, direction, entityStat, weaponStat, entityMask, recoil);
            }

        }   
        else
        {
            BulletShoot(whoShoot, origin, direction, entityStat, weaponStat, entityMask, recoil);
        }

    }
    
    public void BulletShoot(GameObject whoShoot, Vector3 origin, Vector3 direction, EntityStatSO entityStat, WeaponStatSO weaponStat, LayerMask entityMask, float recoil)
    {
        float maxRecoilMod = recoil + ((100 - entityStat.acuracy) * recoil / 100);

        //Debug.Log(maxRecoilMod);

        float x = Random.Range(-maxRecoilMod/2, maxRecoilMod/2);
        float y = Random.Range(-maxRecoilMod, maxRecoilMod);

        Vector3 recoils = new Vector3(x, y, 0);
        Vector3 bulletDirection = (direction + recoils).normalized;  

        // Debug.Log(origin + " " + direction + " " + weaponStat + " " + entityMask);

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, weaponStat.range, entityMask))
        {
            Debug.DrawRay(origin, bulletDirection * weaponStat.range, Color.black);

            GameObject entityGameObject = hit.collider.gameObject;

            if (hit.transform.gameObject.GetComponent<BodyParts>() != null)
            {
                BodyParts body = hit.transform.gameObject.GetComponent<BodyParts>();

                ElshadCalculateDamage(whoShoot, origin, weaponStat, entityStat, entityGameObject, body.bodyType);
            }
            else
            {
                DamageWithoutBodyParts(whoShoot, origin, weaponStat, entityStat, entityGameObject);
            }
            
            // Debug.Log(hit.point);

        }
        else
        {
            hit.point = origin + bulletDirection * weaponStat.range;
            Debug.DrawRay(origin, hit.point, Color.black);
            //Debug.Log(hit.point);
        }
    }

    public void DamageWithoutBodyParts(GameObject whoShoot, Vector3 origin, WeaponStatSO weapon, EntityStatSO entityStat, GameObject entityGameObject)
    {
        if (entityGameObject.CompareTag("Enemy"))
        {
            EnemyAI enemy = entityGameObject.GetComponentInParent<EnemyAI>();
            float enemyHP = enemy.GetEnemyHP();
            enemyHP -= weapon.baseDamage;
            enemy.SetEnemyHP(enemyHP);
            Debug.Log("Enemy Hit!");
        }
        else if (entityGameObject.CompareTag("Player"))
        {
            if (entityGameObject.GetComponentInParent<PlayerAction>().enabled)
            {
                PlayerAction player = entityGameObject.GetComponentInParent<PlayerAction>();
                float playerHP = player.GetPlayerHP();
                player.InstantiateArrowDamage(whoShoot);
                player.SetPlayerHP(playerHP);
            }
            else
            {
                FriendsAI friends = entityGameObject.GetComponentInParent<FriendsAI>();
                float friendsHP = friends.GetFriendHP();
                friends.SetFriendsHP(friendsHP);
            }
        }
    }
    public void ElshadCalculateDamage(GameObject whoShoot, Vector3 origin, WeaponStatSO weapon, EntityStatSO entityStat, GameObject entityGameObject, bodyParts parts)
    {
        Debug.Log(entityGameObject);
         if(entityGameObject.CompareTag("Enemy"))
         {
            EnemyAI enemy = entityGameObject.GetComponentInParent<EnemyAI>(); 
            float enemyHP = enemy.GetEnemyHP();

            if(parts == bodyParts.head)
            {
                enemyHP -= ((weapon.baseDamage * weapon.headDamageMultiplier) - ((weapon.baseDamage * weapon.headDamageMultiplier) * (int)entityStat.armourType / 100));
                Debug.Log("Hit Head");
            }
            else if(parts == bodyParts.body)
            {
                enemyHP -= ((weapon.baseDamage) - ((weapon.baseDamage) * (int)entityStat.armourType / 100));
                Debug.Log("Hit Body");
            }
            else
            {
                enemyHP -= ((weapon.baseDamage * weapon.legDamageMultiplier) - ((weapon.baseDamage * weapon.legDamageMultiplier) * (int)entityStat.armourType / 100));
                Debug.Log("Hit Leg");
            }

            enemyHP -= weapon.baseDamage;

            enemy.SetEnemyHP(enemyHP);
            Debug.Log("Enemy Hit!");
         }
         else if(entityGameObject.CompareTag("Player"))
         {
            if(entityGameObject.GetComponentInParent<PlayerAction>().enabled)
            {
                PlayerAction player = entityGameObject.GetComponentInParent<PlayerAction>();
                float playerHP = player.GetPlayerHP();

                player.InstantiateArrowDamage(whoShoot);

                if (parts == bodyParts.head)
                {
                    playerHP -= ((weapon.baseDamage * weapon.headDamageMultiplier) - ((weapon.baseDamage * weapon.headDamageMultiplier) * (int)entityStat.armourType / 100));
                    Debug.Log("Hit Head");
                }
                else if (parts == bodyParts.body)
                {
                    playerHP -= ((weapon.baseDamage) - ((weapon.baseDamage) * (int)entityStat.armourType / 100));
                    Debug.Log("Hit Body");
                }
                else
                {
                    playerHP -= ((weapon.baseDamage * weapon.legDamageMultiplier) - ((weapon.baseDamage * weapon.legDamageMultiplier) * (int)entityStat.armourType / 100));
                    Debug.Log("Hit Leg");
                }

                player.SetPlayerHP(playerHP);
            }
            else
            {
                FriendsAI friends = entityGameObject.GetComponentInParent<FriendsAI>();
                float friendsHP = friends.GetFriendHP();

                if (parts == bodyParts.head)
                {
                    friendsHP -= ((weapon.baseDamage * weapon.headDamageMultiplier) - ((weapon.baseDamage * weapon.headDamageMultiplier) * (int)entityStat.armourType / 100));
                    Debug.Log("Hit Head");
                }
                else if (parts == bodyParts.body)
                {
                    friendsHP -= ((weapon.baseDamage) - ((weapon.baseDamage) * (int)entityStat.armourType / 100));
                    Debug.Log("Hit Body");
                }
                else
                {
                    friendsHP -= ((weapon.baseDamage * weapon.legDamageMultiplier) - ((weapon.baseDamage * weapon.legDamageMultiplier) * (int)entityStat.armourType / 100));
                    Debug.Log("Hit Leg");
                }

                friends.SetFriendsHP(friendsHP);
            }       
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
            Debug.Log(entityGameObject.name + " Hit!" + " HP:" + _getHealthFunction.CurrHealth);
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

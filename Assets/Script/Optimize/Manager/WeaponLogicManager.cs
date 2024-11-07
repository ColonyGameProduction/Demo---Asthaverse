using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogicManager : MonoBehaviour
{
    public bool Debugs;
    public static WeaponLogicManager Instance {get; private set;}
    private void Awake() 
    {
        Instance = this;
    }
    //Logic Shooting
    public void ShootingPerformed(Vector3 origin, Vector3 direction, float aimAccuracy, WeaponStatSO weaponStat, LayerMask entityMask, float shootRecoil)
    {       
        
        weaponStat.currBullet -= weaponStat.bulletPerTap;
        if (weaponStat.bulletPerTap > 1)
        {
            for(int i = 0; i < weaponStat.bulletPerTap; i++)
            {
                BulletShoot(origin, direction, aimAccuracy, weaponStat, entityMask, shootRecoil);
            }

        }   
        else
        {
            BulletShoot(origin, direction, aimAccuracy, weaponStat, entityMask, shootRecoil);
        }

    }
    
    public void BulletShoot(Vector3 origin, Vector3 direction, float aimAccuracy, WeaponStatSO weaponStat, LayerMask entityMask, float shootRecoil)
    {
        float recoilMod = shootRecoil + ((100 - aimAccuracy) * shootRecoil / 100);

        float x = Random.Range(-recoilMod/2, recoilMod/2);
        float y = Random.Range(-recoilMod, recoilMod);

        Vector3 recoil = new Vector3(x, y, 0);
        Vector3 bulletDirection = (direction + recoil).normalized; 

        // Debug.Log(origin + " " + direction + " " + weaponStat + " " + entityMask);

        RaycastHit hit;
        // Debug.Log(origin + " and " + bulletDirection);
        if (Physics.Raycast(origin, direction, out hit, weaponStat.range, entityMask))
        {
            Debug.DrawRay(origin, bulletDirection * weaponStat.range, Color.black);


            BodyParts body = hit.transform.gameObject.GetComponent<BodyParts>();
            if(body != null)
            {
                GameObject entityGameObject = hit.collider.gameObject;
                Debug.Log(entityGameObject.name + " di sini " + body);
                CalculateDamage(weaponStat, entityGameObject, body.bodyType);
            }
            else
            {
                Debug.Log("I hit Obstacle");
            }

            // Debug.Log(hit.point);

        }
        else
        {
            hit.point = origin + bulletDirection * weaponStat.range;
            Debug.DrawRay(origin, hit.point, Color.red);
            //Debug.Log(hit.point);
        }
    }

    public void CalculateDamage(WeaponStatSO weapon, GameObject entityGameObject, bodyParts hitBodyPart)
    {
        IHealth _getHealthFunction =  entityGameObject.transform.GetComponentInParent<IHealth>();

        // if(_getHealthFunction == null) _getHealthFunction = entityGameObject.transform.GetComponentInParent<IHealth>();
        // Debug.Log("Halooo????");
        if(_getHealthFunction != null)
        {
            float totalDamage = 0;
            if(hitBodyPart == bodyParts.head)
            {
                totalDamage = (weapon.baseDamage * weapon.headDamageMultiplier) - ((weapon.baseDamage * weapon.headDamageMultiplier) * ((int)_getHealthFunction.GetCharaArmourType/100));
                Debug.Log("I hit Head");
            }
            else if(hitBodyPart == bodyParts.body)
            {
                totalDamage = (weapon.baseDamage) - ((weapon.baseDamage) * ((int)_getHealthFunction.GetCharaArmourType/100));
                Debug.Log("I hit Body");
            }
            else if(hitBodyPart == bodyParts.leg)
            {
                totalDamage = (weapon.baseDamage * weapon.legDamageMultiplier) - ((weapon.baseDamage * weapon.legDamageMultiplier) * ((int)_getHealthFunction.GetCharaArmourType/100));
                Debug.Log("I hit Leg");
            }
            if(!Debugs)_getHealthFunction.Hurt(totalDamage);
            Debug.Log(entityGameObject.name + " Hit!" + " HP:" + _getHealthFunction.CurrHealth);
        }
 
    }
}

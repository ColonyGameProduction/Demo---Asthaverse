using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponLogicManager : MonoBehaviour
{
    public bool Debugs;
    public bool DebugPart2;
    public bool DebugDrawBiasa = true;
    public static WeaponLogicManager Instance {get; private set;}
    private void Awake() 
    {
        Instance = this;
    }
    //Logic Shooting
    public void ShootingPerformed(Vector3 origin, Vector3 direction, float aimAccuracy, WeaponStatSO weaponStat, LayerMask entityMask, float shootRecoil, Vector3 gunOriginShootPoint, bool isAIInput)
    {       
        
        weaponStat.currBullet -= weaponStat.bulletPerTap;
        if (weaponStat.bulletPerTap > 1)
        {
            for(int i = 0; i < weaponStat.bulletPerTap; i++)
            {
                BulletShoot(origin, direction, aimAccuracy, weaponStat, entityMask, shootRecoil, gunOriginShootPoint, isAIInput);
            }

        }   
        else
        {
            BulletShoot(origin, direction, aimAccuracy, weaponStat, entityMask, shootRecoil, gunOriginShootPoint, isAIInput);
        }

    }
    
    public void BulletShoot(Vector3 origin, Vector3 direction, float aimAccuracy, WeaponStatSO weaponStat, LayerMask entityMask, float shootRecoil, Vector3 gunOriginShootPoint, bool isAIInput)
    {
        float recoilMod = shootRecoil + ((100 - aimAccuracy) * shootRecoil / 100);

        float x = Random.Range(-recoilMod/2, recoilMod/2);
        float y = Random.Range(-recoilMod, recoilMod);

        Vector3 recoil = new Vector3(x, y, 0);
        Vector3 bulletDirection = (direction + recoil).normalized; 

        // Debug.Log(origin + " " + direction + " " + weaponStat + " " + entityMask);

        RaycastHit hit;
        // Debug.Log(origin + " and " + bulletDirection);
        Debug.Log("I hit" + origin + " sebelum berubah");
        if(!DebugPart2)if(!isAIInput)origin = gunOriginShootPoint;
        Debug.Log("I hit" + origin + " sesudah berubah");
        if(DebugDrawBiasa)Debug.DrawRay(gunOriginShootPoint, bulletDirection * weaponStat.range, Color.black, 0.2f, false);
        if (Physics.Raycast(origin, direction, out hit, weaponStat.range, entityMask))
        {
            

            BodyParts body = hit.transform.gameObject.GetComponent<BodyParts>();
            if(body != null)
            {
                float dis = Vector3.Distance(origin, hit.point);
                if(!isAIInput)Debug.DrawRay(origin, bulletDirection * dis, Color.blue, 0.2f, false);
                else Debug.DrawRay(gunOriginShootPoint, bulletDirection * dis, Color.red, 0.2f, false);
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
            // Debug.DrawRay(origin, hit.point, Color.red);
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

using System;
using System.Collections;
using UnityEngine;

public class WeaponLogicManager : MonoBehaviour
{
    public bool Debugs;
    public bool DebugDrawBiasa = true;
    public static WeaponLogicManager Instance {get; private set;}


    public Action<Transform, CharacterIdentity> OnCharacterGotHurt;
    private void Awake() 
    {
        Instance = this;
    }
    //Logic Shooting
    public void ShootingPerformed(Transform shooter, Vector3 origin, Vector3 direction, float aimAccuracy, WeaponStatSO weaponStat, LayerMask entityMask, float shootRecoil, Vector3 gunOriginShootPoint, bool isAIInput, bool isInsideWall, WeaponShootVFX weaponShootVFX)
    {       
        
        weaponStat.currBullet -= weaponStat.bulletPerTap;
        if (weaponStat.bulletPerTap > 1)
        {
            for(int i = 0; i < weaponStat.bulletPerTap; i++)
            {
                BulletShoot(shooter, origin, direction, aimAccuracy, weaponStat, entityMask, shootRecoil, gunOriginShootPoint, isAIInput, isInsideWall, weaponShootVFX);
            }

        }   
        else
        {
            BulletShoot(shooter, origin, direction, aimAccuracy, weaponStat, entityMask, shootRecoil, gunOriginShootPoint, isAIInput, isInsideWall, weaponShootVFX);
        }

    }
    
    public void BulletShoot(Transform shooter, Vector3 origin, Vector3 direction, float aimAccuracy, WeaponStatSO weaponStat, LayerMask entityMask, float shootRecoil, Vector3 gunOriginShootPoint, bool isAIInput, bool isInsideWall, WeaponShootVFX weaponShootVFX)
    {
        bool isHitBody = false;
        bool isHitAnything = false;
        float recoilMod = shootRecoil + ((100 - aimAccuracy) * shootRecoil / 100);

        float x = UnityEngine.Random.Range(-recoilMod*0.9F, recoilMod*0.9F);
        float y = UnityEngine.Random.Range(-recoilMod, recoilMod);

        Vector3 recoil = new Vector3(x, y, 0);
        Vector3 bulletDirection = (direction + recoil).normalized; 

        // Debug.Log(origin + " " + direction + " " + weaponStat + " " + entityMask);

        RaycastHit hit;
        // Debug.Log(origin + " and " + bulletDirection);
        // Debug.Log("I hit" + origin + " sebelum berubah");
        // if(DebugDrawBiasa)Debug.DrawRay(origin, bulletDirection * weaponStat.range, Color.magenta, 0.2f, false);


        // if(!DebugPart2)if(!isAIInput)origin = gunOriginShootPoint;

        // Debug.Log("I hit" + origin + " sesudah berubah");
        // if(DebugDrawBiasa)Debug.DrawRay(gunOriginShootPoint, bulletDirection * weaponStat.range, Color.black, 0.2f, false);
        // if(DebugDrawBiasa)Debug.DrawRay(origin, bulletDirection * weaponStat.range, Color.red, 0.2f, false);

        weaponShootVFX.CallGunFlash(gunOriginShootPoint);

        if (Physics.Raycast(origin, bulletDirection, out hit, weaponStat.range, entityMask))
        {
            isHitAnything = true;
            if(!isAIInput)
            {
                if(!isInsideWall)
                {
                    Vector3 newDirBasedOnGunPoint = (hit.point - gunOriginShootPoint).normalized;
                    if (Physics.Raycast(gunOriginShootPoint, newDirBasedOnGunPoint, out hit, weaponStat.range, entityMask))
                    {
                        BodyParts body = hit.transform.gameObject.GetComponent<BodyParts>();
                        if(body != null)
                        {
                            isHitBody = true;
                            // float dis = Vector3.Distance(origin, hit.point);

                            GameObject entityGameObject = hit.collider.gameObject;
                            // Debug.Log(entityGameObject.name + " di sini " + body);
                            CalculateDamage(shooter, weaponStat, entityGameObject, body.bodyType);
                        }
                        else
                        {
                            isHitBody = false;
                            // Debug.Log("I hit Obstacle");
                        }
                    }
                    else
                    {
                        isHitAnything = false;
                        hit.point = origin + bulletDirection * weaponStat.range;
                    }
                }
                else
                {
                    Vector3 newDirOriginToGunOrigin = (gunOriginShootPoint - origin).normalized;
                    // Debug.DrawRay(origin, newDirOriginToGunOrigin * 20, Color.red, 2f, false);
                    if (Physics.Raycast(origin, newDirOriginToGunOrigin, out hit, weaponStat.range, entityMask))
                    {
                        // Debug.Log("Hit pointnya adalahh" + hit.point + " WAT" + gunOriginShootPoint);
                        isHitBody = false;
                        BodyParts body = hit.transform.gameObject.GetComponent<BodyParts>();
                        if(body != null)
                        {
                            isHitBody = true;
                            // float dis = Vector3.Distance(origin, hit.point);

                            GameObject entityGameObject = hit.collider.gameObject;
                            // Debug.Log(entityGameObject.name + " di sini " + body);
                            CalculateDamage(shooter, weaponStat, entityGameObject, body.bodyType);
                        }
                        else
                        {
                            isHitBody = false;
                            // Debug.Log("I hit Obstacle");
                        }
                    }
                }

            }
            else
            {
                // Debug.Log("Hit point now" + hit.point);
                BodyParts body = hit.transform.gameObject.GetComponent<BodyParts>();
                if(body != null)
                {
                    isHitBody = true;
                    // float dis = Vector3.Distance(origin, hit.point);

                    GameObject entityGameObject = hit.collider.gameObject;
                    // Debug.Log(entityGameObject.name + " di sini " + body);
                    CalculateDamage(shooter, weaponStat, entityGameObject, body.bodyType);
                }
                else
                {
                    isHitBody = false;
                    // Debug.Log("I hit Obstacle");
                }
            }

            

            // Debug.Log(hit.point);

        }
        else
        {
            isHitAnything = false;
            hit.point = origin + bulletDirection * weaponStat.range;
            // Debug.DrawRay(origin, hit.point, Color.red);
            //Debug.Log(hit.point);
        }
        // TrailRenderer trail = Instantiate(_bulletTrailPrefab, gunOriginShootPoint, Quaternion.identity);

        // if(isInsideWall)gunOriginShootPoint = origin;
        StartCoroutine(SpawnTrail(weaponShootVFX.GetWeaponVFX(), gunOriginShootPoint, hit, weaponShootVFX, isHitBody, isHitAnything));
        Vector3 dir = (hit.point - gunOriginShootPoint).normalized;
        if(DebugDrawBiasa)Debug.DrawRay(gunOriginShootPoint, dir * 100f, Color.red, 1f, false);
    }

    public void CalculateDamage(Transform shooter, WeaponStatSO weapon, GameObject entityGameObject, bodyParts hitBodyPart)
    {
        IHealth _getHealthFunction =  entityGameObject.transform.GetComponentInParent<IHealth>();

        CharacterIdentity character = entityGameObject.transform.GetComponentInParent<CharacterIdentity>();
        OnCharacterGotHurt?.Invoke(shooter, character);
        // if(_getHealthFunction == null) _getHealthFunction = entityGameObject.transform.GetComponentInParent<IHealth>();
        // Debug.Log("Halooo????");
        if(_getHealthFunction != null)
        {
            float totalDamage = 0;
            if(hitBodyPart == bodyParts.head)
            {
                totalDamage = (weapon.baseDamage * weapon.headDamageMultiplier) - ((weapon.baseDamage * weapon.headDamageMultiplier) * ((int)_getHealthFunction.GetCharaArmourType/100));
                // Debug.Log("I hit Head");
            }
            else if(hitBodyPart == bodyParts.body)
            {
                totalDamage = (weapon.baseDamage) - ((weapon.baseDamage) * ((int)_getHealthFunction.GetCharaArmourType/100));
                // Debug.Log("I hit Body");
            }
            else if(hitBodyPart == bodyParts.leg)
            {
                totalDamage = (weapon.baseDamage * weapon.legDamageMultiplier) - ((weapon.baseDamage * weapon.legDamageMultiplier) * ((int)_getHealthFunction.GetCharaArmourType/100));
                // Debug.Log("I hit Leg");
            }
            if(!Debugs)_getHealthFunction.Hurt(totalDamage);
            // Debug.Log(entityGameObject.name + " Hit!" + " HP:" + _getHealthFunction.CurrHealth);
        }
 
    }
    private IEnumerator SpawnTrail(WeaponVFX weaponVFX, Vector3 origin, RaycastHit hit, WeaponShootVFX weaponShootVFX, bool isHitBody, bool isHitAnything)
    {
        float timer = 0;
        TrailRenderer trail = weaponVFX.bulletTrail;
        trail.transform.parent = null;
        trail.transform.position = origin;
        trail.transform.rotation = Quaternion.identity;
        Vector3 startPos = trail.transform.position;
        trail.gameObject.SetActive(true);

        while(timer < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, timer);
            timer += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hit.point;
        
        if(!isHitBody && isHitAnything)
        {
            // Instantiate(_impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
            ParticleSystem particle = weaponVFX.impactParticle;
            particle.transform.parent = null;
            particle.transform.position = hit.point;
            particle.transform.rotation = Quaternion.LookRotation(hit.normal);
            StartCoroutine(SpawnParticle(particle, weaponShootVFX));
        }
        if(DebugDrawBiasa)Debug.DrawRay(hit.point, hit.normal * 50, Color.red, 0.2f, false);
        // Debug.LogError("Stop");
        // Destroy(trail.gameObject, trail.time);
        weaponShootVFX.SetVFXBackToNormal(trail.transform);
    }
    private IEnumerator SpawnParticle(ParticleSystem impactParticle, WeaponShootVFX weaponShootVFX)
    {
        // float timer = 0;
        impactParticle.gameObject.SetActive(true);
        impactParticle.Play();
        // while(timer < 1)
        // {
        //     timer += Time.deltaTime / impactParticle.time;

        //     yield return null;
        // }

        // impactParticle.Stop();
        yield return new WaitUntil(() => impactParticle.isStopped);
        impactParticle.Stop();
        weaponShootVFX.SetVFXBackToNormal(impactParticle.transform);
    }
}

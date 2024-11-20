using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponVFX
{
    public WeaponVFX(TrailRenderer trail, ParticleSystem particle)
    {
        bulletTrail = trail;
        impactParticle = particle;
    }
    public TrailRenderer bulletTrail;
    public ParticleSystem impactParticle;
}
[Serializable]
public class WeaponsVFX
{
    public WeaponsVFX(List<WeaponVFX> newWeaponVFXPool)
    {
        weaponVFXPool = newWeaponVFXPool;
        poolIdx = 0;
    }
    public int poolIdx;
    public List<WeaponVFX> weaponVFXPool;
}

public class WeaponShootVFX : MonoBehaviour
{
    [SerializeField]private Transform trailParent;
    private List<WeaponsVFX> _weaponsVFXPool = new List<WeaponsVFX>();

    [Header("Prefab")]
    [SerializeField]private ParticleSystem _impactParticlePrefab;
    private ParticleSystem _gunFlash;
    [SerializeField]private int _totalBuffer = 10;
    public int CurrWeaponIdx{get; set;}

    public void SpawnTrail(int total, Vector3 originShootPoint, TrailRenderer _bulletTrailPrefab, ParticleSystem _gunFlashPrefab)
    {
        if(_gunFlash == null)
        {
            _gunFlash = Instantiate(_gunFlashPrefab, originShootPoint, Quaternion.identity, trailParent); 
            _gunFlash.Stop();
        }
        List<WeaponVFX> weaponVFXList = new List<WeaponVFX>();

        for(int i=0; i < total + _totalBuffer; i++)
        {
            TrailRenderer trail = Instantiate(_bulletTrailPrefab, originShootPoint, Quaternion.identity, trailParent);
            trail.gameObject.SetActive(false);

            ParticleSystem particle = Instantiate(_impactParticlePrefab, originShootPoint, Quaternion.identity, trailParent);
            particle.gameObject.SetActive(false);
            particle.Stop();
            
            WeaponVFX weaponVFX = new WeaponVFX(trail, particle);
            weaponVFXList.Add(weaponVFX);
        }
        WeaponsVFX newWeaponTrailList = new WeaponsVFX(weaponVFXList);
        _weaponsVFXPool.Add(newWeaponTrailList);
    }
    public WeaponVFX GetWeaponVFX()
    {
        WeaponsVFX weaponsVFX = _weaponsVFXPool[CurrWeaponIdx];
        WeaponVFX chosen = weaponsVFX.weaponVFXPool[weaponsVFX.poolIdx];
        weaponsVFX.poolIdx += 1;
        if(weaponsVFX.poolIdx == weaponsVFX.weaponVFXPool.Count)
        {
            weaponsVFX.poolIdx = 0;
        }
        return chosen;
    }
    public void SetVFXBackToNormal(Transform VFX)
    {
        VFX.gameObject.SetActive(false);
        VFX.transform.parent = trailParent;
        VFX.transform.rotation = Quaternion.identity;
    }
    public void CallGunFlash(Vector3 position)
    {
        _gunFlash.transform.position = position;
        _gunFlash.Play();
    }

}

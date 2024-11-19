using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponShootVFX : MonoBehaviour
{
    [SerializeField]private Transform _bulletTrailPrefab;
    private ObjectPool<TrailRenderer> _trailPool;

    public void SpawnTrail(Transform Parent)
    {
        // _trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        
    }
}

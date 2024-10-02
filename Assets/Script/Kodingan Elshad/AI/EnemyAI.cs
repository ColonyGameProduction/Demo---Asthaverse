using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : ExecuteLogic
{
    private float timer;
    
    //hal yang diperlukan untuk pathfinding
    private NavMeshAgent enemyNavmesh;
    [SerializeField] private GameObject destination;

    [Header("Untuk mengkalkulasi sudut benda")]
    [SerializeField] private int edgeResolveIteration;
    [SerializeField] private float edgeDistanceTreshold;
    [Header("Untuk Besarnya FOV")]
    [SerializeField] private float viewRadius;
    [Range(0, 360)]
    [SerializeField] private float viewAngle;
    [Header("FOV Resolution")]
    [SerializeField] private float meshResolution;
    [Header("")]
    [SerializeField] private Transform FOVPoint;
    [SerializeField] private List<Transform> visibleTargets = new List<Transform>();
    [Header("Misc")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    [Header("Untuk Stat")]
    [SerializeField]
    private EntityStatSO enemyStat;
    [SerializeField]
    private float enemyHP;
    private bool isReloading;
    private bool fireRateOn;
    private WeaponStatSO weapon;
    public LayerMask isItEnemy;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine(FindTargetWithDelay(.2f));
        enemyNavmesh = GetComponent<NavMeshAgent>();
        enemyHP = enemyStat.health;
        weapon = enemyStat.weaponStat[0];
    }

    private void LateUpdate()
    {
        DrawFieldOfView(edgeResolveIteration, edgeDistanceTreshold, viewRadius, viewAngle, meshResolution, FOVPoint, viewMesh, groundMask);
    }

    private void Update()
    {
        Moving();
        if(enemyHP <= 0)
        {
            Debug.Log("Dead");
        }

        if(!fireRateOn && !isReloading)
        {
            Shooting();
            StartCoroutine(FireRate(FireRateFlag, weapon.fireRate));
            if (weapon.currBullet == 0)
            {
                isReloading = true;
                Reload(weapon);
                StartCoroutine(ReloadTime(ReloadFlag, weapon.reloadTime));
            }

        }
    }      

    //pathfinding untuk enemy
    private void Moving()
    {
        if (destination != null)
        {
            MoveToDestination(enemyNavmesh, destination.transform.position);
        }
        else
        {
            MoveToDestination(enemyNavmesh, visibleTargets[0].position);
        }
    }

    private void Shooting()
    {
        Shoot(FOVPoint.position, visibleTargets[0].position, weapon, isItEnemy);
    }

    public IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargetsForEnemy(viewRadius, viewAngle, visibleTargets, FOVPoint, playerMask, groundMask);
        }
    }

    public float GetEnemyHP()
    {
        return enemyHP;
    }

    public void SetEnemyHP(float hp)
    {
        enemyHP = hp;
    }

    private void ReloadFlag(bool value)
    {
        isReloading = value;
    }

    private void FireRateFlag(bool value)
    {
        fireRateOn = value;
    }

}

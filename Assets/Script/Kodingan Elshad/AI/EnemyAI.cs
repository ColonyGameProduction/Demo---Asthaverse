using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : ExecuteLogic
{
    public bool stopMoving;
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

    //Untuk Alert
    alertState enemyState;
    [SerializeField]
    private float alertValue;

    private void Start()
    {
        enemyState = alertState.Idle;
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
        switch(enemyState)
        {
            case alertState.Idle:
                Moving();
                Debug.Log("idle");
                break;
            case alertState.Hunted:
                Debug.Log("hunted");
                break;
            case alertState.Engage:
                Debug.Log("engage");
                destination = visibleTargets[0].gameObject;
                Shoot();
                break;
        }

        if(enemyHP <= 0)
        {
            Debug.Log("Dead");
        }

        if (visibleTargets.Count > 0)
        {

            if (alertValue <= 90)
            {
                alertValue += Time.deltaTime * 10;
            }

            if (alertValue == 0)
            {
                enemyState = alertState.Idle;
            }
            else if (alertValue >= 90 / 2 && alertValue < 90)
            {
                enemyState = alertState.Hunted;
            }
            else if (alertValue >= 90)
            {
                enemyState = alertState.Engage;
            }
        }
        else
        {
            if (alertValue >= 0)
            {
                alertValue -= Time.deltaTime * 10;
            }
        }
        
    }      

    private void Shoot()
    {
        if (!fireRateOn && !isReloading)
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
        if(stopMoving)
        {
            
            return;
        }
        if (destination != null)
        {
            MoveToDestination(enemyNavmesh, destination.transform.position);
        }
        
    }

    private void Shooting()
    {
        if(visibleTargets.Count > 0)Shoot(FOVPoint.position, visibleTargets[0].position, weapon, isItEnemy);
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

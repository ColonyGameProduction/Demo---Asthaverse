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
    [SerializeField] private List<Transform> otherVisibleTargets = new List<Transform>();
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
    FOVDistState FOVState;
    [SerializeField]
    private float alertValue;
    [SerializeField]
    private float maxAlertValue;

    //untuk patrol
    [SerializeField]
    private Vector3[] patrolPath;
    private bool switchingPath;
    private int currPath;
    private Vector3 lastSeenPosition;

    private void Start()
    {
        currPath = 0;
        patrolPath[0] = transform.position;

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
        ChangingState();
        switch (enemyState)
        {

            case alertState.Idle:
                if(visibleTargets.Count == 0)
                {
                    enemyNavmesh.isStopped = false;
                    Patrol();
                }
                else
                {
                    enemyNavmesh.isStopped = true;
                    lastSeenPosition = visibleTargets[0].position;
                    
                }
                break;
            case alertState.Hunted:
                enemyNavmesh.isStopped = false;
                if (otherVisibleTargets.Count > 0)
                {
                    Moving(otherVisibleTargets[0].position);
                }
                else
                {
                    if (lastSeenPosition != Vector3.zero)
                    {
                        Moving(lastSeenPosition);
                    }
                    else
                    {
                        enemyNavmesh.isStopped = true;
                    }

                    if (Vector3.Distance(transform.position, lastSeenPosition) < 0.5f)
                    {
                        lastSeenPosition = Vector3.zero;
                    }
                }
                break;
            case alertState.Engage:
                FOVStateHandler();
                Shoot();
                break;
        }

        if(enemyHP <= 0)
        {
            Debug.Log("Dead");
        }


    }      

    private void ChangingState()
    {
        if (visibleTargets.Count > 0)
        {
            foreach(Transform transform in visibleTargets)
            {
                if (transform.GetComponent<PlayerAction>().enabled == true)
                {
                    if(maxAlertValue > transform.GetComponent<PlayerAction>().GetPlayerStat().stealth || maxAlertValue == 0)
                    {
                        maxAlertValue = transform.GetComponent<PlayerAction>().GetPlayerStat().stealth;
                    }
                }
                else
                {
                    if (maxAlertValue > transform.GetComponent<FriendsAI>().GetFriendsStat().stealth || maxAlertValue == 0)
                    {
                        maxAlertValue = transform.GetComponent<FriendsAI>().GetFriendsStat().stealth;
                    }
                }
            }
            

            if (alertValue <= maxAlertValue)
            {
                alertValue += Time.deltaTime * 10;
            }
        }
        else
        {
            if (alertValue >= 0 && otherVisibleTargets.Count == 0 && lastSeenPosition == Vector3.zero)
            {
                alertValue -= Time.deltaTime * 10;
            }
        }


        if (alertValue <= maxAlertValue/2)
        {
            enemyState = alertState.Idle;
        }
        else if (alertValue >= maxAlertValue / 2 && alertValue < maxAlertValue)
        {
            enemyState = alertState.Hunted;
        }
        else if (alertValue >= maxAlertValue)
        {
            enemyState = alertState.Engage;
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

    private void Patrol()
    {

        if (patrolPath.Length > 1)
        {
            if (Vector3.Distance(transform.position, patrolPath[currPath]) < 0.1f)
            {
                if (!switchingPath)
                {
                    currPath++;
                }
                else
                {
                    currPath--;
                }

                if (currPath == patrolPath.Length - 1)
                {
                    switchingPath = true;
                }
                else if (currPath == 0)
                {
                    switchingPath = false;
                }
            }
            Moving(patrolPath[currPath]);
        }

    }

    //pathfinding untuk enemy
    private void Moving(Vector3 destination)
    {
        if(stopMoving)
        {
            
            return;
        }
        if (destination != null)
        {
            MoveToDestination(enemyNavmesh, destination);
        }
        
    }

    private void Shooting()
    {
        if(visibleTargets.Count > 0)Shoot(FOVPoint.position, visibleTargets[0].transform.position, weapon, isItEnemy);
    }

    public IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargetsForEnemy(viewRadius, viewAngle, visibleTargets, FOVPoint, playerMask, groundMask);
            SplittingTheObject();
        }
    }

    private void SplittingTheObject()
    {
        List<Transform> toRemove = new List<Transform>();
        toRemove.Clear();
        otherVisibleTargets.Clear();

        foreach (Transform transform in visibleTargets)
        {
            if (!transform.gameObject.CompareTag("Player"))
            {
                otherVisibleTargets.Add(transform);
                toRemove.Add(transform);
            }
        }
        foreach (Transform transform in toRemove)
        {
            visibleTargets.Remove(transform);
        }
    }

    private void FOVStateHandler()
    {
        float distance;
        if (visibleTargets.Count > 0)
        {
            distance = Vector3.Distance(transform.position, visibleTargets[0].position);
            lastSeenPosition = visibleTargets[0].position;
        }
        else
        {            
            distance = Vector3.Distance(transform.position, lastSeenPosition);
        }


        if (distance <= viewRadius && distance > viewRadius - (viewRadius/3))
        {
            FOVState = FOVDistState.far;
        }
        else if(distance <= viewRadius - (viewRadius / 3) && distance > viewRadius - (viewRadius / 3 * 2))
        {
            FOVState = FOVDistState.middle;
        }
        else if(distance <= viewRadius - (viewRadius / 3*2) && distance >= 0)
        {
            FOVState = FOVDistState.close;
        }

        switch(FOVState)
        {
            case FOVDistState.far:                
                Moving(visibleTargets[0].position);
                Shoot();
                break;
            case FOVDistState.middle:
                Shoot();
                break; 
            case FOVDistState.close:
                Shoot();
                break;
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

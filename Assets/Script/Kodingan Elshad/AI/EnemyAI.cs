using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;


public class EnemyAI : ExecuteLogic
{
    EnemyManager EM;

    public bool stopMoving;
    private float timer;
    //hal yang diperlukan untuk pathfinding
    [SerializeField]
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
    [SerializeField] private Transform POI;
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
    private float tempAlertValue;

    //untuk patrol
    [SerializeField]
    private Vector3[] patrolPath;
    private bool switchingPath;
    private int currPath;
    private Vector3 lastSeenPosition;
    private float distance;
    private float tempDistance;
    private float tempPOIDistance;
    private Vector3 tempLastSeenPos;
    private Transform tempPOI;

    

    private void Start()
    {
        EM = EnemyManager.instance;

        EM.enemyHunted += CombatStateHunted;
        EM.closestPOI += ClosestPOI;
        EM.enemyEngage += CombatStateEngage;

        distance = 0;
        currPath = 0;
        patrolPath[0] = transform.position;

        enemyState = alertState.Idle;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine(FindTargetWithDelay(.2f));
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
                if (visibleTargets.Count == 0)
                {
                    enemyNavmesh.speed = enemyStat.speed;
                    enemyNavmesh.isStopped = false;
                    Patrol();
                }
                else
                {
                    Vector3 dir = visibleTargets[0].position - transform.position;
                    enemyNavmesh.speed = 0;
                    transform.forward = Vector3.Slerp(transform.forward, dir, enemyNavmesh.angularSpeed).normalized;
                    lastSeenPosition = visibleTargets[0].position;
                    
                }
                break;
            case alertState.Hunted:
                if(POI == null)
                {
                    if(visibleTargets.Count != 0 || otherVisibleTargets.Count != 0)
                    {
                        EM.enemyHunted?.Invoke(this);
                    }
                    else if(visibleTargets.Count == 0 && otherVisibleTargets.Count == 0)
                    {
                        if(EM.enemyCaptain.Contains(this))
                        {
                            EM.enemyCaptain.Remove(this);
                        }
                    }
                }

                enemyNavmesh.speed = enemyStat.speed;
                enemyNavmesh.isStopped = false;
                if (otherVisibleTargets.Count > 0)
                {
                    lastSeenPosition = otherVisibleTargets[0].position;
                    Moving(otherVisibleTargets[0].position);
                }
                else
                {
                    if (lastSeenPosition != Vector3.zero && otherVisibleTargets.Count == 0)
                    {
                        Moving(lastSeenPosition);
                    }
                    else if(otherVisibleTargets.Count != 0 && lastSeenPosition == Vector3.zero)
                    {
                        lastSeenPosition = otherVisibleTargets[0].position;
                        Moving(otherVisibleTargets[0].position);
                    }
                    else if (otherVisibleTargets.Count == 0 && lastSeenPosition == Vector3.zero && POI != null)
                    {
                        Moving(POI.position);
                    }
                    else
                    {
                        enemyNavmesh.isStopped = true;
                    }

                    if (Vector3.Distance(transform.position, lastSeenPosition) < 0.5f)
                    {
                        EM.lastPosIsFound.Invoke(this);
                        lastSeenPosition = Vector3.zero;
                    }

                    if (POI != null && Vector3.Distance(transform.position, POI.position) < 0.5f)
                    {
                        if(EM.POIPosList.Count > 0)
                        {
                            ClosestPOI();
                        }
                        else
                        {
                            EM.tempPOIPosList.Clear();
                            if(EM.enemyList.Contains(this))
                            {
                                EM.enemyList.Remove(this);
                            }
                            POI = null;
                        }
                    }
                }
                break;
            case alertState.Engage:
                enemyNavmesh.speed = enemyStat.speed;
                enemyNavmesh.isStopped = false;

                if(visibleTargets.Count != 0 || otherVisibleTargets.Count != 0)
                {
                    EM.enemyEngage?.Invoke(this);
                }
                else if (visibleTargets.Count == 0 && otherVisibleTargets.Count == 0)
                {
                    if (EM.enemyCaptain.Contains(this))
                    {
                        EM.enemyCaptain.Remove(this);
                    }
                }

                FOVStateHandler();
                Shoot();
                break;
        }

        if(enemyHP <= 0)
        {
            Debug.Log("Dead");
        }


    }      

    private void ClosestPOI()
    {
        if(!EM.enemyList.Contains(this))
        {
            return;
        }

        tempPOIDistance = 0;
        tempPOI = null;

        if(EM.POIPosList.Count > 0)
        {
            foreach (Transform currPOI in EM.POIPosList)
            {

                if (tempPOIDistance > Vector3.Distance(transform.position, currPOI.position) || tempPOIDistance == 0)
                {
                    tempPOIDistance = Vector3.Distance(transform.position, currPOI.position);
                    tempPOI = currPOI;
                }
            }

            enemyState = alertState.Hunted;
            lastSeenPosition = Vector3.zero;
            POI = tempPOI;
            EM.POIPosList.Remove(POI);
        }

    }

    private void CombatStateHunted(EnemyAI enemy)
    {
        if(enemy == this)
        {
            if (!EM.enemyList.Contains(this))
            {
                EM.enemyList.Add(enemy);
            }

            if (!EM.enemyCaptain.Contains(this))
            {
                EM.enemyCaptain.Add(enemy);
            }

            return;
        }        

        if(Vector3.Distance(enemy.transform.position, transform.position) < 100f || EM.enemyList.Contains(this))
        {
            maxAlertValue = enemy.maxAlertValue;
            alertValue = enemy.alertValue;
            enemyState = alertState.Hunted;

            tempLastSeenPos = Vector3.zero;

            foreach (EnemyAI enemyAI in EM.enemyCaptain)
            {
                if (Vector3.Distance(tempLastSeenPos, transform.position) > Vector3.Distance(enemyAI.lastSeenPosition, transform.position) || tempLastSeenPos == Vector3.zero)
                {
                    tempLastSeenPos = enemyAI.lastSeenPosition;
                }
            }

            lastSeenPosition = tempLastSeenPos;

            if (!EM.enemyList.Contains(this))
            {
                EM.enemyList.Add(this);
            }
        }
    }

    private void CombatStateEngage(EnemyAI enemy)
    {
        EM.POIPosList.Clear();
        EM.tempPOIPosList.Clear();

        if (enemy == this)
        {
                if (!EM.enemyList.Contains(this))
            {
                EM.enemyList.Add(enemy);
            }

            if (!EM.enemyCaptain.Contains(this))
            {
                EM.enemyCaptain.Add(enemy);
            }

            return;
        }

        if (Vector3.Distance(enemy.transform.position, transform.position) < 100f || EM.enemyList.Contains(this))
        {
            maxAlertValue = enemy.maxAlertValue;
            alertValue = enemy.alertValue;
            enemyState = alertState.Engage;

            tempLastSeenPos = Vector3.zero;

            foreach (EnemyAI enemyAI in EM.enemyCaptain)
            {
                if (Vector3.Distance(tempLastSeenPos, transform.position) > Vector3.Distance(enemyAI.lastSeenPosition, transform.position) || tempLastSeenPos == Vector3.zero)
                {
                    tempLastSeenPos = enemyAI.lastSeenPosition;
                }
            }

            lastSeenPosition = tempLastSeenPos;

            if (!EM.enemyList.Contains(this))
            {
                EM.enemyList.Add(this);
            }
        }
    }

    private void ChangingState()
    {
        if (visibleTargets.Count > 0)
        {
            foreach (Transform transform in visibleTargets)
            {


                if (transform.GetComponent<PlayerAction>().enabled == true)
                {
                    if (maxAlertValue > transform.GetComponent<PlayerAction>().GetPlayerStat().stealth || maxAlertValue == 0)
                    {
                        if(enemyState == alertState.Idle)
                        {
                            tempAlertValue = transform.GetComponent<PlayerAction>().GetPlayerStat().stealth;
                        }
                        else
                        {
                            maxAlertValue = transform.GetComponent<PlayerAction>().GetPlayerStat().stealth;
                        }
                    }
                }
                else
                {
                    if (maxAlertValue > transform.GetComponent<FriendsAI>().GetFriendsStat().stealth || maxAlertValue == 0)
                    {
                        if (enemyState == alertState.Idle)
                        {
                            tempAlertValue = transform.GetComponent<FriendsAI>().GetFriendsStat().stealth;
                        }
                        else
                        {
                            maxAlertValue = transform.GetComponent<FriendsAI>().GetFriendsStat().stealth;
                        }
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
            if (alertValue >= 0 && otherVisibleTargets.Count == 0 && lastSeenPosition == Vector3.zero && POI == null)
            {
                alertValue -= Time.deltaTime * 10;
            }
        }

        if(enemyState == alertState.Idle)
        {
            FOVStateHandler();
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
        if (!fireRateOn && !isReloading && visibleTargets.Count != 0)
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
        else
        {
            Moving(patrolPath[0]);
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
        Vector3 dis = visibleTargets[0].transform.position - transform.position;
        // Debug.Log("Shoot direction " + dis + " " + FOVPoint.position + " " + visibleTargets[0].transform.position);
        if(visibleTargets.Count > 0)Shoot(FOVPoint.position, dis, enemyStat , weapon, isItEnemy);
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
        if (visibleTargets.Count > 0)
        {
            foreach (Transform enemy in visibleTargets)
            {
                tempDistance = 0;
                if(tempDistance > Vector3.Distance(transform.position, enemy.position) || tempDistance == 0)
                {
                    Vector3 dis = enemy.position - transform.position;
                    tempDistance = Vector3.Distance(transform.position, enemy.position);
                    transform.forward = Vector3.Slerp(transform.forward, dis.normalized, 10f);
                    
                    lastSeenPosition = enemy.position;
                }
            }
            distance = tempDistance;
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

        if (enemyState == alertState.Idle)
        {
            switch (FOVState)
            {
                case FOVDistState.far:
                    maxAlertValue = tempAlertValue;
                    break;
                case FOVDistState.middle:
                    maxAlertValue = tempAlertValue * 0.5f;
                    break;
                case FOVDistState.close:
                    maxAlertValue = 0;
                    break;
            }
        }
        else if(enemyState == alertState.Engage)
        {
            switch (FOVState)
            {
                case FOVDistState.far:
                    enemyNavmesh.speed = enemyStat.speed;
                    if(visibleTargets.Count != 0)
                    {
                        foreach (Transform enemy in visibleTargets)
                        {
                            tempDistance = 0;
                            if (tempDistance > Vector3.Distance(transform.position, enemy.position) || tempDistance == 0)
                            {
                                Moving(enemy.position);
                                lastSeenPosition = enemy.position;
                            }
                        }
                    }                    
                    EnemyOutOfBounds();
                    break;
                case FOVDistState.middle:
                    EnemyOutOfBounds();
                    Shoot();
                    break;
                case FOVDistState.close:
                    EnemyOutOfBounds();
                    Shoot();
                    break;
            }
        }               
    }

    private void EnemyOutOfBounds()
    {
        if (visibleTargets.Count == 0)
        {
            if (otherVisibleTargets.Count == 0)
            {
                if (lastSeenPosition != Vector3.zero)
                {
                    Moving(lastSeenPosition);
                }
                if (Vector3.Distance(transform.position, lastSeenPosition) < 0.5f)
                {
                    lastSeenPosition = Vector3.zero;
                }
            }
            else
            {
                Moving(otherVisibleTargets[0].position);
                lastSeenPosition = otherVisibleTargets[0].position;
            }
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

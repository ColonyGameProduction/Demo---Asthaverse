using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FriendsAI : ExecuteLogic
{

    GameManager gm;
    EnemyManager EM;
    private GameObject[] destination = new GameObject[4];

    public int friendsID;


    //hal yang diperlukan untuk FOV
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
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    [SerializeField]
    private EntityStatSO character;


    private bool commandActive = false;
    private bool holdPositionActive = false;

    private PlayerAction activePlayerAction;


    public alertState friendsState;
    public bool gotDetected = false;
    public List<EnemyAI> detectedByEnemy;
    public List<EnemyAI> tempDetectedByEnemy;

    [Header("Untuk Stat")]
    [SerializeField]
    private EntityStatSO friendsStat;
    [SerializeField]
    private float friendsHP;
    private bool isReloading = false;
    private bool fireRateOn = false;
    private WeaponStatSO currentWeapon;
    private WeaponStatSO[] weapon;
    public LayerMask isItFriend;
    public float engageTimer;
    public float detectedTimer;
    public float tempDistance;

    private bool isIdle;
    private bool isTakingCover;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine(FindTargetWithDelay(0.2f));
        gm = GameManager.instance;
        EM = EnemyManager.instance;

        EM.isEngaging += HandleState;

        weapon = friendsStat.weaponStat;
        currentWeapon = weapon[0];

        friendsState = alertState.Idle;
        isIdle = true;
    }

    private void LateUpdate()
    {
        DrawFieldOfView(edgeResolveIteration, edgeDistanceTreshold, viewRadius, viewAngle, meshResolution, FOVPoint, viewMesh, groundMask);
    }
    void Update()
    {
        if (detectedTimer > 0)
        {
            detectedTimer -= Time.deltaTime;
        }
        else
        {
            detectedByEnemy.Clear();
            gotDetected = false;
        }

        FindActivePlayerAction();

        switch (friendsState)
        {
            case alertState.Idle:
                if (gotDetected)
                {
                    if (friendsID == 1 && !commandActive)
                    {
                        MoveDestinationAwayFromEnemy(ref destination[2]);
                    }
                    if (friendsID == 2 && !commandActive)
                    {
                        MoveDestinationAwayFromEnemy(ref destination[3]);
                    }
                }
                break;

            case alertState.Hunted:

                if (gotDetected)
                {
                    if (friendsID == 1 && !commandActive)
                    {
                        MoveDestinationAwayFromEnemy(ref destination[2]);
                    }
                    if (friendsID == 2 && !commandActive)
                    {
                        MoveDestinationAwayFromEnemy(ref destination[3]);
                    }
                }
                break;

            case alertState.Engage:

                if(engageTimer > 0)
                {
                    engageTimer -= Time.deltaTime;
                }
                else
                {
                    friendsState = alertState.Idle;
                    isTakingCover = false;
                }

                if (gotDetected)
                {
                    if(visibleTargets.Count > 0)
                    {
                        TakingCover(GetNavMesh(), visibleTargets[0]);
                    }
                    isTakingCover = true;
                }

                isIdle = false;
                Shoot();
                break;
        }

        // Setiap frame, periksa status dari isCommand
        if (activePlayerAction != null && activePlayerAction.enabled)
        {
            bool isCommandActive = activePlayerAction.isCommandActive;
            bool isHoldPositionActive = activePlayerAction.isHoldPosition;

            // Lakukan sesuatu berdasarkan status isCommand
            if (isCommandActive)
            {
                // Logika ketika isCommand aktif
                commandActive = true;
            }
            else
            {
                // Logika ketika isCommand tidak aktif
                commandActive = false;
            }

            if (isHoldPositionActive)
            {
                holdPositionActive = true;
            }
            else
            {
                holdPositionActive = false;
            }
        }
    }

    private void HandleState()
    {
        engageTimer = 0.3f;
        friendsState = alertState.Engage;
    }

    private void Shooting()
    {
        Vector3 dis = new Vector3();
        foreach (Transform enemy in visibleTargets)
        {
            tempDistance = 0;
            if (tempDistance > Vector3.Distance(transform.position, enemy.position) || tempDistance == 0)
            {
                dis = visibleTargets[0].transform.position - transform.position;
                tempDistance = Vector3.Distance(transform.position, enemy.position);
            }
        }
        // Debug.Log("Shoot direction " + dis + " " + FOVPoint.position + " " + visibleTargets[0].transform.position);
        if (visibleTargets.Count > 0) Shoot(FOVPoint.position, dis, friendsStat, currentWeapon, isItFriend);
    }

    private void Shoot()
    {
        if (!fireRateOn && !isReloading && visibleTargets.Count != 0)
        {
            Shooting();
            StartCoroutine(FireRate(FireRateFlag, currentWeapon.fireRate));
            if (currentWeapon.currBullet == 0)
            {
                isReloading = true;
                Reload(currentWeapon);
                StartCoroutine(ReloadTime(ReloadFlag, currentWeapon.reloadTime));
            }

        }
    }

    private void MoveDestinationAwayFromEnemy(ref GameObject destination)
    {
        tempDetectedByEnemy = detectedByEnemy;        
        Vector3 directionTotal = Vector3.zero;
        destination.transform.position = transform.position;
        
        foreach (EnemyAI enemy in tempDetectedByEnemy)
        {
            Vector3 directionAwayFromEnemy = (transform.position - enemy.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < enemy.GetEnemyStat().FOVRadius + 5f)
            {
                directionTotal += directionAwayFromEnemy;
                directionTotal = directionTotal.normalized;
            }
            else
            {
                detectedByEnemy.Remove(enemy);
            }
        }

        destination.transform.position += directionTotal;
        MoveToDestination(GetNavMesh(), destination.transform.position);

    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        DetectingPosition();
        DetectingPlayer();
        //if(commandFollow)
        //{
        //
        //}
        CommandFollow();
    }


    //untuk follow player
    private void CommandFollow()
    {
        if (commandActive == true || holdPositionActive == true)
        {
            if (friendsID == 1)
            {
                MoveToDestination(GetNavMesh(), destination[2].transform.position);
                SnapToWallIfNeeded(destination[2].transform.position); // kalo misalkan dia deket sama tembok ato nempel dia bakal posisiin diri secara otomatis ke tembok

                if (gotDetected && isIdle)
                {
                    MoveDestinationAwayFromEnemy(ref destination[2]);
                }
            }
            else if (friendsID == 2)
            {
                MoveToDestination(GetNavMesh(), destination[3].transform.position);
                SnapToWallIfNeeded(destination[3].transform.position); // kalo misalkan dia deket sama tembok ato nempel dia bakal posisiin diri secara otomatis ke tembok

                if (gotDetected && isIdle)
                {
                    MoveDestinationAwayFromEnemy(ref destination[3]);
                }
            }
        }
        else
        {
            if(!gotDetected && !isTakingCover)
            {
                if (friendsID == 1)
                {
                    MoveToDestination(GetNavMesh(), destination[0].transform.position);
                }
                else if (friendsID == 2)
                {
                    MoveToDestination(GetNavMesh(), destination[1].transform.position);
                }
            }            
        }
    }

    // untuk snap ke wall secara mandiri wkwk
    private void SnapToWallIfNeeded(Vector3 destinationPosition)
    {
        RaycastHit hit;
        Vector3 direction = (destinationPosition - transform.position).normalized;

        // shoot ray buat ngedetect wall
        if (Physics.Raycast(transform.position, direction, out hit, 2f, LayerMask.GetMask("Wall")))
        {
            // nah kalo misalkan ray ngedetect wall, snap ke poin dimana ray shoot
            Vector3 snapPosition = hit.point;

            // pas pengen nempel tembok posisiin si friend aga menjauh dari tembok posisinya biar nggk nempel
            snapPosition += hit.normal * 0.5f;  

            // Set AI position and rotation towards the wall nah ini buat nge-set posisi sama rotasi si friend 
            GetNavMesh().enabled = false; 
            transform.position = snapPosition;
            transform.rotation = Quaternion.LookRotation(-hit.normal); // rotate si friend biar ngadep ke tembok
            GetNavMesh().enabled = true;  
        }
    }

    //set destination supaya friends bisa mengikuti player
    public void DetectingPlayer()
    {
        for(int i = 0; i<gm.playerGameObject.Length; i++)
        {
            if(gm.playableCharacterNum == i)
            {
                destination[0] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetDestinationGameObject()[0];
                destination[1] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetDestinationGameObject()[1];
            }
        }
    }

    //untuk go to position
    public void DetectingPosition()
    {
        for (int i = 0; i < gm.playerGameObject.Length; i++)
        {
            if (gm.playableCharacterNum == i)
            {
                destination[2] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetPositionGameObject()[0];
                destination[3] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetPositionGameObject()[1];
            }
        }
    }

    //balikin friends ke posisi awal player
    private void FindActivePlayerAction() 
    { 
        // Cari semua PlayerAction
        PlayerAction[] allPlayerActions = FindObjectsOfType<PlayerAction>();

        // Temukan yang aktif
        foreach (PlayerAction playerAction in allPlayerActions)
        {
            if (playerAction.enabled)
            {
                activePlayerAction = playerAction;
                break;
            }
        }
    }

    //untuk passing navmesh
    public NavMeshAgent GetNavMesh()
    {
        return this.GetComponent<NavMeshAgent>();
    }

    

    private void OnDisable()
    {
        viewMesh.Clear();
    }

    public IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargetsForEnemy(viewRadius, viewAngle, visibleTargets, FOVPoint, enemyMask, groundMask);
        }
    }

    public float GetFriendHP()
    {
        return friendsHP;
    }

    public void SetEnemyHP(float hp)
    {
        friendsHP = hp;
    }

    private void ReloadFlag(bool value)
    {
        isReloading = value;
    }

    private void FireRateFlag(bool value)
    {
        fireRateOn = value;
    }

    public EntityStatSO GetFriendsStat()
    {
        return character;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FriendsAI : ExecuteLogic
{

    GameManager gm;
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



    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine(FindTargetWithDelay(0.2f));
        gm = GameManager.instance;
    }

    private void LateUpdate()
    {
        DrawFieldOfView(edgeResolveIteration, edgeDistanceTreshold, viewRadius, viewAngle, meshResolution, FOVPoint, viewMesh, groundMask);
    }
    void Update()
    {
        FindActivePlayerAction();

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
            }
            else if (friendsID == 2)
            {
                MoveToDestination(GetNavMesh(), destination[3].transform.position);
            }
        }
        else
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

    public EntityStatSO GetFriendsStat()
    {
        return character;
    }


}

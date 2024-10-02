using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FriendsAI : ExecuteLogic
{

    GameManager gm;
    private GameObject[] destination = new GameObject[2];

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
    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        DetectingPlayer();
        //if(commandFollow)
        //{
        //
        //}
        CommandFollow();
    }

    

    private void CommandFollow()
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
}

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

    //hal yang diperlukan untuk FOV
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
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh viewMesh; 

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetWithDelay", .2f);
        enemyNavmesh = GetComponent<NavMeshAgent>();
    }

    private void LateUpdate()
    {
        DrawFieldOfView(edgeResolveIteration, edgeDistanceTreshold, viewRadius, viewAngle, meshResolution, FOVPoint, viewMesh, groundMask);
    }

    private void Update()
    {
        Moving();
        //FieldOfView(this.transform, viewRadius, viewAngle);

        timer += Time.deltaTime;
        if(timer > 2)
        {
            //Shoot();
            timer = 0;            
        }        
    }      

    //pathfinding untuk enemy
    private void Moving()
    {
        if (destination != null)
        {
            MoveToDestination(enemyNavmesh, destination.transform.position);
        }
    }


    public IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargetsForEnemy(viewRadius, viewAngle, visibleTargets, FOVPoint, playerMask, groundMask);
        }
    }


}

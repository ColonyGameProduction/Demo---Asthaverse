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

    private void Start()
    {
        enemyNavmesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Moving();

        timer += Time.deltaTime;
        if(timer > 2)
        {
            Shoot();
            timer = 0;            
        }        
    }      

    //pathfinding untuk enemy
    private void Moving()
    {
        if (destination != null)
        {
            MoveToDestination(enemyNavmesh, destination);
        }
    }

}

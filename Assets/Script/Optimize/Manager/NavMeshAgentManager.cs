
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentManager : MonoBehaviour
{
    [SerializeField] private float _avoidancePredictionTime = 2f;
    [SerializeField] private int _pathFindingIterationsPerFrame = 100;
    [SerializeField] private ObstacleAvoidanceType _obsAvoidanceType;

    List<NavMeshAgent> _navMeshAgents = new List<NavMeshAgent>();

    private void Awake() 
    {
        NavMesh.avoidancePredictionTime = _avoidancePredictionTime;
        NavMesh.pathfindingIterationsPerFrame = _pathFindingIterationsPerFrame;

        _navMeshAgents = FindObjectsOfType<NavMeshAgent>().ToList();
        foreach(NavMeshAgent agent in _navMeshAgents)
        {
            agent.obstacleAvoidanceType = _obsAvoidanceType;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogic : MonoBehaviour
{
    //AI Navmesh untuk ke tempat yang ditentukan, memerlukan parsing navmesh agent dan tempat destinasinya
    public void MoveToDestination(NavMeshAgent agent, GameObject destination)
    {
        agent.destination = destination.transform.position;
    }

    public void FriendsMoveAI(GameObject [] destination)
    {
        SetDestination(destination[0]);
    }

    public Vector3 SetDestination(GameObject destination)
    {
        return destination.transform.position;
    }
}

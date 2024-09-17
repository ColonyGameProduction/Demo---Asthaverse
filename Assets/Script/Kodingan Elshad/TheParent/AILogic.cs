using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogic : MonoBehaviour
{    
    //AI Navmesh untuk ke tempat yang ditentukan, memerlukan parsing navmesh agent dan tempat destinasinya
    public void MoveToDestination(NavMeshAgent agent, Vector3 destination)
    {
        agent.destination = destination;
    }

    
}

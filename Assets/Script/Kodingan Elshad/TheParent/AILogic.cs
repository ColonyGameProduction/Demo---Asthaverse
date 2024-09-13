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

    public void FieldOfView(Transform character)
    {
        float viewRadius;
        float viewAngle;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

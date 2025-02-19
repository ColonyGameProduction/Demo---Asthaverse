
using UnityEngine;
using UnityEngine.AI;

public class Navmesh : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool yes;
    public Transform Setdes;
    private void Update() {
        if(yes)
        {
            yes = false;
            agent.SetDestination(Setdes.position);
        }   
        if(agent.hasPath)
        {
            Debug.Log(agent.remainingDistance);
        }
        if(Setdes != null)
        {
            float dis = CountDistance(transform, Setdes);
            Debug.Log(dis + " " + Vector3.Distance(transform.position, Setdes.position));
            
        }
        
    }
    public float CountDistance(Transform origin, Transform target)
    {
        NavMeshPath path = new NavMeshPath();
        if(NavMesh.CalculatePath(origin.position, target.position, agent.areaMask, path))
        {
            float distance = Vector3.Distance(origin.position, path.corners[0]);
            for(int j = 1; j < path.corners.Length; j++)
            {
                distance += Vector3.Distance(path.corners[j-1], path.corners[j]);
            }
            return distance;
        }
        return 0;
    }
}

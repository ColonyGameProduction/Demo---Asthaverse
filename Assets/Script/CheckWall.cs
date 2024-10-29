using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;

public class CheckWall : MonoBehaviour
{
    public GameObject wall;
    public GameObject target;
    [Space(10)]
    public Vector3 WallCenter;
    public Vector3 wallLocal;
    public Vector3 waalTRANSFORM;
    [Space(10)]

    public float angle;
    public float buffer = 0.5f;
    public bool GoToEdge;
    public bool isLeft, isX;
    public GameObject navmesh;
    
    private void Update() 
    {
        // Debug.Log("ARAH " + wall.transform.forward);
        // NewCheckPos(target.transform.position, wall.GetComponent<Collider>());
        Vector3 newPos = NewCheckPos(target.transform.position, wall.GetComponent<Collider>());
        // Debug.lo
        if(GoToEdge)
        {
            GoToEdge = false;
            // if(navmesh != null)navmesh.SetDestination(newPos);
            if(navmesh!= null)navmesh.transform.position = newPos;
        }
    }
    public Vector3 CheckPositionBasedOnWall(Vector3 targetPos, Collider wallColl)
    {
        Vector3 wallCenter = wallColl.bounds.center;
        float wallSize = 0f;
        Vector3 newPos = Vector3.zero;
        Vector3 wallToTargetDir = wallColl.transform.position - targetPos;

        angle = Vector3.Angle(wallColl.transform.forward, wallToTargetDir);
        if((angle >= 0 && angle < 45) || (angle > 135 && angle <= 180))
        {
            isX = true;
            wallSize = wallColl.transform.localScale.x * 0.5f + buffer;
            if(targetPos.z >= wallCenter.z)
            {
                if(targetPos.x >= wallCenter.x)
                {
                    isLeft = false;
                    newPos = new Vector3(wallCenter.x + wallSize, targetPos.y, targetPos.z);
                }
                else
                {
                    isLeft = true;
                    newPos = new Vector3(wallCenter.x - wallSize, targetPos.y, targetPos.z);
                    
                }
            }
            else
            {
                if(targetPos.x >= wallCenter.x)
                {
                    isLeft = true;
                    newPos = new Vector3(wallCenter.x + wallSize, targetPos.y, targetPos.z);
                }
                else
                {
                    isLeft = false;
                    newPos = new Vector3(wallCenter.x - wallSize, targetPos.y, targetPos.z);
                }
            }

        }
        else if(angle >= 45 && angle <= 135)
        {
            wallSize = wallColl.transform.localScale.z * 0.5f + buffer;
            isX = false;
            if(targetPos.x >= wallCenter.x)
            {
                if(targetPos.z >= wallCenter.z)
                {
                    isLeft = true;
                    newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z + wallSize);
                }
                else
                {
                    isLeft = false;
                    newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z - wallSize);
                }
            }
            else
            {
                if(targetPos.z >= wallCenter.z)
                {
                    isLeft = false;
                    newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z + wallSize);
                }
                else
                {
                    isLeft = true;
                    newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z - wallSize);
                    
                }
            }
        }
        return newPos;
    }
    public Vector3 NewCheckPositionBasedOnWall(Vector3 targetPos, Collider wallColl)
    {
        // waalTRANSFORM = wall.transform.forward;
        // wallLocal = wall.transform.localPosition;
        // targetINVERSEWALL = wall.transform.InverseTransformPoint(target.transform.position);
        Vector3 wallCenter = wallColl.bounds.center;
        float wallSize = 0f;
        Vector3 newPos = Vector3.zero;

        float wallSizeX = wallColl.transform.localScale.x * 0.5f;
        float wallSizeZ = wallColl.transform.localScale.z * 0.5f;
        if(targetPos.z > wallCenter.z + wallSizeZ)
        {
            isX = true;
            wallSize = wallSizeX + buffer;
            if(targetPos.x >= wallCenter.x)
            {
                isLeft = false;
            }
            else
            {
                isLeft = true;
                
            }
            newPos = new Vector3(wallCenter.x + (isLeft? - wallSize : wallSize), targetPos.y, targetPos.z);
        }
        else if(targetPos.z < wallCenter.z - wallSizeZ)
        {
            isX = true;
            wallSize = wallSizeX + buffer;
            if(targetPos.x >= wallCenter.x)
            {
                isLeft = true;
            }
            else
            {
                isLeft = false;
            }
            newPos = new Vector3(wallCenter.x + (isLeft? wallSize : -wallSize), targetPos.y, targetPos.z);
        }
        else if((targetPos.z <= wallCenter.z + wallSizeZ) && (targetPos.z >= wallCenter.z - wallSizeZ))
        {
            isX = false;
            wallSize = wallSizeZ + buffer;
            if(targetPos.x > wallCenter.x + wallSizeX)
            {
                if(targetPos.z >= wallCenter.z)
                {
                    isLeft = true;
                }
                else
                {
                    isLeft = false;
                }
                newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z + (isLeft? wallSize : -wallSize));
            }
            else
            {
                if(targetPos.z >= wallCenter.z)
                {
                    isLeft = false;
                }
                else
                {
                    isLeft = true;
                    
                }
                newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z + (isLeft? - wallSize : wallSize));
            }
        }

        return newPos;
    }
    
    public Vector3 NewCheckPos(Vector3 targetPos, Collider wallColl)
    {
        Vector3 wallCenter = wallColl.bounds.center;
        Vector3 wallFwd = wallColl.transform.forward;
        Vector3 wallRight = wallColl.transform.right;

        float halfWallLength = wallColl.transform.localScale.z * 0.5f;
        float halfWallWidth = wallColl.transform.localScale.x * 0.5f;
        
        Vector3 wallToTarget = targetPos - wallCenter;

        float forwardDistance = Vector3.Dot(wallToTarget, wallFwd);
        float rightDistance = Vector3.Dot(wallToTarget, wallRight);
        
        Vector3 newPos = wallCenter;
        WallCenter = newPos;
        Debug.Log("fwd" + forwardDistance + " and " + rightDistance);
        
        float newHalfWallWidth = 0;
        float newHalfWallLength = 0;
        // if(Mathf.Abs(forwardDistance) > halfWallLength)
        // {
            
        //     isX = true;
        //     newPos += wallFwd * (forwardDistance > 0 ? newHalfWallLength : -newHalfWallLength);
        //     newPos += wallRight * Mathf.Clamp(rightDistance, -newHalfWallWidth, newHalfWallWidth);

        // }
        // else
        // {
        //     isX = false;
        //     newPos += wallRight * (rightDistance > 0 ? newHalfWallWidth : -newHalfWallWidth);
        //     newPos += wallFwd * Mathf.Clamp(forwardDistance, -newHalfWallLength, newHalfWallLength);
        // }

        if(Mathf.Abs(forwardDistance) > halfWallLength)
        {
            newHalfWallWidth = halfWallWidth + buffer;
            newHalfWallLength = halfWallLength - buffer;
            isX = true;
            newPos += wallRight * (rightDistance >= 0 ? newHalfWallWidth : -newHalfWallWidth);
            newPos += wallFwd * Mathf.Clamp(forwardDistance, -newHalfWallLength, newHalfWallLength);
            if(rightDistance >= 0)
            {
                if(forwardDistance >= 0) isLeft = false;
                else isLeft = true;
            }
            else
            {
                if(forwardDistance > 0) isLeft = true;
                else isLeft = false;
            }
            
        }
        else
        {
            newHalfWallLength = halfWallLength + buffer;
            newHalfWallWidth = halfWallWidth - buffer;
            isX = false;
            newPos += wallFwd * (forwardDistance >= 0 ? newHalfWallLength : -newHalfWallLength);
            newPos += wallRight * Mathf.Clamp(rightDistance, -newHalfWallWidth, newHalfWallWidth);
            if(forwardDistance >= 0)
            {
                if(rightDistance > 0) isLeft = true;
                else isLeft = false;
            }
            else
            {
                if(rightDistance >= 0) isLeft = false;
                else isLeft = true;
            }
        }
        // isLeft = rightDistance < 0;

        return newPos;
    }
}

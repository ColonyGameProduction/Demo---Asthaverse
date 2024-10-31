using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBehaviourStateMachine : BaseStateMachine
{
    #region Normal Variable
    [Header("Other Important Variable")]
    [SerializeField] protected FOVMachine _fovMachine;
    [SerializeField] protected Transform _aimAIPoint;
    [Header("Take Cover Component")]
    [SerializeField] protected Vector3 _leaveDirection;
    [SerializeField] protected bool _isTakingCover;
    [SerializeField] protected Collider[] _wallArrayNearChara;
    [SerializeField] protected float _wallScannerDistance;
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField][Range(-1, 1f)] protected float _HideDotMin = 0f;
    protected float _charaWidth;
    protected Collider _charaColl;
    [SerializeField] protected float _charaWidthBuffer = 0.2f;
    [SerializeField] protected float _buffer = -1.5f;
    protected Vector3 _takeCoverPosition;
    protected bool _canTakeCoverInThePosition;

    [Header("Component to Save Enemy Who sees us")]
    [SerializeField] protected List<Transform> _enemyWhoSawAIList;
    [SerializeField] protected float _enemyMaxDistanceFromWalls = 10f;
    protected NavMeshPath path;
    #endregion
    #region  GETTER SETTER VARIABLE
    public FOVMachine GetFOVMachine { get { return _fovMachine; } }

    #endregion
    protected override void Awake() 
    {
        //Dari Chara Identity bisa akses ke semua yg berhubungan dgn characteridentity
        base.Awake();
        _charaColl = GetComponent<CharacterController>();
        _charaWidth = GetComponent<CharacterController>().radius * 2 + _charaWidthBuffer;
        
        if(_agent)_agent = GetComponent<NavMeshAgent>();
        if(_fovMachine == null)_fovMachine = GetComponent<FOVMachine>();
        
    }
    public void AimAIPointLookAt(Transform lookAt)
    {
        if(lookAt != null) _aimAIPoint.LookAt(lookAt);
        else
        {
            if(_aimAIPoint.localRotation != Quaternion.identity)_aimAIPoint.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void TakingCover()
    {
        _wallArrayNearChara = Physics.OverlapSphere(transform.position, _wallScannerDistance, _wallLayer);
        float _wallTotal = _wallArrayNearChara.Length;
        
        for(int i=0; i < _wallArrayNearChara.Length; i++)
        {
            foreach(Transform enemy in _enemyWhoSawAIList)
            {
                // Debug.Log(_wallArrayNearChara[i].name + " " + Vector3.Distance(_wallArrayNearChara[i].transform.position, enemy.transform.position));
                if(Vector3.Distance(_wallArrayNearChara[i].transform.position, enemy.transform.position) <= _enemyMaxDistanceFromWalls)
                {
                    _wallTotal -= 1;
                    _wallArrayNearChara[i] = null;
                    break;
                }
            }
        }

        if(_wallTotal == 0) return;

        System.Array.Sort(_wallArrayNearChara,SortWallBasedOnClosestDistance);

        //distance

        for(int i = 0; i < _wallTotal; i++)
        {
            Collider currColl = _wallArrayNearChara[i];
            if(currColl.bounds.max.y <= _charaColl.bounds.max.y * (5/8)) continue; // Kalo lebi pendek dr crouch height, skip

            Transform currWall = currColl.transform;

            Vector3 wallCenter = currColl.bounds.center;
            Vector3 wallForward = currWall.forward;
            Vector3 wallRight = currWall.right;
            
            float halfWallWidth = currWall.localScale.x * 0.5f;
            float halfWallLength = currWall.localScale.z * 0.5f;

            bool canCheckFrontBehind = true;
            bool canCheckLeftRightSide = true;

            if(halfWallWidth * 2 <= _charaWidth)canCheckFrontBehind = false;
            if(halfWallLength * 2 <= _charaWidth)canCheckLeftRightSide = false;

            Debug.Log(halfWallWidth * 2 + " " + halfWallLength * 2 + " " + _charaWidth + " aa" + canCheckFrontBehind + canCheckLeftRightSide);
            if(!canCheckLeftRightSide && !canCheckFrontBehind) continue;
            Vector3 directionTotalEnemyToWall = GetTotalDirectionTargetPosAndEnemy(currWall, true);
            Debug.DrawRay(_wallArrayNearChara[i].transform.position, directionTotalEnemyToWall * 100f, Color.black);

            bool isWallTallerThanChara = currColl.bounds.max.y > _charaColl.bounds.max.y;


            float closestDistance = Mathf.Infinity; // ini cari jarak terdekat dr player 
            
            Vector3 newPos = Vector3.zero;

            if(canCheckFrontBehind)
            {
                float forwardWithEnemy = Vector3.Dot(directionTotalEnemyToWall, wallForward);

                if(forwardWithEnemy < _HideDotMin)
                {
                    // Debug.Log("Dot fwd normal " + forwardWithEnemy);
                    Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallForward * 100f, Color.blue);

                    GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanChara, wallCenter, wallForward, wallRight);
                    

                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    forwardWithEnemy = Vector3.Dot(directionTotalEnemyToWall, -wallForward);
                    if(forwardWithEnemy < _HideDotMin)
                    {
                        // Debug.Log("Dot fwd balik " + forwardWithEnemy);
                        Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallForward * 100f, Color.red);

                        GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanChara, wallCenter, -wallForward, wallRight);
                    } 
                }
                
            }

            // tempNewPos = wallCenter;
            if(canCheckLeftRightSide)
            {
                float rightWithEnemy = Vector3.Dot(directionTotalEnemyToWall, wallRight);
                if(rightWithEnemy < _HideDotMin)
                {
                    // Debug.Log("Dot right normal " + rightWithEnemy);
                    Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallRight * 100f, Color.grey);
                    GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanChara, wallCenter, wallForward, wallRight);
                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    rightWithEnemy = Vector3.Dot(directionTotalEnemyToWall, -wallRight);
                    if(rightWithEnemy < _HideDotMin)
                    {
                        // Debug.Log("Dot right balik " + rightWithEnemy);
                        Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallRight * 100f, Color.magenta);
                        GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanChara, wallCenter, wallForward, -wallRight);
                    }
                    
                }
            }

            

            // Vector3 dotEnemyWallFwd = Vector3.Dot()
            if(closestDistance != Mathf.Infinity)
            {
                _canTakeCoverInThePosition = true;
                _takeCoverPosition = newPos;
                Debug.DrawRay(_takeCoverPosition, Vector3.up * 100f, Color.red);
                break;
            } 
            else _canTakeCoverInThePosition = false;
            

            
        }
    }

    //KALO MO NYARI INI HARUS NYARI LEAVEDIRECTION DL
    public void GetClosestPosition(bool isFrontBehind, ref float closestDistance, ref Vector3 newPos, float halfWallLength, float halfWallWidth, bool isWallTallerThanChara, Vector3 wallCenter, Vector3 wallForwardDir, Vector3 wallRightDir)
    {
        float newHalfWallWidth = 0;
        float newHalfWallLength = 0;
        
        Vector3 tempNewPos = wallCenter;
        if(isFrontBehind)
        {
            newHalfWallLength = halfWallLength - _buffer;
            
            if(isWallTallerThanChara)
            {
                newHalfWallWidth = halfWallWidth + _buffer;
                for(int x = 0; x < 2; x++)
                {
                    tempNewPos = wallCenter;
                    if(x == 0) tempNewPos += wallRightDir * newHalfWallWidth;
                    if(x == 1) tempNewPos += wallRightDir * -newHalfWallWidth;
                    tempNewPos += wallForwardDir * newHalfWallLength;

                    float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);

                    if(closestDistance > distanceCharaToWall)
                    {
                        Debug.Log(x + " " + tempNewPos + "before");
                        Vector3 newPosToPlayer = (tempNewPos - transform.position).normalized;
                        float dotLeaveDirWithNewPosDir = Vector3.Dot(newPosToPlayer, _leaveDirection);
                        if(dotLeaveDirWithNewPosDir >= _HideDotMin)
                        {
                            closestDistance = distanceCharaToWall;
                            newPos = tempNewPos;
                            Debug.Log(x + " " + newPos + "after");
                        }
                        
                    }
                }
            }
            else
            {
                if((int)halfWallWidth > 1)
                {
                
                    for(int x = -(int)halfWallWidth + 1; x < (int)halfWallWidth; x++)
                    {
                        tempNewPos = wallCenter;
                        newHalfWallWidth = (halfWallWidth * halfWallWidth * x)/ (halfWallWidth * (halfWallWidth - 1));

                        if(newHalfWallWidth == halfWallWidth) newHalfWallWidth = halfWallWidth + _buffer;
                        else if(newHalfWallWidth == -halfWallWidth) newHalfWallWidth = -(halfWallWidth + _buffer);

                        Debug.Log(x + " " + newHalfWallWidth + " " + halfWallWidth + " " + wallRightDir * newHalfWallWidth + " FrontBehindx");
                        tempNewPos += wallRightDir * newHalfWallWidth;
                        tempNewPos += wallForwardDir * newHalfWallLength;

                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall)
                        {
                            Vector3 newPosToPlayer = (tempNewPos - transform.position).normalized;
                            float dotLeaveDirWithNewPosDir = Vector3.Dot(newPosToPlayer, _leaveDirection);
                            if(dotLeaveDirWithNewPosDir >= _HideDotMin)
                            {
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;
                            }
                            Debug.Log(x + " " + newPos + " FrontBehind");
                        }
                    }
                }
                else
                {
                    for(int x = 0; x < 2; x++)
                    {
                        tempNewPos = wallCenter;
                        if(x == 0) tempNewPos += wallRightDir * newHalfWallWidth;
                        if(x == 1) tempNewPos += wallRightDir * -newHalfWallWidth;
                        tempNewPos += wallForwardDir * newHalfWallLength;

                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall)
                        {
                            Vector3 newPosToPlayer = (tempNewPos - transform.position).normalized;
                            float dotLeaveDirWithNewPosDir = Vector3.Dot(newPosToPlayer, _leaveDirection);
                            if(dotLeaveDirWithNewPosDir >= _HideDotMin)
                            {
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            newHalfWallWidth = halfWallWidth - _buffer;
            if(isWallTallerThanChara)
            {
                newHalfWallLength = halfWallLength + _buffer;
                for(int x = 0; x < 2; x++)
                {
                    tempNewPos = wallCenter;
                    if(x == 0) tempNewPos += wallForwardDir * newHalfWallLength;
                    if(x == 1) tempNewPos += wallForwardDir * -newHalfWallLength;
                    tempNewPos += wallRightDir * newHalfWallWidth;

                    float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                    if(closestDistance > distanceCharaToWall)
                    {
                        Vector3 newPosToPlayer = (tempNewPos - transform.position).normalized;
                        float dotLeaveDirWithNewPosDir = Vector3.Dot(newPosToPlayer, _leaveDirection);
                        if(dotLeaveDirWithNewPosDir >= _HideDotMin)
                        {
                            closestDistance = distanceCharaToWall;
                            newPos = tempNewPos;
                        }
                    }
                }
            }
            else
            {
                if((int)halfWallLength > 1)
                {
                    for(int x = -(int)halfWallLength + 1; x < (int)halfWallLength; x++)
                    {
                        tempNewPos = wallCenter;
                        newHalfWallLength = (halfWallLength * halfWallLength * x)/ (halfWallLength * (halfWallLength - 1));

                        if(newHalfWallLength == halfWallLength) newHalfWallLength = halfWallLength + _buffer;
                        else if(newHalfWallLength == -halfWallLength) newHalfWallLength = -(halfWallLength + _buffer);

                        Debug.Log(x + " " + newHalfWallLength + " " + halfWallLength + " " + wallForwardDir * newHalfWallLength + " LeftRightx");
                        tempNewPos += wallForwardDir * newHalfWallLength;
                        tempNewPos += wallRightDir * newHalfWallWidth;

                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall)
                        {
                            Vector3 newPosToPlayer = (tempNewPos - transform.position).normalized;
                            float dotLeaveDirWithNewPosDir = Vector3.Dot(newPosToPlayer, _leaveDirection);
                            if(dotLeaveDirWithNewPosDir >= _HideDotMin)
                            {
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;
                            }
                            Debug.Log(x + " " + newPos + " LeftRight");
                        }
                    }
                }
                else
                {
                    newHalfWallLength = halfWallLength + _buffer;
                    for(int x = 0; x < 2; x++)
                    {
                        tempNewPos = wallCenter;
                        if(x == 0) tempNewPos += wallForwardDir * newHalfWallLength;
                        if(x == 1) tempNewPos += wallForwardDir * -newHalfWallLength;
                        tempNewPos += wallRightDir * newHalfWallWidth;

                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall)
                        {
                            Vector3 newPosToPlayer = (tempNewPos - transform.position).normalized;
                            float dotLeaveDirWithNewPosDir = Vector3.Dot(newPosToPlayer, _leaveDirection);
                            if(dotLeaveDirWithNewPosDir >= _HideDotMin)
                            {
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;
                            }
                        }
                    }
                }
            }
        }
    }


    public float CountNavMeshPathDistance(Vector3 origin, Vector3 target)
    {
        path = new NavMeshPath();
        if(NavMesh.CalculatePath(origin, target, _agent.areaMask, path))
        {
            float distance = Vector3.Distance(origin, path.corners[0]);
            for(int j = 1; j < path.corners.Length; j++)
            {
                distance += Vector3.Distance(path.corners[j-1], path.corners[j]);
            }
            return distance;
        }
        return 0;
    }
    public Vector3 GetTotalDirectionTargetPosAndEnemy(Transform targetPos, bool isFromTarget)
    {
        Vector3 directionTotal = Vector3.zero;
        
        foreach(Transform enemy in _enemyWhoSawAIList)
        {
            Vector3 direction = Vector3.zero;
            if(isFromTarget) direction = enemy.transform.position - targetPos.position;
            else direction = targetPos.position - enemy.transform.position;

            direction = direction.normalized;
            directionTotal += direction;
        }

        return directionTotal;
    }
    private int SortWallBasedOnClosestDistance(Collider A, Collider B)
    {
        if(A != null && B == null) return -1;
        if(A == null && B != null) return 1;
        if(A == null && B == null) return 0;
        float distanceA = Vector3.Distance(A.transform.position, transform.position);
        float distanceB = Vector3.Distance(B.transform.position, transform.position);
        return distanceA.CompareTo(distanceB);
    }
}

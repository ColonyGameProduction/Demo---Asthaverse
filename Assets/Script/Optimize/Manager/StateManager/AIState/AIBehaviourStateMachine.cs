using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBehaviourStateMachine : BaseStateMachine
{
    #region Normal Variable
    protected bool _isAIInput = true;
    [SerializeField] protected TakeCoverManager _takeCoverManager;
    [Header("Other Important Variable")]
    [SerializeField] protected FOVMachine _fovMachine;
    [SerializeField] protected Transform _aimAIPoint;
    [SerializeField] protected Transform _noEnemyToPointObj;
    [Header("Take Cover Component")]
    [SerializeField] protected Vector3 _leaveDirection;
    [SerializeField] protected bool _isTakingCover;
    [SerializeField] protected bool _isHiding, _isChecking;
    [SerializeField] protected bool _isAtTakingCoverHidingPlace;
    [SerializeField] protected bool _isAtTakingCoverCheckingPlace;
    [SerializeField] protected Collider[] _wallArrayNearChara;
    protected float _wallTotal;
    protected Collider _currWall;
    [SerializeField] protected float _wallScannerDistance;
    [SerializeField] protected LayerMask _wallTakeCoverLayer;
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField][Range(-1, 1f)] protected float _HideDotMin = 0f;
    [SerializeField] protected float _charaMaxTakeCoverDistance = 100f;
    protected float _charaWidth;
    protected float _currWallHeight;
    [SerializeField] protected Collider _charaHeadColl;
    [SerializeField] protected float _charaHeightBuffer = 0.15f;
    [SerializeField] protected float _charaWidthBuffer = 0.2f;
    [SerializeField] protected float _buffer = -1.5f;
    protected Vector3 _takeCoverPosition;
    protected bool _canTakeCoverInThePosition;
    protected bool _isWallTallerThanChara;
    protected Vector3 _dirToLookAtWhenTakingCover;
    protected Vector3 _dirToLookAtWhenChecking;
    protected Vector3 _posToGoWhenCheckingWhenWallIsHigher;
    protected bool _isAtTheLeftSideOfTheWall;
    protected bool _isMovingOnXPos;

    [Header("Component to Save Enemy Who sees us")]
    [SerializeField] protected List<Transform> _enemyWhoSawAIList = new List<Transform>();
    [SerializeField] protected List<Transform> _enemyWhoSawAIListContainer = new List<Transform>();
    [SerializeField] protected List<Transform> _pastVisibleTargets = new List<Transform>();
    protected Transform _closestEnemyWhoSawAI;
    [SerializeField] protected int _minEnemyMakeCharaFeelOverwhelmed = 3;
    [SerializeField] protected bool _gotDetectedByEnemy;
    [SerializeField] protected float _gotDetectedTimer;
    [SerializeField] protected float _gotDetectedTimerMax = 0.3f;
    [SerializeField] protected float _enemyMaxDistanceFromWalls = 10f;
    protected NavMeshPath path;
    [Header("Running Away Component")]
    [SerializeField]protected LayerMask _runAwayObstacleMask;
    protected RaycastHit _runAwayObstacleHit;
    protected bool _isThereNoPathInRunAwayDirection;
    protected Vector3 _runAwayPos;

    [Header("Advanced Shooting AI")]
    [SerializeField]protected bodyParts _focusedBodyPartsToShoot;
    protected Transform _focusedBodyPartToShootTransform;
    protected Transform _bodyPartToShootTransform;
    protected LayerMask _bodyPartMask;

    protected Vector3 _tempFirstPathPos;
    protected float _hidingCheckDelayTimer;
    [SerializeField] protected float _isCheckingLastPosTimer;
    [SerializeField] protected float _isCheckingLastPosTimerMax = 1f;

    #endregion
    #region  GETTER SETTER VARIABLE
    public NavMeshAgent Agent {get {return _agent;}}
    public FOVMachine GetFOVMachine { get { return _fovMachine; } }
    public bool GotDetectedbyEnemy {get { return _gotDetectedByEnemy;}}
    public List<Transform> EnemyWhoSawAIList { get{return _enemyWhoSawAIList;}} 
    public List<Transform> EnemyWhoSawAIListContainer { get{return _enemyWhoSawAIListContainer;}} 
    public Transform ClosestEnemyWhoSawAI { get { return _closestEnemyWhoSawAI;}}
    public Vector3 LeaveDirection {get { return _leaveDirection;}}

    public Vector3 TakeCoverPosition {get { return _takeCoverPosition;}}
    public bool CanTakeCoverInThePosition {get { return _canTakeCoverInThePosition;}}
    public bool IsTakingCover {get { return _isTakingCover;} set { _isTakingCover = value;}}
    public bool IsHiding {get { return _isHiding;} set { _isHiding = value;}}
    public bool IsChecking {get { return _isChecking;} set { _isChecking = value;}}
    public bool IsAtTakingCoverHidingPlace {get { return _isAtTakingCoverHidingPlace;} set { _isAtTakingCoverHidingPlace = value;}}
    public bool IsAtTakingCoverCheckingPlace {get { return _isAtTakingCoverCheckingPlace;} set { _isAtTakingCoverCheckingPlace = value;}}
    public bool isWallTallerThanChara {get { return _isWallTallerThanChara;}}
    public Vector3 DirToLookAtWhenTakingCover {get { return _dirToLookAtWhenTakingCover;}}
    public Vector3 DirToLookAtWhenChecking {get { return _dirToLookAtWhenChecking;}}
    public Vector3 PosToGoWhenCheckingWhenWallIsHigher {get { return _posToGoWhenCheckingWhenWallIsHigher;}}
    public Transform NoEnemyToPointObj {get { return _noEnemyToPointObj;}}
    public LayerMask RunAwayObstacleMask {get {return _runAwayObstacleMask;}}
    public Vector3 RunAwayPos {get {return _runAwayPos;} }
    public int MinEnemyMakeCharaFeelOverwhelmed {get {return _minEnemyMakeCharaFeelOverwhelmed;}}
    public float HidingCheckDelayTimer {get{ return _hidingCheckDelayTimer;} set{_hidingCheckDelayTimer = value;}}

    public Transform FocusedBodyPartToShootTransform {get {return _focusedBodyPartToShootTransform;}}
    public Transform BodyPartToShootTransform {get {return _bodyPartToShootTransform;}}
    public Collider CurrWall {get {return _currWall;}}
    public Collider CharaHeadColl {get {return _charaHeadColl;}}
    public float CharaHeightBuffer {get {return _charaHeightBuffer;}}
    public float CharaWidth {get {return _charaWidth;}}
    public float IsCheckingLastPosTimer {get {return _isCheckingLastPosTimer; } set {_isCheckingLastPosTimer = value;}}
    public float IsCheckingLastPosTimerMax {get {return _isCheckingLastPosTimerMax;}}

    #endregion
    protected override void Awake() 
    {
        //Dari Chara Identity bisa akses ke semua yg berhubungan dgn characteridentity
        base.Awake();
        
        
        if(_agent == null)_agent = GetComponent<NavMeshAgent>();
        _charaWidth = _agent.radius * 2 + _charaWidthBuffer;
        if(_fovMachine == null)_fovMachine = GetComponent<FOVMachine>();
        
    }
    protected virtual void Start()
    {
        _takeCoverManager = TakeCoverManager.Instance;
    }
    public void AimAIPointLookAt(Transform lookAt)
    {
        if(lookAt != null) _aimAIPoint.LookAt(lookAt);
        else
        {
            if(_aimAIPoint.localRotation != Quaternion.identity)_aimAIPoint.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void SetAllowLookTarget(bool isAllowed, MovementStateMachine machine, Vector3 target, bool isReceivePosADirection)
    {
        if(isAllowed)machine.SetAITargetToLook(target, isReceivePosADirection);
        machine.AllowLookTarget = isAllowed;
    }


    #region TakeCover
    public void TakingCover()
    {
        _wallArrayNearChara = Physics.OverlapSphere(transform.position, _wallScannerDistance, _wallTakeCoverLayer);
        _wallTotal = _wallArrayNearChara.Length;
        
        WallListChecker();

        if(_wallTotal == 0) 
        {
            _canTakeCoverInThePosition = false;
            _currWall = null;
            return;
        }

        System.Array.Sort(_wallArrayNearChara,SortWallBasedOnClosestDistance);

        //distance

        for(int i = 0; i < _wallTotal; i++)
        {
            Collider currColl = _wallArrayNearChara[i];
            _currWallHeight = currColl.bounds.max.y;
            _isWallTallerThanChara = currColl.bounds.max.y > _charaHeadColl.bounds.max.y + _charaHeightBuffer;
            if(!IsPassedWallHeightChecker(currColl.bounds.max.y))continue;
            
            

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

            // Debug.Log("Wallsss1" + i + " " + _wallTotal + " " + _wallArrayNearChara[i].name + " " + transform.name + " " + canCheckFrontBehind + " " + canCheckLeftRightSide);
            // Debug.Log(halfWallWidth * 2 + " " + halfWallLength * 2 + " " + _charaWidth + " aa" + canCheckFrontBehind + canCheckLeftRightSide);
            if(!canCheckLeftRightSide && !canCheckFrontBehind) continue;
            Vector3 directionTotalEnemyToWall = GetTotalDirectionTargetPosAndEnemy(currWall, true);
            // Debug.DrawRay(_wallArrayNearChara[i].transform.position, directionTotalEnemyToWall * 100f, Color.black, 2f);

            // Debug.Log("Tolong ini enemy ga masuk sini ato apa maksudnya "+ i + " " + transform.name);



            float closestDistance = Mathf.Infinity; // ini cari jarak terdekat dr player 
            
            Vector3 newPos = Vector3.zero;

            if(canCheckFrontBehind)
            {
                float forwardWithEnemy = Vector3.Dot(directionTotalEnemyToWall, wallForward);

                if(forwardWithEnemy < _HideDotMin)
                {
                    // Debug.Log("Dot fwd normal " + forwardWithEnemy);
                    // Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallForward * 100f, Color.blue, 2f);

                    GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, wallForward, wallRight, directionTotalEnemyToWall);
                    

                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    forwardWithEnemy = Vector3.Dot(directionTotalEnemyToWall, -wallForward);
                    if(forwardWithEnemy < _HideDotMin)
                    {
                        // Debug.Log("Dot fwd balik " + forwardWithEnemy);
                        // Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallForward * 100f, Color.red, 2f);

                        GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, -wallForward, wallRight,directionTotalEnemyToWall);
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
                    // Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallRight * 100f, Color.grey, 2f);
                    GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, wallForward, wallRight, directionTotalEnemyToWall);
                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    rightWithEnemy = Vector3.Dot(directionTotalEnemyToWall, -wallRight);
                    if(rightWithEnemy < _HideDotMin)
                    {
                        // Debug.Log("Dot right balik " + rightWithEnemy);
                        // Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallRight * 100f, Color.magenta, 2f);
                        GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, wallForward, -wallRight, directionTotalEnemyToWall);
                    }
                    
                }
            }

            // Debug.Log("Wallsss2" + i + " " + _wallTotal + " " + _wallArrayNearChara[i].name + " " + closestDistance + " " + transform.name);

            // Vector3 dotEnemyWallFwd = Vector3.Dot()
            if(closestDistance != Mathf.Infinity)
            {
                _canTakeCoverInThePosition = true;
                _currWall = _wallArrayNearChara[i];
                _takeCoverPosition = newPos;
                // Debug.DrawRay(_takeCoverPosition, Vector3.up * 100f, Color.red);
                
                Vector3 NewPosToWall = (newPos - wallCenter).normalized;
                float NewPosForwardDistance = Vector3.Dot(NewPosToWall, wallForward); 
                float NewPosRightDistance = Vector3.Dot(NewPosToWall, wallRight); 

                // Debug.DrawRay(wallCenter, wallForward * 100f, Color.red);
                // Debug.DrawRay(wallCenter, wallRight * 100f, Color.blue);
                if(_isMovingOnXPos)
                {
                    if(NewPosRightDistance >= 0)
                    {
                        if(NewPosForwardDistance >= 0)
                        {
                            Debug.Log("Depan, kanan" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = -wallForward;
                        }
                        else
                        {
                            Debug.Log("Bawah, kiri" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = wallForward;
                        }
                        _dirToLookAtWhenTakingCover = wallRight;
                        
                    }
                    else
                    {
                        if(NewPosForwardDistance > 0)
                        {
                            Debug.Log("Depan, kiri" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = -wallForward;
                            
                        }
                        else
                        {
                            Debug.Log("Bawah, kanan" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = wallForward;
                        }
                        _dirToLookAtWhenTakingCover = -wallRight;
                    }
                    // Debug.Log(_dirToLookAtWhenChecking + " " + transform.name);
                    // if(isWallTallerThanChara) _dirToLookAtWhenChecking = new Vector3(_posToGoWhenCheckingWhenWallIsHigher.x, 0, _dirToLookAtWhenChecking.z);
                    // else _dirToLookAtWhenChecking = new Vector3(newPos.x, 0, _dirToLookAtWhenChecking.z);
                }
                else
                {
                    if(NewPosForwardDistance >= 0)
                    {
                        if(NewPosRightDistance > 0)
                        {
                            Debug.Log("kiri, atas" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = -wallRight;
                        }
                        else
                        {
                            Debug.Log("kanan, atas" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = wallRight;
                        }
                        _dirToLookAtWhenTakingCover = wallForward;
                    }
                    else
                    {
                        if(NewPosRightDistance >= 0) 
                        {
                            Debug.Log("kanan, bawah" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = -wallRight;
                        }
                        else 
                        {
                            Debug.Log("kiri, bawah" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = wallRight;
                        }
                        _dirToLookAtWhenTakingCover = -wallForward;
                    }
                    // Debug.Log(_dirToLookAtWhenChecking + " " + transform.name);
                    // if(isWallTallerThanChara) _dirToLookAtWhenChecking = new Vector3(_dirToLookAtWhenChecking.x, 0, _posToGoWhenCheckingWhenWallIsHigher.z);
                    // else _dirToLookAtWhenChecking = new Vector3(_dirToLookAtWhenChecking.x, 0, newPos.z);
                }

                
                break;
            } 
            else
            {
                _canTakeCoverInThePosition = false;
                _currWall = null;
            }
            

        }
    }
    protected virtual bool IsPassedWallHeightChecker(float wallHeight)
    {
        float charaHeightCrouch = _charaHeadColl.bounds.max.y + _charaHeightBuffer - 0.6f;
        if(wallHeight <= charaHeightCrouch) return false;
        return true;
    }
    public bool IsCrouchingBehindWall()
    {
        if(_currWallHeight > _charaHeadColl.bounds.max.y + _charaHeightBuffer - 0.6f)return true;
        return false;
    }
    //KALO MO NYARI INI HARUS NYARI LEAVEDIRECTION DL
    protected virtual bool IsThisASafePathToGo(Vector3 tempNewPos)
    {
        Vector3 firstPathNewPosToPlayer = (_tempFirstPathPos - transform.position).normalized;
        float dotLeaveDirWithNewPosDir = Vector3.Dot(firstPathNewPosToPlayer, _leaveDirection);

        if(dotLeaveDirWithNewPosDir >= -0.5f)return true;

        float dotForwardWithNewPosDir = Vector3.Dot(firstPathNewPosToPlayer, transform.forward);
        // if(dotForwardWithNewPosDir >= 0.95f && _fovMachine.VisibleTargets.Count == 0)return true;
        return false;
    }
    public void GetClosestPosition(bool isFrontBehind, ref float closestDistance, ref Vector3 newPos, float halfWallLength, float halfWallWidth, bool isWallTallerThanChara, Vector3 wallCenter, Vector3 wallForwardDir, Vector3 wallRightDir, Vector3 dirEnemyToWall)
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

                    if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                    // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue; // if it's before I can take cover, but i go through here again and it's the same place, meaning it's not a safe place,
                    float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);

                    if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                    {
                        Debug.Log(x + " " + distanceCharaToWall + "beforefb");
                        
                        

                        //ini dicek biar tau apakah masi keliatan ama enemy ga di posisi wall itu; wallcenter = transform wall
                        Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                        float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);
                        if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                        {
                            Debug.Log(x + " " + distanceCharaToWall + "afterfb");
                            closestDistance = distanceCharaToWall;
                            newPos = tempNewPos;
                            _isMovingOnXPos = isFrontBehind;

                            _posToGoWhenCheckingWhenWallIsHigher = wallCenter;
                            if(x == 0) _posToGoWhenCheckingWhenWallIsHigher += wallRightDir * (halfWallWidth - _buffer);
                            if(x == 1) _posToGoWhenCheckingWhenWallIsHigher += wallRightDir * -(halfWallWidth - _buffer);
                            _posToGoWhenCheckingWhenWallIsHigher += wallForwardDir * newHalfWallLength;
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

                        // Debug.Log(x + " " + newHalfWallWidth + " " + halfWallWidth + " " + wallRightDir * newHalfWallWidth + " FrontBehindx");
                        tempNewPos += wallRightDir * newHalfWallWidth;
                        tempNewPos += wallForwardDir * newHalfWallLength;

                        if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                        // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);

                        if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                        {
                            Debug.Log(x + " " + distanceCharaToWall + "beforefb");

                            Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                            float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);
                            if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                            {
                                Debug.Log(x + " " + distanceCharaToWall + "afterfb");
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;
                                _isMovingOnXPos = isFrontBehind;
                            }
                            // Debug.Log(x + " " + newPos + " FrontBehind");
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

                        if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                        // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                        {
                            Debug.Log(x + " " + distanceCharaToWall + "beforefb");

                            Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                            float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);
                            if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                            {
                                Debug.Log(x + " " + distanceCharaToWall + "afterfb");
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;
                                _isMovingOnXPos = isFrontBehind;
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

                    if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                    // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                    float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                    if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                    {
                        Debug.Log(x + " " + distanceCharaToWall + "beforelr");

                        Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                        float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);
                        
                        if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                        {
                            Debug.Log(x + " " + distanceCharaToWall + "afterlr");
                            closestDistance = distanceCharaToWall;
                            newPos = tempNewPos;

                            _posToGoWhenCheckingWhenWallIsHigher = wallCenter;
                            if(x == 0) _posToGoWhenCheckingWhenWallIsHigher += wallForwardDir * (halfWallLength - _buffer);
                            if(x == 1) _posToGoWhenCheckingWhenWallIsHigher += wallForwardDir * -(halfWallLength - _buffer);
                            _posToGoWhenCheckingWhenWallIsHigher += wallRightDir * newHalfWallWidth;

                            _isMovingOnXPos = isFrontBehind;
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

                        // Debug.Log(x + " " + newHalfWallLength + " " + halfWallLength + " " + wallForwardDir * newHalfWallLength + " LeftRightx");
                        tempNewPos += wallForwardDir * newHalfWallLength;
                        tempNewPos += wallRightDir * newHalfWallWidth;

                        if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                        // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                        {
                            Debug.Log(x + " " + distanceCharaToWall + "beforelr");

                            Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                            float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);
                            
                            if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                            {
                                Debug.Log(x + " " + distanceCharaToWall + "afterlr");
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;

                                _isMovingOnXPos = isFrontBehind;
                            }
                            // Debug.Log(x + " " + newPos + " LeftRight");
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

                        if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                        // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                        {
                            Debug.Log(x + " " + distanceCharaToWall + "beforelr");

                            Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                            float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);
                            
                            if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                            {
                                Debug.Log(x + " " + distanceCharaToWall + "afterlr");
                                closestDistance = distanceCharaToWall;
                                newPos = tempNewPos;

                                _isMovingOnXPos = isFrontBehind;
                            }
                        }
                    }
                }
            }
        }
    }
    
    protected virtual void WallListChecker()
    {
        for(int i=0; i < _wallArrayNearChara.Length; i++)
        {
            foreach(Transform enemy in _enemyWhoSawAIListContainer)
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
    }
    protected int SortWallBasedOnClosestDistance(Collider A, Collider B)
    {
        if(A != null && B == null) return -1;
        if(A == null && B != null) return 1;
        if(A == null && B == null) return 0;
        float distanceA = Vector3.Distance(A.transform.position, transform.position);
        float distanceB = Vector3.Distance(B.transform.position, transform.position);
        return distanceA.CompareTo(distanceB);
    }
    #endregion
    public float CountNavMeshPathDistance(Vector3 origin, Vector3 target)
    {
        path = new NavMeshPath();
        if(NavMesh.CalculatePath(origin, target, _agent.areaMask, path))
        {
            Debug.Log(path.status + " PATH STATUS NOW" + target);
            _tempFirstPathPos = path.corners[0];
            float distance = Vector3.Distance(origin, path.corners[0]);
            for(int j = 1; j < path.corners.Length; j++)
            {
                if(path.status != NavMeshPathStatus.PathComplete)return Mathf.Infinity;
                distance += Vector3.Distance(path.corners[j-1], path.corners[j]);
            }
            return distance;
        }
        Debug.Log("no path");
        return Mathf.Infinity;
    }
    public Vector3 GetTotalDirectionTargetPosAndEnemy(Transform targetPos, bool isFromTarget)
    {
        Vector3 directionTotal = Vector3.zero;
        
        foreach(Transform enemy in _enemyWhoSawAIListContainer)
        {
            Vector3 direction = Vector3.zero;
            if(isFromTarget) direction = enemy.transform.position - targetPos.position;
            else direction = targetPos.position - enemy.transform.position;

            direction = direction.normalized;
            directionTotal += direction;
        }

        return directionTotal;
    }

    #region Getting Enemy
    //Both enemy and friend ai can use this; if friend use this, that means enemy transform is enemy; if enemy use this, that means friend ai is the enemy
    public void DetectEnemy()
    {
        if(GetFOVMachine.VisibleTargets.Count > 0)
        {
            foreach(Transform target in GetFOVMachine.VisibleTargets)
            {
                AIBehaviourStateMachine targetAI = target.GetComponent<AIBehaviourStateMachine>();
                if(targetAI != null)targetAI.EnemyDetectedChara(transform);
            }
        }
    }
    public void CheckPastVisibleTargets()
    {
        if(_pastVisibleTargets.Count > 0)
        {
            for(int i=0; i < _pastVisibleTargets.Count; i++)
            {
                if(!_fovMachine.VisibleTargets.Contains(_pastVisibleTargets[i]))
                {
                    AIBehaviourStateMachine _pastVisibleTargetsAI = _pastVisibleTargets[i].GetComponent<AIBehaviourStateMachine>();
                    if(_pastVisibleTargetsAI != null)_pastVisibleTargetsAI.EnemyNotDetectCharaAnymore(transform);
                }

            }
        }
        _pastVisibleTargets = new List<Transform>(_fovMachine.VisibleTargets);
    }
    public void EnemyDetectedChara(Transform enemyTransform)
    {
        if(!_enemyWhoSawAIList.Contains(enemyTransform))_enemyWhoSawAIList.Add(enemyTransform);
        _enemyWhoSawAIListContainer = new List<Transform>(EnemyWhoSawAIList);

        _gotDetectedByEnemy = true;
        _gotDetectedTimer = _gotDetectedTimerMax;
    }
    public void EnemyNotDetectCharaAnymore(Transform enemyTransform)
    {
        if(_enemyWhoSawAIList.Contains(enemyTransform))_enemyWhoSawAIList.Remove(enemyTransform);
    }
    public void GotDetectedTimerCounter()
    {
        if(_gotDetectedTimer > 0)_gotDetectedTimer -= Time.deltaTime;
        else
        {
            NotDetectedAnymore();
        }
    }
    public void NotDetectedAnymore()
    {
        _gotDetectedTimer = 0f;
        _gotDetectedByEnemy = false;
        _enemyWhoSawAIList.Clear();
        _enemyWhoSawAIListContainer.Clear();
    }

    #endregion
    #region RunAway
    public void RunAwayDirCalculation()
    {

        Vector3 runAwayDir = LeaveDirection;
        _isThereNoPathInRunAwayDirection = Physics.Raycast(transform.position, LeaveDirection, out _runAwayObstacleHit, 4f, RunAwayObstacleMask);
        // Debug.DrawRay(transform.position, LeaveDirection, Color.blue);
        
        if(_isThereNoPathInRunAwayDirection)
        {
            Vector3 alternativeDir = Vector3.Cross(LeaveDirection, Vector3.up).normalized;
            // Debug.DrawRay(transform.position, alternativeDir, Color.red);
            runAwayDir = alternativeDir;
        }
        _runAwayPos = transform.position + runAwayDir * 2f;
        // }

    }
    public virtual void RunAway()
    {
        RunAwayDirCalculation();
    }
    #endregion

    public virtual void GetClosestEnemyWhoSawAI()
    {
        float _tempDistanceEnemy = Mathf.Infinity;
        _closestEnemyWhoSawAI = null;
        if(EnemyWhoSawAIListContainer.Count > 0)
        {
            foreach(Transform enemy in EnemyWhoSawAIListContainer)
            {
                // Debug.Log("Shoot dirrr23" + enemy.position);
                float currDis = Vector3.Distance(transform.position, enemy.position);
                if(_tempDistanceEnemy > currDis)
                {
                    _tempDistanceEnemy = currDis;
                    _closestEnemyWhoSawAI = enemy;
                }
            }
        }
    }

    #region BestBodyPartToShoot
    public Transform SearchBestBodyPartToShoot(Transform ClosestEnemy)
    {
        _focusedBodyPartToShootTransform = null;
        _bodyPartToShootTransform = null;
        Body closestEnemyBody = ClosestEnemy.GetComponent<Body>();
        for(int i=0; i < closestEnemyBody.bodyParts.Count; i++)
        {
            Vector3 dir = (closestEnemyBody.bodyParts[i].transform.position - _fovMachine.GetFOVPoint.position).normalized;
            if(Physics.Raycast(_fovMachine.GetFOVPoint.position, dir, out RaycastHit hit, _fovMachine.viewRadius,_bodyPartMask))
            {
                // Debug.DrawRay(_fovMachine.GetFOVPoint.position, dir * 100f, Color.red);
                // Debug.Log(_fovMachine.CharaEnemyMask + " LAYER MASK BICH");
                // Debug.Log(transform.name + " AAAAAAAAAAAAA" + hit.transform + "   " + i );
                if(hit.transform == closestEnemyBody.bodyParts[i].transform)
                {
                    if(closestEnemyBody.bodyParts[i].bodyType == _focusedBodyPartsToShoot)
                    {
                        _focusedBodyPartToShootTransform = closestEnemyBody.bodyParts[i].transform;
                        break;
                    }
                    if(_bodyPartToShootTransform == null)_bodyPartToShootTransform = closestEnemyBody.bodyParts[i].transform;
                    
                }
            }
        }
        // Debug.Log(transform.name + "Focus keeee" + _focusedBodyPartsToShoot + " lalu kee" + _bodyPartToShootTransform + " atauu " + ClosestEnemy + "raa" + closestEnemyBody);
        if(_focusedBodyPartToShootTransform != null)
        {
            return _focusedBodyPartToShootTransform;
        }
        else if(_bodyPartToShootTransform != null)
        {
            return _bodyPartToShootTransform;
        }
        else
        {
            return ClosestEnemy;
        }
    }
    #endregion

}

using System;

using System.Collections.Generic;


using UnityEngine;
using UnityEngine.AI;

public abstract class AIBehaviourStateMachine : BaseStateMachine, IUnsubscribeEvent
{
    protected GameManager _gm;
    #region Normal Variable
    protected bool _isAIInput = true;
    protected EnemyAIManager _enemyAIManager;
    protected TakeCoverManager _takeCoverManager;
    protected FOVMachine _fovMachine;
    protected NavMeshAgent _agent;
    protected MovementStateMachine _moveStateMachine;
    protected UseWeaponStateMachine _useWeaponStateMachine;

    [Header("Other Important Variable")]
    [SerializeField] protected Transform _aimAIPoint;

    #region  Take Cover Component
    [Header("Take Cover Component")]
    [ReadOnly(false), SerializeField] protected bool _isTakingCover;
    [ReadOnly(false), SerializeField] protected bool _isHiding, _isChecking;
    [ReadOnly(false), SerializeField] protected bool _isAtTakingCoverHidingPlace;
    [ReadOnly(false), SerializeField] protected bool _isAtTakingCoverCheckingPlace;

    [Space(1)]
    [ReadOnly(false), SerializeField] protected Collider[] _wallArrayNearChara;
    [ReadOnly(false), SerializeField] protected float _wallTotal;
    [SerializeField] protected float _wallScannerDistance;
    [SerializeField] protected LayerMask _wallTakeCoverLayer;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField][Range(-1, 1f)] protected float _HideDotMin = 0f;
    [SerializeField] protected float _charaMaxTakeCoverDistance = 100f;
    [SerializeField] protected float _wallTakeCoverPosBuffer = -1.5f;

    [Space(1)]
    [SerializeField] protected Collider _charaHeadColl;
    [SerializeField] protected float _charaHeightBuffer = 0.15f;
    protected float _charaWidth;
    [SerializeField] protected float _charaWidthBuffer = 0.2f;
    // [SerializeField] protected float _wallShortStepSize = 0.5f;

    [Space(1)]
    [ReadOnly(false), SerializeField] protected Collider _currWall;
    [ReadOnly(false), SerializeField] protected float _currWallHeight;
    [ReadOnly(false), SerializeField] protected bool _isWallTallerThanChara;
    [ReadOnly(false), SerializeField] protected Vector3 _takeCoverPosition;
    [ReadOnly(false), SerializeField] protected bool _canTakeCoverInThePosition;
    [ReadOnly(false), SerializeField] protected Vector3 _leaveDirection;
    protected Vector3 _dirToLookAtWhenTakingCover;
    protected Vector3 _dirToLookAtWhenChecking;
    protected Vector3 _posToGoWhenCheckingWhenWallIsHigher, _tempPosToGoWhenCheckingWhenWallIsHigher;
    protected bool _isAtTheLeftSideOfTheWall;
    protected bool _isMovingOnXPos;
    #endregion

    [Space(1)]
    [Header("Component to Save Enemy Who sees us")]
    [ReadOnly(false), SerializeField] protected List<Transform> _enemyWhoSawAIList = new List<Transform>();
    [ReadOnly(false), SerializeField] protected List<Transform> _enemyWhoSawAIListContainer = new List<Transform>();
    [ReadOnly(false), SerializeField] protected List<Transform> _pastVisibleTargets = new List<Transform>();
    protected Transform _closestEnemyWhoSawAI;
    [SerializeField] protected int _minEnemyMakeCharaFeelOverwhelmed = 3;
    [SerializeField] protected float _enemyMaxDistanceFromWalls = 10f;
    [ReadOnly(false), SerializeField] protected bool _gotDetectedByEnemy;
    [ReadOnly(false), SerializeField] protected float _gotDetectedTimer;
    [SerializeField] protected float _gotDetectedTimerMax = 0.3f;
    protected NavMeshPath path;

    [Space(1)]
    [Header("Running Away Component")]
    [SerializeField]protected LayerMask _runAwayObstacleMask;
    protected RaycastHit _runAwayObstacleHit;
    protected bool _isThereNoPathInRunAwayDirection;
    protected Vector3 _runAwayPos;

    [Space(1)]
    [Header("Advanced Shooting AI")]
    [SerializeField] protected bodyParts _focusedBodyPartsToShoot;
    protected Transform _focusedBodyPartToShootTransform;
    protected Transform _bodyPartToShootTransform;
    protected LayerMask _bodyPartMask;

    protected Vector3 _tempFirstPathPos;
    protected float _hidingCheckDelayTimer;
    [ReadOnly(false), SerializeField] protected float _isCheckingLastPosTimer;
    [SerializeField] protected float _isCheckingLastPosTimerMax = 1f;

    #endregion
    #region  GETTER SETTER VARIABLE
    public MovementStateMachine GetMoveStateMachine { get { return _moveStateMachine; } }
    public UseWeaponStateMachine GetUseWeaponStateMachine { get {return _useWeaponStateMachine;}}
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
        _gm = GameManager.instance;
        _enemyAIManager = EnemyAIManager.Instance;
        _takeCoverManager = TakeCoverManager.Instance;
        _moveStateMachine = _charaIdentity.GetMovementStateMachine;
        _moveStateMachine.OnIsTheSamePosition += MoveStateMachine_OnIsTheSamePosition;

        _useWeaponStateMachine = _charaIdentity.GetUseWeaponStateMachine;
        _bodyPartMask = _useWeaponStateMachine.CharaEnemyMask;
    }
    public void AimAIPointLookAt(Transform lookAt)
    {
        if(lookAt != null) _aimAIPoint.LookAt(lookAt);
        else
        {
            if(_aimAIPoint.localRotation != Quaternion.identity) _aimAIPoint.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void SetAllowLookTarget(bool isAllowed, Vector3 target, bool isReceivePosADirection)
    {
        if(isAllowed) _moveStateMachine.SetAITargetToLook(target, isReceivePosADirection);
        _moveStateMachine.AllowLookTarget = isAllowed;
    }
    public virtual void HandleGotDetected()
    {
        GotDetectedTimerCounter();
        if(_gotDetectedByEnemy)
        {   
            _leaveDirection = GetTotalDirectionTargetPosAndEnemy(transform, false);
        }
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

            if(!canCheckLeftRightSide && !canCheckFrontBehind) continue;
            Vector3 directionTotalEnemyToWall = GetTotalDirectionTargetPosAndEnemy(currWall, true);
            // Debug.DrawRay(_wallArrayNearChara[i].transform.position, directionTotalEnemyToWall * 100f, Color.black, 2f);

            float closestDistance = Mathf.Infinity; // ini cari jarak terdekat dr player 
            
            Vector3 newPos = Vector3.zero;

            if(canCheckFrontBehind)
            {
                float forwardWithEnemy = Vector3.Dot(directionTotalEnemyToWall, wallForward);

                if(forwardWithEnemy < _HideDotMin)
                {
                    // Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallForward * 100f, Color.blue, 2f);

                    GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, wallForward, wallRight, directionTotalEnemyToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                    

                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    forwardWithEnemy = Vector3.Dot(directionTotalEnemyToWall, -wallForward);
                    if(forwardWithEnemy < _HideDotMin)
                    {
                      // Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallForward * 100f, Color.red, 2f);

                        GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, -wallForward, wallRight,directionTotalEnemyToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                    } 
                }
                
            }

            // tempNewPos = wallCenter;
            if(canCheckLeftRightSide)
            {
                float rightWithEnemy = Vector3.Dot(directionTotalEnemyToWall, wallRight);
                if(rightWithEnemy < _HideDotMin)
                {
                    // Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallRight * 100f, Color.grey, 2f);
                    GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, wallForward, wallRight, directionTotalEnemyToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    rightWithEnemy = Vector3.Dot(directionTotalEnemyToWall, -wallRight);
                    if(rightWithEnemy < _HideDotMin)
                    {
                        // Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallRight * 100f, Color.magenta, 2f);
                        GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, _isWallTallerThanChara, wallCenter, wallForward, -wallRight, directionTotalEnemyToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                    }
                    
                }
            }

            
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
                            // Debug.Log("Depan, kanan" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = -wallForward;
                        }
                        else
                        {
                            // Debug.Log("Bawah, kiri" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = wallForward;
                        }
                        _dirToLookAtWhenTakingCover = wallRight;
                        
                    }
                    else
                    {
                        if(NewPosForwardDistance > 0)
                        {
                            // Debug.Log("Depan, kiri" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = -wallForward;
                            
                        }
                        else
                        {
                            // Debug.Log("Bawah, kanan" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = wallForward;
                        }
                        _dirToLookAtWhenTakingCover = -wallRight;
                    }
                }
                else
                {
                    if(NewPosForwardDistance >= 0)
                    {
                        if(NewPosRightDistance > 0)
                        {
                            // Debug.Log("kiri, atas" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = -wallRight;
                        }
                        else
                        {
                            // Debug.Log("kanan, atas" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = wallRight;
                        }
                        _dirToLookAtWhenTakingCover = wallForward;
                    }
                    else
                    {
                        if(NewPosRightDistance >= 0) 
                        {
                            // Debug.Log("kanan, bawah" + transform.name);
                            _isAtTheLeftSideOfTheWall = false;
                            _dirToLookAtWhenChecking = -wallRight;
                        }
                        else 
                        {
                            // Debug.Log("kiri, bawah" + transform.name);
                            _isAtTheLeftSideOfTheWall = true;
                            _dirToLookAtWhenChecking = wallRight;
                        }
                        _dirToLookAtWhenTakingCover = -wallForward;
                    }
                }

                if(isWallTallerThanChara)
                {
                    Vector3 dir = (_posToGoWhenCheckingWhenWallIsHigher - _takeCoverPosition).normalized;
                    float distance = Vector3.Distance(_posToGoWhenCheckingWhenWallIsHigher, _takeCoverPosition);
                    if(Physics.Raycast(_takeCoverPosition, dir, out RaycastHit hitObstacle, distance, _runAwayObstacleMask))
                    {
                        _posToGoWhenCheckingWhenWallIsHigher = _tempPosToGoWhenCheckingWhenWallIsHigher;
                        _dirToLookAtWhenTakingCover = -_dirToLookAtWhenTakingCover;
                    }
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

        // Debug.DrawRay(transform.position, firstPathNewPosToPlayer * 100f, Color.red, 0.5f, false);
        // Debug.DrawRay(transform.position, _leaveDirection * 100f, Color.black, 0.5f, false);
        // Debug.Log(transform.name + "ceking AI again" + " dot" + dotLeaveDirWithNewPosDir);

        if(dotLeaveDirWithNewPosDir >= -0.5f)return true;


        float dotForwardWithNewPosDir = Vector3.Dot(firstPathNewPosToPlayer, transform.forward);
        // if(dotForwardWithNewPosDir >= 0.95f && _fovMachine.VisibleTargets.Count == 0)return true;
        return false;
    }
    protected virtual float FindGroundPosition(Vector3 tempNewPos, float wallHeight, float wallMinBound, float wallCenterY)
    {
        if(Physics.Raycast(tempNewPos, Vector3.down, out RaycastHit hitGround, wallHeight, _groundLayer))
        {
            if(hitGround.point.y < wallMinBound - 0.2f)
            {
                // Debug.Log(transform.name + "ceking AI again" + " it hit ground tp trlalu rendah dr bounds " + wallMinBound + " tmpt ke hitnya di" + hitGround.point.y);
                return wallCenterY;
            }
            // Debug.Log(transform.name + "ceking AI again" + " it hit ground" + wallMinBound + " tmpt ke hitnya di" + hitGround.point.y);
            return hitGround.point.y;
        }
        
        return wallCenterY;
    }
    public void GetClosestPosition(bool isFrontBehind, ref float closestDistance, ref Vector3 newPos, float halfWallLength, float halfWallWidth, bool isWallTallerThanChara, Vector3 wallCenter, Vector3 wallForwardDir, Vector3 wallRightDir, Vector3 dirEnemyToWall, float wallMaxBoundsY, float wallMinBoundsY, float wallHeight)
    {
        float newHalfWallWidth = 0;
        float newHalfWallLength = 0;
        
        Vector3 tempNewPos = wallCenter;
        
        if(isFrontBehind)
        {
            newHalfWallLength = halfWallLength - _wallTakeCoverPosBuffer;
            
            if(isWallTallerThanChara)
            {
                newHalfWallWidth = halfWallWidth + _wallTakeCoverPosBuffer;
                for(int x = 0; x < 2; x++)
                {
                    tempNewPos = wallCenter;
                    if(x == 0) tempNewPos += wallRightDir * newHalfWallWidth;
                    if(x == 1) tempNewPos += wallRightDir * -newHalfWallWidth;
                    tempNewPos += wallForwardDir * newHalfWallLength;

                    tempNewPos = new Vector3(tempNewPos.x, wallMaxBoundsY, tempNewPos.z);
                    float newY = FindGroundPosition(tempNewPos, wallHeight, wallMinBoundsY, wallCenter.y);
                    tempNewPos = new Vector3(tempNewPos.x, newY, tempNewPos.z);

                    if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                    
                    float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);

                    // Debug.Log(transform.name + "ceking AI again" + " wallcenter is " + wallCenter + " distancenya" + distanceCharaToWall + " posisi barunya" + tempNewPos);
                    if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                    {
                        
                        
                        Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                        float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);

                        // Debug.DrawRay(wallCenter, dirNewPosToWall * 100f, Color.red, 1f, false);
                        // Debug.DrawRay(wallCenter, dirEnemyToWall * 100f, Color.black, 1f, false);

                        // Debug.Log(transform.name + "ceking AI again" + "DotEnemyVSNewPOS" + dotEnemyVSNewPOs);

                        if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                        {
                        
                            closestDistance = distanceCharaToWall;
                            newPos = tempNewPos;
                            _isMovingOnXPos = isFrontBehind;

                            _posToGoWhenCheckingWhenWallIsHigher = wallCenter;
                            _tempPosToGoWhenCheckingWhenWallIsHigher = wallCenter;
                            if(x == 0)
                            {
                                _posToGoWhenCheckingWhenWallIsHigher += wallRightDir * (halfWallWidth - _wallTakeCoverPosBuffer);

                                _tempPosToGoWhenCheckingWhenWallIsHigher += wallRightDir * -(halfWallWidth - _wallTakeCoverPosBuffer);
                            }
                            if(x == 1)
                            {
                                _posToGoWhenCheckingWhenWallIsHigher += wallRightDir * -(halfWallWidth - _wallTakeCoverPosBuffer);

                                _tempPosToGoWhenCheckingWhenWallIsHigher += wallRightDir * (halfWallWidth - _wallTakeCoverPosBuffer);
                            }
                            _posToGoWhenCheckingWhenWallIsHigher += wallForwardDir * newHalfWallLength;
                            _tempPosToGoWhenCheckingWhenWallIsHigher += wallForwardDir * newHalfWallLength;

                            // _posToGoWhenCheckingWhenWallIsHigher = new Vector3(_posToGoWhenCheckingWhenWallIsHigher.x, transform.position.y, _posToGoWhenCheckingWhenWallIsHigher.z);
                            // _tempPosToGoWhenCheckingWhenWallIsHigher = new Vector3(_tempPosToGoWhenCheckingWhenWallIsHigher.x, transform.position.y, _tempPosToGoWhenCheckingWhenWallIsHigher.z);
                        }
                        
                    }
                }
            }
            else
            {
                if(halfWallWidth > _charaWidth)
                {
                    for(float x = 0; x <= halfWallWidth + _wallTakeCoverPosBuffer; x += _charaWidth)
                    {
                        for(float y = 0; y < 2; y++)
                        {
                            tempNewPos = wallCenter;
                            tempNewPos += wallRightDir * x;
                            tempNewPos += wallForwardDir * newHalfWallLength;


                            tempNewPos = new Vector3(tempNewPos.x, wallMaxBoundsY, tempNewPos.z);
                            float newY = FindGroundPosition(tempNewPos, wallHeight, wallMinBoundsY, wallCenter.y);
                            tempNewPos = new Vector3(tempNewPos.x, newY, tempNewPos.z);

                            // Debug.Log(transform.name + " newhalfwallWidth" + x + "tempnewPos" + tempNewPos);
                            if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                            // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                            float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);

                            if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                            {
                                

                                Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                                float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);

                                // Debug.DrawRay(wallCenter, dirNewPosToWall * 100f, Color.red, 1f, false);
                                // Debug.DrawRay(wallCenter, dirEnemyToWall * 100f, Color.black, 1f, false);

                                // Debug.Log(transform.name + "ceking AI again" + " wallcenter is " + wallCenter + " distancenya" + distanceCharaToWall + "Dot" + dotEnemyVSNewPOs);
                                if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                                {
                                    
                                    closestDistance = distanceCharaToWall;
                                    newPos = tempNewPos;
                                    _isMovingOnXPos = isFrontBehind;
                                }
                                // Debug.Log(x + " " + newPos + " FrontBehind");
                            }
                            if(x == 0)break;
                            x *= -1;
                        }
                    }
                    
                }
                else
                {
                    newHalfWallWidth = halfWallWidth + _wallTakeCoverPosBuffer;
                    for(int x = 0; x < 2; x++)
                    {
                        tempNewPos = wallCenter;
                        if(x == 0) tempNewPos += wallRightDir * newHalfWallWidth;
                        if(x == 1) tempNewPos += wallRightDir * -newHalfWallWidth;
                        tempNewPos += wallForwardDir * newHalfWallLength;
                        // tempNewPos = new Vector3(tempNewPos.x, transform.position.y, tempNewPos.z);
                        tempNewPos = new Vector3(tempNewPos.x, wallMaxBoundsY, tempNewPos.z);
                        float newY = FindGroundPosition(tempNewPos, wallHeight, wallMinBoundsY, wallCenter.y);
                        tempNewPos = new Vector3(tempNewPos.x, newY, tempNewPos.z);

                        if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                        // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                        {


                            Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                            float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);


                            // Debug.DrawRay(wallCenter, dirNewPosToWall * 100f, Color.red, 1f, false);
                            // Debug.DrawRay(wallCenter, dirEnemyToWall * 100f, Color.black, 1f, false);

                            // Debug.Log(transform.name + "ceking AI again" + " wallcenter is " + wallCenter + " distancenya" + distanceCharaToWall + "Dot" + dotEnemyVSNewPOs);
                            if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                            {

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
            newHalfWallWidth = halfWallWidth - _wallTakeCoverPosBuffer;
            if(isWallTallerThanChara)
            {
                newHalfWallLength = halfWallLength + _wallTakeCoverPosBuffer;
                for(int x = 0; x < 2; x++)
                {
                    tempNewPos = wallCenter;
                    if(x == 0) tempNewPos += wallForwardDir * newHalfWallLength;
                    if(x == 1) tempNewPos += wallForwardDir * -newHalfWallLength;
                    tempNewPos += wallRightDir * newHalfWallWidth;

                    tempNewPos = new Vector3(tempNewPos.x, wallMaxBoundsY, tempNewPos.z);
                    float newY = FindGroundPosition(tempNewPos, wallHeight, wallMinBoundsY, wallCenter.y);
                    tempNewPos = new Vector3(tempNewPos.x, newY, tempNewPos.z);
                    // tempNewPos = new Vector3(tempNewPos.x, transform.position.y, tempNewPos.z);

                    if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                    // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                    float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                    if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                    {

                        Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                        float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);

                        // Debug.DrawRay(wallCenter, dirNewPosToWall * 100f, Color.red, 1f, false);
                        // Debug.DrawRay(wallCenter, dirEnemyToWall * 100f, Color.black, 1f, false);

                        // Debug.Log(transform.name + "ceking AI again" + " wallcenter is " + wallCenter + " distancenya" + distanceCharaToWall + "Dot" + dotEnemyVSNewPOs);

                        if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                        {
                            
                            closestDistance = distanceCharaToWall;
                            newPos = tempNewPos;

                            _posToGoWhenCheckingWhenWallIsHigher = wallCenter;
                            _tempPosToGoWhenCheckingWhenWallIsHigher = wallCenter;
                            if(x == 0) 
                            {
                                _posToGoWhenCheckingWhenWallIsHigher += wallForwardDir * (halfWallLength - _wallTakeCoverPosBuffer);
                                
                                _tempPosToGoWhenCheckingWhenWallIsHigher += wallForwardDir * -(halfWallLength - _wallTakeCoverPosBuffer);
                            }
                            if(x == 1)
                            {
                                _posToGoWhenCheckingWhenWallIsHigher += wallForwardDir * -(halfWallLength - _wallTakeCoverPosBuffer);

                                _tempPosToGoWhenCheckingWhenWallIsHigher += wallForwardDir * (halfWallLength - _wallTakeCoverPosBuffer);
                            }
                            _posToGoWhenCheckingWhenWallIsHigher += wallRightDir * newHalfWallWidth;
                            _tempPosToGoWhenCheckingWhenWallIsHigher += wallRightDir * newHalfWallWidth;

                            // _posToGoWhenCheckingWhenWallIsHigher = new Vector3(_posToGoWhenCheckingWhenWallIsHigher.x, transform.position.y, _posToGoWhenCheckingWhenWallIsHigher.z);
                            // _tempPosToGoWhenCheckingWhenWallIsHigher = new Vector3(_tempPosToGoWhenCheckingWhenWallIsHigher.x, transform.position.y, _tempPosToGoWhenCheckingWhenWallIsHigher.z);

                            _isMovingOnXPos = isFrontBehind;
                        }
                    }
                }
            }
            else
            {
                if(halfWallLength > _charaWidth)
                {
                    for(float x = 0; x <= halfWallLength + _wallTakeCoverPosBuffer; x += _charaWidth)
                    {
                        for(float y = 0; y < 2; y++)
                        {
                            tempNewPos = wallCenter;
                            tempNewPos += wallForwardDir * x;
                            tempNewPos += wallRightDir * newHalfWallWidth;
                            // Debug.Log(transform.name + " newhalfwallLength" + x + "tempnewPos" + tempNewPos);

                            tempNewPos = new Vector3(tempNewPos.x, wallMaxBoundsY, tempNewPos.z);
                            float newY = FindGroundPosition(tempNewPos, wallHeight, wallMinBoundsY, wallCenter.y);
                            tempNewPos = new Vector3(tempNewPos.x, newY, tempNewPos.z);

                            if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                            // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                            float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                            if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                            {
                                

                                Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                                float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);

                                // Debug.DrawRay(wallCenter, dirNewPosToWall * 100f, Color.red, 1f, false);
                                // Debug.DrawRay(wallCenter, dirEnemyToWall * 100f, Color.black, 1f, false);

                                // Debug.Log(transform.name + "ceking AI again" + " wallcenter is " + wallCenter + " distancenya" + distanceCharaToWall + "Dot" + dotEnemyVSNewPOs);
                                
                                if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                                {
                                    
                                    closestDistance = distanceCharaToWall;
                                    newPos = tempNewPos;

                                    _isMovingOnXPos = isFrontBehind;
                                }
                                
                            }
                    
                            if(x == 0)break;
                            x *= -1;
                        }
                    }
                }
                else
                {
                    newHalfWallLength = halfWallLength + _wallTakeCoverPosBuffer;
                    for(int x = 0; x < 2; x++)
                    {
                        tempNewPos = wallCenter;
                        if(x == 0) tempNewPos += wallForwardDir * newHalfWallLength;
                        if(x == 1) tempNewPos += wallForwardDir * -newHalfWallLength;
                        tempNewPos += wallRightDir * newHalfWallWidth;

                        tempNewPos = new Vector3(tempNewPos.x, wallMaxBoundsY, tempNewPos.z);
                        float newY = FindGroundPosition(tempNewPos, wallHeight, wallMinBoundsY, wallCenter.y);
                        tempNewPos = new Vector3(tempNewPos.x, newY, tempNewPos.z);
                        // tempNewPos = new Vector3(tempNewPos.x, transform.position.y, tempNewPos.z);
                        if(_takeCoverManager.IsTakeCoverPosOccupied(tempNewPos, this))continue;
                        // if(_canTakeCoverInThePosition)if(tempNewPos == TakeCoverPosition)continue;
                        float distanceCharaToWall = CountNavMeshPathDistance(transform.position, tempNewPos);
                        if(closestDistance > distanceCharaToWall && distanceCharaToWall <= _charaMaxTakeCoverDistance)
                        {
                            

                            Vector3 dirNewPosToWall = (tempNewPos - wallCenter).normalized;
                            float dotEnemyVSNewPOs = Vector3.Dot(dirNewPosToWall, dirEnemyToWall);

                            // Debug.DrawRay(wallCenter, dirNewPosToWall * 100f, Color.red, 1f, false);
                            // Debug.DrawRay(wallCenter, dirEnemyToWall * 100f, Color.black, 1f, false);

                            // Debug.Log(transform.name + "ceking AI again" + " wallcenter is " + wallCenter + " distancenya" + distanceCharaToWall + "Dot" + dotEnemyVSNewPOs);
                            
                            if(IsThisASafePathToGo(tempNewPos) && dotEnemyVSNewPOs < _HideDotMin)
                            {
                                
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
            // Debug.Log(path.status + " PATH STATUS NOW" + target);
            _tempFirstPathPos = path.corners[0];
            float distance = Vector3.Distance(origin, path.corners[0]);
            for(int j = 1; j < path.corners.Length; j++)
            {
                if(path.status != NavMeshPathStatus.PathComplete)return Mathf.Infinity;
                distance += Vector3.Distance(path.corners[j-1], path.corners[j]);
            }
            return distance;
        }
        // Debug.Log("no path");
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
    public virtual void RunAwayOption()
    {
        IsTakingCover = false;
        
        RunAway();
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

    protected abstract void MoveStateMachine_OnIsTheSamePosition(Vector3 agentPos);
    public virtual void UnsubscribeEvent()
    {
        _moveStateMachine.OnIsTheSamePosition -= MoveStateMachine_OnIsTheSamePosition;
    }

    public virtual void StartShooting(Transform chosenTarget)
    {
        AimAIPointLookAt(chosenTarget);
        _useWeaponStateMachine.GiveChosenTarget(chosenTarget);
        _charaIdentity.Shooting(true);
    }
    public virtual void StopShooting()
    {
        if(_useWeaponStateMachine.ChosenTarget != null) _useWeaponStateMachine.GiveChosenTarget(null);
        _charaIdentity.Shooting(false);
    }

    public void DeleteKilledEnemyFromList(Transform enemy)
    {
        if(_enemyWhoSawAIList.Contains(enemy))_enemyWhoSawAIList.Remove(enemy);
        if(_enemyWhoSawAIListContainer.Contains(enemy))_enemyWhoSawAIListContainer.Remove(enemy);
        if(_enemyWhoSawAIListContainer.Count == 0)NotDetectedAnymore();
    }
}

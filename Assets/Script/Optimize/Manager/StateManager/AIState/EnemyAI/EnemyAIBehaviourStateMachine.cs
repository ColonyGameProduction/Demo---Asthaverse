using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAIBehaviourStateMachine : AIBehaviourStateMachine, IHearSound
{
    [Header("Manager")]
    
    [Header("State Machine")]
    private EnemyIdentity _enemyIdentity;
    

    [Header("Enemy Alert Value")]
    [ReadOnly(false), SerializeField] private float _alertValue;
    [ReadOnly(false), SerializeField] private float _maxAlertValue;
    [SerializeField] private float _alertValueCountMultiplier = 10f;

    [Header("Enemy AI States")]
    [ReadOnly(false), SerializeField] private bool _isAIIdle;
    [ReadOnly(false), SerializeField] private bool _isAIHunted, _isAIEngage, _isCheckingEnemyInHunt, _isGoingToLastPos;
    private IFOVMachineState _getFOVState;
    private IHuntPlayable _getFOVAdvancedData;
    private EnemyAIState _currState;
    private EnemyAIStateFactory _states;
    private Transform _currPOI;
    
    [Header("Patrol Path")]
    [SerializeField] private Transform[] _patrolPath;
    private bool _switchingPath;
    private int _currPath;

    public Action<float, float> OnAlertValueChanged;

    #region GETTERSETTER Variable
    public Transform CurrPOI {get { return _currPOI;} set { _currPOI = value;}}
    public EnemyAIManager EnemyAIManager { get { return _enemyAIManager;}}
    public bool IsAIIdle {get {return _isAIIdle;} set{ _isAIIdle = value;} }
    public bool IsAIHunted {get {return _isAIHunted;} set{ _isAIHunted = value;} }
    public bool IsAIEngage {get {return _isAIEngage;} set{ _isAIEngage = value;} }
    public bool IsCheckingEnemyInHunt {get {return _isCheckingEnemyInHunt;} set{ _isCheckingEnemyInHunt = value;} }
    public bool IsGoingToLastPos {get {return _isGoingToLastPos;} set{ _isGoingToLastPos = value;} }

    public float AlertValue
    {
        get {return _alertValue;} 
        set 
        {
            if(_alertValue != value) OnAlertValueChanged?.Invoke(value, MaxAlertValue);
            _alertValue = value;
        }
    }
    public float MaxAlertValue 
    {
        get {return _maxAlertValue;} 
        set 
        {
            if(_maxAlertValue != value) OnAlertValueChanged?.Invoke(AlertValue, value);
            _maxAlertValue = value;
        }
    }
    public float AlertValueCountMultiplier {get {return _alertValueCountMultiplier;}}


    // public MovementStateMachine GetMoveStateMachine { get { return _moveStateMachine; } }
    // public UseWeaponStateMachine GetUseWeaponStateMachine { get {return _useWeaponStateMachine;}}
    public IFOVMachineState GetFOVState { get { return _getFOVState;}}
    public IHuntPlayable GetFOVAdvancedData { get { return _getFOVAdvancedData;}}

    public Transform[] PatrolPath {get {return _patrolPath;} }
    public int CurrPath {get {return _currPath;} }
    public EnemyIdentity EnemyIdentity {get {return _enemyIdentity;}}
    #endregion

    protected override void Awake() 
    {
        base.Awake();
        _getFOVState = _fovMachine as IFOVMachineState;
        _getFOVAdvancedData = _fovMachine as IHuntPlayable;
        _enemyIdentity = _charaIdentity as EnemyIdentity;
        _states = new EnemyAIStateFactory(this);
    }
    protected override void Start() 
    {
        base.Start();
        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnCaptainsStartHunting += EnemyAIManager_OnCaptainsStartHunting;
        _enemyAIManager.OnCaptainsStartEngaging += EnemyAIManager_OnCaptainsStartEngaging;
        _enemyAIManager.OnGoToClosestPOI += EnemyAIManager_OnGoToClosestPOI;
        _enemyAIManager.OnRemovedPlayable += DeleteRemovedPlayerFromList;
        
        SwitchState(_states.AI_IdleState());
    }


    
    private void Update() 
    {
        if(!_gm.IsGamePlaying() || _charaIdentity.IsDead) return;

        _fovMachine.FOVJob();
        CalculateAlertValue();
        CheckPastVisibleTargets();
        DetectEnemy();

        HandleGotDetected();
        
        _currState?.UpdateState();
    }
    public void CalculateAlertValue()
    {
        if(_fovMachine.VisibleTargets.Count > 0)
        {
            float oldmaxAlert = _maxAlertValue;
            MaxAlertValue = _getFOVAdvancedData.GetMinimalPlayableStealth();
            if(oldmaxAlert != _maxAlertValue)
            {
                if(IsAIHunted)
                {
                    if(AlertValue < _maxAlertValue/2) AlertValue = _maxAlertValue/2 + _alertValueCountMultiplier;
                }
                else if(IsAIEngage)
                {
                    if(AlertValue < _maxAlertValue) AlertValue = _maxAlertValue + _alertValueCountMultiplier;
                }
            }
            if(AlertValue <= _maxAlertValue) AlertValue += Time.deltaTime * _alertValueCountMultiplier;
        }
        else
        {

            if(AlertValue >= 0 && _fovMachine.VisibleTargets.Count == 0 && _getFOVAdvancedData.OtherVisibleTargets.Count == 0 && (IsAIIdle || (!IsAIIdle &&GetMoveStateMachine.IsIdle && !_isCheckingEnemyInHunt && !_isTakingCover)))
            {
                AlertValue -= Time.deltaTime * _alertValueCountMultiplier;
            }
        }
    }
    public override void SwitchState(BaseState newState)
    {
        if(_currState != null)
        {
            _currState?.ExitState();
        }
        _currState = newState as EnemyAIState;
        _currState?.EnterState();
    }

    public void RunningTowardsEnemy()
    {
        if(GetFOVMachine.ClosestEnemy != null)
        {
            GetMoveStateMachine.SetAIDirection(GetFOVMachine.ClosestEnemy.position);
            IsGoingToLastPos = false;
            _isCheckingEnemyInHunt = false;
        }
        else 
        {
            RunningToEnemyLastPosition();
        }
    }
    public void RunningToEnemyLastPosition()
    {
        AimAIPointLookAt(null);
        GetFOVAdvancedData.GetClosestBreadCrumbs();
        if(GetFOVAdvancedData.ClosestBreadCrumbs != null)
        {
            GetMoveStateMachine.SetAIDirection(GetFOVAdvancedData.ClosestBreadCrumbs.position);
            IsGoingToLastPos = false;
            _isCheckingEnemyInHunt = false;
        }
        else
        {
            if(GetFOVAdvancedData.HasToCheckEnemyLastSeenPosition)
            {
                // _stateMachine.GetFOVMachine.IHaveCheckEnemyLastPosition();
                // Debug.Log("The last seen pos I'm")
                GetMoveStateMachine.SetAIDirection(GetFOVAdvancedData.EnemyCharalastSeenPosition);
                IsGoingToLastPos = true;
                GetFOVAdvancedData.IsCheckingEnemyLastPosition();
            }
            else
            {
                if(CurrPOI != null)
                {
                    IsGoingToLastPos = false;
                    GetMoveStateMachine.SetAIDirection(CurrPOI.position);
                }
            }
        }
    }

    protected override void MoveStateMachine_OnIsTheSamePosition(Vector3 agentPos)
    {
        if(IsAIIdle && GetFOVState.CurrState == FOVDistState.none && _patrolPath.Length > 0 && agentPos == _patrolPath[_currPath].position)
        {
            // _isIdlePatroling = false;

            if (!_switchingPath)
            {
                _currPath++;
            }
            else
            {
                _currPath--;
            }

            if (_currPath == _patrolPath.Length - 1)
            {
                _switchingPath = true;
            }
            else if (_currPath == 0)
            {
                _switchingPath = false;
            }
            
        }
        if(!IsAIIdle)
        {
            if(agentPos == GetFOVAdvancedData.EnemyCharalastSeenPosition && IsGoingToLastPos) 
            {
                IsGoingToLastPos = false;
                if(IsAIEngage)AlertValue = MaxAlertValue/2 + _alertValueCountMultiplier;
                if(EnemyAIManager.EnemyHearAnnouncementList.Contains(this))
                {
                    // Debug.Log("TEsstt??");
                    _isCheckingEnemyInHunt = true;
                    EnemyAIManager.OnFoundLastCharaSeenPos?.Invoke(this);
                }
            }
            else if(CurrPOI != null && CurrPOI.position == agentPos)
            {
                if(EnemyAIManager.POIPosNearLastSeenPosList.Count > 0)
                {
                    _currPOI = EnemyAIManager.GetClosestPOI(this);
                    _getFOVAdvancedData.IsCheckingEnemyLastPosition();
                    AlertValue = MaxAlertValue/2 + _alertValueCountMultiplier;
                    _isCheckingEnemyInHunt = true;
                }
                else
                {
                    EnemyAIManager.POIPosNearLastSeenPosListSave.Clear();
                    EnemyAIManager.EditEnemyHearAnnouncementList(this, false);
                    _currPOI = null;
                    _isCheckingEnemyInHunt = false;
                }
            }
        }
        if(IsTakingCover && !IsAIIdle)
        {
            if(agentPos == TakeCoverPosition)
            {
                IsAtTakingCoverHidingPlace = true;
            }
            else
            {
                IsAtTakingCoverHidingPlace = false;
            }

            if(isWallTallerThanChara && agentPos == PosToGoWhenCheckingWhenWallIsHigher)
            {
                IsAtTakingCoverCheckingPlace = true;
            }
            else
            {
                IsAtTakingCoverCheckingPlace = false;
            }

        }
        
    }

    private void EnemyAIManager_OnCaptainsStartHunting(EnemyAIBehaviourStateMachine enemy)
    {
        if(_charaIdentity.IsDead)return;
        if(enemy == this)
        {
            EnemyAIManager.EditEnemyCaptainList(this, true);
            EnemyAIManager.EditEnemyHearAnnouncementList(this, true);
            if(CurrPOI != null)CurrPOI = null;
            return;
        }
        //we can actually use the sound D:
        if(Vector3.Distance(enemy.transform.position, transform.position) <= EnemyAIManager.EnemyAnnouncementMaxRange || EnemyAIManager.EnemyHearAnnouncementList.Contains(this))
        {
            if(!IsAIEngage) //hmmmm... gmn ya
            {
                if(GetUseWeaponStateMachine.ChosenTarget != null)GetUseWeaponStateMachine.GiveChosenTarget(null);
                if(GetUseWeaponStateMachine.IsUsingWeapon)GetUseWeaponStateMachine.IsUsingWeapon = false;
            }
            if(MaxAlertValue == 0)MaxAlertValue = enemy.MaxAlertValue;

            if(!IsAIEngage)AlertValue = MaxAlertValue/2 + _alertValueCountMultiplier;
            Vector3 closestLastSeenPos = Vector3.zero;
            if(EnemyAIManager.EnemyCaptainList.Count == 0)closestLastSeenPos = enemy.GetFOVAdvancedData.EnemyCharalastSeenPosition;
            else closestLastSeenPos = EnemyAIManager.GetClosestLastSeenPosInfoFromCaptain(transform);
            GetFOVAdvancedData.GoToEnemyLastSeenPosition(closestLastSeenPos);
            if(CurrPOI != null)CurrPOI = null;

            EnemyAIManager.EditEnemyHearAnnouncementList(this, true);
        }
    }


    private void EnemyAIManager_OnCaptainsStartEngaging(EnemyAIBehaviourStateMachine enemy)
    {
        EnemyAIManager.ResetPOIPosNearLastSeenPos();
        
        if(_charaIdentity.IsDead)return;
        if(enemy == this)
        {
            EnemyAIManager.EditEnemyCaptainList(this, true);
            EnemyAIManager.EditEnemyHearAnnouncementList(this, true);
            if(CurrPOI != null)CurrPOI = null;
            return;
        }

        //we can actually use the sound D:
        if(Vector3.Distance(enemy.transform.position, transform.position) <= EnemyAIManager.EnemyAnnouncementMaxRange || EnemyAIManager.EnemyHearAnnouncementList.Contains(this))
        {
            if(MaxAlertValue == 0)MaxAlertValue = enemy.MaxAlertValue;
            AlertValue = MaxAlertValue + _alertValueCountMultiplier;

            Vector3 closestLastSeenPos = Vector3.zero;
            if(EnemyAIManager.EnemyCaptainList.Count == 0)closestLastSeenPos = enemy.GetFOVAdvancedData.EnemyCharalastSeenPosition;
            else closestLastSeenPos = EnemyAIManager.GetClosestLastSeenPosInfoFromCaptain(transform);
            GetFOVAdvancedData.GoToEnemyLastSeenPosition(closestLastSeenPos);
            if(CurrPOI != null)CurrPOI = null;

            EnemyAIManager.EditEnemyHearAnnouncementList(this, true);
        }
    }

    private void EnemyAIManager_OnGoToClosestPOI(Vector3 enemyLastPos)
    {
        Debug.Log(transform.name + " receive this yess POIIIII");
        if(!EnemyAIManager.EnemyHearAnnouncementList.Contains(this))return;
        if(enemyLastPos != GetFOVAdvancedData.EnemyCharalastSeenPosition)return; // ->the problem is, krn 1 poi list sama yg lain, yg td ga disuru ke sini, bs ke sini...; tp kalo ga gini ntr yg lain ngejer apa, jg poi ikut ini
        if(_currPOI == null)_currPOI = EnemyAIManager.GetClosestPOI(this);
        _getFOVAdvancedData.IsCheckingEnemyLastPosition(); // make this false so it wont go to lastseenpos                      
        AlertValue = MaxAlertValue/2 + 10f;
    }

    public override void UnsubscribeEvent()
    {
        _enemyAIManager.OnCaptainsStartHunting -= EnemyAIManager_OnCaptainsStartHunting;
        _enemyAIManager.OnCaptainsStartEngaging -= EnemyAIManager_OnCaptainsStartEngaging;
        _enemyAIManager.OnGoToClosestPOI -= EnemyAIManager_OnGoToClosestPOI;
        base.UnsubscribeEvent();
    }

    public bool IsThePersonImLookingAlsoSeeMe(Transform enemyISaw)
    {
        
        if(_enemyWhoSawAIList.Count == 0) return false;
        foreach(Transform enemy in _enemyWhoSawAIList)
        {
            if(enemyISaw == enemy)return true;
        }
        return false;
        
    }
    public override void RunAway()
    {
        base.RunAway();
        GetMoveStateMachine.SetAIDirection(RunAwayPos);
        
    }
    protected override void WallListChecker()
    {
        for(int i=0; i < _wallArrayNearChara.Length; i++)
        {
            Vector3 wallToChara = (_wallArrayNearChara[i].transform.position - transform.position).normalized;
            float dotWallChara = Vector3.Dot(wallToChara, transform.forward);

            if(dotWallChara <= 0 && CurrWall != _wallArrayNearChara[i]) //currwallada biar kalo misalnya wall yg sebelumnya dh diambil dan lg otw, titiknya ada d belakang, dia ga merasa titiknya ada d belakang gitu
            {
                // Debug.Log(_wallArrayNearChara[i].transform.name + "  aaa " + " dotwallChara" + dotWallChara + " " + wallToChara + " " + transform.forward + " " + transform.name);
                _wallTotal--;
                _wallArrayNearChara[i] = null;
                continue;
            }
            //yg ini biar d blkg ga ikutan, yg bawah biar lurus doang yg diambil


            if(IsHiding || IsChecking)
            {
                if(_currWall != null)
                {
                    if(_wallArrayNearChara[i] == _currWall)
                    {
                        _wallTotal--;
                        _wallArrayNearChara[i] = null;
                        continue;
                    }
                }
                float distance = Vector3.Distance(_wallArrayNearChara[i].transform.position, GetFOVAdvancedData.EnemyCharalastSeenPosition);
                if(distance <= 5f)
                {
                    _wallTotal--;
                    _wallArrayNearChara[i] = null;
                    continue;
                }
                // Vector3 wallToCharaNow = (_wallArrayNearChara[i].transform.position - transform.position).normalized;
                // float wallvschara = Vector3.Dot(wallToCharaNow, transform.forward);
                // Debug.DrawRay(transform.position, wallToChara * 100f, Color.black, 2f);
                // Debug.DrawRay(transform.position, transform.forward * 100f, Color.green, 2f);
                if(dotWallChara < 0.95f)
                {
                    _wallTotal--;
                    _wallArrayNearChara[i] = null;
                    continue;
                }
                
            }

            // Vector3 CharaToWall = (transform.position - _wallArrayNearChara[i].transform.position).normalized;
            
            
        }
    }
    // public void StartShooting(Transform chosenTarget)
    // {
    //     // Debug.Log("shoot di 2" + _sm.GetFOVMachine.ClosestEnemy.position);
    //     AimAIPointLookAt(chosenTarget);
    //     GetUseWeaponStateMachine.GiveChosenTarget(chosenTarget);
    //     EnemyIdentity.Shooting(true);
    // }
    // public void StopShooting()
    // {
    //     if(GetUseWeaponStateMachine.ChosenTarget != null)GetUseWeaponStateMachine.GiveChosenTarget(null);
    //     EnemyIdentity.Shooting(false);
    // }
    protected override bool IsThisASafePathToGo(Vector3 tempNewPos)
    {
        Vector3 firstPathNewPosToPlayer = (_tempFirstPathPos - transform.position).normalized;
        float dotForwardWithNewPosDir = Vector3.Dot(firstPathNewPosToPlayer, transform.forward);
        // if(dotForwardWithNewPosDir >= 0.95f && _fovMachine.VisibleTargets.Count > 0)return false;
        return true;
    }
    public void SearchForNextWallToHideWhileTakingCover()
    {
        _wallArrayNearChara = Physics.OverlapSphere(transform.position, _wallScannerDistance, _wallTakeCoverLayer);
        _wallTotal = _wallArrayNearChara.Length;
        WallListChecker();
        if(_wallTotal == 0) 
        {
            _canTakeCoverInThePosition = false;
            return;
        }
        System.Array.Sort(_wallArrayNearChara,SortWallBasedOnClosestDistance);
        for(int i=0; i<_wallTotal;i++)
        {
            // Vector3 wallToChara = (_wallArrayNearChara[i].transform.position - transform.position).normalized;
            // float wallvschara = Vector3.Dot(wallToChara, transform.forward);
            // if(wallvschara < 0.85f) continue;

            Collider currColl = _wallArrayNearChara[i];
            float currWallHeight = currColl.bounds.max.y;
            bool isWallTallerThanCharaHere = currColl.bounds.max.y > _charaHeadColl.bounds.max.y + _charaHeightBuffer;
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
            Vector3 directionLastSeenPosToWall = (_fovMachine.EnemyCharalastSeenPosition - currWall.position).normalized;

            float closestDistance = Mathf.Infinity; // ini cari jarak terdekat dr player 
            
            Vector3 newPos = Vector3.zero;
            if(canCheckFrontBehind)
            {
                float forwardWithEnemy = Vector3.Dot(directionLastSeenPosToWall, wallForward);

                if(forwardWithEnemy < _HideDotMin)
                {
                    // Debug.Log("Dot fwd normal " + forwardWithEnemy);
                    // Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallForward * 100f, Color.blue, 2f);

                    GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanCharaHere, wallCenter, wallForward, wallRight, directionLastSeenPosToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                    

                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    forwardWithEnemy = Vector3.Dot(directionLastSeenPosToWall, -wallForward);
                    if(forwardWithEnemy < _HideDotMin)
                    {
                        // Debug.Log("Dot fwd balik " + forwardWithEnemy);
                        // Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallForward * 100f, Color.red, 2f);

                        GetClosestPosition(true, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanCharaHere, wallCenter, -wallForward, wallRight,directionLastSeenPosToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                    } 
                }
                
            }

            // tempNewPos = wallCenter;
            if(canCheckLeftRightSide)
            {
                float rightWithEnemy = Vector3.Dot(directionLastSeenPosToWall, wallRight);
                if(rightWithEnemy < _HideDotMin)
                {
                    // Debug.Log("Dot right normal " + rightWithEnemy);
                    // Debug.DrawRay(_wallArrayNearChara[i].transform.position, wallRight * 100f, Color.grey, 2f);
                    GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanCharaHere, wallCenter, wallForward, wallRight, directionLastSeenPosToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                }
                else // real life wise, ga mungkin di sisi 1 aman, sisi 1 lg aman juga
                {
                    rightWithEnemy = Vector3.Dot(directionLastSeenPosToWall, -wallRight);
                    if(rightWithEnemy < _HideDotMin)
                    {
                        // Debug.Log("Dot right balik " + rightWithEnemy);
                        // Debug.DrawRay(_wallArrayNearChara[i].transform.position, -wallRight * 100f, Color.magenta, 2f);
                        GetClosestPosition(false, ref closestDistance, ref newPos, halfWallLength, halfWallWidth, isWallTallerThanCharaHere, wallCenter, wallForward, -wallRight, directionLastSeenPosToWall, currColl.bounds.max.y, currColl.bounds.min.y, currWall.localScale.y);
                    }
                    
                }
            }
            if(closestDistance != Mathf.Infinity)
            {
                _canTakeCoverInThePosition = true;
                _currWall = _wallArrayNearChara[i];
                _currWallHeight = currWallHeight;
                _isWallTallerThanChara = isWallTallerThanCharaHere;
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
            }
        }
    }

    public void RespondToSound(Vector3 soundOriginPos)
    {
        Debug.Log(transform.name + "heard sound from " + soundOriginPos);
        if(_fovMachine.VisibleTargets.Count > 0 || _getFOVAdvancedData.OtherVisibleTargets.Count > 0)return;
        if(!IsAIEngage)
        {
            if(GetUseWeaponStateMachine.ChosenTarget != null)GetUseWeaponStateMachine.GiveChosenTarget(null);
            if(GetUseWeaponStateMachine.IsUsingWeapon)GetUseWeaponStateMachine.IsUsingWeapon = false;
        }

        if(MaxAlertValue == 0)MaxAlertValue = 25f;
        if(!IsAIEngage)AlertValue = MaxAlertValue/2 + _alertValueCountMultiplier;

        GetFOVAdvancedData.GoToEnemyLastSeenPosition(soundOriginPos);
        if(CurrPOI != null)CurrPOI = null;

        // EnemyAIManager.EditEnemyHearAnnouncementList(this, true); perlua keluarin dr list ga ya hm
    }

    public void DeleteRemovedPlayerFromList(Transform transform)
    {
        if(_fovMachine.VisibleTargets.Contains(transform)) _fovMachine.VisibleTargets.Remove(transform);
        DeleteKilledEnemyFromList(transform);
    }
}

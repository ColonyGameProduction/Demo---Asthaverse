using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAIBehaviourStateMachine : AIBehaviourStateMachine, IUnsubscribeEvent
{
    [Header("Manager")]
    private EnemyAIManager _enemyAIManager;
    [Header("State Machine")]
    [SerializeField] private MovementStateMachine _moveStateMachine;
    [SerializeField] private UseWeaponStateMachine _useWeaponStateMachine;
    private EnemyIdentity _enemyIdentity;
    

    [Header("Enemy Alert Value")]
    [SerializeField] private float _alertValue;
    [SerializeField] private float _maxAlertValue;
    [SerializeField] private float _alertValueCountMultiplier = 10f;

    [Header("Enemy AI States")]
    [SerializeField] private bool _isAIIdle, _isAIHunted, _isAIEngage, _isIdlePatroling;
    private IFOVMachineState _getFOVState;
    private IHuntPlayable _getFOVAdvancedData;
    private EnemyAIState _currState;
    private EnemyAIStateFactory _states;
    private Transform _currPOI;
    
    [Header("Patrol Path")]
    [SerializeField] private Transform[] _patrolPath;
    private bool _switchingPath;
    private int _currPath;

    #region GETTERSETTER Variable
    public Transform CurrPOI {get { return _currPOI;} set { _currPOI = value;}}
    public EnemyAIManager EnemyAIManager { get { return _enemyAIManager;}}
    public bool IsAIIdle {get {return _isAIIdle;} set{ _isAIIdle = value;} }
    public bool IsAIHunted {get {return _isAIHunted;} set{ _isAIHunted = value;} }
    public bool IsAIEngage {get {return _isAIEngage;} set{ _isAIEngage = value;} }
    // public bool IsIdlePatroling {get {return _isIdlePatroling;} set{ _isIdlePatroling = value;} }

    public float AlertValue {get {return _alertValue;} set { _alertValue = value;}}
    public float MaxAlertValue {get {return _maxAlertValue;} set { _maxAlertValue = value;}}


    public MovementStateMachine GetMoveStateMachine { get { return _moveStateMachine; } }
    public UseWeaponStateMachine GetUseWeaponStateMachine { get {return _useWeaponStateMachine;}}
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
    private void Start() 
    {
        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnCaptainsStartHunting += EnemyAIManager_OnCaptainsStartHunting;
        _enemyAIManager.OnCaptainsStartEngaging += EnemyAIManager_OnCaptainsStartEngaging;
        _enemyAIManager.OnGoToClosestPOI += EnemyAIManager_OnGoToClosestPOI;

        if(_moveStateMachine == null) _moveStateMachine = _charaIdentity.MovementStateMachine;
        if(_useWeaponStateMachine == null) _useWeaponStateMachine = _charaIdentity.UseWeaponStateMachine;
        _moveStateMachine.OnIsTheSamePosition += MoveStateMachine_OnIsTheSamePosition;
        SwitchState(_states.AI_IdleState());
    }


    
    private void Update() 
    {
        _fovMachine.FOVJob();
        CalculateAlertValue();
        _currState?.UpdateState();
    }
    public void CalculateAlertValue()
    {
        // if(_charaIdentity.IsDead || _enemyIdentity.IsSilentKilled) return;
        if(_fovMachine.VisibleTargets.Count > 0)
        {
            _maxAlertValue = _getFOVAdvancedData.GetMinimalPlayableStealth();
            if(_alertValue <= _maxAlertValue) _alertValue += Time.deltaTime * _alertValueCountMultiplier;
        }
        else
        {

            if(_alertValue >= 0 && _fovMachine.VisibleTargets.Count == 0 && _getFOVAdvancedData.OtherVisibleTargets.Count == 0 && (IsAIIdle || (!IsAIIdle && GetMoveStateMachine.IsIdle)))
            {
                _alertValue -= Time.deltaTime * _alertValueCountMultiplier;
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
            AimAIPointLookAt(GetFOVMachine.ClosestEnemy);
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
        }
        else
        {
            if(GetFOVAdvancedData.HasToCheckEnemyLastSeenPosition)
            {
                // _stateMachine.GetFOVMachine.IHaveCheckEnemyLastPosition();
                GetMoveStateMachine.SetAIDirection(GetFOVAdvancedData.EnemyCharalastSeenPosition);

                GetFOVAdvancedData.IsCheckingEnemyLastPosition();
            }
            else
            {
                if(CurrPOI != null)
                {
                    GetMoveStateMachine.SetAIDirection(CurrPOI.position);
                }
            }
        }
    }

    private void MoveStateMachine_OnIsTheSamePosition(Vector3 agentPos)
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
            if(IsAIHunted && agentPos == GetFOVAdvancedData.EnemyCharalastSeenPosition) 
            {
                if(EnemyAIManager.EnemyHearAnnouncementList.Contains(this))
                {
                    // Debug.Log("TEsstt??");
                    EnemyAIManager.OnFoundLastCharaSeenPos?.Invoke(this);
                }
            }
            else if(CurrPOI != null && CurrPOI.position == agentPos)
            {
                if(EnemyAIManager.POIPosNearLastSeenPosList.Count > 0)
                {
                    _currPOI = EnemyAIManager.GetClosestPOI(this);
                    _getFOVAdvancedData.IsCheckingEnemyLastPosition();
                    _alertValue = MaxAlertValue/2 + 10f;
                }
                else
                {
                    EnemyAIManager.POIPosNearLastSeenPosListSave.Clear();
                    EnemyAIManager.EditEnemyHearAnnouncementList(this, false);
                    _currPOI = null;
                }
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
            return;
        }
        //we can actually use the sound D:
        if(Vector3.Distance(enemy.transform.position, transform.position) <= EnemyAIManager.EnemyAnnouncementMaxRange || EnemyAIManager.EnemyHearAnnouncementList.Contains(this))
        {
            _alertValue = MaxAlertValue/2 + 10f;
            Vector3 closestLastSeenPos = Vector3.zero;
            if(EnemyAIManager.EnemyCaptainList.Count == 0)closestLastSeenPos = enemy.GetFOVAdvancedData.EnemyCharalastSeenPosition;
            else closestLastSeenPos = EnemyAIManager.GetClosestLastSeenPosInfoFromCaptain(transform);
            GetFOVAdvancedData.GoToEnemyLastSeenPosition(closestLastSeenPos);

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
            return;
        }

        //we can actually use the sound D:
        if(Vector3.Distance(enemy.transform.position, transform.position) <= EnemyAIManager.EnemyAnnouncementMaxRange || EnemyAIManager.EnemyHearAnnouncementList.Contains(this))
        {
            _alertValue = MaxAlertValue + 10f;

            Vector3 closestLastSeenPos = Vector3.zero;
            if(EnemyAIManager.EnemyCaptainList.Count == 0)closestLastSeenPos = enemy.GetFOVAdvancedData.EnemyCharalastSeenPosition;
            else closestLastSeenPos = EnemyAIManager.GetClosestLastSeenPosInfoFromCaptain(transform);
            GetFOVAdvancedData.GoToEnemyLastSeenPosition(closestLastSeenPos);

            EnemyAIManager.EditEnemyHearAnnouncementList(this, true);
        }
    }

    private void EnemyAIManager_OnGoToClosestPOI()
    {
        // Debug.Log("Test");
        if(_currPOI == null)_currPOI = EnemyAIManager.GetClosestPOI(this);
        _getFOVAdvancedData.IsCheckingEnemyLastPosition(); // make this false so it wont go to lastseenpos                      
        _alertValue = MaxAlertValue/2 + 10f;
    }

    public void UnsubscribeEvent()
    {
        _enemyAIManager.OnCaptainsStartHunting -= EnemyAIManager_OnCaptainsStartHunting;
        _enemyAIManager.OnCaptainsStartEngaging -= EnemyAIManager_OnCaptainsStartEngaging;
        _enemyAIManager.OnGoToClosestPOI -= EnemyAIManager_OnGoToClosestPOI;
        _moveStateMachine.OnIsTheSamePosition -= MoveStateMachine_OnIsTheSamePosition;
    }
    // public void ForceStopMachine()
    // {
    //     _alertValue = 0;
    // }
}

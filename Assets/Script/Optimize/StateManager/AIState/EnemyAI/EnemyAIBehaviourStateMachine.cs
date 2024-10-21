using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIBehaviourStateMachine : AIBehaviourStateMachine
{
    [Header("State Machine")]
    [SerializeField] private MovementStateMachine _moveStateMachine;
    [SerializeField] private UseWeaponStateMachine _useWeaponStateMachine;
    

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
    
    [Header("Patrol Path")]
    [SerializeField] private Transform[] _patrolPath;
    private bool _switchingPath;
    private int _currPath;
    private Vector3 _enemyCharaLastSeenPosition;

    #region GETTERSETTER Variable
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
    #endregion

    protected override void Awake() 
    {
        base.Awake();
        _getFOVState = _fovMachine as IFOVMachineState;
        _getFOVAdvancedData = _fovMachine as IHuntPlayable;

        _states = new EnemyAIStateFactory(this);
    }
    private void Start() 
    {
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
        if(_fovMachine.VisibleTargets.Count > 0)
        {
            _maxAlertValue = _getFOVAdvancedData.GetMinimalPlayableStealth();
            if(_alertValue <= _maxAlertValue) _alertValue += Time.deltaTime * _alertValueCountMultiplier;
        }
        else
        {
            //kalau 2-2nya null meaning 
            // if(_alertValue >= 0 && _fovMachine.VisibleTargets.Count == 0 && _getFOVAdvancedData.OtherVisibleTargets.Count == 0 && (IsAIIdle || (!IsAIIdle && !_fovMachine.HasToCheckEnemyLastSeenPosition)))
            // {
            //     _alertValue -= Time.deltaTime * _alertValueCountMultiplier;
            // }
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
            GetMoveStateMachine.GiveAIDirection(GetFOVMachine.ClosestEnemy.position);
        }
        else 
        {
            RunningToEnemyLastPosition();
        }
    }
    public void RunningToEnemyLastPosition()
    {
        GetFOVAdvancedData.GetClosestBreadCrumbs();
        if(GetFOVAdvancedData.ClosestBreadCrumbs != null)
        {
            GetMoveStateMachine.GiveAIDirection(GetFOVAdvancedData.ClosestBreadCrumbs.position);
        }
        else
        {
            if(GetFOVAdvancedData.HasToCheckEnemyLastSeenPosition)
            {
                // _stateMachine.GetFOVMachine.IHaveCheckEnemyLastPosition();
                GetMoveStateMachine.GiveAIDirection(GetFOVAdvancedData.EnemyCharalastSeenPosition);

                GetFOVAdvancedData.IsCheckingEnemyLastPosition();
            }
            
        }
    }

    private void MoveStateMachine_OnIsTheSamePosition(Vector3 agentPos)
    {
        if(IsAIIdle && GetFOVState.CurrState == FOVDistState.none && agentPos == _patrolPath[_currPath].position)
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
    }

}

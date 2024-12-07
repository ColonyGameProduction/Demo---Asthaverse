using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FriendAIBehaviourStateMachine : AIBehaviourStateMachine, IFriendBehaviourStateData, IUnsubscribeEvent
{
    #region  Normal Variable
    [Space(2)]
    [Header("Other Component Variable")]
    [SerializeField] private EnemyAIManager _enemyAIManager;
    
    protected IReceiveInputFromPlayer _getCanInputPlayer;
    private bool isToldHold;
    [SerializeField]private Transform _friendsDefaultDirection;
    [SerializeField]private PlayableCharacterIdentity _currPlayableIdentity;
    private Transform _friendsCommandDirection;
    private Transform _currPlayable;
    [SerializeField] protected PlayableCharacterIdentity _playableCharaIdentity;
    [SerializeField] protected PlayableMovementStateMachine _playableMoveStateMachine;
    [SerializeField] protected PlayableUseWeaponStateMachine _playableUseWeaponStateMachine;

    [SerializeField] private float _mainPlayableMaxDistance = 3.5f;
    [SerializeField] private float _friendsDefaultMaxDistanceFromPlayer = 2.5f;

    [Header("Friend AI States")]
    [SerializeField] protected bool _isAIIdle;
    [SerializeField] protected bool _isAIEngage;
    [SerializeField] protected float _isEngageTimer;
    [SerializeField] protected float _isEngageTimerMax = 0.3f;

    
    protected FriendAIState _currState;
    protected FriendAIStateFactory _states;
    



    #endregion

    #region GETTER SETTER
    public bool IsToldHold {get {return isToldHold;} set {isToldHold = value;} }

    public bool IsAIIdle {get {return _isAIIdle;} set{ _isAIIdle = value;} }
    public bool IsAIEngage {get {return _isAIEngage;} set {_isAIEngage = value;}}

    public bool IsAIInput {get {return _isAIInput;}}
    public Transform FriendsDefaultDirection {get {return _friendsDefaultDirection;}}   
    public Transform FriendsCommandDirection {get {return _friendsCommandDirection;}}    
    public PlayableCharacterIdentity CurrPlayableIdentity {get {return _currPlayableIdentity;}}




    public PlayableCharacterIdentity GetPlayableCharaIdentity { get { return _playableCharaIdentity; } }    
    public PlayableMovementStateMachine GetMoveStateMachine { get { return _playableMoveStateMachine; } }
    public PlayableUseWeaponStateMachine GetUseWeaponStateMachine { get {return _playableUseWeaponStateMachine;}}
    #endregion
    protected override void Awake()
    {
        base.Awake();
        _playableCharaIdentity = _charaIdentity as PlayableCharacterIdentity;

        _getCanInputPlayer = GetComponent<IReceiveInputFromPlayer>();
        _isAIInput = !_getCanInputPlayer.IsPlayerInput;
        _getCanInputPlayer.OnIsPlayerInputChange += CharaIdentity_OnIsPlayerInputChange;

        _states = new FriendAIStateFactory(this);
    }
    protected override void Start()
    {
        base.Start();
        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnEnemyisEngaging += OnEnemyStartedEngaging;
        _enemyAIManager.OnEnemyStopEngaging += OnEnemyStopEngaging;

        
        _playableMoveStateMachine = _playableCharaIdentity.GetPlayableMovementData;
        _playableUseWeaponStateMachine = _playableCharaIdentity.GetPlayableUseWeaponData;
        _bodyPartMask = _playableUseWeaponStateMachine.CharaEnemyMask;
        _playableMoveStateMachine.OnIsTheSamePosition += MoveStateMachine_OnIsTheSamePosition;

        SwitchState(_states.AI_IdleState());
    }


    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _wallScannerDistance);
    }
    void Update()
    {
        _fovMachine.FOVJob();
        CheckPastVisibleTargets();
        DetectEnemy();
        if(IsAIInput)
        {
            GotDetectedTimerCounter();
            if(_gotDetectedByEnemy)
            {   
                _leaveDirection = GetTotalDirectionTargetPosAndEnemy(transform, false);
            }
            _currState?.UpdateState();
        }

    }
    public override void SwitchState(BaseState newState)
    {
        if(_currState != null)
        {
            _currState?.ExitState();
        }
        _currState = newState as FriendAIState;
        _currState?.EnterState();
    }
    private void CharaIdentity_OnIsPlayerInputChange(bool obj)
    {
        _isAIInput = !obj;
    }
    public void GiveUpdateFriendDirection(PlayableCharacterIdentity currPlayable, Transform commandPos)
    {   
        _currPlayableIdentity = currPlayable;
        
        _currPlayable = currPlayable.transform;
        Debug.Log("Apa error di sini ?>" + _currPlayableIdentity);
        _friendsDefaultDirection = _currPlayable.transform;

        _friendsCommandDirection = commandPos;
    }

    public bool IsFriendTooFarFromPlayerWhenIdle()
    {
        // Debug.Log(transform.position + " " + _currPlayable.position + " " + Vector3.Distance(transform.position, _currPlayable.position));
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _mainPlayableMaxDistance) return false;
        return true;
    }
    public bool IsFriendAlreadyAtDefaultDistance()
    {
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _friendsDefaultMaxDistanceFromPlayer) return true;
        return false;
    }

    private void MoveStateMachine_OnIsTheSamePosition(Vector3 agentPos)
    {
        if(IsAIIdle && GotDetectedbyEnemy && LeaveDirection != Vector3.zero && agentPos == _runAwayPos && IsAIInput)
        {
            GetMoveStateMachine.IsRunning = false;
        }

        if(IsTakingCover && !IsAIIdle && IsAIInput)
        {
            // Debug.Log(agentPos + " AAAAAAAAAA POSSS "+transform.position + " " + TakeCoverPosition + " " + PosToGoWhenCheckingWhenWallIsHigher);
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


    private void OnEnemyStartedEngaging()
    {
        if(IsAIInput)IsAIEngage = true;
    }
    private void OnEnemyStopEngaging()
    {
        if(IsAIInput)IsAIEngage = false;
    }

    public void UnsubscribeEvent()
    {
        _getCanInputPlayer.OnIsPlayerInputChange -= CharaIdentity_OnIsPlayerInputChange;
        _enemyAIManager.OnEnemyisEngaging -= OnEnemyStartedEngaging;
        _enemyAIManager.OnEnemyStopEngaging -= OnEnemyStopEngaging;
        _playableMoveStateMachine.OnIsTheSamePosition -= MoveStateMachine_OnIsTheSamePosition;
    }

    
    public override void RunAway()
    {
        _charaIdentity.Run(true);
        base.RunAway();
        GetMoveStateMachine.SetAIDirection(RunAwayPos);
    }
    public bool IsCurrTakeCoverAlreadyOccupied()
    {
        return _takeCoverManager.IsTakeCoverPosOccupied(TakeCoverPosition, this);
    }

    public void StartShooting(Transform chosenTarget)
    {
        AimAIPointLookAt(chosenTarget);
        GetUseWeaponStateMachine.GiveChosenTarget(chosenTarget);
        _charaIdentity.Shooting(true);
    }
    public void StopShooting()
    {
        GetUseWeaponStateMachine.GiveChosenTarget(null);
        _charaIdentity.Shooting(false);
    }
    public void DeleteKilledEnemyFromList(Transform enemy)
    {
        if(_enemyWhoSawAIList.Contains(enemy))_enemyWhoSawAIList.Remove(enemy);
        if(_enemyWhoSawAIListContainer.Contains(enemy))_enemyWhoSawAIListContainer.Remove(enemy);
        if(_enemyWhoSawAIListContainer.Count == 0)NotDetectedAnymore();
    }
    
}

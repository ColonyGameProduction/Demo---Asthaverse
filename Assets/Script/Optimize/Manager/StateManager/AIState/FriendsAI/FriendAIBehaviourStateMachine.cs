
using UnityEngine;

public class FriendAIBehaviourStateMachine : AIBehaviourStateMachine, IFriendBehaviourStateData
{
    #region  Normal Variable
    [Space(2)]
    [Header("Other Component Variable")]

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
    [SerializeField] private float _friendDefaultMaxDistanceFromPlayerWhenEngaged = 15f;

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
    public PlayableMovementStateMachine GetPlayableMoveStateMachine { get { return _playableMoveStateMachine; } }
    public PlayableUseWeaponStateMachine GetPlayableUseWeaponStateMachine { get {return _playableUseWeaponStateMachine;}}
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
        
        
        _enemyAIManager.OnEnemyisEngaging += OnEnemyStartedEngaging;
        _enemyAIManager.OnEnemyStopEngaging += OnEnemyStopEngaging;

        _playableMoveStateMachine = _playableCharaIdentity.GetPlayableMovementStateMachine;
        _playableUseWeaponStateMachine = _playableCharaIdentity.GetPlayableUseWeaponStateMachine;
        

        SwitchState(_states.AI_IdleState());
    }


    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _wallScannerDistance);
    }

    void Update()
    {
        if(!_gm.IsGamePlaying()) return;
        
        _fovMachine.FOVJob();
        CheckPastVisibleTargets();
        DetectEnemy();
        HandleGotDetected();
        if(IsAIInput)
        {
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

    #region Friend Function
    public void GiveUpdateFriendDirection(PlayableCharacterIdentity currPlayable, Transform commandPos)
    {   
        _currPlayableIdentity = currPlayable;
        
        _currPlayable = currPlayable.transform;
        
        _friendsDefaultDirection = _currPlayable.transform;

        _friendsCommandDirection = commandPos;
    }

    public bool IsFriendTooFarFromPlayerWhenIdle()
    {
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _mainPlayableMaxDistance) return false;
        return true;
    }
    public bool IsFriendAlreadyAtDefaultDistance()
    {
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _friendsDefaultMaxDistanceFromPlayer) return true;
        return false;
    }
    public bool IsFriendAlreadyAtDefaultDistanceWhenEngaged()
    {
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _friendDefaultMaxDistanceFromPlayerWhenEngaged) return true;
        return false;
    }
    #endregion

    protected override void MoveStateMachine_OnIsTheSamePosition(Vector3 agentPos)
    {
        if(IsAIIdle && GotDetectedbyEnemy && LeaveDirection != Vector3.zero && agentPos == _runAwayPos && IsAIInput)
        {
            _charaIdentity.Run(false);
            // _leaveDirection = Vector3.zero;
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
        if(IsAIInput) IsAIEngage = true;
    }
    private void OnEnemyStopEngaging()
    {
        if(IsAIInput) IsAIEngage = false;
    }

    public override void UnsubscribeEvent()
    {
        _getCanInputPlayer.OnIsPlayerInputChange -= CharaIdentity_OnIsPlayerInputChange;
        _enemyAIManager.OnEnemyisEngaging -= OnEnemyStartedEngaging;
        _enemyAIManager.OnEnemyStopEngaging -= OnEnemyStopEngaging;
        base.UnsubscribeEvent();
    }

    
    public override void RunAway()
    {
        _charaIdentity.Run(true);
        base.RunAway();
        _moveStateMachine.SetAIDirection(RunAwayPos);
    }
    public bool IsCurrTakeCoverAlreadyOccupied()
    {
        return _takeCoverManager.IsTakeCoverPosOccupied(TakeCoverPosition, this);
    }

    // public void StartShooting(Transform chosenTarget)
    // {
    //     AimAIPointLookAt(chosenTarget);
    //     _useWeaponStateMachine.GiveChosenTarget(chosenTarget);
    //     _charaIdentity.Shooting(true);
    // }
    // public void StopShooting()
    // {
    //     _useWeaponStateMachine.GiveChosenTarget(null);
    //     _charaIdentity.Shooting(false);
    // }
    

    public void ChangeFriendDefaultDirectionWhenSplit(Transform newPos)
    {
        _friendsDefaultDirection = newPos;
    }
    
}

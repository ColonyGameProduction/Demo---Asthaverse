using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIBehaviourStateMachine : AIBehaviourStateMachine, IFriendBehaviourStateData
{
    #region  Normal Variable
    [Space(2)]
    [Header("Other Component Variable")]
    protected IReceiveInputFromPlayer _getCanInputPlayer;
    private bool isToldHold;
    private Transform _friendsDefaultDirection;
    private Transform _friendsCommandDirection;
    private Transform _currPlayable;
    [SerializeField] protected PlayableCharacterIdentity _playableCharaIdentity;
    [SerializeField] protected PlayableMovementStateMachine _playableMoveStateMachine;
    [SerializeField] protected PlayableUseWeaponStateMachine _playableUseWeaponStateMachine;

    [SerializeField] private float _mainPlayableMaxDistance;

    [Header("Friend AI States")]
    [SerializeField] protected bool _isAIIdle;
    protected FriendAIState _currState;
    protected FriendAIStateFactory _states;
    protected bool _isAIInput;

    #endregion

    #region GETTER SETTER
    public bool IsToldHold {get {return isToldHold;} set {isToldHold = value;} }

    public bool IsAIIdle {get {return _isAIIdle;} set{ _isAIIdle = value;} }

    public bool IsAIInput {get {return _isAIInput;}}

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
    void Start()
    {
        _playableMoveStateMachine = _playableCharaIdentity.GetPlayableMovementData;
        _playableUseWeaponStateMachine = _playableCharaIdentity.GetPlayableUseWeaponData;

        // SwitchState(_states.AI_IdleState());
    }

    
    void Update()
    {
        if(PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || !IsAIInput || _playableCharaIdentity.IsAnimatingOtherAnimation) 
        {
            if(_playableMoveStateMachine.CurrAIDirPos != transform.position)_playableMoveStateMachine.ForceStopMoving();
            return;
            // if(_charaIdentity.MovementStateMachine.CurrAIDirection != null)_charaIdentity.MovementStateMachine.ForceStopMoving();
            // return;
        }

        if(!PlayableCharacterManager.IsCommandingFriend)
        {
            if(!IsToldHold)
            {
                if(_playableMoveStateMachine.IsIdle && !IsFriendTooFarFromPlayer()) _playableMoveStateMachine.SetAIDirection(transform.position);
                else _playableMoveStateMachine.SetAIDirection(_friendsDefaultDirection.position);
            }
            else
            {
                if(_charaIdentity.IsDead)
                {
                    isToldHold = false;
                }
                else _playableMoveStateMachine.SetAIDirection(_friendsCommandDirection.position);
            }
        }
        else
        {
            if(_charaIdentity.IsDead)
            {
                _playableMoveStateMachine.SetAIDirection(_friendsDefaultDirection.position);
            }
            else
            {
                _playableMoveStateMachine.SetAIDirection(_friendsCommandDirection.position);
            }
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
    public void GiveUpdateFriendDirection(Transform currPlayable, Transform defaultPos, Transform commandPos)
    {   
        _currPlayable = currPlayable;
        _friendsDefaultDirection = defaultPos;
        _friendsCommandDirection = commandPos;
    }

    private bool IsFriendTooFarFromPlayer()
    {
        // Debug.Log(transform.position + " " + _currPlayable.position + " " + Vector3.Distance(transform.position, _currPlayable.position));
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _mainPlayableMaxDistance) return false;
        return true;
    }
}

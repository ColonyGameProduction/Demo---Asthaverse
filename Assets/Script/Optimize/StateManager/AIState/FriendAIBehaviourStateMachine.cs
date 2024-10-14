using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIBehaviourStateMachine : AIBehaviourStateMachine, IFriendBehaviourStateData
{
    #region  Normal Variable
    [Space(2)]
    [Header("Other Component Variable")]
    protected ICanInputPlayer _getCanInputPlayer;
    private bool isToldHold;
    private Transform _friendsDefaultDirection;
    private Transform _friendsCommandDirection;
    private Transform _currPlayable;

    [SerializeField] private float _mainPlayableMaxDistance;

    #endregion

    #region GETTER SETTER
    public bool IsToldHold {get {return isToldHold;} set {isToldHold = value;} }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        _getCanInputPlayer = GetComponent<ICanInputPlayer>();
        _isInputPlayer = _getCanInputPlayer.IsInputPlayer;
        _getCanInputPlayer.OnInputPlayerChange += CharaIdentity_OnInputPlayerChange;
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        if(PlayableCharacterManager.IsSwitchingCharacter ||_isInputPlayer) 
        {
            if(_charaIdentity.MovementStateMachine.CurrAIDirPos != transform.position)_charaIdentity.MovementStateMachine.ForceStopMoving();
            return;
            // if(_charaIdentity.MovementStateMachine.CurrAIDirection != null)_charaIdentity.MovementStateMachine.ForceStopMoving();
            // return;
        }
         

        if(!PlayableCharacterManager.IsCommandingFriend)
        {
            if(!IsToldHold)
            {
                if(_charaIdentity.MovementStateMachine.IsIdle && !IsFriendTooFarFromPlayer()) _charaIdentity.MovementStateMachine.GiveAIDirection(transform.position);
                else _charaIdentity.MovementStateMachine.GiveAIDirection(_friendsDefaultDirection.position);
            }
            else
            {
                _charaIdentity.MovementStateMachine.GiveAIDirection(_friendsCommandDirection.position);
            }
        }
        else
        {
            _charaIdentity.MovementStateMachine.GiveAIDirection(_friendsCommandDirection.position);
        }
    }
    public override void SwitchState(BaseState newState)
    {
        throw new System.NotImplementedException();
    }
    private void CharaIdentity_OnInputPlayerChange(bool obj)
    {
        _isInputPlayer = obj;
    }
    public void GiveUpdateFriendDirection(Transform currPlayable, Transform defaultPos, Transform commandPos)
    {   
        _currPlayable = currPlayable;
        _friendsDefaultDirection = defaultPos;
        _friendsCommandDirection = commandPos;
    }

    private bool IsFriendTooFarFromPlayer()
    {
        Debug.Log(transform.position + " " + _currPlayable.position + " " + Vector3.Distance(transform.position, _currPlayable.position));
        if(Vector3.Distance(transform.position, _currPlayable.position) <= _mainPlayableMaxDistance) return false;
        return true;
    }
}

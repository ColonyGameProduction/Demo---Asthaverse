using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIBehaviourStateMachine : AIBehaviourStateMachine, IFriendBehaviourStateData
{
    #region  Normal Variable
    [Space(2)]
    [Header("Other Component Variable")]
    protected ICanInputPlayer _getCanInputPlayer;
    private Transform _friendsDefaultDirection;
    private Transform _friendsCommandDirection;

    #endregion

    #region GETTER SETTER
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
            if(_charaIdentity.MovementStateMachine.CurrAIDirection != null)_charaIdentity.MovementStateMachine.ForceStopMoving();
            return;
        }
         

        if(!PlayableCharacterManager.IsCommandingFriend)
        {
            if(!PlayableCharacterManager.IsHoldInPlaceFriend)
            {
                _charaIdentity.MovementStateMachine.GiveAIDirection(_friendsDefaultDirection);
            }
            else
            {
                _charaIdentity.MovementStateMachine.GiveAIDirection(_friendsCommandDirection);
            }
        }
        else
        {
            _charaIdentity.MovementStateMachine.GiveAIDirection(_friendsCommandDirection);
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
    public void GiveUpdateFriendDirection(Transform defaultPos, Transform commandPos)
    {   
        _friendsDefaultDirection = defaultPos;
        _friendsCommandDirection = commandPos;
    }
}

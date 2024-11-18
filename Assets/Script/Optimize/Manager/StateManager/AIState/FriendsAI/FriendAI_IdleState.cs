using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendAI_IdleState : FriendAIState
{

    public FriendAI_IdleState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        _sm.IsAIIdle = true;
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)_sm.GetFOVMachine.IsCheckingEnemyLastPosition();
    }

    public override void UpdateState()
    {
        if(PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || !_sm.IsAIInput || _sm.GetPlayableCharaIdentity.IsAnimatingOtherAnimation || _sm.GetPlayableCharaIdentity.IsReviving || _sm.GetPlayableCharaIdentity.IsSilentKilling) 
        {
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
            return;
            // if(_charaIdentity.MovementStateMachine.CurrAIDirection != null)_charaIdentity.MovementStateMachine.ForceStopMoving();
            // return;
        }
        if(_sm.IsAIEngage && !_sm.IsCharacterDead)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            if(_sm.IsToldHold)_sm.IsToldHold = false;
            _sm.FriendsCommandDirection.position = _sm.FriendsDefaultDirection.position;
        }
        
        if(_sm.GotDetectedbyEnemy && _sm.LeaveDirection != Vector3.zero && !_sm.IsCharacterDead)
        {
            if(_sm.IsToldHold)_sm.IsToldHold = false;
            _sm.GetMoveStateMachine.IsRunning = true;
            _sm.RunAwayDirCalculation();
            _sm.GetMoveStateMachine.SetAIDirection(_sm.RunAwayPos);
            _sm.FriendsCommandDirection.position = _sm.FriendsDefaultDirection.position;
            return;
        }
        if(!PlayableCharacterManager.IsCommandingFriend)
        {
            if(!_sm.IsToldHold)
            {
                if(_sm.GetMoveStateMachine.IsIdle && !_sm.IsFriendTooFarFromPlayer()) _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                else _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                                        
            }
            else
            {
                if(_sm.IsCharacterDead)
                {
                    _sm.IsToldHold = false;
                }
                else
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsCommandDirection.position);
                }
            }
        }
        else if(PlayableCharacterManager.IsCommandingFriend && _sm.GetPlayableCharaIdentity.FriendID == PlayableCharacterCommandManager.SelectedFriendID)
        {
            if(_sm.IsCharacterDead)
            {
                _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
            }
            else
            {
                _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsCommandDirection.position);
            }
        }
        // if(_sm.GetFOVMachine.VisibleTargets.Count > 0)
        // {   
        //     _leaveDirection = GetTotalDirectionTargetPosAndEnemy(transform, false);
        //     TakingCover();
        //     isToldHold = false;
        // }
        

    }
    public override void ExitState()
    {
        _sm.IsAIIdle = false;
    }
    

}

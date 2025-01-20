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
        _sm.AimAIPointLookAt(null);
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)_sm.GetFOVMachine.IsCheckingEnemyLastPosition();
    }

    public override void UpdateState()
    {
        if(PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || _sm.GetPlayableCharaIdentity.IsAnimatingOtherAnimation || _sm.GetPlayableCharaIdentity.IsReviving || _sm.GetPlayableCharaIdentity.IsSilentKilling || !_sm.IsAIInput) 
        {
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
            return;
        }
        

        if(_sm.IsAIEngage && !_sm.IsCharacterDead && !_sm.CharaIdentity.IgnoreThisCharacter)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            if(_sm.IsToldHold)_sm.IsToldHold = false;
            _sm.FriendsCommandDirection.position = _sm.FriendsDefaultDirection.position;

            return;
        }
        if(_sm.CharaIdentity.IgnoreThisCharacter)
        {
            if(_sm.GetPlayableCharaIdentity.IsStillPlayable)
            {
                if(_sm.IsToldHold)
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                }
                else if(!_sm.IsFriendAlreadyAtDefaultDistance()) _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                else
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                }
            }
            else
            {
                _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
            }
            
            return;
        }
        
        if(_sm.GotDetectedbyEnemy && _sm.LeaveDirection != Vector3.zero && !_sm.IsCharacterDead)
        {
            if(_sm.IsToldHold)_sm.IsToldHold = false;
            _sm.RunAway();
            
            _sm.FriendsCommandDirection.position = _sm.FriendsDefaultDirection.position;
            return;
        }
        if(!PlayableCharacterManager.IsCommandingFriend)
        {
            if(!_sm.IsToldHold)
            {
                if(_sm.GetMoveStateMachine.IsIdle && !_sm.IsFriendTooFarFromPlayerWhenIdle()) _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                else
                {
                    if(!_sm.IsFriendAlreadyAtDefaultDistance())
                    {
                        if(!_sm.CurrPlayableIdentity.GetPlayableMovementStateMachine.IsTakingCoverAtWall)_sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                        else
                        {
                            _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position + _sm.CurrPlayableIdentity.GetPlayableMovementStateMachine.GetCharaGameObjectFaceDir() * 1.5f);
                        }
                    }
                    else
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                    }

                }
                                        
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
                if(!_sm.IsFriendAlreadyAtDefaultDistance())
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                }
                else
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                }
            }
            else
            {
                if(_sm.FriendsCommandDirection.position == _sm.FriendsDefaultDirection.position)
                {
                    if(!_sm.IsFriendAlreadyAtDefaultDistance())
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                    }
                    else
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
                    }
                }
                else
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsCommandDirection.position);
                }
                
            }
        }
        

    }
    public override void ExitState()
    {
        _sm.IsAIIdle = false;
        _sm.GetPlayableCharaIdentity.Crouch(false);
    }
    

}

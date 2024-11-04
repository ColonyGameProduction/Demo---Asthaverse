using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAI_EngageState : FriendAIState
{
    private bool isFacingToWallWhenCrouching = false;
    public FriendAI_EngageState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
    }

    public override void UpdateState()
    {
        if(!_sm.IsAIEngage || PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || !_sm.IsAIInput || _sm.GetPlayableCharaIdentity.IsAnimatingOtherAnimation || _sm.GetPlayableCharaIdentity.IsReviving || _sm.GetPlayableCharaIdentity.IsSilentKilling || _sm.IsCharacterDead)
        {
            _sm.SwitchState(_factory.AI_IdleState());
        }

        _sm.GetFOVMachine.GetClosestEnemy();
        if(_sm.GetFOVMachine.ClosestEnemy != null)
        {
            if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
            if(!_sm.GotDetectedbyEnemy)
            {
                Debug.Log(_sm.gameObject.name + "not detected but see person");
                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                
                StartShooting();
                if(_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    StopShooting();
                    
                    _sm.GetMoveStateMachine.IsRunning = true;
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    _sm.TakingCover();
                    if(_sm.CanTakeCoverInThePosition)
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                        _sm.IsTakingCover = true;
                    }
                    else
                    {
                        _sm.IsTakingCover = false;
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;

                        _sm.RunAway();
                    }
                }
            }
            else
            {
                Debug.Log(_sm.gameObject.name + "detected but see person");
                StopShooting();
                _sm.GetMoveStateMachine.IsRunning = true;
                if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
                {
                    _sm.IsTakingCover = false;
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    
                    _sm.RunAway();
                }
                else
                {
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    _sm.TakingCover();
                    if(_sm.CanTakeCoverInThePosition)
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                        _sm.IsTakingCover = true;
                    }
                    else
                    {
                        _sm.IsTakingCover = false;
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                        
                        _sm.RunAway();
                    }
                }
                
            }
            
        }
        else
        {
            StopShooting();
            _sm.GetMoveStateMachine.IsRunning = true;
            if(!_sm.GotDetectedbyEnemy)
            {
                Debug.Log(_sm.gameObject.name + "not detected not see person");
                if(!_sm.IsTakingCover)
                {
                    Debug.Log(_sm.gameObject.name + "not detected not see person I'mnot taking cover" );
                    if(!_sm.GetMoveStateMachine.IsCrouching)
                    {

                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {
                            float distanceBetweenCharaWithEnemyLastSeenPos = Vector3.Distance(_sm.GetFOVMachine.EnemyCharalastSeenPosition, _sm.transform.position);
                            if(distanceBetweenCharaWithEnemyLastSeenPos > _sm.GetFOVMachine.viewRadius || Physics.Raycast(_sm.transform.position, _sm.GetFOVMachine.EnemyCharalastSeenPosition, out RaycastHit hit,_sm.GetFOVMachine.GroundMask))
                            {
                                _sm.GetMoveStateMachine.SetAIDirection(_sm.GetFOVMachine.EnemyCharalastSeenPosition);
                            }
                            else
                            {
                                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                            }
                            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                            if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                            
                            Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                            float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                            if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                            {   
                                _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                            }
                        }
                    }
                    else
                    {
                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {
                            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                            if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                            Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                            float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                            if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                            {   

                                _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
                            }
                        }
                    }
                    
                }
                else
                {
                    Debug.Log(_sm.gameObject.name + "not detected not see person I'm taking cover" );
                    if(_sm.GetMoveStateMachine.IsIdle) // is at taking cover pos
                    {
                        _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenTakingCover, true);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                        
                        if(!_sm.isWallTallerThanChara)_sm.GetMoveStateMachine.IsCrouching = true;
                        if(_sm.GetPlayableCharaIdentity.CurrHealth == _sm.GetPlayableCharaIdentity.TotalHealth && !_sm.GetUseWeaponStateMachine.IsReloading)
                        {
                            _sm.IsTakingCover = false;
                            
                        }
                    }
                }
            }
            else if(_sm.GotDetectedbyEnemy)
            {
                if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
                Debug.Log(_sm.gameObject.name + "detected not see person");
                if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
                {
                    _sm.IsTakingCover = false;
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    
                    _sm.RunAway();
                }
                else
                {
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    _sm.TakingCover();
                    if(_sm.CanTakeCoverInThePosition)
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                        _sm.IsTakingCover = true;
                    }
                    else
                    {
                        _sm.IsTakingCover = false;
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                        
                        _sm.RunAway();
                    }
                }
            }
        }
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)_sm.GetFOVMachine.IsCheckingEnemyLastPosition();
    }

    private void StartShooting()
    {
        // Debug.Log("shoot di 2" + _sm.GetFOVMachine.ClosestEnemy.position);
        _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
        _sm.GetUseWeaponStateMachine.GiveChosenTarget(_sm.GetFOVMachine.ClosestEnemy);
        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
        if(!_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = true;
    }
    private void StopShooting()
    {
        _sm.AimAIPointLookAt(null);
        if(_sm.GetUseWeaponStateMachine.ChosenTarget != null)_sm.GetUseWeaponStateMachine.GiveChosenTarget(null);
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = false;
    }
}

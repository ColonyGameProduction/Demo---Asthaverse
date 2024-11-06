using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
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
            return;
        }

        // if(!_sm.IsTakingCover)
        // {
        //     _sm.SwitchState(_factory.AI_TakingCoverState());
        //     return;
        // }

        _sm.GetFOVMachine.GetClosestEnemy();
        
        if(!_sm.GotDetectedbyEnemy)
        {
            
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                Debug.Log(_sm.transform.name + "no one see me but I see someone");
                if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
                if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                // if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {

                    _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    StartShooting(_sm.GetFOVMachine.ClosestEnemy);
                }
                
                if(_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    StopShooting();

                    _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    //Diam di tempat sambil ngelihat target
                }
            }
            else
            {
                Debug.Log(_sm.transform.name + "no one see me I no see someone");
                StopShooting();
                if(!_sm.GetMoveStateMachine.IsIdle)
                {
                    if(!_sm.GetMoveStateMachine.IsRunning && _sm.ClosestEnemyWhoSawAI != null)
                    {
                        Vector3 facedir = (_sm.Agent.steeringTarget - _sm.transform.position).normalized;
                        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                        _sm.GetMoveStateMachine.SetAITargetToLook(-facedir, true);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    }
                }
                else
                {
                    if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
                    if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                    if(_sm.IsTakingCover)
                    {
                        
                        Debug.Log(_sm.transform.name + "I'm at the place");
                        
                        _sm.SwitchState(_factory.AI_TakingCoverState());
                        return;
                    }
                    else
                    {
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    }
                    
                }
            }
        }
        else
        {
            
            _sm.GetClosestEnemyWhoSawAI();
            Transform currTarget = null;
            bool isClosestEnemy = false;
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                currTarget = _sm.GetFOVMachine.ClosestEnemy;
                isClosestEnemy = true;
            }
            else
            {
                currTarget = _sm.ClosestEnemyWhoSawAI;
            }
            if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
            {
                Debug.Log(_sm.transform.name + "someone see me I'm still healthy");
                if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
                if(_sm.EnemyWhoSawAIList.Count < _sm.MinEnemyMakeCharaFeelOverwhelmed)
                {
                    Debug.Log(_sm.transform.name + "I can handle this");
                    if(!_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        // Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                        _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                        if(isClosestEnemy)StartShooting(currTarget);
                        
                    }
                    
                    if(_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        StopShooting();
                        
                        // _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy != null? _sm.GetFOVMachine.ClosestEnemy.position : _sm.ClosestEnemyWhoSawAI.position, false);
                        _sm.TakingCover();
                        if(_sm.CanTakeCoverInThePosition || _sm.LeaveDirection != Vector3.zero)
                        {
                            if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                            if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                            _sm.GetMoveStateMachine.IsRunning = true;
                            if(_sm.CanTakeCoverInThePosition)
                            {
                                _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                                _sm.IsTakingCover = true;
                            }
                            else if(_sm.LeaveDirection != Vector3.zero)
                            {
                                RunAwayOption();
                            }
                            
                        }
                        else if(!_sm.CanTakeCoverInThePosition && _sm.LeaveDirection == Vector3.zero)
                        {
                            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                            if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;

                            _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                            if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                            
                        }
                    }
                }
                else
                {
                    Debug.Log(_sm.transform.name + "I cant handle this");
                    if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
                    if(!_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                        _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                        if(isClosestEnemy)StartShooting(currTarget);
                        
                    }
                    else
                    {
                        StopShooting();
                        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                        _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    }
                    _sm.TakingCover();
                    if(_sm.CanTakeCoverInThePosition || _sm.LeaveDirection != Vector3.zero)
                    {
                        if(_sm.CanTakeCoverInThePosition)
                        {
                            _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                            _sm.IsTakingCover = true;
                        }
                        else if(_sm.LeaveDirection != Vector3.zero)
                        {
                            RunAwayOption();
                        }
                    }
                    else if(!_sm.CanTakeCoverInThePosition && _sm.LeaveDirection == Vector3.zero)
                    {
                        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                    }
                }
            }
            else
            {
                Debug.Log(_sm.transform.name + "someone see me I'm not healthy");
                _sm.TakingCover();
                if(_sm.CanTakeCoverInThePosition || _sm.LeaveDirection != Vector3.zero)
                {
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                    _sm.GetMoveStateMachine.IsRunning = true;
                    if(_sm.CanTakeCoverInThePosition)
                    {
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                        _sm.IsTakingCover = true;
                    }
                    else if(_sm.LeaveDirection != Vector3.zero)
                    {
                        RunAwayOption();
                    }
                    
                }
                else if(!_sm.CanTakeCoverInThePosition && _sm.LeaveDirection == Vector3.zero)
                {
                    if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                    if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
                    if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                    
                    if(!_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                        _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                        if(isClosestEnemy)StartShooting(currTarget);
                        
                    }

                    
                    if(_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        StopShooting();
                        
                        _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    }
                }
            }
        }




        
        // {
        //     StopShooting();
        //     _sm.GetMoveStateMachine.IsRunning = true;
        //     if(!_sm.GotDetectedbyEnemy)
        //     {
        //         Debug.Log(_sm.gameObject.name + "not detected not see person");
        //         if(!_sm.IsTakingCover)
        //         {
        //             Debug.Log(_sm.gameObject.name + "not detected not see person I'mnot taking cover" );
        //             if(!_sm.GetMoveStateMachine.IsCrouching)
        //             {

        //                 if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
        //                 {
        //                     float distanceBetweenCharaWithEnemyLastSeenPos = Vector3.Distance(_sm.GetFOVMachine.EnemyCharalastSeenPosition, _sm.transform.position);
        //                     if(distanceBetweenCharaWithEnemyLastSeenPos > _sm.GetFOVMachine.viewRadius || Physics.Raycast(_sm.transform.position, _sm.GetFOVMachine.EnemyCharalastSeenPosition, out RaycastHit hit,_sm.GetFOVMachine.GroundMask))
        //                     {
        //                         _sm.GetMoveStateMachine.SetAIDirection(_sm.GetFOVMachine.EnemyCharalastSeenPosition);
        //                     }
        //                     else
        //                     {
        //                         if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
        //                     }
        //                     _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
        //                     if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                            
        //                     Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
        //                     float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
        //                     if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
        //                     {   
        //                         _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
        //                 {
        //                     _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
        //                     if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

        //                     Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
        //                     float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
        //                     if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
        //                     {   

        //                         _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
        //                         if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
        //                     }
        //                 }
        //             }
                    
        //         }
        //         else
        //         {
        //             Debug.Log(_sm.gameObject.name + "not detected not see person I'm taking cover" );
        //             if(_sm.GetMoveStateMachine.IsIdle) // is at taking cover pos
        //             {
        //                 _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenTakingCover, true);
        //                 if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                        
        //                 if(!_sm.isWallTallerThanChara)_sm.GetMoveStateMachine.IsCrouching = true;
        //                 if(_sm.GetPlayableCharaIdentity.CurrHealth == _sm.GetPlayableCharaIdentity.TotalHealth && !_sm.GetUseWeaponStateMachine.IsReloading)
        //                 {
        //                     _sm.IsTakingCover = false;
                            
        //                 }
        //             }
        //         }
        //     }
        //     else if(_sm.GotDetectedbyEnemy)
        //     {
        //         if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
        //         Debug.Log(_sm.gameObject.name + "detected not see person");
        //         AllRunAwayOption();
        //     }
        // }
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
        if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)_sm.GetFOVMachine.IsCheckingEnemyLastPosition();
    }

    private void StartShooting(Transform chosenTarget)
    {
        // Debug.Log("shoot di 2" + _sm.GetFOVMachine.ClosestEnemy.position);
        _sm.AimAIPointLookAt(chosenTarget);
        _sm.GetUseWeaponStateMachine.GiveChosenTarget(chosenTarget);
        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
        if(!_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = true;
    }
    private void StopShooting()
    {
        _sm.AimAIPointLookAt(null);
        if(_sm.GetUseWeaponStateMachine.ChosenTarget != null)_sm.GetUseWeaponStateMachine.GiveChosenTarget(null);
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = false;
    }

    private void AllRunAwayOption()
    {
        if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
        {
            RunAwayOption();
        }
        else
        {
            TakeCoverOption();
        }
    }
    private void TakeCoverOption()
    {
        _sm.TakingCover();
        if(_sm.CanTakeCoverInThePosition)
        {
            _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
            _sm.IsTakingCover = true;
        }
        else
        {
            RunAwayOption();
        }
    }
    private void RunAwayOption()
    {
        _sm.IsTakingCover = false;
        
        _sm.RunAway();
    }


}

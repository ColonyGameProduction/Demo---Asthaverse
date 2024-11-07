using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//runaway - includes running away or run to take cover
public class FriendAI_TakingCoverState : FriendAIState
{
    bool _isHiding;
    bool _isChecking;
    float changeTimer;
    float changeTimerMax = 3f;
    public FriendAI_TakingCoverState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        _isHiding = true;
        _isChecking = false;
        changeTimer = changeTimerMax;

        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
    }

    public override void UpdateState()
    {
        Debug.Log("I'm taking cover" + _sm.transform.name);

        if(_isHiding)Debug.Log("I'm hiding");
        else Debug.Log("I'm checking");
        if(!_sm.IsAIEngage || PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || !_sm.IsAIInput || _sm.GetPlayableCharaIdentity.IsAnimatingOtherAnimation || _sm.GetPlayableCharaIdentity.IsReviving || _sm.GetPlayableCharaIdentity.IsSilentKilling || _sm.IsCharacterDead)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            return;
        }


        //I'm on my way to the take cover place
        _sm.GetFOVMachine.GetClosestEnemy();
        _sm.GetClosestEnemyWhoSawAI();
        
        ChangeTimerCounter();
        
        if(!_sm.GotDetectedbyEnemy)
        {
            Patroling();
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                _sm.IsCheckingLastPosTImer = _sm.IsCheckingLastPosTImerMax;
                _sm.GetMoveStateMachine.IsRunning = false;
                if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                // if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {

                    _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                    
                    // StartShooting(_sm.GetFOVMachine.ClosestEnemy);
                    StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                   
                }
                
                if(_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    StopShooting();

                    _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    //Diam di tempat sambil ngelihat target
                }
                if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
            }
            else
            {
                ChangeState();
                
                if(_isHiding)
                {
                    if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.TakeCoverPosition && _sm.GetMoveStateMachine.IsIdle)
                    {
                        _sm.GetMoveStateMachine.IsRunning = false;
                        _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenTakingCover, true);
                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                        if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = true;
                    }
                }
                else if(_isChecking)
                {

                    float dotBetweenCharaWithEnemyLastSeen = 0;
                    if(!_sm.isWallTallerThanChara)
                    {
                        _sm.GetMoveStateMachine.IsRunning = false;

                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {

                            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                            Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                            dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                            Debug.Log("Muncul di sini" + dirEnemyLastSeenToChara + " xx" + _sm.transform.name);
                        }
                        else
                        {

                            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenChecking, true);
                            dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);
                            Debug.Log("Muncul di sina" + _sm.DirToLookAtWhenChecking + " xx" + _sm.transform.name);
                        }

                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                        if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                        {   
                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {
                                Debug.Log("I'm checking !!!" + _sm.transform.name);
                                if(_sm.IsCheckingLastPosTImer > 0)_sm.IsCheckingLastPosTImer -= Time.deltaTime;
                                else
                                {
                                    Debug.Log("I'm  done checking !!!" + _sm.transform.name);
                                    _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                }
                            }
                            Debug.Log("Check when wall is nmot taller" + _sm.transform.name);
                            if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
                        }
                    }
                    else
                    {
                        if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.PosToGoWhenCheckingWhenWallIsHigher && _sm.GetMoveStateMachine.IsIdle)
                        {
                            _sm.GetMoveStateMachine.IsRunning = false;

                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {

                                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                                Vector3 dirEnemyLastSeenToCharas = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                                dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToCharas, _sm.transform.forward);
                                Debug.Log("Muncul di sini" + dirEnemyLastSeenToCharas + " xx" + _sm.transform.name);
                            }
                            else
                            {

                                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenChecking, true);
                                dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);
                                Debug.Log("Muncul di sina" + _sm.DirToLookAtWhenChecking + " xx" + _sm.transform.name);
                            }
                            
                            if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;


                            if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                            {   
                                if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                                {
                                    Debug.Log("I'm checking !!!" + _sm.transform.name);
                                    if(_sm.IsCheckingLastPosTImer > 0)_sm.IsCheckingLastPosTImer -= Time.deltaTime;
                                    else
                                    {
                                        Debug.Log("I'm  done checking !!!" + _sm.transform.name);
                                        _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                    }
                                }
                                Debug.Log("I did it" + _sm.transform.name);
                            }

                        }
                    }
                }
            }
        }
        else
        {
            Transform currTarget = null;
            bool isClosestEnemy = false;
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                _sm.IsCheckingLastPosTImer = _sm.IsCheckingLastPosTImerMax;
                currTarget = _sm.GetFOVMachine.ClosestEnemy;
                isClosestEnemy = true;
            }
            else
            {
                currTarget = _sm.ClosestEnemyWhoSawAI;
            }

            if(_isHiding)Patroling();
            if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
            {
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    // Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                    _sm.GetMoveStateMachine.IsRunning = false;
                    if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                    if(_isChecking)if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                    _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    if(isClosestEnemy)
                    {
                        StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                    }
                    if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                }
                if(_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    StopShooting();
                    
                    if(_isChecking)
                    {
                        ForceChange(true, false);
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                        if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                        _sm.GetMoveStateMachine.IsRunning = true;
                    }
                    else if(_isHiding)
                    {
                        if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                        if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.TakeCoverPosition && _sm.GetMoveStateMachine.IsIdle)
                        {
                            _sm.SwitchState(_factory.AI_EngageState());
                            return;
                        }
                        
                    }
                    
                }
                if(_sm.EnemyWhoSawAIList.Count >= _sm.MinEnemyMakeCharaFeelOverwhelmed)
                {

                    if(_isChecking)
                    {
                        ForceChange(true, false);
                    }
                    else if(_isHiding)
                    {
                        if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                        if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.TakeCoverPosition && _sm.GetMoveStateMachine.IsIdle)
                        {
                            _sm.SwitchState(_factory.AI_EngageState());
                            return;
                        }
                        
                    }
                }

            }
            else
            {
                StopShooting();
                    
                if(_isChecking)
                {
                    ForceChange(true, false);
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                    _sm.GetMoveStateMachine.IsRunning = true;
                }
                else if(_isHiding)
                {
                    if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                    if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.TakeCoverPosition && _sm.GetMoveStateMachine.IsIdle)
                    {
                        _sm.SwitchState(_factory.AI_EngageState());
                        return;
                    }
                    
                }
            }
        }
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
        _sm.IsTakingCover = false;
    }
    private void Patroling()
    {
        Debug.Log(_sm.GetMoveStateMachine.CurrAIDirPos + " " + _sm.TakeCoverPosition + " " + _sm.PosToGoWhenCheckingWhenWallIsHigher + " " + _sm.transform.name);
        if(_isHiding)
        {
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.TakeCoverPosition)
            {
                _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
            }
            
            
        }
        else if(_isChecking)
        {
            if(!_sm.isWallTallerThanChara)
            {

            }
            else
            {
                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.PosToGoWhenCheckingWhenWallIsHigher)
                {
                    _sm.GetMoveStateMachine.SetAIDirection(_sm.PosToGoWhenCheckingWhenWallIsHigher);
                }
            }
            
        }
    }
    private void ChangeTimerCounter()
    {
        if(changeTimer > 0 && _sm.GetMoveStateMachine.IsIdle)changeTimer -=Time.deltaTime;
    }
    private void ChangeState()
    {
        if(changeTimer <= 0)
        {
            // Debug.Log("Change now" + _sm.transform.name);
            if(_isHiding)
            {
                if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower && !_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    _isHiding = !_isHiding;
                    _isChecking = !_isChecking;
                }
            }
            else if(_isChecking)
            {
                _isHiding = !_isHiding;
                _isChecking = !_isChecking;
            }

            changeTimer = changeTimerMax;
        }
    }
    private void ForceChange(bool isHiding, bool isChecking)
    {
        _isHiding = isHiding;
        _isChecking = isChecking;
        changeTimer = changeTimerMax;
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
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//runaway - includes running away or run to take cover
public class FriendAI_TakingCoverState : FriendAIState
{
    float changeTimer;
    float changeTimerMax = 3f;

    public FriendAI_TakingCoverState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        _sm.IsHiding = true;
        _sm.IsChecking = false;
        changeTimer = changeTimerMax;
        _sm.HidingCheckDelayTimer = _sm.GetFOVMachine.FindDelayTimerNow+ 0.15f;
        _sm.EnemyWhoSawAIList.Clear();

        _sm.GetPlayableCharaIdentity.Aiming(true);
        _sm.SetAllowLookTarget(true, _sm.DirToLookAtWhenTakingCover, true);

        // if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching && _sm.GetMoveStateMachine.IsIdle)_sm.GetPlayableCharaIdentity.Crouch(true);
        if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching )_sm.GetPlayableCharaIdentity.Crouch(true);

    }

    public override void UpdateState()
    {
        
        if(!_sm.IsAIEngage || PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || !_sm.IsAIInput || _sm.GetPlayableCharaIdentity.IsAnimatingOtherAnimation || _sm.GetPlayableCharaIdentity.IsReviving || _sm.GetPlayableCharaIdentity.IsSilentKilling || _sm.IsCharacterDead || _sm.IsCurrTakeCoverAlreadyOccupied() || _sm.GetPlayableCharaIdentity.IgnoreThisCharacter)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            return;
        }


        //I'm on my way to the take cover place
        _sm.GetFOVMachine.GetClosestEnemy();
    
        ChangeTimerCounter();
        if(!_sm.GotDetectedbyEnemy)
        {
            Patroling();
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                // Debug.Log("T-NO ONE SEE ME; I SEE SOMEONE" + _sm.transform.name);
                _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;
                
                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();

                _sm.GetPlayableCharaIdentity.Aiming(true);
                _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
                _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.ClosestEnemy.position, false);

                if(!_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    // Debug.Log("T-NO ONE SEE ME; I SEE SOMEONE - I'm SHOOTING THIS PERSON" + _sm.transform.name);
                    _sm.StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                }
                if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    _sm.StopShooting();
                    // Debug.Log("T-NO ONE SEE ME; I SEE SOMEONE - I'm RELOADING WHILE STILL LOOKING" + _sm.transform.name);
                    //Diam di tempat sambil ngelihat target
                }
                if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
            }
            else
            {
                // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE" + _sm.transform.name + " ");
                ChangeState();
                _sm.AimAIPointLookAt(null);
                _sm.StopShooting();
                if(_sm.IsHiding)
                {
                    if(_sm.IsAtTakingCoverHidingPlace)
                    {
                        // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M HIDING AND IDLE AND LOOKING AT TAKE COVER DIR" + _sm.transform.name);
                        
                        _sm.GetPlayableCharaIdentity.Aiming(true);
                        _sm.SetAllowLookTarget(true, _sm.DirToLookAtWhenTakingCover, true);

                        if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.GetPlayableCharaIdentity.Crouch(true);
                    }
                }
                else if(_sm.IsChecking)
                {
                    float dotCharaWithEnemyLastSeen = 0;
                    if(!_sm.isWallTallerThanChara)
                    {
                        // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS NOT TALLER THAN ME" + _sm.transform.name);
                        _sm.GetMoveStateMachine.IsRunning = false;

                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {
                            // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING ENEMYLASTPOS" + _sm.transform.name);
                            _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);

                            Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                            dotCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                            // Debug.Log("Muncul di sini" + dirEnemyLastSeenToChara + " xx" + _sm.transform.name);
                        }
                        else
                        {

                            _sm.SetAllowLookTarget(true, _sm.DirToLookAtWhenChecking, true);
                            dotCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);
                            // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING NORMALPOS" + _sm.transform.name);
                        }


                        if(dotCharaWithEnemyLastSeen >= 0.95f)
                        {   
                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {

                                if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
                                else
                                {
                                    // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M DONE CHECKING POS" + _sm.transform.name);
                                    _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                }
                            }

                            if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetPlayableCharaIdentity.Crouch(false);
                        }
                    }
                    else
                    {
                        // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME" + _sm.transform.name);
                        if(_sm.IsAtTakingCoverCheckingPlace)
                        {
                            _sm.GetMoveStateMachine.IsRunning = false;

                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {
                                // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M CHECKING ENEMYLASTPOS" + _sm.transform.name);
                                _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                                Vector3 dirEnemyLastSeenToCharas = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                                dotCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToCharas, _sm.transform.forward);

                            }
                            else
                            {
                                // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M CHECKING NORMALPOS" + _sm.transform.name);
                                _sm.SetAllowLookTarget(true, _sm.DirToLookAtWhenChecking, true);
                                dotCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);

                            }
                            


                            if(dotCharaWithEnemyLastSeen >= 0.95f)
                            {   
                                if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                                {

                                    if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
                                    else
                                    {
                                        // Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M DONE CHECKING POS" + _sm.transform.name);
                                        _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                    }
                                }
                            }
                        }
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
                _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;
                currTarget = _sm.GetFOVMachine.ClosestEnemy;
                isClosestEnemy = true;
                _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
                // Debug.Log("T-SOMEONE SEE ME; I SEE SOMEONE" + _sm.transform.name);
            }
            else
            {
                currTarget = _sm.ClosestEnemyWhoSawAI;
                _sm.AimAIPointLookAt(null);
                // Debug.Log("T-SOMEONE SEE ME; I SEE NO ONE" + _sm.transform.name);
            }

            if(_sm.IsHiding)Patroling();
            if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
            {
                // Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY" + _sm.transform.name);
                if(!_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    // Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                    // Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I KEEP SHOOTING" + _sm.transform.name);

                    if(_sm.IsChecking)if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();

                    _sm.GetPlayableCharaIdentity.Aiming(true);
                    _sm.SetAllowLookTarget(true, currTarget.position, false);

                    if(!isClosestEnemy && _sm.GetMoveStateMachine.IsIdle)
                    {
                        Vector3 dirTargetToCharaFOV = (currTarget.position - _sm.GetFOVMachine.GetFOVPoint.position).normalized;
                        float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirTargetToCharaFOV, _sm.GetFOVMachine.GetFOVPoint.forward);
                        if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                        {
                            float dotRight = Vector3.Dot(dirTargetToCharaFOV, _sm.GetFOVMachine.GetFOVPoint.right);
                            if(dotRight >= 0)
                            {
                                _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position + _sm.GetFOVMachine.GetFOVPoint.right);
                            }
                            else
                            {
                                _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position - _sm.GetFOVMachine.GetFOVPoint.right);
                            }
                            // Debug.Log("Sometimes they went here" + _sm.transform.name);
                        }
                    }
                    if(isClosestEnemy)
                    {
                        _sm.StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                    }
                    else
                    {
                        _sm.StopShooting();
                    }
                    if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                }
                if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets || _sm.EnemyWhoSawAIList.Count >= _sm.MinEnemyMakeCharaFeelOverwhelmed)
                {
                    if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)_sm.StopShooting();
                    
                    if(_sm.IsChecking)
                    {
                        // Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN CHECKING" + _sm.PosToGoWhenCheckingWhenWallIsHigher + " " +_sm.transform.name);
                        if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                        {
                            _sm.GetPlayableCharaIdentity.Run(true);
                        }
                        
                        ForceChange(true, false);
                    }
                    else if(_sm.IsHiding)
                    {
                        // Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN I WENT HIDING" + _sm.TakeCoverPosition + " " + _sm.transform.name);
                        if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                        
                        if(_sm.IsAtTakingCoverHidingPlace && _sm.EnemyWhoSawAIList.Count > 0 && _sm.HidingCheckDelayTimer <= 0 && ((!_sm.isWallTallerThanChara && _sm.IsCrouchingBehindWall()) || _sm.isWallTallerThanChara))
                        {
                            // Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN I HIDE - I NEED TO SEARCH NEW PLACE" + _sm.transform.name);
                            _sm.SwitchState(_factory.AI_EngageState());
                            return;
                        }
                        
                    }
                }

            }
            else
            {
                _sm.StopShooting();
                    
                if(_sm.IsChecking)
                {
                    // Debug.Log("T-SOMEONE SEE ME - I'M NOT HEALTHY - WHEN CHECKING SO I HIDE WHILE RUNNING" + _sm.PosToGoWhenCheckingWhenWallIsHigher + " "  + _sm.transform.name);
                    _sm.GetPlayableCharaIdentity.Run(true);
                    ForceChange(true, false);
                }
                else if(_sm.IsHiding)
                {
                    // Debug.Log("T-SOMEONE SEE ME - I'M NOT HEALTHY - WHEN I WENT HIDING" + _sm.TakeCoverPosition + " " +_sm.transform.name);
                    if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                    if(_sm.IsAtTakingCoverHidingPlace && _sm.EnemyWhoSawAIList.Count > 0 && _sm.HidingCheckDelayTimer <= 0 && ((!_sm.isWallTallerThanChara && _sm.IsCrouchingBehindWall()) || _sm.isWallTallerThanChara))
                    {
                        _sm.IsAtTakingCoverHidingPlace = false;
                        // Debug.Log("T-SOMEONE SEE ME - I'M NOT HEALTHY - WHEN I HIDE SO I SEARCH NEW PLACE" + _sm.transform.name);
                        _sm.SwitchState(_factory.AI_EngageState());
                        return;
                    }
                    
                }
            }
        }
        if(_sm.HidingCheckDelayTimer > 0 && _sm.IsAtTakingCoverHidingPlace && ((!_sm.isWallTallerThanChara && _sm.IsCrouchingBehindWall()) || _sm.isWallTallerThanChara)) _sm.HidingCheckDelayTimer -= Time.deltaTime;
      
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
        if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
        _sm.IsTakingCover = false;
        _sm.IsAtTakingCoverHidingPlace = false;
        _sm.IsAtTakingCoverCheckingPlace = false;
    }
    private void Patroling()
    {
        if(_sm.IsHiding)
        {
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.TakeCoverPosition)
            {
                
                _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
            }
            
        }
        else if(_sm.IsChecking)
        {
            if(_sm.isWallTallerThanChara)
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
        // Debug.Log("Change Timer Now" + changeTimer + " aaaaa" + _sm.name);
    }
    private void ChangeState()
    {
        if(changeTimer <= 0)
        {
            // Debug.Log("Change now" + _sm.transform.name);
            if(_sm.IsHiding)
            {
                if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower && !_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    _sm.IsHiding = !_sm.IsHiding;
                    _sm.IsChecking = !_sm.IsChecking;
                }
            }
            else if(_sm.IsChecking)
            {
                _sm.IsHiding = !_sm.IsHiding;
                _sm.IsChecking = !_sm.IsChecking;
            }

            changeTimer = changeTimerMax;
        }
    }
    private void ForceChange(bool isHiding, bool isChecking)
    {
        _sm.IsHiding = isHiding;
        _sm.IsChecking = isChecking;
        changeTimer = changeTimerMax;
        if(_sm.IsHiding)
        {
            _sm.EnemyWhoSawAIList.Clear();
            _sm.HidingCheckDelayTimer = _sm.GetFOVMachine.FindDelayTimerNow+ 0.15f;
        }
        Patroling();
        if(_sm.IsHiding && !_sm.isWallTallerThanChara)
        {
            _sm.GetPlayableCharaIdentity.Aiming(true);
        _sm.SetAllowLookTarget(true, _sm.DirToLookAtWhenTakingCover, true);

            if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.GetPlayableCharaIdentity.Crouch(true);
        }
    }

}

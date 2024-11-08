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
        _sm.GetMoveStateMachine.IsRunning = false;
        _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenTakingCover, true);
        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

        if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching && _sm.GetMoveStateMachine.IsIdle)_sm.GetMoveStateMachine.IsCrouching = true;
    }

    public override void UpdateState()
    {
        Debug.Log("I'm taking cover" + _sm.transform.name);

        if(_isHiding)Debug.Log("T-HIDING"+ _sm.transform.name);
        else 
        {
            _sm.IsAtTakingCoverPlace = false;
        }
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
                Debug.Log("T-NO ONE SEE ME; I SEE SOMEONE" + _sm.transform.name);
                _sm.IsCheckingLastPosTImer = _sm.IsCheckingLastPosTImerMax;
                _sm.GetMoveStateMachine.IsRunning = false;
                if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                // if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    Debug.Log("T-NO ONE SEE ME; I SEE SOMEONE - I'm SHOOTING THIS PERSON" + _sm.transform.name);
                    _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                    
                    // StartShooting(_sm.GetFOVMachine.ClosestEnemy);
                    StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                   
                }
                
                if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    StopShooting();
                    Debug.Log("T-NO ONE SEE ME; I SEE SOMEONE - I'm RELOADING WHILE STILL LOOKING" + _sm.transform.name);
                    _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
                    //Diam di tempat sambil ngelihat target
                }
                if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
            }
            else
            {
                Debug.Log("T-NO ONE SEE ME; I SEE NO ONE" + _sm.transform.name);
                ChangeState();
                
                if(_isHiding)
                {
                    if(_sm.IsAtTakingCoverPlace)
                    {
                        _sm.IsAtTakingCoverPlace = false;
                        Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M HIDING AND IDLE AND LOOKING AT TAKE COVER DIR" + _sm.transform.name);
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
                        Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS NOT TALLER THAN ME" + _sm.transform.name);
                        _sm.GetMoveStateMachine.IsRunning = false;

                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {
                            Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING ENEMYLASTPOS" + _sm.transform.name);
                            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                            Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                            dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                            // Debug.Log("Muncul di sini" + dirEnemyLastSeenToChara + " xx" + _sm.transform.name);
                        }
                        else
                        {

                            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenChecking, true);
                            dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);
                            Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING NORMALPOS" + _sm.transform.name);
                        }

                        if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                        if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                        {   
                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {

                                if(_sm.IsCheckingLastPosTImer > 0)_sm.IsCheckingLastPosTImer -= Time.deltaTime;
                                else
                                {
                                    Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M DONE CHECKING POS" + _sm.transform.name);
                                    _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                }
                            }

                            if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
                        }
                    }
                    else
                    {
                        Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME" + _sm.transform.name);
                        if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.PosToGoWhenCheckingWhenWallIsHigher && _sm.GetMoveStateMachine.IsIdle)
                        {
                            _sm.GetMoveStateMachine.IsRunning = false;

                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {
                                Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M CHECKING ENEMYLASTPOS" + _sm.transform.name);
                                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                                Vector3 dirEnemyLastSeenToCharas = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                                dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToCharas, _sm.transform.forward);

                            }
                            else
                            {
                                Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M CHECKING NORMALPOS" + _sm.transform.name);
                                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenChecking, true);
                                dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);

                            }
                            
                            if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;


                            if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                            {   
                                if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                                {

                                    if(_sm.IsCheckingLastPosTImer > 0)_sm.IsCheckingLastPosTImer -= Time.deltaTime;
                                    else
                                    {
                                        Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M DONE CHECKING POS" + _sm.transform.name);
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
            
            Transform currTarget = null;
            bool isClosestEnemy = false;
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                _sm.IsCheckingLastPosTImer = _sm.IsCheckingLastPosTImerMax;
                currTarget = _sm.GetFOVMachine.ClosestEnemy;
                isClosestEnemy = true;
                Debug.Log("T-SOMEONE SEE ME; I SEE SOMEONE" + _sm.transform.name);
            }
            else
            {
                currTarget = _sm.ClosestEnemyWhoSawAI;
                Debug.Log("T-SOMEONE SEE ME; I SEE NO ONE" + _sm.transform.name);
            }

            if(_isHiding)Patroling();
            if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
            {
                Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY" + _sm.transform.name);
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    // Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                    Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I KEEP SHOOTING" + _sm.transform.name);
                    _sm.GetMoveStateMachine.IsRunning = false;
                    if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
                    if(_isChecking)if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();

                    _sm.GetMoveStateMachine.SetAITargetToLook(currTarget.position, false);
                    if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

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
                            Debug.Log("Sometimes they went here" + _sm.transform.name);
                        }
                    }
                    if(isClosestEnemy)
                    {
                        StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                    }
                    if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                }
                if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    StopShooting();
                    
                    if(_isChecking)
                    {
                        Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN CHECKING" + _sm.PosToGoWhenCheckingWhenWallIsHigher + " " +_sm.transform.name);
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                        if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                        _sm.GetMoveStateMachine.IsRunning = true;
                        ForceChange(true, false);
                    }
                    else if(_isHiding)
                    {
                        Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN I WENT HIDING" + _sm.TakeCoverPosition + " " + _sm.transform.name);
                        if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                        // if(_sm.GetMoveStateMachine.CurrAIDirPos == _sm.TakeCoverPosition && _sm.GetMoveStateMachine.IsIdle)
                        // {
                        //     Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN I HIDE - I NEED TO SEARCH NEW PLACE" + _sm.transform.name);
                        //     _sm.SwitchState(_factory.AI_EngageState());
                        //     return;
                        // }
                        if(_sm.IsAtTakingCoverPlace)
                        {
                            _sm.IsAtTakingCoverPlace = false;
                            Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I RELOAD WHEN I HIDE - I NEED TO SEARCH NEW PLACE" + _sm.transform.name);
                            _sm.SwitchState(_factory.AI_EngageState());
                            return;
                        }
                        
                    }
                    
                }
                if(_sm.EnemyWhoSawAIList.Count >= _sm.MinEnemyMakeCharaFeelOverwhelmed)
                {
                    if(_isChecking)
                    {
                        StopShooting();
                        Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I'M OVERWHELEMED WHEN CHECKING SO I HIDE WHILE KEEP SHOOTING OR DOING IDK" + _sm.PosToGoWhenCheckingWhenWallIsHigher + " " + _sm.transform.name);
                        ForceChange(true, false);
                    }
                    else if(_isHiding)
                    {
                        Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I'M OVERWHELEMED WHEN I WENT HIDING SO I HIDE WHILE KEEP SHOOTING OR DOING IDK" +  _sm.TakeCoverPosition + " " +_sm.transform.name);
                        if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                        if(_sm.IsAtTakingCoverPlace)
                        {
                            _sm.IsAtTakingCoverPlace = false;
                            Debug.Log("T-SOMEONE SEE ME - I'M HEALTHY - I'M OVERWHELEMED WHEN I HIDING SO I SEARCH NEW PLACE" + _sm.transform.name);
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
                    Debug.Log("T-SOMEONE SEE ME - I'M NOT HEALTHY - WHEN CHECKING SO I HIDE WHILE RUNNING" + _sm.PosToGoWhenCheckingWhenWallIsHigher + " "  + _sm.transform.name);
                    if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                    if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                    _sm.GetMoveStateMachine.IsRunning = true;
                    ForceChange(true, false);
                }
                else if(_isHiding)
                {
                    Debug.Log("T-SOMEONE SEE ME - I'M NOT HEALTHY - WHEN I WENT HIDING" + _sm.TakeCoverPosition + " " +_sm.transform.name);
                    if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
                    if(_sm.IsAtTakingCoverPlace)
                    {
                        _sm.IsAtTakingCoverPlace = false;
                        Debug.Log("T-SOMEONE SEE ME - I'M NOT HEALTHY - WHEN I HIDE SO I SEARCH NEW PLACE" + _sm.transform.name);
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
        _sm.IsAtTakingCoverPlace = false;
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
                if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower && !_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
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
        Patroling();
        if(_isHiding && !_sm.isWallTallerThanChara)
        {
            if(_sm.GetMoveStateMachine.IsIdle)
            {
                _sm.GetMoveStateMachine.IsRunning = false;
                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.DirToLookAtWhenTakingCover, true);
                if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;

                if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = true;
            }
        }
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

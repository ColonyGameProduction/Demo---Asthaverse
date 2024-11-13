using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_TakingCoverState : EnemyAIState
{
    float changeTimer;
    float changeTimerMax = 3f;
    public EnemyAI_TakingCoverState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        _sm.IsHiding = true;
        _sm.IsChecking = false;
        changeTimer = changeTimerMax;
        _sm.HidingCheckDelayTimer = _sm.GetFOVMachine.FindDelayTimerNow;
        _sm.EnemyWhoSawAIList.Clear();

        _sm.EnemyIdentity.Aiming(true);
        _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenTakingCover, true);

        // if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching && _sm.GetMoveStateMachine.IsIdle)_sm.GetPlayableCharaIdentity.Crouch(true);
        if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.EnemyIdentity.Crouch(true);
    }

    public override void UpdateState()
    {
        if(_sm.IsCharacterDead || _sm.EnemyIdentity.IsSilentKilled)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            return;
        }
        _sm.EnemyAIManager.OnEnemyisEngaging?.Invoke();
        _sm.GetFOVState.FOVStateHandler();
        ChangeTimerCounter();
        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            // _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;
            _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
            if(_sm.IsHiding)
            {
                _sm.EnemyIdentity.Aiming(true);
                _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.GetFOVMachine.ClosestEnemy.position, false);

                if(_sm.IsAtTakingCoverHidingPlace && _sm.HidingCheckDelayTimer <= 0 && ((!_sm.isWallTallerThanChara && _sm.IsCrouchingBehindWall()) || _sm.isWallTallerThanChara))
                {
                    if(_sm.GetFOVState.CurrState == FOVDistState.far || _sm.GetFOVState.CurrState == FOVDistState.close)
                    {
                        if(_sm.GetFOVState.CurrState == FOVDistState.far)_sm.StopShooting();
                        _sm.SwitchState(_factory.AI_EngageState());
                        return;
                    }
                    if(_sm.GetFOVState.CurrState == FOVDistState.middle && _sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        _sm.SwitchState(_factory.AI_EngageState());
                        return;
                    }
                }
                if(_sm.GetFOVState.CurrState == FOVDistState.middle || _sm.GetFOVState.CurrState == FOVDistState.close)
                {
                    if(!_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        if(_sm.GetFOVMachine.ClosestEnemy != null)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                    }
                    else
                    {
                        _sm.StopShooting();
                    }
                }
                if(changeTimer <= 0)changeTimer = changeTimerMax * 0.5f;
            }
            else if(_sm.IsChecking)
            {
                _sm.EnemyIdentity.Aiming(true);
                _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.GetFOVMachine.ClosestEnemy.position, false);

                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();

                if(_sm.GetFOVState.CurrState == FOVDistState.middle || _sm.GetFOVState.CurrState == FOVDistState.close)
                {
                    if(!_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        if(_sm.GetFOVMachine.ClosestEnemy != null)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                    }
                    else
                    {
                        _sm.StopShooting();
                        ForceChangeHide();
                    }
                }
                if(_sm.GetFOVState.CurrState == FOVDistState.far)
                {
                    _sm.StopShooting();
                    if(_sm.IsAtTakingCoverCheckingPlace)
                    if(changeTimer <= 0)
                    {
                        ForceChangeHide();
                    }
                }

            }
            
        }
        if(_sm.GetFOVState.CurrState == FOVDistState.none || _sm.GetFOVMachine.ClosestEnemy == null)    
        {
            _sm.AimAIPointLookAt(null);
            Patroling();
            _sm.StopShooting();
            if(_sm.IsHiding)
            {
                if(_sm.IsAtTakingCoverHidingPlace)
                {
                    _sm.EnemyIdentity.Aiming(true);
                    _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenTakingCover, true);

                    if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.EnemyIdentity.Crouch(true);
                }
                if(changeTimer <= 0)
                {
                    if(!_sm.GetUseWeaponStateMachine.IsReloading)
                    {
                        _sm.IsHiding = !_sm.IsHiding;
                        _sm.IsChecking = !_sm.IsChecking;
                        _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;
                    }
                    changeTimer = changeTimerMax;
                }

            }
            else if(_sm.IsChecking)
            {
                float dotCharaWithEnemyLastSeen = 0;
                if(!_sm.isWallTallerThanChara)
                {
                    if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                    {
                        _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);

                        Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                        dotCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                    }
                    else
                    {
                        _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenChecking, true);
                        dotCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);
                    }


                    if(dotCharaWithEnemyLastSeen >= 0.95f)
                    {   
                        if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
                        else
                        {
                            //Checking lastseenpos, get wall dkk raaa
                        }

                        if(_sm.GetMoveStateMachine.IsCrouching)_sm.EnemyIdentity.Crouch(false);
                    }
                }
                else
                {
                    if(_sm.IsAtTakingCoverCheckingPlace)
                    {
                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {
                            _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                            Vector3 dirEnemyLastSeenToCharas = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                            dotCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToCharas, _sm.transform.forward);

                        }
                        else
                        {
                            _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenChecking, true);
                            dotCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);

                        }
                        


                        if(dotCharaWithEnemyLastSeen >= 0.95f)
                        {   
                            if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
                            else
                            {
                                //Checking lastseenpos, get wall dkk raaa
                            }
                        }
                    }
                }
            
            }
        }

        // if(_sm.FOVMachine.ClosestEnemy == null)
        // {
        //     ChangeState();
        //     Patroling();
        //     _sm.StopShooting();
        //     // if(_sm.IsHiding)
        //     // {
        //     //     if(_sm.IsAtTakingCoverHidingPlace)
        //     //     {
        //     //         Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M HIDING AND IDLE AND LOOKING AT TAKE COVER DIR" + _sm.transform.name);
                    
        //     //         _sm.GetPlayableCharaIdentity.Aiming(true);
        //     //         _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenTakingCover, true);

        //     //         if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.GetPlayableCharaIdentity.Crouch(true);
        //     //     }
        //     // }
        //     // else if(_sm.IsChecking)
        //     // {
        //     //     float dotCharaWithEnemyLastSeen = 0;
        //     //     if(!_sm.isWallTallerThanChara)
        //     //     {
        //     //         Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS NOT TALLER THAN ME" + _sm.transform.name);
        //     //         _sm.GetMoveStateMachine.IsRunning = false;

        //     //         if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
        //     //         {
        //     //             Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING ENEMYLASTPOS" + _sm.transform.name);
        //     //             _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);

        //     //             Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
        //     //             dotCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
        //     //             // Debug.Log("Muncul di sini" + dirEnemyLastSeenToChara + " xx" + _sm.transform.name);
        //     //         }
        //     //         else
        //     //         {

        //     //             _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenChecking, true);
        //     //             dotCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);
        //     //             Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING NORMALPOS" + _sm.transform.name);
        //     //         }


        //     //         if(dotCharaWithEnemyLastSeen >= 0.95f)
        //     //         {   
        //     //             if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
        //     //             {

        //     //                 if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
        //     //                 else
        //     //                 {
        //     //                     Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M DONE CHECKING POS" + _sm.transform.name);
        //     //                     _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
        //     //                 }
        //     //             }

        //     //             if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetPlayableCharaIdentity.Crouch(false);
        //     //         }
        //     //     }
        //     //     else
        //     //     {
        //     //         Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME" + _sm.transform.name);
        //     //         if(_sm.IsAtTakingCoverCheckingPlace)
        //     //         {
        //     //             _sm.GetMoveStateMachine.IsRunning = false;

        //     //             if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
        //     //             {
        //     //                 Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M CHECKING ENEMYLASTPOS" + _sm.transform.name);
        //     //                 _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
        //     //                 Vector3 dirEnemyLastSeenToCharas = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
        //     //                 dotCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToCharas, _sm.transform.forward);

        //     //             }
        //     //             else
        //     //             {
        //     //                 Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M CHECKING NORMALPOS" + _sm.transform.name);
        //     //                 _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenChecking, true);
        //     //                 dotCharaWithEnemyLastSeen = Vector3.Dot(_sm.DirToLookAtWhenChecking, _sm.transform.forward);

        //     //             }
                        


        //     //             if(dotCharaWithEnemyLastSeen >= 0.95f)
        //     //             {   
        //     //                 if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
        //     //                 {

        //     //                     if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
        //     //                     else
        //     //                     {
        //     //                         Debug.Log("T-NO ONE SEE ME; I SEE NO ONE - I'M CHECKING WALL IS TALLER THAN ME - I'M DONE CHECKING POS" + _sm.transform.name);
        //     //                         _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
        //     //                     }
        //     //                 }
        //     //             }
        //     //         }
        //     //     }
        //     // }
        // }
        

        if(_sm.HidingCheckDelayTimer > 0 && _sm.IsAtTakingCoverHidingPlace && ((!_sm.isWallTallerThanChara && _sm.IsCrouchingBehindWall()) || _sm.isWallTallerThanChara)) _sm.HidingCheckDelayTimer -= Time.deltaTime;
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
        _sm.IsTakingCover = false;
        _sm.IsAtTakingCoverHidingPlace = false;
        _sm.IsAtTakingCoverCheckingPlace = false;
        _sm.IsHiding = false;
        _sm.IsChecking = false;
    }
    private void ChangeTimerCounter()
    {
        if(changeTimer > 0 && _sm.GetMoveStateMachine.IsIdle)changeTimer -=Time.deltaTime;
        Debug.Log("Change Timer Now" + changeTimer + " aaaaa" + _sm.name);
    }
    private void ChangeState()
    {
        if(changeTimer <= 0)
        {
            // Debug.Log("Change now" + _sm.transform.name);
            if(_sm.IsHiding)
            {
                // if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower && !_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    _sm.IsHiding = !_sm.IsHiding;
                    _sm.IsChecking = !_sm.IsChecking;
                }
            }
            changeTimer = changeTimerMax;
        }
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
    private void ForceChangeHide()
    {
        _sm.IsHiding = !_sm.IsHiding;
        _sm.IsChecking = !_sm.IsChecking;
        changeTimer = changeTimerMax;
        if(_sm.IsHiding)
        {
            _sm.EnemyWhoSawAIList.Clear();
            _sm.HidingCheckDelayTimer = _sm.GetFOVMachine.FindDelayTimerNow;
        }
        Patroling();
        if(_sm.IsHiding && !_sm.isWallTallerThanChara)
        {
            _sm.EnemyIdentity.Aiming(true);
            _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenTakingCover, true);

            // if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching && _sm.GetMoveStateMachine.IsIdle)_sm.GetPlayableCharaIdentity.Crouch(true);
            if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching)_sm.EnemyIdentity.Crouch(true);   
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI_EngageState : EnemyAIState
{
    public EnemyAI_EngageState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _sm.IsAIEngage = true;
        _sm.IsCheckingEnemyInHunt = false;
        _sm.EnemyIdentity.Aiming(true);
    }

    public override void UpdateState()
    {
        if(_sm.AlertValue < _sm.MaxAlertValue / 2 || _sm.IsCharacterDead || _sm.EnemyIdentity.IsSilentKilled)
        {
            _sm.SwitchState(_factory.AI_IdleState());
            return;
        }
        else if(_sm.AlertValue >= _sm.MaxAlertValue / 2 && _sm.AlertValue < _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_HuntedState());
            return;
        }

        _sm.EnemyAIManager.OnEnemyisEngaging?.Invoke();
        _sm.GetFOVState.FOVStateHandler();
        if(_sm.IsTakingCover && _sm.IsAtTakingCoverHidingPlace)
        {
            Debug.Log("Taking cover now" + _sm.transform.name);
            // _sm.IsTakingCover = false;
            _sm.EnemyAIManager.EditEnemyCaptainList(_sm, false);
            _sm.SwitchState(_factory.AI_TakingCoverState());
            return;
            
        }

        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            // _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;
            _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
            _sm.EnemyIdentity.Aiming(true);
            _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.ClosestEnemy.position, false);
            if(_sm.GetFOVState.CurrState == FOVDistState.far)
            {
                if(_sm.GotDetectedbyEnemy && _sm.IsThePersonImLookingAlsoSeeMe(_sm.GetFOVMachine.ClosestEnemy))
                {
                    _sm.TakingCover();
                    if(_sm.CanTakeCoverInThePosition)
                    {

                        _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                        _sm.IsTakingCover = true;
                    }
                    else
                    {
                        _sm.IsTakingCover = false;
                        _sm.RunningTowardsEnemy();
                    }
                }
                else
                {
                    if(!_sm.IsTakingCover)
                    {
                        _sm.RunningTowardsEnemy();
                    }
                    
                }
                // _sm.RunningTowardsEnemy();
                _sm.StopShooting();

                // if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                
            }
            else if(_sm.GetFOVState.CurrState == FOVDistState.middle || _sm.GetFOVState.CurrState == FOVDistState.close)
            {
                

                // _sm.IsTakingCover = false;
                //kalo masuk sini pasti ada enemy, kalo ga pasti jd none || aturan d bwh jaga jaga biar ga error
                if(!_sm.GetUseWeaponStateMachine.IsReloading)
                {
                    if(_sm.GetFOVMachine.ClosestEnemy != null)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                }
                else
                {
                    // _sm.GetPlayableCharaIdentity.Run();
                    _sm.StopShooting();
                }
                if(!_sm.IsTakingCover)
                {
                    if(_sm.GetFOVAdvancedData.IsAlreadyAtMiddleSafeDistance())
                    {
                        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving(); //Stop gerak
                    }
                    else
                    {
                        if(_sm.LeaveDirection != Vector3.zero)
                        {
                            _sm.RunAwayOption();
                        }
                        else
                        {
                            Vector3 runAwayPos = _sm.transform.position - _sm.transform.forward * 2f;
                            _sm.GetMoveStateMachine.SetAIDirection(runAwayPos);
                        }
                    }
                }
                else
                {
                    if(_sm.GotDetectedbyEnemy && _sm.IsThePersonImLookingAlsoSeeMe(_sm.GetFOVMachine.ClosestEnemy))
                    {
                        _sm.TakingCover();
                        if(_sm.CanTakeCoverInThePosition)
                        {

                            _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                            _sm.IsTakingCover = true;
                        }
                        else
                        {
                            _sm.IsTakingCover = false;
                        }
                    }
                }
                // Debug.Log("pew pew pew");
            }
        }
        if(_sm.GetFOVState.CurrState == FOVDistState.none || _sm.GetFOVMachine.ClosestEnemy == null)    
        {
            _sm.AimAIPointLookAt(null);
            if(_sm.IsTakingCover)
            {
                _sm.EnemyIdentity.Aiming(true);
                Vector3 facedir = (_sm.Agent.steeringTarget - _sm.transform.position).normalized;
                _sm.SetAllowLookTarget(true, -facedir, true);
                _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
            }
            else
            {
                if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                WhenEnemyMissingInEngage();
            }
        }

        if(_sm.GetFOVAdvancedData.HasToCheckEnemyLastSeenPosition) //meaning visible targets or other visible ada
        {
            _sm.EnemyAIManager.OnCaptainsStartEngaging?.Invoke(_sm);
        }
        else
        {
            _sm.EnemyAIManager.EditEnemyCaptainList(_sm, false);
        }
        
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;

        _sm.StopShooting();
        _sm.IsAIEngage = false;
    }
    private void WhenEnemyMissingInEngage()
    {
        _sm.StopShooting();
        _sm.RunningToEnemyLastPosition();
    }

}

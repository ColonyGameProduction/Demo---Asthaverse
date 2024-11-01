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
        
        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
    }

    public override void UpdateState()
    {
        if(_sm.AlertValue < _sm.MaxAlertValue / 2 || _sm.IsCharacterDead || _sm.EnemyIdentity.IsSilentKilled)
        {
            _sm.SwitchState(_factory.AI_IdleState());
        }
        else if(_sm.AlertValue >= _sm.MaxAlertValue / 2 && _sm.AlertValue < _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_HuntedState());
        }

        _sm.EnemyAIManager.OnEnemyisEngaging?.Invoke();
        _sm.GetFOVState.FOVStateHandler();

        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            if(_sm.GetFOVState.CurrState == FOVDistState.far)
            {
                _sm.RunningTowardsEnemy();
                StopShooting();
                if(_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = false;
                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
            }
            else if(_sm.GetFOVState.CurrState == FOVDistState.middle || _sm.GetFOVState.CurrState == FOVDistState.close)
            {
                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving(); //Stop gerak

                _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
                if(!_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = true;
                //kalo masuk sini pasti ada enemy, kalo ga pasti jd none || aturan d bwh jaga jaga biar ga error
                if(_sm.GetFOVMachine.ClosestEnemy != null)StartShooting();
                // Debug.Log("pew pew pew");
            }
        }
        if(_sm.GetFOVState.CurrState == FOVDistState.none || _sm.GetFOVMachine.ClosestEnemy == null)
        {
            if(_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = false;
            WhenEnemyMissingInEngage();
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
        if(_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = false;

        if(_sm.GetUseWeaponStateMachine.ChosenTarget != null)_sm.GetUseWeaponStateMachine.GiveChosenTarget(null);
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = false;
        _sm.IsAIEngage = false;
    }
    private void WhenEnemyMissingInEngage()
    {
        StopShooting();
        _sm.RunningToEnemyLastPosition();
    }
    private void StartShooting()
    {
        // Debug.Log("shoot di 2" + _sm.GetFOVMachine.ClosestEnemy.position);
        _sm.GetUseWeaponStateMachine.GiveChosenTarget(_sm.GetFOVMachine.ClosestEnemy);
        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
        if(!_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = true;
    }
    private void StopShooting()
    {
        if(_sm.GetUseWeaponStateMachine.ChosenTarget != null)_sm.GetUseWeaponStateMachine.GiveChosenTarget(null);
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon)_sm.GetUseWeaponStateMachine.IsUsingWeapon = false;
    }
}

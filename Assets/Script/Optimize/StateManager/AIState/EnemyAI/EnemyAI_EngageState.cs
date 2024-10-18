using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI_EngageState : EnemyAIState
{
    public EnemyAI_EngageState(EnemyAIBehaviourStateMachine stateMachine, EnemyAIStateFactory factory) : base(stateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _stateMachine.IsAIEngage = true;
        if(!_stateMachine.GetUseWeaponStateMachine.IsAiming)_stateMachine.GetUseWeaponStateMachine.IsAiming = true;
    }

    public override void UpdateState()
    {
        if(_stateMachine.AlertValue < _stateMachine.MaxAlertValue / 2 || _stateMachine.IsCharacterDead)
        {
            _stateMachine.SwitchState(_factory.AI_IdleState());
        }
        else if(_stateMachine.AlertValue >= _stateMachine.MaxAlertValue / 2 && _stateMachine.AlertValue < _stateMachine.MaxAlertValue)
        {
            _stateMachine.SwitchState(_factory.AI_HuntedState());
        }

        _stateMachine.GetFOVState.FOVStateHandler();

        if(_stateMachine.GetFOVState.CurrState != FOVDistState.none)
        {
            if(_stateMachine.GetFOVState.CurrState == FOVDistState.far)
            {
                // _stateMachine.RunningTowardsEnemy();
                StopShooting();
                if(_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = false;
                _stateMachine.GetMoveStateMachine.GiveAIDirection(_stateMachine.GetFOVMachine.ClosestEnemy.position);
            }
            else if(_stateMachine.GetFOVState.CurrState == FOVDistState.middle || _stateMachine.GetFOVState.CurrState == FOVDistState.close)
            {
                if(_stateMachine.GetMoveStateMachine.CurrAIDirPos != _stateMachine.transform.position)_stateMachine.GetMoveStateMachine.ForceStopMoving(); //Stop gerak

                _stateMachine.GetMoveStateMachine.GiveAIPlaceToLook(_stateMachine.GetFOVMachine.ClosestEnemy.position);
                if(!_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = true;
                //kalo masuk sini pasti ada enemy, kalo ga pasti jd none || aturan d bwh jaga jaga biar ga error
                // if(_stateMachine.GetFOVMachine.ClosestEnemy != null)StartShooting();

            }
        }
        if(_stateMachine.GetFOVState.CurrState == FOVDistState.none || _stateMachine.GetFOVMachine.ClosestEnemy == null)
        {
            if(_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = false;
            WhenEnemyMissingInEngage();
        }
        
    }
    public override void ExitState()
    {
        if(_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = false;

        if(_stateMachine.GetUseWeaponStateMachine.ChosenTarget != null)_stateMachine.GetUseWeaponStateMachine.GiveChosenTarget(null);
        if(_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon)_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon = false;
        _stateMachine.IsAIEngage = false;
    }
    private void WhenEnemyMissingInEngage()
    {
        StopShooting();
        _stateMachine.GetFOVAdvancedData.GetClosestBreadCrumbs();
        if(_stateMachine.GetFOVAdvancedData.ClosestBreadCrumbs != null)
        {
            _stateMachine.GetMoveStateMachine.GiveAIDirection(_stateMachine.GetFOVAdvancedData.ClosestBreadCrumbs.position);
        }
        else
        {
            if(_stateMachine.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
            {
                // _stateMachine.GetFOVMachine.IHaveCheckEnemyLastPosition();
                _stateMachine.GetMoveStateMachine.GiveAIDirection(_stateMachine.GetFOVMachine.EnemyCharalastSeenPosition);
                Debug.Log("wehn enggage but no person");
                // if(Vector3.Distance(_stateMachine.transform.position, _stateMachine.GetFOVMachine.EnemyCharalastSeenPosition) < 0.5f)
                // {
                //     _stateMachine.GetFOVMachine.IHaveCheckEnemyLastPosition();
                //     _stateMachine.GetMoveStateMachine.ForceStopMoving();
                // }
            }
            
        }
    }
    private void StopShooting()
    {
        if(_stateMachine.GetUseWeaponStateMachine.ChosenTarget != null)_stateMachine.GetUseWeaponStateMachine.GiveChosenTarget(null);
        if(_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon)_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon = false;
    }
    private void StartShooting()
    {
        Debug.Log("shoot di 2" + _stateMachine.GetFOVMachine.ClosestEnemy.position);
        _stateMachine.GetUseWeaponStateMachine.GiveChosenTarget(_stateMachine.GetFOVMachine.ClosestEnemy);
        if(!_stateMachine.GetUseWeaponStateMachine.IsAiming)_stateMachine.GetUseWeaponStateMachine.IsAiming = true;
        if(!_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon)_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon = true;
    }
}

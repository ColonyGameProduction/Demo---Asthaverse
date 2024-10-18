using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_HuntedState : EnemyAIState
{
    public EnemyAI_HuntedState(EnemyAIBehaviourStateMachine stateMachine, EnemyAIStateFactory factory) : base(stateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _stateMachine.IsAIHunted = true;
        // if(_stateMachine.GetUseWeaponStateMachine.)_stateMachine.GetUseWeaponStateMachine.ForceStopUseWeapon();
        if(!_stateMachine.GetUseWeaponStateMachine.IsAiming)_stateMachine.GetUseWeaponStateMachine.IsAiming = true;
        
    }

    public override void UpdateState()
    {
        if(_stateMachine.AlertValue < _stateMachine.MaxAlertValue / 2 || _stateMachine.IsCharacterDead)
        {
            _stateMachine.SwitchState(_factory.AI_IdleState());
        }
        else if(_stateMachine.AlertValue >= _stateMachine.MaxAlertValue)
        {
            _stateMachine.SwitchState(_factory.AI_EngageState());
        }

        _stateMachine.GetFOVMachine.GetClosestEnemy();
        _stateMachine.RunningTowardsEnemy();


    }
    public override void ExitState()
    {
        _stateMachine.IsAIHunted = false;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_HuntedState : EnemyAIState
{
    float tempAlertValue = 0;
    public EnemyAI_HuntedState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _sm.IsAIHunted = true;
        // if(_stateMachine.GetUseWeaponStateMachine.)_stateMachine.GetUseWeaponStateMachine.ForceStopUseWeapon();
        if(!_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = true;
        
    }

    public override void UpdateState()
    {
        _sm.GetFOVState.FOVStateHandler();
        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            if(_sm.GetFOVState.CurrState == FOVDistState.middle)
            {
                // _sm.MaxAlertValue *= 0.5f;
                tempAlertValue = _sm.MaxAlertValue*0.5f;
                if(_sm.AlertValue < tempAlertValue) _sm.AlertValue = tempAlertValue + 10f;
            }
            else if(_sm.GetFOVState.CurrState == FOVDistState.close)
            {
                // _sm.MaxAlertValue = 0f;
                tempAlertValue = _sm.MaxAlertValue;
                if(_sm.AlertValue < tempAlertValue) _sm.AlertValue = tempAlertValue + 10f;
            }
            // Debug.Log("HALOOO");
        }

        if(_sm.AlertValue < _sm.MaxAlertValue / 2 || _sm.IsCharacterDead)
        {
            _sm.SwitchState(_factory.AI_IdleState());
        }
        else if(_sm.AlertValue >= _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_EngageState());
        }



        // _sm.GetFOVMachine.GetClosestEnemy();
        _sm.RunningTowardsEnemy();


    }
    public override void ExitState()
    {
        _sm.IsAIHunted = false;
    }
    
    
}

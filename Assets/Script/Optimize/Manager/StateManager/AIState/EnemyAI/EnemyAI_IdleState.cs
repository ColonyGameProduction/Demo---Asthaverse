using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_IdleState : EnemyAIState
{
    public EnemyAI_IdleState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _sm.IsAIIdle = true;
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
    }

    public override void UpdateState()
    {
        if(_sm.IsCharacterDead)
        {
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
            return;
        }

        _sm.GetFOVState.FOVStateHandler();
        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position);
            if(!_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = true;
            
            if(_sm.GetFOVState.CurrState == FOVDistState.middle)
            {
                _sm.AlertValue = _sm.MaxAlertValue*0.5f + 10;
            }
            else if(_sm.GetFOVState.CurrState == FOVDistState.close)
            {
                _sm.AlertValue = _sm.MaxAlertValue + 10;
            }
        }

        if(_sm.AlertValue >= _sm.MaxAlertValue / 2 && _sm.AlertValue < _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_HuntedState());
        }
        else if(_sm.AlertValue >= _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_EngageState());
        }

        if(_sm.GetFOVState.CurrState == FOVDistState.none) //no person
        {
            if(_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = false;
            Patrol();
        }
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.AllowLookTargetWhileIdle)_sm.GetMoveStateMachine.AllowLookTargetWhileIdle = false;
        // else 

        _sm.IsAIIdle = false;
    }

    public void Patrol()
    {
        if(_sm.GetMoveStateMachine.IsRunning) _sm.GetMoveStateMachine.IsRunning = false;
        if (_sm.PatrolPath.Length > 1)
        {
            _sm.GetMoveStateMachine.SetAIDirection(_sm.PatrolPath[_sm.CurrPath].position);
        }
        else
        {
            _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
        }
    }
    
}

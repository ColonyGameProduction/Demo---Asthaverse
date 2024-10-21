using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_IdleState : EnemyAIState
{
    public EnemyAI_IdleState(EnemyAIBehaviourStateMachine stateMachine, EnemyAIStateFactory factory) : base(stateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _stateMachine.IsAIIdle = true;
        if(_stateMachine.GetUseWeaponStateMachine.IsUsingWeapon)_stateMachine.GetUseWeaponStateMachine.ForceStopUseWeapon();
    }

    public override void UpdateState()
    {
        if(_stateMachine.IsCharacterDead)
        {
            if(_stateMachine.GetMoveStateMachine.CurrAIDirPos != _stateMachine.transform.position)_stateMachine.GetMoveStateMachine.ForceStopMoving();
            return;
        }

        _stateMachine.GetFOVState.FOVStateHandler();
        if(_stateMachine.GetFOVState.CurrState != FOVDistState.none)
        {
            // if(_stateMachine.GetFOVMachine.VisibleTargets.Count > 0)
            // {
                //do we need to check closest enemy again?
                if(_stateMachine.GetMoveStateMachine.CurrAIDirPos != _stateMachine.transform.position)_stateMachine.GetMoveStateMachine.ForceStopMoving();
                _stateMachine.GetMoveStateMachine.GiveAIPlaceToLook(_stateMachine.GetFOVMachine.ClosestEnemy.position);
                if(!_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = true;
            
            // }
            if(_stateMachine.GetFOVState.CurrState == FOVDistState.middle)
            {
                // _stateMachine.MaxAlertValue *= 0.5f;
                _stateMachine.AlertValue = _stateMachine.MaxAlertValue*0.5f + 10;
            }
            else if(_stateMachine.GetFOVState.CurrState == FOVDistState.close)
            {
                // _stateMachine.MaxAlertValue = 0f;
                _stateMachine.AlertValue = _stateMachine.MaxAlertValue + 10;
            }
            // Debug.Log("HALOOO");
        }

        if(_stateMachine.AlertValue >= _stateMachine.MaxAlertValue / 2 && _stateMachine.AlertValue < _stateMachine.MaxAlertValue)
        {
            _stateMachine.SwitchState(_factory.AI_HuntedState());
        }
        else if(_stateMachine.AlertValue >= _stateMachine.MaxAlertValue)
        {
            _stateMachine.SwitchState(_factory.AI_EngageState());
        }

        if(_stateMachine.GetFOVState.CurrState == FOVDistState.none) //no person
        {
            if(_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = false;
            Patrol();
        }
    }
    public override void ExitState()
    {
        if(_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle)_stateMachine.GetMoveStateMachine.AskAIToLookWhileIdle = false;

        _stateMachine.IsAIIdle = false;
    }

    public void Patrol()
    {
        if(_stateMachine.GetMoveStateMachine.IsRunning) _stateMachine.GetMoveStateMachine.IsRunning = false;
        if (_stateMachine.PatrolPath.Length > 1)
        {
            _stateMachine.GetMoveStateMachine.GiveAIDirection(_stateMachine.PatrolPath[_stateMachine.CurrPath].position);
            
        }
        else
        {
            _stateMachine.GetMoveStateMachine.GiveAIDirection(_stateMachine.transform.position);
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crouch
/// </summary>
public class CrouchState : MovementState
{
    public CrouchState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {

    }
    public override void EnterState()
    {
        // base.EnterState(); // Jalankan animasi
        Debug.Log("Crouching");
        _stateMachine.ChangeCurrSpeed(_crouch.CrouchSpeed);
    }
    public override void UpdateState()
    {
        if((!_stateMachine.isAI && _playableData.InputMovement != Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection != null))
        {
            if(_stateMachine.isAI)_stateMachine.Move();
            if(!_crouch.IsCrouching)
            {
                if(_standMovement.IsRunning)
                {
                    _stateMachine.SwitchState(_factory.RunState());
                }
                else _stateMachine.SwitchState(_factory.WalkState());
            }
        }
        else if((!_stateMachine.isAI && _playableData.InputMovement == Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection == null))
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExiState()
    {
        // if(!_crouch.IsCrouching) //Matikan state animasi crouch
    }
    public override void PhysicsLogicUpdateState()
    {
        if(!_stateMachine.isAI)_stateMachine.Move();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Walk
/// </summary>
public class WalkState : MovementState
{
    public WalkState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        // StateAnimationName = "IdleAnimation";
    }
    public override void EnterState()
    {
        // base.EnterState(); // Jalankan animasi
        Debug.Log("Walking");
        _stateMachine.ChangeCurrSpeed(_stateMachine.WalkSpeed);
    }
    public override void UpdateState()
    {
        
        if((!_stateMachine.isAI && _playableData.InputMovement != Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection != null))
        {
            if(_stateMachine.isAI)_stateMachine.Move();
            if(_crouch != null && _crouch.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState());
            else if(_standMovement.IsRunning)_stateMachine.SwitchState(_factory.RunState());

        }
        else if((!_stateMachine.isAI && _playableData.InputMovement == Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection == null))
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExiState()
    {
        // base.EnterState(); // Matikan animasi
    }
    public override void PhysicsLogicUpdateState()
    {
        if(!_stateMachine.isAI)_stateMachine.Move();
    }
}

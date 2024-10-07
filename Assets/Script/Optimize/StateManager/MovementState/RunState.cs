using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Run
/// </summary>
public class RunState : MovementState
{    
    public RunState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        // StateAnimationName = "IdleAnimation";
    }
    public override void EnterState()
    {
        // base.EnterState(); // Jalankan animasi
        Debug.Log("Running");
        _stateMachine.ChangeCurrSpeed(_stateMachine.RunSpeed);
    }
    public override void UpdateState()
    {
        if((!_stateMachine.isAI && _playableData.InputMovement != Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection != null))
        {
            if(_stateMachine.isAI)_stateMachine.Move();
            if(!_standMovement.IsRunning)
            {
                if(_crouch != null && _crouch.IsCrouching) _stateMachine.SwitchState(_factory.CrouchState());
                else
                {
                    _stateMachine.SwitchState(_factory.WalkState());
                }
            }
        
        }
        else if((!_stateMachine.isAI && _playableData.InputMovement == Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection == null))
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExiState()
    {

        // //Matikan state animasi Run -> mau state tombol masih true pun tetep matiin krn ga ada animasi idle d run
    }
    public override void PhysicsLogicUpdateState()
    {
        if(!_stateMachine.isAI)_stateMachine.Move();
    }
}

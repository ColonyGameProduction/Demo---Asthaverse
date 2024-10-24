using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Walk, menjalankan karakter dan animasi karakter jalan
/// </summary>
public class WalkState : MovementState
{
    public WalkState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory) => _activeStateAnimParamName = "Walk";
    public override void EnterState()
    {
        base.EnterState();

        _standData.IsWalking = true;

        _sm.ChangeCurrSpeed(_sm.WalkSpeed);
    }
    public override void UpdateState()
    {
        if((!_sm.IsAIInput && _playableData.InputMovement != Vector3.zero) || (_sm.IsAIInput && !_sm.IsAIAtDirPos()))
        {
            if(_sm.IsAIInput)_sm.Move();
            CheckStateWhileMoving();

        }
        else if((!_sm.IsAIInput && _playableData.InputMovement == Vector3.zero) || (_sm.IsAIInput && _sm.IsAIAtDirPos()))
        {
            _sm.SwitchState(_factory.IdleState());
        }
    }
    public override void ExitState()
    {
        _standData.IsWalking = false;
        base.ExitState();
    }
    public override void PhysicsLogicUpdateState()
    {
        if(!_sm.IsAIInput)_sm.Move();
    }

    protected virtual void CheckStateWhileMoving()
    {
        if(_groundData != null && _groundData.IsCrouching)_sm.SwitchState(_factory.CrouchState());
        else if(_standData.IsRunning)_sm.SwitchState(_factory.RunState());
    }
}

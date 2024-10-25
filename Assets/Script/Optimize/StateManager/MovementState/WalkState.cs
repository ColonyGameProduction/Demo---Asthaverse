using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Walk, menjalankan karakter dan animasi karakter jalan
/// </summary>
public class WalkState : MovementState
{
    public WalkState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        StateAnimationName = "Walk";
    }
    public override void EnterState()
    {
        
        base.EnterState();

        _standMovement.IsWalking = true;

        //Menganti kecepatan
        _stateMachine.ChangeCurrSpeed(_stateMachine.WalkSpeed);
    }
    public override void UpdateState()
    {
        //Menunggu logika lain yang dapat mengubah statenya sembari melakukan pergerakan, untuk AI ditaro di update state, untuk Input player di physisc logic atau fixed update
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            if(!_stateMachine.IsInputPlayer)_stateMachine.Move();
            if(_groundMovement != null && _groundMovement.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState());
            else if(_standMovement.IsRunning)_stateMachine.SwitchState(_factory.RunState());

        }
        else if((_stateMachine.IsInputPlayer && _playableData.InputMovement == Vector3.zero) || (!_stateMachine.IsInputPlayer && _stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExitState()
    {
        _standMovement.IsWalking = false;
        base.ExitState();
    }
    public override void PhysicsLogicUpdateState()
    {
        if(_stateMachine.IsInputPlayer)_stateMachine.Move();
    }
}

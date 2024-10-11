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
        // StateAnimationName = "Crouch";
        StateAnimationName = "Move";
    }
    public override void EnterState()
    {
        base.EnterState(); // Jalankan animasi
        Debug.Log("Crouching" + _stateMachine.gameObject.name);
        _stateMachine.ChangeCurrSpeed(_crouch.CrouchSpeed);

        //mungkin di sini bisa ditambah kalau masuknya zero atau masih idle dan iscrouching false, maka animasi dimatikan trus lsg ke exit
    }
    public override void UpdateState()
    {
        //sama seperti walk dkk
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            if(!_stateMachine.IsInputPlayer)_stateMachine.Move();
            if(!_crouch.IsCrouching)
            {
                if(_standMovement.IsRunning)
                {
                    _stateMachine.SwitchState(_factory.RunState());
                }
                else _stateMachine.SwitchState(_factory.WalkState());
            }
        }
        else if((_stateMachine.IsInputPlayer && _playableData.InputMovement == Vector3.zero) || (!_stateMachine.IsInputPlayer && _stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExiState()
    {
        // if(!_crouch.IsCrouching) //Matikan state animasi crouch
        base.ExiState();
    }
    public override void PhysicsLogicUpdateState()
    {
        if(_stateMachine.IsInputPlayer)_stateMachine.Move();
    }
}

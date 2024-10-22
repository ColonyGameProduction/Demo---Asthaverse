using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Run
/// </summary>
public class RunState : MovementState
{    
    public RunState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        // StateAnimationName = "Sprint";
        StateAnimationName = "Move";
    }
    public override void EnterState()
    {
        base.EnterState(); 
        // base.EnterState(); // Jalankan animasi
        Debug.Log("Running" + _stateMachine.gameObject.name);

        //Mengganti kecepatan
        _stateMachine.ChangeCurrSpeed(_stateMachine.RunSpeed);

        //Harus membuat semua state aim di aim manager, false, dn balik ke posisi not aiming
    }
    public override void UpdateState()
    {
        //Menunggu syarat
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            if(!_stateMachine.IsInputPlayer)_stateMachine.Move();
            if(!_standMovement.IsRunning)
            {
                if(_groundMovement != null && _groundMovement.IsCrouching) _stateMachine.SwitchState(_factory.CrouchState());
                else
                {
                    _stateMachine.SwitchState(_factory.WalkState());
                }
            }
        
        }
        else if((_stateMachine.IsInputPlayer && _playableData.InputMovement == Vector3.zero) || (!_stateMachine.IsInputPlayer && _stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExitState()
    {
        base.ExitState();
        // //Matikan state animasi Run -> mau state tombol masih true pun tetep matiin krn ga ada animasi idle d run
    }
    public override void PhysicsLogicUpdateState()
    {
        if(_stateMachine.IsInputPlayer)_stateMachine.Move();
    }
}

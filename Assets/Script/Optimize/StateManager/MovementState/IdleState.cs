using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle
/// </summary>
public class IdleState : MovementState
{
    
    
    public IdleState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        StateAnimationName = "IdleAnimation";
    }
    public override void EnterState()
    {
        Debug.Log("Idle");
        _stateMachine.CharaAnimator.SetFloat("Horizontal", 0);
        _stateMachine.CharaAnimator.SetFloat("Vertical", 0);
    }
    public override void UpdateState()
    {

        if((!_stateMachine.isAI && _playableData.InputMovement != Vector3.zero) || (_stateMachine.isAI && _stateMachine.CurrAIDirection != null))
        {
            if(_crouch != null && _crouch.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState());
            else if(_standMovement.IsRunning)_stateMachine.SwitchState(_factory.RunState());
            else _stateMachine.SwitchState(_factory.WalkState());

        }

        //Ini emang sudah diam di tmpt
        ///Urusan animasi aja, soalnya kek kondisi crouch kan dia msh bs idle
        ///
        //Cek animasinya msh jalan ga, kalo ga jalan gausa pindah ke sana
        // if(_crouch != null)
        // {
        //     if(!_crouch.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState()); //lwt bntr utk stop animasi
        // }

        
    }
    public override void ExiState()
    {
        // base.EnterState(); //Stop Idle Anim
    }
    public override void PhysicsLogicUpdateState()
    {
        // throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crouch
/// </summary>
public class CrouchState : WalkState
{
    public CrouchState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory) => _activeStateAnimParamName = "Crouch";
    public override void EnterState()
    {
        SetAnimParamActive(_activeStateAnimParamName);
        
        _sm.ChangeCurrSpeed(_standData.CrouchSpeed);
    }

    public override void ExitState()
    {
        if(_sm.IsCharacterDead) _standData.IsCrouching = false;
        
        if(!_standData.IsCrouching) base.ExitState();
    }

    protected override void CheckStateWhileMoving()
    {
        if(!_standData.IsCrouching)
        {
            if(_standData.IsRunning) _sm.SwitchState(_factory.RunState());
            else _sm.SwitchState(_factory.WalkState());
        }
    }
}

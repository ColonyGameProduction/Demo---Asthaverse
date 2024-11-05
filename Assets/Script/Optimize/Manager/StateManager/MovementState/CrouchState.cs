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
        base.EnterState();
        
        _sm.ChangeCurrSpeed(_groundData.CrouchSpeed);
    }

    public override void ExitState()
    {
        if(_sm.IsCharacterDead) _groundData.IsCrouching = false;
        
        if(!_groundData.IsCrouching) base.ExitState();
    }

    protected override void CheckStateWhileMoving()
    {
        if(!_groundData.IsCrouching)
        {
            if(_standData.IsRunning) _sm.SwitchState(_factory.RunState());
            else _sm.SwitchState(_factory.WalkState());
        }
    }
}

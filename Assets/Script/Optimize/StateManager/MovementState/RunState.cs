using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Run
/// </summary>
public class RunState : WalkState
{    
    public RunState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory) =>_activeStateAnimParamName = "Run";
    public override void EnterState()
    {
        base.EnterState(); 

        _sm.ChangeCurrSpeed(_sm.RunSpeed);
    }
    public override void UpdateState()
    {
        base.UpdateState();
    }
    public override void ExitState()
    {
        base.ExitState();
        // //Matikan state animasi Run -> mau state tombol masih true pun tetep matiin krn ga ada animasi idle d run
    }
    public override void PhysicsLogicUpdateState()
    {
        if(!_sm.IsAIInput)_sm.Move();
    }
    protected override void CheckStateWhileMoving()
    {
        if(!_standData.IsRunning)
        {
            if(_groundData != null && _groundData.IsCrouching) _sm.SwitchState(_factory.CrouchState());
            else
            {
                _sm.SwitchState(_factory.WalkState());
            }
        }
    }
}

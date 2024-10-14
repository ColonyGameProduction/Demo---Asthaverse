using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement state -> semua state pergerakan: Idle, Walk, Run, Crouch
/// </summary>
public abstract class MovementState : CharacterBaseState<MovementStateMachine>
{
    protected MovementStateFactory _factory;
    protected IStandMovementData _standMovement;
    protected ICrouchMovementData _crouch;
    protected IPlayableMovementDataNeeded _playableData;
    public MovementState(MovementStateMachine stateMachine, MovementStateFactory factory) : base(stateMachine) 
    {
        _factory = factory;
        if(stateMachine is IStandMovementData s)
        {
            _standMovement = s;
        }
        if(stateMachine is ICrouchMovementData c)
        {
            _crouch = c;
        }
        if(stateMachine is IPlayableMovementDataNeeded m)
        {
            _playableData = m;
        }
    }
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
    public virtual void PhysicsLogicUpdateState(){}

}

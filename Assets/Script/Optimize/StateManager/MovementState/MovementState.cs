using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementState : CharacterBaseState<MovementStateMachine>
{
    protected MovementStateFactory _factory;
    protected IStandMovement _standMovement;
    protected ICrouch _crouch;
    protected IPlayableMovementDataNeeded _playableData;
    public MovementState(MovementStateMachine stateMachine, MovementStateFactory factory) : base(stateMachine) 
    {
        _factory = factory;
        if(stateMachine is IStandMovement s)
        {
            _standMovement = s;
        }
        if(stateMachine is ICrouch c)
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

    public override void ExiState()
    {
        base.EnterState();
    }
    public abstract void PhysicsLogicUpdateState();

}

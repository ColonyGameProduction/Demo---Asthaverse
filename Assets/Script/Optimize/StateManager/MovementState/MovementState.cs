using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement state -> semua state pergerakan: Idle, Walk, Run, Crouch
/// </summary>
public abstract class MovementState : CharacterBaseState<MovementStateMachine>
{
    [Header("To Get the data from statemachine class and child class in a group")]
    protected MovementStateFactory _factory;
    protected IStandMovementData _standMovement;
    protected IGroundMovementData _groundMovement;
    protected IPlayableMovementDataNeeded _playableData;

    protected const string ANIMATION_MOVE_PARAMETER_ISMOVING = "IsMoving";
    public MovementState(MovementStateMachine stateMachine, MovementStateFactory factory) : base(stateMachine) 
    {
        _factory = factory;
        if(stateMachine is IStandMovementData s)
        {
            _standMovement = s;
        }
        if(stateMachine is IGroundMovementData g)
        {
            _groundMovement = g;
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

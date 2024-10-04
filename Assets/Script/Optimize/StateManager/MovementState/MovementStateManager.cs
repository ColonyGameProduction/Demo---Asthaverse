using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : CharacterStateManager
{
    MovementState currState;
    IdleState idleState = new IdleState();
    WalkState walkState = new WalkState();
    RunState runState = new RunState();
    CrouchState crouchState= new CrouchState();
    [Header("Move Speed List - Each State")]
    [SerializeField]private float _walkSpeed;
    public float WalkSpeed{get {return _walkSpeed;}}
    [SerializeField]private float _runSpeed;
    public float RunSpeed{get {return _runSpeed;}}
    [SerializeField]private float _crouchSpeed;
    public float CrouchSpeed{get {return _crouchSpeed;}}

    private void Start() 
    {
        SwitchState(idleState);
    }
    private void Update() 
    {
        currState?.UpdateState(this);
    }
    public override void SwitchState<T>(BaseState<T> newState)
    {
        currState = newState as MovementState;
        currState?.EnterState(this);
    }

}

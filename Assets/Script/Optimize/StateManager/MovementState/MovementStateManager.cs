using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementStateManager : CharacterStateManager, IMovement
{
    MovementState currState;
    public IdleState idleState = new IdleState();
    public WalkState walkState = new WalkState();
    public RunState runState = new RunState();
    // public CrouchState crouchState= new CrouchState();
    [Header("Move Speed List - Each State")]
    [SerializeField]private float _walkSpeed;
    public float WalkSpeed{get {return _walkSpeed;}}
    [SerializeField]private float _runSpeed;
    public float RunSpeed{get {return _runSpeed;}}
    // [SerializeField]private float _crouchSpeed;
    // public float CrouchSpeed{get {return _crouchSpeed;}}

    private float currSpeed;

    [Header("AI Moving Controller")]
    [SerializeField]private NavMeshAgent agentNavMesh;

    protected override void Awake() 
    {
        base.Awake();
        if(agentNavMesh == null)agentNavMesh = GetComponent<NavMeshAgent>();
    }
    private void Start() 
    {
        currSpeed = WalkSpeed;
        SwitchState(idleState);
    }
    private void Update() 
    {
        currState?.UpdateState(this);
    }
    private void FixedUpdate() 
    {
        
    }
    public override void SwitchState<T>(BaseState<T> newState)
    {
        if(currState != null)
        {
            currState?.ExiState(this);
        }
        currState = newState as MovementState;
        currState?.EnterState(this);
    }
    /// <summary>
    /// if AI -> Get Destination, if Player -> Get Direction -> Tp krn ini purely buat AI jd d sini aja
    /// </summary>
    public void Move(Vector3 direction)
    {
        //if AI
        if(agentNavMesh.destination != direction)
        {
            agentNavMesh.destination = direction;
            SwitchState(walkState);
        }
    }
    public void ChangeCurrSpeed(float newSpeed)
    {
        currSpeed = newSpeed;
    }

}

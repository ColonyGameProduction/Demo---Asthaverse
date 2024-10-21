using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MovementStateMachine : CharacterStateMachine, IMovement, IStandMovementData
{
    #region Normal Variable

    [Space(1)]
    [Header("Move States - Stand")]
    [SerializeField] protected bool _isIdle;
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRun;
    protected MovementStateFactory _states;
    protected MovementState _currState;

    [Space(1)]
    [Header("NavMesh Component")]
    [SerializeField]protected NavMeshAgent _agentNavMesh;
    protected Vector3 _currAIDirPos;

    [Space(1)]
    [Header("Move Speed - State Multiplier")]
    [SerializeField] protected float _runMultiplier;
    protected float _walkSpeed;
    protected float _runSpeed;
    protected float _currSpeed;

    [Space(1)]
    [Header("AI Rotation")]
    protected Vector3 _posToLookAt;
    protected bool _askAIToLookWhileIdle;

    public Action<Vector3> OnIsTheSamePosition;

    #endregion

    #region CONST Variable
    //CONST
    public const string ANIMATION_MOVE_PARAMETER_HORIZONTAL = "Horizontal";
    public const string ANIMATION_MOVE_PARAMETER_VERTICAL = "Vertical";
    #endregion

    #region GETTERSETTER Variable
    //Getter Setter
    public virtual float WalkSpeed 
    { 
        get {return _walkSpeed;}
        set 
        {
            _walkSpeed = value;
            _runSpeed = _walkSpeed * _runMultiplier;
        }
    }
    public float RunSpeed { get {return _runSpeed;}}
    
    public bool IsIdle {get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsWalking {get {return _isWalking;}set{ _isWalking = value;} }
    public bool IsRunning { get {return _isRun;}set{ _isRun = value;} }

    public NavMeshAgent AgentNavMesh {get {return _agentNavMesh;}}
    public Vector3 CurrAIDirPos { get {return _currAIDirPos;}}
    public bool AskAIToLookWhileIdle {get {return _askAIToLookWhileIdle;} set{_askAIToLookWhileIdle = value;}}
    
    
    #endregion

    protected override void Awake() 
    {        
        base.Awake();

        _states = new MovementStateFactory(this);

        if(AgentNavMesh == null)_agentNavMesh = GetComponent<NavMeshAgent>();
    }
    private void Start() 
    {
        SwitchState(_states.IdleState());
    }
    protected virtual void Update() 
    {
        _currState?.UpdateState();
    }
    private void FixedUpdate() 
    {
        _currState?.PhysicsLogicUpdateState();
    }

    public override void SwitchState(BaseState newState)
    {
        if(_currState != null)
        {
            _currState?.ExitState();
        }
        _currState = newState as MovementState;
        _currState?.EnterState();
    }

    #region Move
    /// <summary>
    /// if AI -> Get Destination, if Player -> Get Direction -> Tp krn ini purely buat AI jd d sini aja
    /// </summary>
    
    public virtual void Move()
    {
        MoveAI(CurrAIDirPos);
    }
    //Function to make AI move based on direction
    protected void MoveAI(Vector3 direction)
    {
        if(AgentNavMesh.speed != _currSpeed)AgentNavMesh.speed = _currSpeed;

        Vector3 facedir = (AgentNavMesh.steeringTarget - transform.position).normalized;
        Vector3 animatedFaceDir = transform.InverseTransformDirection(facedir);

        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, animatedFaceDir.x);
        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, animatedFaceDir.z);

    }

    //function to check if ai should move or not
    public bool IsTargetTheSamePositionAsTransform()
    {
        if(AgentNavMesh.destination != CurrAIDirPos)
        {
            AgentNavMesh.destination = CurrAIDirPos;
        }
        // Debug.Log(AgentNavMesh.hasPath + " " + gameObject.name);
        if(!AgentNavMesh.hasPath)
        {
            OnIsTheSamePosition?.Invoke(CurrAIDirPos);
            return true;
        }
        else
        {
            if(Vector3.Distance(transform.position, AgentNavMesh.destination) < AgentNavMesh.radius)
            {

                AgentNavMesh.ResetPath();
                OnIsTheSamePosition?.Invoke(CurrAIDirPos);

                return true;
            }
        }

        return false;
    }

    public void IdleAI_RotateToEnemy()
    {
        Vector3 facedir = (_posToLookAt - transform.position).normalized;
        Quaternion rotateTo = Quaternion.LookRotation(facedir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), 180 * Time.deltaTime);
    }

    public virtual void ForceStopMoving()
    {
        ForceAIStopMoving();
    }
    private void ForceAIStopMoving()
    {
        AgentNavMesh.speed = 0;
        AgentNavMesh.ResetPath();

        _currAIDirPos = transform.position;
        AgentNavMesh.velocity = Vector3.zero;

        IsWalking = false;
        IsRunning = false;
        // CharaAnimator.SetBool("Scope", false);
    }

    public void GiveAIDirection(Vector3 newPos)
    {
        _currAIDirPos = newPos;
    }
    public void GiveAIPlaceToLook(Vector3 posToLook)
    {
        _posToLookAt = posToLook;
    }
    #endregion

    public void ChangeCurrSpeed(float newSpeed)
    {
        if(_currSpeed != newSpeed)_currSpeed = newSpeed;
    }
}

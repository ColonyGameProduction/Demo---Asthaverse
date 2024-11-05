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
    [SerializeField] protected bool _isIdle = true;
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRun;
    
    protected MovementStateFactory _states;
    protected MovementState _currState;

    [Space(1)]
    [Header("Move Animator Component")]
    [SerializeField] protected float[] _idleAnimCycleTimeTarget;
    [SerializeField] private float _idleAnimCycleSpeed = 2f;
    protected float _idleAnimCycleIdx;
    protected bool _wasCharacterAiming;


    [Space(1)]
    [Header("Move Speed - State Multiplier")]
    [SerializeField] protected float _runMultiplier;
    protected float _walkSpeed;
    protected float _runSpeed;
    protected float _currSpeed;

    [Space(1)]
    [Header("NavMesh Component")]
    protected NavMeshAgent _agentNavMesh;
    protected Vector3 _currAIDirPos; //Current AI Direction Position

    [Space(1)]
    [Header("AI Rotation")]
    protected Vector3 _AILookTarget; //position for AI To look at
    protected bool _isReceivePosADirection;
    protected bool _allowLookTarget;
    [SerializeField]protected float _faceDirCount = 0.9f;

    public Action<Vector3> OnIsTheSamePosition;

    #endregion

    #region CONST Variable

    public const string ANIMATION_MOVE_PARAMETER_HORIZONTAL = "Horizontal";
    public const string ANIMATION_MOVE_PARAMETER_VERTICAL = "Vertical";
    public const string ANIMATION_MOVE_PARAMETER_IDLECOUNTER ="IdleCounter";
    #endregion

    #region GETTERSETTER Variable

    public bool IsIdle {get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsWalking {get {return _isWalking;}set{ _isWalking = value;} }
    public bool IsRunning { get {return _isRun;}set{ _isRun = value;} }

    public float IdleAnimCycleIdx {get {return _idleAnimCycleIdx;}}
    public float[] IdleAnimCycleTimeTarget {get {return _idleAnimCycleTimeTarget;}}
    public float IdleAnimCycleSpeed {get {return _idleAnimCycleSpeed;}}
    public bool WasCharacterAiming {get {return _wasCharacterAiming;}set{_wasCharacterAiming = value;}}

    public float WalkSpeed { get {return _walkSpeed;}}
    public float RunSpeed { get {return _runSpeed;}}

    public NavMeshAgent AgentNavMesh {get {return _agentNavMesh;}}
    public Vector3 CurrAIDirPos { get {return _currAIDirPos;}}
    public bool AllowLookTarget {get {return _allowLookTarget;} set{_allowLookTarget = value;}}
    
    #endregion

    protected override void Awake() 
    {        
        base.Awake();

        _states = new MovementStateFactory(this);

        if(AgentNavMesh == null)_agentNavMesh = GetComponent<NavMeshAgent>();
    }
    private void Start() 
    {        
        SetIdleAnimToNormal();
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
    
    public virtual void Move()
    {
        MoveAI(CurrAIDirPos);
    }

    public virtual void ForceStopMoving()
    {
        ForceAIStopMoving();
    }
    #endregion

    #region AI ONLY
    protected void MoveAI(Vector3 direction)
    {
        if(AgentNavMesh.speed != _currSpeed)AgentNavMesh.speed = _currSpeed;

        bool isFacingTheDirection = true;
        float checkFaceDir = 0;
        Vector3 facedir = (AgentNavMesh.steeringTarget - transform.position).normalized;
        // Debug.Log("Dot Move" + Vector3.Dot(facedir, transform.forward));
        // bool isFacingMoveDirection = Vector3.Dot(facedir, transform.forward) > .5f;
        Vector3 animatedFaceDir = transform.InverseTransformDirection(facedir);

        if(!AllowLookTarget)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), 180 * Time.deltaTime);
            checkFaceDir = Vector3.Dot(facedir, transform.forward);
            isFacingTheDirection = checkFaceDir > _faceDirCount;
        }
        else
        {
            
            if(!_isReceivePosADirection)facedir = (_AILookTarget - transform.position).normalized;
            else facedir = _AILookTarget;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), 180 * Time.deltaTime);

            checkFaceDir = Vector3.Dot(facedir, transform.forward);
            isFacingTheDirection = checkFaceDir > _faceDirCount;
        }
        

        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, isFacingTheDirection? animatedFaceDir.x : 0.1f* animatedFaceDir.x, 0.5f, Time.deltaTime);
        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, isFacingTheDirection? animatedFaceDir.z : 0.1f * animatedFaceDir.z, 0.5f, Time.deltaTime);

    }
    public bool IsAIAtDirPos()
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

        if(Vector3.Distance(transform.position, AgentNavMesh.destination) < AgentNavMesh.radius)
        {
            AgentNavMesh.ResetPath();
            OnIsTheSamePosition?.Invoke(CurrAIDirPos);

            return true;
        }
        
        return false;
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

    public void SetAIDirection(Vector3 newPos)
    {
        _currAIDirPos = newPos;
    }
    public void SetAITargetToLook(Vector3 posToLook, bool isReceivePosADirection)
    {
        _AILookTarget = posToLook;
        _isReceivePosADirection = isReceivePosADirection;
    }
    public void RotateAIToTarget_Idle()
    {
        Vector3 facedir = Vector3.zero;
        if(!_isReceivePosADirection)facedir = (_AILookTarget - transform.position).normalized;
        else facedir = _AILookTarget;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), 180 * Time.deltaTime);
    }
    #endregion

    #region Movement Speed
    public virtual void InitializeMovementSpeed(float speed)
    {
        _walkSpeed = speed;
        _runSpeed = _walkSpeed * _runMultiplier;
    }
    public void ChangeCurrSpeed(float newSpeed)
    {
        if(_currSpeed != newSpeed)_currSpeed = newSpeed;
    }
    #endregion

    #region IdleAnim
    public void SetIdleAnimAfterAim()
    {
        SetIdleAnimCycleIdx(0);
    }
    public void SetIdleAnimToNormal()
    {
        SetIdleAnimCycleIdx(1);
    }
    public void SetIdleAnimCycleIdx(float x)
    {
        _idleAnimCycleIdx = x;
        _animator.SetFloat(ANIMATION_MOVE_PARAMETER_IDLECOUNTER, IdleAnimCycleIdx);
    }
    #endregion


}

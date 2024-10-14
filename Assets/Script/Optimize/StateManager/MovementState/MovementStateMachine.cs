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
    [Header("Testing")]
    public bool isGo;
    public bool isStop;
    public GameObject objectDir;

    [Space(5)]
    [Header("Other Component Variable")]
    protected MovementStateFactory _states;

    
    [Header("AI")]
    [SerializeField]protected NavMeshAgent _agentNavMesh;
    protected NavMeshPath _checkPath;

    [Header("Move Speed List - Each State")]
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _runMultiplier;
    protected float _runSpeed;
    [SerializeField] protected bool _isIdle;
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRun;


    [Space(5)]
    [Header("No Inspector Variable")]
    protected MovementState _currState;
    protected float _currSpeed;
    protected Transform _currAIDirection; //Nyimpen direction AI yg ntr dikasih jg dr luar

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
    public Transform CurrAIDirection { get {return _currAIDirection;}}
    public bool IsIdle {get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsWalking {get {return _isWalking;}set{ _isWalking = value;} }
    public bool IsRunning { get {return _isRun;}set{ _isRun = value;} }
    public NavMeshAgent AgentNavMesh {get {return _agentNavMesh;}}
    
    #endregion

    protected override void Awake() 
    {
        //Cek path biar tau ini go idle or not
        _checkPath = new NavMeshPath();

        
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
        if(isGo && objectDir != null)
        {
            isGo = false;
            GiveAIDirection(objectDir.transform);
        }
        if(isStop)
        {
            isStop = false;
            ForceStopMoving();
        }
        _currState?.UpdateState();
        // if(objectDir!=null)Move();
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
        MoveAI(_currAIDirection.position);
    }
    protected void MoveAI(Vector3 direction)
    {
        if(AgentNavMesh.speed != _currSpeed)AgentNavMesh.speed = _currSpeed;

        Vector3 facedir = (AgentNavMesh.steeringTarget - transform.position).normalized;
        Vector3 animatedFaceDir = transform.InverseTransformDirection(facedir);

        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, animatedFaceDir.x);
        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, animatedFaceDir.z);

 
    }
    public bool IsTargetTheSamePositionAsTransform()
    {
        if(CurrAIDirection == null) return true;

        if(AgentNavMesh.destination != CurrAIDirection.position)
        {
            AgentNavMesh.destination = CurrAIDirection.position;
        }
        if(!AgentNavMesh.hasPath)return true;
        else
        {
            if(Vector3.Distance(transform.position, AgentNavMesh.destination) < AgentNavMesh.radius)
            {

                AgentNavMesh.ResetPath();

                return true;
            }
        }

        return false;
    }

    public void GiveAIDirection(Transform newDir)
    {
        _currAIDirection = newDir;
    }
    public void ChangeCurrSpeed(float newSpeed)
    {
        if(_currSpeed != newSpeed)_currSpeed = newSpeed;
    }
    public virtual void ForceStopMoving()
    {
        ForceAIStopMoving();
    }
    private void ForceAIStopMoving()
    {
        _currAIDirection = null;
        IsWalking = false;
        IsRunning = false;
        AgentNavMesh.speed = 0;
        // CharaAnimator.SetBool("Scope", false);

        AgentNavMesh.ResetPath();

        Debug.Log("DoneMove");
    }
    #endregion

}

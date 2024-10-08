using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementStateMachine : CharacterStateMachine, IMovement, IStandMovement
{
    [Header("Testing")]
    public bool isGo;
    public bool isStop;
    public GameObject objectDir;

    [Space(5)]
    [Header("States")]
    MovementState _currState;
    MovementStateFactory _states;
    // public CrouchState crouchState= new CrouchState();
    [Header("Move Speed List - Each State")]
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _runSpeed;
    [SerializeField] protected bool _isIdle;
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRun;

    //CONST
    public const string ANIMATION_MOVE_PARAMETER_HORIZONTAL = "Horizontal";
    public const string ANIMATION_MOVE_PARAMETER_VERTICAL = "Vertical";
    [HideInInspector]
    //Getter Setter
    public float WalkSpeed { get {return _walkSpeed;}}
    public float RunSpeed { get {return _runSpeed;}}
    public Transform CurrAIDirection { get {return _currAIDirection;}}
    public bool IsIdle {get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsWalking {get {return _isWalking;}set{ _isWalking = value;} }
    public bool IsRunning { get {return _isRun;}set{ _isRun = value;} }
    public NavMeshAgent AgentNavMesh {get {return _agentNavMesh;}}
    public bool IsInputPlayer {get {return _isInputPlayer;}}

    // [SerializeField]private float _crouchSpeed;
    // public float CrouchSpeed{get {return _crouchSpeed;}}

    [Space(5)]
    protected float _currSpeed;
    protected Transform _currAIDirection; //Nyimpen direction AI yg ntr dikasih jg dr luar
    protected CharacterIdentity _charaIdentity;
    protected bool _isInputPlayer;

    [Header("AI Moving Controller")]
    [SerializeField]protected NavMeshAgent _agentNavMesh;


    protected override void Awake() 
    {
        base.Awake();
        if(_charaIdentity == null)_charaIdentity.GetComponent<CharacterIdentity>();
        _isInputPlayer = _charaIdentity.IsInputPlayer;
        _charaIdentity.OnInputPlayerChange += CharaIdentity_OnInputPlayerChange; // Ditaro di sini biar ga ketinggalan sebelah, krn sebelah diubah di start

        _states = new MovementStateFactory(this);

        if(AgentNavMesh == null)_agentNavMesh = GetComponent<NavMeshAgent>();
    }


    private void Start() 
    {
        SwitchState(_states.IdleState());
    }
    private void Update() 
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
            _currState?.ExiState();
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
        if(AgentNavMesh.hasPath)
        {
            Vector3 facedir = (AgentNavMesh.steeringTarget - transform.position).normalized;
            Vector3 animatedFaceDir = transform.InverseTransformDirection(facedir);
            // Debug.Log("aaa"+dir + "anim" + animatedDir);
            
            ///Animation Part -> Mungkin ntr dipindah
            CharaAnimator.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, animatedFaceDir.x);
            CharaAnimator.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, animatedFaceDir.z);
            CharaAnimator.SetBool("Scope", true);
            ///Animation Part -> Mungkin ntr dipindah

            if(Vector3.Distance(transform.position, AgentNavMesh.destination) < AgentNavMesh.radius)
            {
                ///Animation Part -> Mungkin ntr dipindah
                CharaAnimator.SetBool("Scope", false);
                ///Animation Part -> Mungkin ntr dipindah

                AgentNavMesh.ResetPath();

                // _currAIDirection = null;
                Debug.Log("DoneMove");
            }
        }
        else
        {

            ///Animation Part -> Mungkin ntr dipindah
            CharaAnimator.SetBool("Scope", false);
            ///Animation Part -> Mungkin ntr dipindah
            
        }
        if(AgentNavMesh.destination != direction)
        {
            AgentNavMesh.destination = direction;
        }
 
    }
    public bool IsTargetTheSamePositionAsTransform()
    {
        if(CurrAIDirection == null) return true;
        if(Vector3.Distance(transform.position, CurrAIDirection.position) < AgentNavMesh.radius)
        {
            return true;
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
        CharaAnimator.SetBool("Scope", false);

        AgentNavMesh.ResetPath();

        Debug.Log("DoneMove");
    }
    #endregion
    private void CharaIdentity_OnInputPlayerChange(bool obj)
    {
        _isInputPlayer = obj;
    }

}

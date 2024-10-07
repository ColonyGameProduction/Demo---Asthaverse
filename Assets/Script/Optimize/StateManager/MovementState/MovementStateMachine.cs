using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementStateMachine : CharacterStateMachine, IMovement, IStandMovement
{
    [Header("Testing")]
    public bool isAI;
    public bool isGo;
    public GameObject objectDir;

    [Space(5)]
    [Header("States")]
    MovementState _currState;
    MovementStateFactory _states;
    // public CrouchState crouchState= new CrouchState();
    [Header("Move Speed List - Each State")]
    [SerializeField]protected float _walkSpeed;
    [SerializeField]protected float _runSpeed;
    [SerializeField]protected bool _isRun;
    public bool IsRunning { get {return _isRun;}set{} }

    [HideInInspector]
    //Getter Setter

    public float WalkSpeed { get {return _walkSpeed;}}
    public float RunSpeed { get {return _runSpeed;}}
    public Transform CurrAIDirection { get {return _currAIDirection;}}

    // [SerializeField]private float _crouchSpeed;
    // public float CrouchSpeed{get {return _crouchSpeed;}}

    [Space(5)]
    protected float _currSpeed;
    protected Transform _currAIDirection; //Nyimpen direction AI yg ntr dikasih jg dr luar

    [Header("AI Moving Controller")]
    [SerializeField]protected NavMeshAgent agentNavMesh;


    protected override void Awake() 
    {
        base.Awake();
        _states = new MovementStateFactory(this);

        if(agentNavMesh == null)agentNavMesh = GetComponent<NavMeshAgent>();
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
        if(agentNavMesh.speed != _currSpeed)agentNavMesh.speed = _currSpeed;
        if(agentNavMesh.hasPath)
        {
            Vector3 facedir = (agentNavMesh.steeringTarget - transform.position).normalized;
            Vector3 animatedFaceDir = transform.InverseTransformDirection(facedir);
            // Debug.Log("aaa"+dir + "anim" + animatedDir);
            
            ///Animation Part -> Mungkin ntr dipindah
            CharaAnimator.SetFloat("Horizontal", animatedFaceDir.x);
            CharaAnimator.SetFloat("Vertical", animatedFaceDir.z);
            CharaAnimator.SetBool("Scope", true);
            ///Animation Part -> Mungkin ntr dipindah
            
            if(Vector3.Distance(transform.position, agentNavMesh.destination)< agentNavMesh.radius)
            {
                ///Animation Part -> Mungkin ntr dipindah
                CharaAnimator.SetBool("Scope", false);
                ///Animation Part -> Mungkin ntr dipindah
                
                agentNavMesh.ResetPath();
                _currAIDirection = null;
                Debug.Log("DoneMove");
            }
        }
        else
        {
            ///Animation Part -> Mungkin ntr dipindah
            CharaAnimator.SetBool("Scope", false);
            ///Animation Part -> Mungkin ntr dipindah
            
        }
        if(agentNavMesh.destination != direction)
        {
            agentNavMesh.destination = direction;
        }
    }
    public void GiveAIDirection(Transform newDir)
    {
        _currAIDirection = newDir;
    }
    public void ChangeCurrSpeed(float newSpeed)
    {
        if(_currSpeed != newSpeed)_currSpeed = newSpeed;
    }
    #endregion

}

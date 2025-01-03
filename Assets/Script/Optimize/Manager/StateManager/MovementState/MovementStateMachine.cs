using System;
using UnityEngine;
using UnityEngine.AI;


[Serializable]
public struct CharaControllerData
{
    public Vector3 center;
    public float radius;
    public float height;
}
public class MovementStateMachine : CharacterStateMachine, IMovement, IStandMovementData
{
    #region Normal Variable

    protected GameManager _gm;
    [Space(1)]
    [Header("Move States - Stand")]
    [ReadOnly(true), SerializeField]protected bool _isIdle = true;
    [ReadOnly(true), SerializeField] protected bool _isWalking;
    [ReadOnly(true), SerializeField] protected bool _isRun;
    [ReadOnly(true), SerializeField] protected bool _isCrouch;
    public Action OnStartRunning;
    
    [Space(1)]
    [Header("Move Speed - State Multiplier")]
    [SerializeField] protected float _runSpeedMultiplier;
    [SerializeField] protected float _crouchSpeedMultiplier;
    protected float _walkSpeed;
    protected float _runSpeed;
    private float _crouchSpeed;
    protected float _currSpeed;
    [SerializeField] protected float _rotationSpeedAI = 180;

    protected MovementStateFactory _states;
    protected MovementState _currState;

    [Space(1)]
    [Header("Move Animator Component")]
    [SerializeField] protected float[] _idleAnimCycleTimeTarget;
    [SerializeField] private float _idleAnimCycleSpeed = 2f;
    protected float _idleAnimCycleIdx;
    protected bool _wasCharacterAiming;
    /// <summary> To make the idle keep staying in stay alert phase </summary>
    protected bool _isIdleMustStayAlert; 

    [Space(1)]
    [Header("NavMesh Component")]
    protected NavMeshAgent _agentNavMesh;
    /// <summary> Current AI Direction Position To Go </summary>
    protected Vector3 _currAIDirPos;

    [Space(1)]
    [Header("AI Rotation")]
    protected Vector3 _AILookTarget; //position for AI To look at
    protected bool _isLookTargetDirection;
    protected bool _allowLookTarget;
    [SerializeField]protected float _lookTargetDotMin = 0.9f;

    [Space(1)]
    [Header("CharaCon Data")]
    protected CharaControllerData _normalHeightCharaCon;
    [SerializeField] protected CharaControllerData _crouchHeightCharaCon;

    [Space(1)]
    [Header("AI Crouch Head Checker")]
    [Tooltip("Point to check if there's crouch platform or not for AI")]
    [SerializeField] protected Transform _feetBasePoint;
    [SerializeField] protected LayerMask _crouchPlatformLayer;
    [SerializeField] protected float _crouchRayBuffer = 1.1f;
    [SerializeField] protected float _crouchHeadHitAngleBuffer = 15;
    private bool _isAtCrouchPlatform;
    
    #region Event
    public Action<Vector3> OnIsTheSamePosition;
    #endregion
    #endregion

    #region CONST Variable

    public const string ANIMATION_MOVE_PARAMETER_HORIZONTAL = "Horizontal";
    public const string ANIMATION_MOVE_PARAMETER_VERTICAL = "Vertical";
    public const string ANIMATION_MOVE_PARAMETER_IDLECOUNTER ="IdleCounter";
    #endregion

    #region GETTERSETTER Variable

    public bool IsIdle {get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsWalking {get {return _isWalking;}set{ _isWalking = value;} }
    public virtual bool IsRunning { get {return _isRun;}set{ _isRun = value;} }
    public virtual bool IsCrouching { get {return _isCrouch;}set{ _isCrouch = value;} }

    public float IdleAnimCycleIdx {get {return _idleAnimCycleIdx;}}
    public float[] IdleAnimCycleTimeTarget {get {return _idleAnimCycleTimeTarget;}}
    public float IdleAnimCycleSpeed {get {return _idleAnimCycleSpeed;}}
    public bool WasCharacterAiming {get {return _wasCharacterAiming;}set{_wasCharacterAiming = value;}}

    public float WalkSpeed { get {return _walkSpeed;}}
    public float RunSpeed { get {return _runSpeed;}}
    public float CrouchSpeed {get{return _crouchSpeed;}}

    public NavMeshAgent AgentNavMesh {get {return _agentNavMesh;}}
    /// <summary> Current AI Direction Position To Go </summary>
    public Vector3 CurrAIDirPos { get {return _currAIDirPos;}}
    public bool AllowLookTarget {get {return _allowLookTarget;} set{_allowLookTarget = value;}}
    /// <summary> To make the idle keep staying in stay alert phase </summary>
    public bool IsIdleMustStayAlert { get {return _isIdleMustStayAlert;}set{ _isIdleMustStayAlert = value;} }

    public bool IsAtCrouchPlatform {get {return _isAtCrouchPlatform;} set{_isAtCrouchPlatform = value;}}

    public virtual float AgentRadius
    {
        get 
        {
            return _agentNavMesh.radius;
        }
    }
    
    #endregion

    protected override void Awake() 
    {        
        base.Awake();

        _states = new MovementStateFactory(this);

        if(AgentNavMesh == null)_agentNavMesh = GetComponent<NavMeshAgent>();
        _normalHeightCharaCon.center = Vector3.zero;
        _normalHeightCharaCon.radius = _agentNavMesh.radius;
        _normalHeightCharaCon.height = _agentNavMesh.height;
        // Debug.Log("agent navmesh radiusnya adala" + _agentNavMesh.radius);
    }
    protected virtual void Start() 
    {   
        _gm = GameManager.instance;
        
        SetIdleAnimToNormal();
        SwitchState(_states.IdleState());
    }

    protected virtual void Update() 
    {
        if(!_gm.IsGamePlaying()) return;

        _currState?.UpdateState();
    }
    private void FixedUpdate() 
    {
        if(!_gm.IsGamePlaying()) return;

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
        HandleCrouchAIBehaviour(facedir);
        Vector3 animatedFaceDir = transform.InverseTransformDirection(facedir);

        if(!AllowLookTarget)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), _rotationSpeedAI * Time.deltaTime);
            checkFaceDir = Vector3.Dot(facedir, transform.forward);
            isFacingTheDirection = checkFaceDir > _lookTargetDotMin;
        }
        else
        {
            
            if(!_isLookTargetDirection)facedir = (_AILookTarget - transform.position).normalized;
            else facedir = _AILookTarget;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), _rotationSpeedAI * Time.deltaTime);

            checkFaceDir = Vector3.Dot(facedir, transform.forward);
            isFacingTheDirection = checkFaceDir > _lookTargetDotMin;
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
        if (AgentNavMesh.pathPending)
        {
            return false;
        }
        if(!AgentNavMesh.hasPath)
        {
            OnIsTheSamePosition?.Invoke(CurrAIDirPos);
            return true;
        }

        // Debug.Log(transform.name + " agent Radius" + AgentRadius);
        if(Vector3.Distance(transform.position, AgentNavMesh.destination) < AgentRadius)
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
    }

    public void SetAIDirection(Vector3 newPos)
    {
        _currAIDirPos = newPos;
    }
    public void SetAITargetToLook(Vector3 posToLook, bool isLookTargetDirection)
    {
        _AILookTarget = posToLook;
        _isLookTargetDirection = isLookTargetDirection;
    }
    public void RotateAIToTarget_Idle()
    {
        Vector3 facedir = Vector3.zero;

        if(!_isLookTargetDirection)facedir = (_AILookTarget - transform.position).normalized;
        else facedir = _AILookTarget;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(facedir), _rotationSpeedAI * Time.deltaTime);
    }
    public void HandleCrouchAIBehaviour(Vector3 agentGoalDir)
    {

        if(!IsCrouching)
        {
            // Debug.DrawRay(_feetBasePoint.position, agentGoalDir * (_agentNavMesh.radius * 1.1f), Color.red);
            if(Physics.Raycast(_feetBasePoint.position, agentGoalDir, out RaycastHit hit, _agentNavMesh.radius * _crouchRayBuffer, _crouchPlatformLayer))
            {
                IsAtCrouchPlatform = true;
                if(IsRunning)IsRunning = false;
                IsCrouching = true;
            }
        }
        else
        {
            float distanceToagentGoal = Vector3.Distance(transform.position, _agentNavMesh.steeringTarget);
            // Debug.DrawRay(_feetBasePoint.position, agentGoalDir * distanceToagentGoal, Color.red);
            if(Physics.Raycast(_feetBasePoint.position, agentGoalDir, out RaycastHit hit, distanceToagentGoal, _crouchPlatformLayer))
            {

                IsAtCrouchPlatform = true;
                if(IsRunning)IsRunning = false;
                IsCrouching = true;
            }
            else
            {
                if(IsAtCrouchPlatform)
                {
                    if(IsHeadHitWhenUnCrouch())
                    {
                        IsAtCrouchPlatform = true;
                        if(IsRunning)IsRunning = false;
                        IsCrouching = true;
                    }
                    else
                    {
                        IsCrouching = false;
                        IsAtCrouchPlatform = false;
                    }
                }
            }
        }
    }
    #endregion

    #region Movement Speed
    public virtual void InitializeMovementSpeed(float speed)
    {
        _walkSpeed = speed;
        _runSpeed = _walkSpeed * _runSpeedMultiplier;
        _crouchSpeed = _walkSpeed * _crouchSpeedMultiplier;
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
    #region CharaCon Function
    public virtual void CharaConDataToNormal() => ChangeNavMeshData(_normalHeightCharaCon);
    public virtual void CharaConDataToCrouch() => ChangeNavMeshData(_crouchHeightCharaCon);
    protected void ChangeNavMeshData(CharaControllerData newData)
    {
        _agentNavMesh.radius = newData.radius;
        _agentNavMesh.height = newData.height;
    }
    #endregion

    public virtual bool IsHeadHitWhenUnCrouch()
    {
        Vector3 facedir = (AgentNavMesh.steeringTarget - transform.position).normalized;
        // Debug.DrawRay(_feetBasePoint.position, _feetBasePoint.up * _normalHeightCharaCon.height, Color.black, 0.5f);
        if(Physics.Raycast(_feetBasePoint.position, _feetBasePoint.up, out RaycastHit hitHead, _normalHeightCharaCon.height, _crouchPlatformLayer))
        {
            return true;
        }
        else
        {

            Vector3 crossProduct = Vector3.Cross(_feetBasePoint.up, facedir).normalized;
            Quaternion rotation = Quaternion.AngleAxis(_crouchHeadHitAngleBuffer, crossProduct);
            
            Vector3 newDir = rotation * _feetBasePoint.up;
            // Debug.DrawRay(_feetBasePoint.position, crossProduct * _normalHeightCharaCon.height, Color.red, 0.5f);
            // Debug.DrawRay(_feetBasePoint.position, newDir * _normalHeightCharaCon.height, Color.black, 0.5f);
            if(Physics.Raycast(_feetBasePoint.position, newDir, out RaycastHit hitHead2, _normalHeightCharaCon.height, _crouchPlatformLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }



}

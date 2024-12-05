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
    [Space(1)]
    [Header("Move States - Stand")]
    [SerializeField] protected bool _isIdle = true;
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRun;
    [SerializeField] protected bool _isCrouch;
    
    protected bool _isMustStayAlert;
    
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
    [SerializeField] protected float _crouchMultiplier;
    protected float _walkSpeed;
    protected float _runSpeed;
    private float _crouchSpeed;
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

    [Space(1)]
    [Header("CharaCon Data")]
    protected CharaControllerData _normalHeightCharaCon;
    [SerializeField] protected CharaControllerData _crouchHeightCharaCon;

    [Space(1)]
    [Header("AI Crouch Head Checker")]
    [SerializeField] protected Transform _feetTransformPoint;
    private bool _isAtCrouchPlatform;
    
    [SerializeField] protected LayerMask _crouchPlatformLayer;

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
    public Vector3 CurrAIDirPos { get {return _currAIDirPos;}}
    public bool AllowLookTarget {get {return _allowLookTarget;} set{_allowLookTarget = value;}}
    public bool IsMustStayAlert { get {return _isMustStayAlert;}set{ _isMustStayAlert = value;} }

    public bool IsAtCrouchPlatform {get {return _isAtCrouchPlatform;} set{_isAtCrouchPlatform = value;}}
    
    #endregion

    protected override void Awake() 
    {        
        base.Awake();

        _states = new MovementStateFactory(this);

        if(AgentNavMesh == null)_agentNavMesh = GetComponent<NavMeshAgent>();
        _normalHeightCharaCon.center = Vector3.zero;
        _normalHeightCharaCon.radius = _agentNavMesh.radius;
        _normalHeightCharaCon.height = _agentNavMesh.height;
    }
    protected virtual void Start() 
    {   
        // _headMaxHeightTransform.position = new Vector3(_headMaxHeightTransform.position.x, _headMaxHeightTransform.position.y + _charaHeightBuffer, _headMaxHeightTransform.position.z);
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
        CrouchAIBrain(facedir);
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
        if (AgentNavMesh.pathPending)
        {
            return false;
        }
        if(!AgentNavMesh.hasPath)
        {
            // Debug.Log("I'm at same pos" + transform.name + " " + AgentNavMesh.destination + " " + CurrAIDirPos);
            OnIsTheSamePosition?.Invoke(CurrAIDirPos);
            return true;
        }

        if(Vector3.Distance(transform.position, AgentNavMesh.destination) < AgentNavMesh.radius)
        {
            AgentNavMesh.ResetPath();
            // Debug.Log("I'm at same pos2" + transform.name + " " + AgentNavMesh.destination + " " + CurrAIDirPos);
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
        _crouchSpeed = _walkSpeed * _crouchMultiplier;
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
    public virtual void CharaConDataToNormal() => ChangeNavMeshData(_normalHeightCharaCon);
    public virtual void CharaConDataToCrouch() => ChangeNavMeshData(_crouchHeightCharaCon);
    protected void ChangeNavMeshData(CharaControllerData newData)
    {
        // Debug.Log("lewat siniii");
        _agentNavMesh.radius = newData.radius;
        _agentNavMesh.height = newData.height;
    }

    public virtual bool IsHeadHitWhenUnCrouch()
    {
        Vector3 facedir = (AgentNavMesh.steeringTarget - transform.position).normalized;
        Debug.DrawRay(_feetTransformPoint.position, _feetTransformPoint.up * _normalHeightCharaCon.height, Color.black, 0.5f);
        if(Physics.Raycast(_feetTransformPoint.position, _feetTransformPoint.up, out RaycastHit hitHead, _normalHeightCharaCon.height, _crouchPlatformLayer))
        {
            return true;
        }
        else
        {

            Vector3 crossProduct = Vector3.Cross(_feetTransformPoint.up, facedir).normalized;
            Quaternion rotation = Quaternion.AngleAxis(15, crossProduct);
            
            Vector3 newDir = rotation * _feetTransformPoint.up;
            Debug.DrawRay(_feetTransformPoint.position, crossProduct * _normalHeightCharaCon.height, Color.red, 0.5f);
            Debug.DrawRay(_feetTransformPoint.position, newDir * _normalHeightCharaCon.height, Color.black, 0.5f);
            if(Physics.Raycast(_feetTransformPoint.position, newDir, out RaycastHit hitHead2, _normalHeightCharaCon.height, _crouchPlatformLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }

    public void CrouchAIBrain(Vector3 agentGoalDir)
    {

        if(!IsCrouching)
        {
            
            Debug.DrawRay(_feetTransformPoint.position, agentGoalDir * (_agentNavMesh.radius * 1.1f), Color.red);
            if(Physics.Raycast(_feetTransformPoint.position, agentGoalDir, out RaycastHit hit, _agentNavMesh.radius * 1.1f, _crouchPlatformLayer))
            {
                Debug.Log(transform.name + "I hit something I must crouch");
                IsAtCrouchPlatform = true;
                if(IsRunning)IsRunning = false;
                IsCrouching = true;
            }
        }
        else
        {
            float distanceToagentGoal = Vector3.Distance(transform.position, _agentNavMesh.steeringTarget);
            Debug.DrawRay(_feetTransformPoint.position, agentGoalDir * distanceToagentGoal, Color.red);
            if(Physics.Raycast(_feetTransformPoint.position, agentGoalDir, out RaycastHit hit, distanceToagentGoal, _crouchPlatformLayer))
            {
                Debug.Log(transform.name + "I still hit something I must crouch");
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
                        Debug.Log(transform.name + "I still hit something I must crouch");
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

}

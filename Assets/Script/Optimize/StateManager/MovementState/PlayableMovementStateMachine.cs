using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovementStateMachine : MovementStateMachine, ICrouch, IPlayableMovementDataNeeded
{
    #region Normal Variable
    [Header ("Playable Character Variable")]

    [Space(2)]
    [Header("Other Component Variable")]
    [SerializeField] private CharacterController _cc;

    [Header("For Rotation")]
    [SerializeField] private Transform _charaGameObject;
    [SerializeField] private float _rotateSpeed;

    [Header("Move Speed List - Crouch State")]
    [SerializeField] private float _crouchSpeed;
    [SerializeField] protected bool _isCrouch;

    [Header("Movement - Camera")]
    [SerializeField] private Transform _followTarget;
    [SerializeField] protected bool _isMustLookForward;

    [Header("No Inspector Variable")]
    protected Vector3 _inputMovement;
    #endregion
    
    #region GETTERSETTER Variable
    [HideInInspector]
    //getter setter
    public CharacterController CC {get { return _cc;} }
    public Vector3 InputMovement { get{ return _inputMovement;} set{_inputMovement = value;}} // Getting Input Movement from playercontroller
    public bool IsCrouching { get {return _isCrouch;}set{ _isCrouch = value;} }
    public bool IsMustLookForward { get{return _isMustLookForward;} set{_isMustLookForward = value;} } // Kan kalo nembak gitu hrs liat ke depan selalu, jd is true
    public float CrouchSpeed {get{return _crouchSpeed;}}
    #endregion
    protected override void Awake()
    {
        base.Awake();
        if(_cc == null)_cc = GetComponent<CharacterController>();
        if(_followTarget == null) _followTarget = GetComponent<PlayableCamera>().GetFollowTarget;
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void Move()
    {
        if(!IsInputPlayer)base.Move();
        else MovePlayableChara(InputMovement);
    }
    /// <summary>
    /// Move yang digunakan untuk kontrol dgn input dari player
    /// </summary>
    public void MovePlayableChara(Vector3 direction)
    {
        Vector3 movement = new Vector3(direction.x, 0, direction.y).normalized;

        Vector3 flatForward = new Vector3(_followTarget.forward.x, 0, _followTarget.forward.z).normalized;
        Vector3 Facedir = flatForward * movement.z + _followTarget.right * movement.x; 

        CC.SimpleMove(Facedir * _currSpeed);

        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, movement.x);
        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, movement.z);

        if(!IsMustLookForward)RotatePlayableChara(Facedir);
        else RotatePlayableChara(flatForward);
    }
    public void RotatePlayableChara(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            _charaGameObject.forward = Vector3.Slerp(_charaGameObject.forward, direction.normalized, Time.deltaTime * _rotateSpeed);
        }
    }

    public void Idle_RotateAim()
    {
        Vector3 flatForward = new Vector3(_followTarget.forward.x, 0, _followTarget.forward.z).normalized;
        RotatePlayableChara(flatForward);
    }

    public void ForceStopPlayable()
    {
        InputMovement = Vector3.zero;
        IsWalking = false;
        IsRunning = false;
        IsCrouching = false;
    }
    public override void ForceStopMoving()
    {
        if(!IsInputPlayer)base.ForceStopMoving();
        else ForceStopPlayable();
    }
}

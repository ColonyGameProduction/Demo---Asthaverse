using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovementStateMachine : MovementStateMachine, ICrouch, IPlayableMovementDataNeeded
{
    
    [Header ("Playable Character Variable")]
    [Space(2)]
    [Header("GetComponent Manager")]
    [SerializeField] private CharacterController _cc;
    [Space(1)]
    [Header("For Rotation by Player")]
    [SerializeField] private Transform _charaGameObject;
    [SerializeField] private float _rotateSpeed;
    [Space(1)]
    [Header("Speed")]
    [SerializeField] private float _crouchSpeed;
    [Space(1)]
    [Header("Hubungan dgn Movement dan Camera")]
    [SerializeField] private Transform _followTarget;
    [SerializeField] protected bool _isCrouch;
    [SerializeField] protected bool _isMustLookForward;
    protected Vector3 _inputMovement;
    
    [HideInInspector]
    //getter setter
    public Vector3 InputMovement { get{ return _inputMovement;} set{_inputMovement = value;}} // Getting Input Movement from playercontroller
    public bool IsCrouching { get {return _isCrouch;}set{ _isCrouch = value;} }
    public bool IsMustLookForward { get{return _isMustLookForward;} set{_isMustLookForward = value;} } // Kan kalo nembak gitu hrs liat ke depan selalu, jd is true
    public CharacterController CC {get { return _cc;} }
    public float CrouchSpeed {get{return _crouchSpeed;}}

    protected override void Awake()
    {
        base.Awake();
        if(_cc == null)_cc = GetComponent<CharacterController>();
        if(_followTarget == null) _followTarget = GetComponent<PlayableCamera>().GetFollowTarget;
    }

    public override void Move()
    {
        if(IsInputPlayer)base.Move();
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

        CharaAnimator.SetFloat("Horizontal", movement.x);
        CharaAnimator.SetFloat("Vertical", movement.z);

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

    public void ForceStopPlayable()
    {
        InputMovement = Vector3.zero;
        IsWalking = false;
        IsRunning = false;
        IsCrouching = false;
    }
    public override void ForceStopMoving()
    {
        if(IsInputPlayer)base.ForceStopMoving();
        else ForceStopPlayable();
    }
}

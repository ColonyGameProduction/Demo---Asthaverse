using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovementStateMachine : MovementStateMachine, IGroundMovementData, IPlayableMovementDataNeeded
{
    #region Normal Variable
    [Header ("Playable Character Variable")]

    [Space(2)]
    [Header("Move States - Crouch State")]
    [SerializeField] protected float _crouchMultiplier;
    [SerializeField] protected float _crawlMultiplier;
    private float _crouchSpeed;
    private float _crawlSpeed;
    [SerializeField] protected bool _isCrouch;
    [SerializeField] protected bool _isCrawl;

    [Space(1)]
    [Header("For Rotation")]
    [SerializeField] private Transform _charaGameObject;
    [SerializeField] private float _rotateSpeed;

    [Space(1)]
    [Header("Movement - Camera")]
    [SerializeField] private Transform _followTarget;
    [SerializeField] protected bool _isMustLookForward;

    [Space(1)]
    [Header("Saving other component data")]
    [SerializeField] private CharacterController _cc;
    protected ICanInputPlayer _getCanInputPlayer;
    protected Vector3 _inputMovement;
    #endregion
    
    #region GETTERSETTER Variable
    public override float WalkSpeed 
    { 
        get {return base.WalkSpeed;}
        set 
        {
            _walkSpeed = value;
            _runSpeed = _walkSpeed * _runMultiplier;
            _crouchSpeed = _walkSpeed * _crouchMultiplier;
            _crawlSpeed = _walkSpeed * _crawlMultiplier;
        }
    }

    public CharacterController CC {get { return _cc;} }
    public Vector3 InputMovement { get{ return _inputMovement;} set{_inputMovement = value;}} // Getting Input Movement from playercontroller

    public bool IsMustLookForward { get{return _isMustLookForward;} set{_isMustLookForward = value;} } // Kan kalo nembak gitu hrs liat ke depan selalu, jd is true
    public float CrouchSpeed {get{return _crouchSpeed;}}
    public bool IsCrouching { get {return _isCrouch;}set{ _isCrouch = value;} }
    public float CrawlSpeed { get{return _crawlSpeed;}}
    public bool IsCrawling {  get {return _isCrawl;}set{ _isCrawl = value;} }

    #endregion
    protected override void Awake()
    {
        base.Awake();

        _getCanInputPlayer = GetComponent<ICanInputPlayer>();
        _isInputPlayer = _getCanInputPlayer.IsInputPlayer;
        _getCanInputPlayer.OnInputPlayerChange += CharaIdentity_OnInputPlayerChange; // Ditaro di sini biar ga ketinggalan sebelah, krn sebelah diubah di start

        if(_cc == null)_cc = GetComponent<CharacterController>();
        if(_followTarget == null) _followTarget = GetComponent<PlayableCamera>().GetFollowTarget;
    }

    #region Move
    public override void Move()
    {
        if(!IsInputPlayer)base.Move();
        else MovePlayableChara(InputMovement);
    }
    /// <summary>
    /// Move yang digunakan untuk kontrol dgn input dari player
    /// </summary>
    private void MovePlayableChara(Vector3 direction)
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
    private void RotatePlayableChara(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            _charaGameObject.forward = Vector3.Slerp(_charaGameObject.forward, direction.normalized, Time.deltaTime * _rotateSpeed);
        }
    }

    //Rotating to the aim direction when idle
    public void Idle_RotateAim()
    {
        Vector3 flatForward = new Vector3(_followTarget.forward.x, 0, _followTarget.forward.z).normalized;
        RotatePlayableChara(flatForward);
    }


    public override void ForceStopMoving()
    {
        if(!IsInputPlayer)base.ForceStopMoving();
        else ForceStopPlayable();
        if(IsMustLookForward)IsMustLookForward = false;
    }
    private void ForceStopPlayable()
    {
        InputMovement = Vector3.zero;
        _charaGameObject.localRotation = Quaternion.Euler(0, 0, 0);

        IsWalking = false;
        IsRunning = false;
        // IsCrouching = false; //biar kalo lg crouch di tmpt ga tb tb berdiri
        
    }
    #endregion
    private void CharaIdentity_OnInputPlayerChange(bool obj)
    {
        _isInputPlayer = obj;
    }
}

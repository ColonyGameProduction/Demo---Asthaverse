using System;
using UnityEngine;


[Serializable]
public struct CharaControllerData
{
    public Vector3 center;
    public float radius;
    public float height;
}
public class PlayableMovementStateMachine : MovementStateMachine, IGroundMovementData, IPlayableMovementDataNeeded
{
    #region Normal Variable
    [Header ("Playable Character Variable")]

    [Space(2)]
    [Header("Move States - Ground State")]
    [SerializeField] protected bool _isCrawl;
    [SerializeField] protected float _crawlMultiplier;
    private float _crawlSpeed;

    [Space(1)]
    [Header("CharaCon Data")]
    private CharaControllerData _normalHeightCharaCon;
    [SerializeField] private CharaControllerData _crouchHeightCharaCon;


    [Space(1)]
    [Header("For Rotation")]
    [SerializeField] private Transform _charaGameObject;
    [SerializeField] private float _rotateSpeed;

    [Space(1)]
    [Header("Movement - Camera")]
    private Transform _playableLookTarget;
    [SerializeField] protected bool _isMustLookForward;

    [Space(1)]
    [Header("Saving other component data")]
    private CharacterController _cc;
    protected IReceiveInputFromPlayer _canReceivePlayerInput;
    protected Vector3 _inputMovement;
    protected PlayableCharacterIdentity _getPlayableCharacterIdentity;
    protected WorldSoundManager _worldSoundManager;
    protected FOVMachine _fovMachine;
    #endregion
    
    #region GETTERSETTER Variable
    public float CrawlSpeed { get{return _crawlSpeed;}}
    public bool IsCrawling {  get {return _isCrawl;}set{ _isCrawl = value;} }

    public bool IsMustLookForward { get{return _isMustLookForward;} set{_isMustLookForward = value;} } // Kan kalo nembak gitu hrs liat ke depan selalu, jd is true

    public CharacterController CC {get { return _cc;} }
    public Vector3 InputMovement { get{ return _inputMovement;} set{_inputMovement = value;}} // Getting Input Movement from playercontroller
    public PlayableCharacterIdentity GetPlayableCharacterIdentity{ get {return _getPlayableCharacterIdentity;}}


    #endregion
    protected override void Awake()
    {
        base.Awake();
        _fovMachine = GetComponent<FOVMachine>();

        _canReceivePlayerInput = GetComponent<IReceiveInputFromPlayer>();
        _getPlayableCharacterIdentity = _charaIdentity as PlayableCharacterIdentity;
        _isAIInput = !_canReceivePlayerInput.IsPlayerInput;
        _canReceivePlayerInput.OnIsPlayerInputChange += CharaIdentity_OnIsPlayerInputChange; //When IsInputPlayerChange

        if(_cc == null)_cc = GetComponent<CharacterController>();
        _normalHeightCharaCon.center = _cc.center;
        _normalHeightCharaCon.radius = _cc.radius;
        _normalHeightCharaCon.height = _cc.height;


        if(_playableLookTarget == null) _playableLookTarget = GetComponent<PlayableCamera>().GetFollowTarget;
    }
    protected override void Start()
    {
        _worldSoundManager = WorldSoundManager.Instance;
        base.Start();
    }

    #region Move
    public override void Move()
    {
        if(!IsAIInput) MovePlayableChara(InputMovement);
        else base.Move();
        if(IsWalking || IsRunning)
        {
            Debug.Log(transform.position + " produce walk sound");
            _worldSoundManager.MakeSound(WorldSoundName.Walk, transform.position, _fovMachine.CharaEnemyMask);
        }
    }
    /// <summary>
    /// Move yang digunakan untuk kontrol dgn input dari player
    /// </summary>
    public override void ForceStopMoving()
    {
        if(!IsAIInput)ForceStopPlayable(); 
        else base.ForceStopMoving();

        if(IsMustLookForward)IsMustLookForward = false;
    }
    #endregion
    #region PlayableChara Only
    private void MovePlayableChara(Vector3 direction)
    {
        Vector3 movement = new Vector3(direction.x, 0, direction.y).normalized;

        Vector3 flatForward = new Vector3(_playableLookTarget.forward.x, 0, _playableLookTarget.forward.z).normalized;
        Vector3 Facedir = flatForward * movement.z + _playableLookTarget.right * movement.x; 

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

    public void RotateToAim_Idle()
    {
        Vector3 flatForward = new Vector3(_playableLookTarget.forward.x, 0, _playableLookTarget.forward.z).normalized;
        RotatePlayableChara(flatForward);
    }

    private void ForceStopPlayable()
    {
        InputMovement = Vector3.zero;
        if(PlayableCharacterManager.IsSwitchingCharacter)SetCharaGameObjRotationToNormal();

        IsWalking = false;
        IsRunning = false;
        // IsCrouching = false; //biar kalo lg crouch di tmpt ga tb tb berdiri
    }
    public void SetCharaGameObjRotationToNormal() => _charaGameObject.localRotation = Quaternion.Euler(0, 0, 0);
    #endregion

    #region Movement Speed

    public override void InitializeMovementSpeed(float speed)
    {
        base.InitializeMovementSpeed(speed);
        
        _crawlSpeed = _walkSpeed * _crawlMultiplier;
    }
    #endregion
    private void CharaIdentity_OnIsPlayerInputChange(bool isPlayerInput) => _isAIInput = !isPlayerInput;

    private void SetCharaConHeight(CharaControllerData data)
    {
        _cc.center = data.center;
        _cc.radius = data.radius;
        _cc.height = data.height;
    }
}

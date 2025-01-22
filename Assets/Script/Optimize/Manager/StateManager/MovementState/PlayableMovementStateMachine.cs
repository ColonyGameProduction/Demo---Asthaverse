using System;
using UnityEngine;
using UnityEngine.Serialization;



public class PlayableMovementStateMachine : MovementStateMachine, IGroundMovementData, IPlayableMovementDataNeeded, IUnsubscribeEvent
{
    #region Normal Variable
    [Header ("Playable Character Variable")]
    protected bool _isAskedToCrouchByPlayable, _isAskedToRunByPlayable;

    [Space(2)]
    [Header("Move States - Ground State")]
    [ReadOnly(true), SerializeField] protected bool _isCrawl;
    [SerializeField] protected float _crawlSpeedMultiplier;
    private float _crawlSpeed;

    [Space(1)]
    [Header("Taking Cover At Wall Data")]
    [ReadOnly(false), SerializeField] protected bool _isTakeCoverAtWall;
    [SerializeField] protected LayerMask _wallLayerMask;
    [SerializeField][Range(0,3f)] protected float _playerToWallMinDistance = 2f;
    [SerializeField] protected float _takeCoverWallCheckerAngleBuffer = 45;
    [SerializeField] protected float _takeCoverWallCheckerMultiplierBuffer = 0.1f;
    [SerializeField] protected float _takeCoverAnimateCharaPosMultiplierBuffer = 0.5f;

    protected Transform _animateCharaTransform;
    protected Transform _originToLookAtWall;
    protected Transform _tempAnimateCharaTransform;
    protected Collider _chosenWallToTakeCover, _charaHeadColl;
    protected float _charaHeightBuffer, _charaWidth, _wallLength, _wallWidth;
    protected Vector3 _takeCoverPosition, _takeCoverDirection;
    protected bool _isGoingToTakeCover, _isAtTakingCoverPos, _isFrontBehind, _isFront, _isRight;
    protected bool _isWallTallerThanChara;

    [Space(1)]
    [Header("CharaCon Data - advanced")]
    [SerializeField] private CharaControllerData _crawlHeightCharaCon;


    [Space(1)]
    [Header("For Rotation")]
    [SerializeField] private Transform _charaGameObject;
    [SerializeField] private float _rotateSpeed;

    [Space(1)]
    [Header("Movement - Camera")]
    private Transform _playableLookTarget;
    [ReadOnly(true),SerializeField] protected bool _isMustLookForward;

    [Space(1)]
    [Header("Saving other component data")]
    private CharacterController _cc;
    protected IReceiveInputFromPlayer _canReceivePlayerInput;
    protected Vector3 _inputMovement;
    protected PlayableCharacterIdentity _getPlayableCharacterIdentity;
    protected WorldSoundManager _worldSoundManager;
    protected PlayableMakeSFX _getPlayableMakeSFX;
    protected FOVMachine _fovMachine;
    #endregion
    private RaycastHit _wallTakeCoverHit;
    public const string ANIMATION_MOVE_PARAMETER_TAKECOVERPOS = "TakeCoverPos";
    public const string ANIMATION_MOVE_PARAMETER_TAKECOVER = "TakeCover";
    
    #region GETTERSETTER Variable
    public bool IsAskedToCrouchByPlayable{get {return _isAskedToCrouchByPlayable;} set{_isAskedToCrouchByPlayable = value;}}
    public bool IsAskedToRunByPlayable{get {return _isAskedToRunByPlayable;} set{_isAskedToRunByPlayable = value;}}

    public Transform CharaGameObject {get {return _charaGameObject;}}

    public override bool IsRunning
    {
        get
        {
            return _isRun;
        }
        set
        {
            if(!IsAIInput)_isRun = value;
            else
            {
                if(!value)
                {
                    if(IsAskedToRunByPlayable && !_getPlayableCharacterIdentity.GetFriendAIStateMachine.IsAIEngage && !_getPlayableCharacterIdentity.GetFriendAIStateMachine.GotDetectedbyEnemy && !IsAtCrouchPlatform)_isRun = IsAskedToRunByPlayable;
                    else _isRun = value;
                }
                else
                {
                    _isRun = value;
                }
            }
        }
    }
    public override bool IsCrouching
    {
        get
        {
            return _isCrouch;
        }
        set
        {
            if(!IsAIInput)_isCrouch = value;
            else
            {
                if(!value)
                {
                    if(IsAskedToCrouchByPlayable && !_getPlayableCharacterIdentity.GetFriendAIStateMachine.IsAIEngage && !_getPlayableCharacterIdentity.GetFriendAIStateMachine.GotDetectedbyEnemy)_isCrouch = IsAskedToCrouchByPlayable;
                    else _isCrouch = value;
                }
                else
                {
                    _isCrouch = value;
                }
            }
        }
    }
    public float CrawlSpeed { get{return _crawlSpeed;}}
    public bool IsCrawling {  get {return _isCrawl;}
    set
        { 
        
            if(_isCrawl != value)OnIsCrawlingChange?.Invoke(value);
            _isCrawl = value;
        } 
    }

    public bool IsMustLookForward { get{return _isMustLookForward;} set{_isMustLookForward = value;} } // Kan kalo nembak gitu hrs liat ke depan selalu, jd is true
    public bool IsTakingCoverAtWall { get{return _isTakeCoverAtWall;} set{_isTakeCoverAtWall = value;} } 
    public bool IsWallTallerThanChara {get {return _isWallTallerThanChara;}}

    public CharacterController CC {get { return _cc;} }
    public Vector3 InputMovement { get{ return _inputMovement;} set{_inputMovement = value;}} // Getting Input Movement from playercontroller
    public PlayableCharacterIdentity GetPlayableCharacterIdentity{ get {return _getPlayableCharacterIdentity;}}
    public PlayableMakeSFX GetPlayableMakeSFX {get {return _getPlayableMakeSFX;}}
    public Action<bool> OnIsCrawlingChange;

    public override float AgentRadius
    {
        get 
        {
            return IsTakingCoverAtWall ?  0.1f :  _agentNavMesh.radius;
            
        }
    }

    #endregion
    protected override void Awake()
    {
        base.Awake();
        _animateCharaTransform = _animator.transform;
        _fovMachine = GetComponent<FOVMachine>();
        _originToLookAtWall = _fovMachine.GetFOVPoint;
        _getPlayableMakeSFX = GetComponentInChildren<PlayableMakeSFX>();

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
        GameObject go = new GameObject("tempCharaTransform");
        
        _tempAnimateCharaTransform = go.transform;
        _tempAnimateCharaTransform.parent = _charaGameObject;
        _tempAnimateCharaTransform.localPosition = _animateCharaTransform.transform.localPosition;

        _worldSoundManager = WorldSoundManager.Instance;
        _charaHeadColl = _getPlayableCharacterIdentity.GetFriendAIStateMachine.CharaHeadColl;
        _charaHeightBuffer = _getPlayableCharacterIdentity.GetFriendAIStateMachine.CharaHeightBuffer;
        _charaWidth = _getPlayableCharacterIdentity.GetFriendAIStateMachine.CharaWidth;

        OnIsTheSamePosition += OnIsTheSamePositionTakeCover;
        base.Start();
    }



    protected override void Update()
    {
        if(!_gm.IsGamePlaying()) _getPlayableMakeSFX.StopMovementTypeSFX();

        base.Update();
        GoingToTakeCover();
    }

    protected override void FixedUpdate() 
    {
        base.FixedUpdate();

        if(!_gm.IsGamePlaying()) return;
        NearWallTakeCoverHandler();
    }

    #region Move
    public override void Move()
    {
        if(!IsAIInput)
        {
            Vector3 movement = new Vector3(InputMovement.x, 0, InputMovement.y).normalized;
            if(!IsTakingCoverAtWall)MovePlayableChara(movement);
            else MovePlayableOnWall(movement);
        }
        else base.Move();

        if(IsRunning)
        {
            // Debug.Log(transform.position + " produce walk sound");
            _worldSoundManager.MakeSound(WorldSoundName.Walk, transform.position, _fovMachine.CharaEnemyMask);
            _getPlayableMakeSFX.PlayRunSFX();
        }
        else if(IsWalking)
        {
            _getPlayableMakeSFX.PlayWalkSFX();
        }
        else if(IsCrouching)
        {
            _getPlayableMakeSFX.PlayCrouchSFX();
        }
    }
    /// <summary>
    /// Move yang digunakan untuk kontrol dgn input dari player
    /// </summary>
    public override void ForceStopMoving()
    {
        if(!IsAIInput)ForceStopPlayable(); 
        else base.ForceStopMoving();

        if(PlayableCharacterManager.IsSwitchingCharacter)
        {
            SetCharaGameObjRotationToNormal();
            IsAskedToCrouchByPlayable = false;
            IsAskedToRunByPlayable = false;
        }
        if(IsMustLookForward)IsMustLookForward = false;
    }
    #endregion
    #region PlayableChara Only
    private void MovePlayableChara(Vector3 direction)
    {
        Vector3 flatForward = new Vector3(_playableLookTarget.forward.x, 0, _playableLookTarget.forward.z).normalized;
        Vector3 Facedir = flatForward * direction.z + _playableLookTarget.right * direction.x; 
        
        CC.SimpleMove(Facedir * _currSpeed);

        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, direction.x);
        CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, direction.z);

        if(!IsMustLookForward)RotatePlayableChara(Facedir);
        else RotatePlayableChara(flatForward);
    }
    private void MovePlayableOnWall(Vector3 direction)
    {
        Vector3 flatForward = new Vector3(_playableLookTarget.forward.x, 0, _playableLookTarget.forward.z).normalized;
        Vector3 Facedir = flatForward * direction.z + _playableLookTarget.right * direction.x; 

        Vector3 wallSurfaceDir = Vector3.Cross(_takeCoverDirection, Vector3.up).normalized;
        Vector3 wallMovementDir = Vector3.Project(Facedir, wallSurfaceDir);

        // Debug.Log(" Input " +direction + " Wallsurfacedir" + wallSurfaceDir + " TakeCover muvment" + wallMovementDir );
        // Debug.DrawRay(transform.position, wallMovementDir * 100f, Color.red, 1f, false);
        // Debug.DrawRay(transform.position, wallSurfaceDir * 100f, Color.blue, 1f, false);
        // Debug.DrawRay(transform.position, wallSurfaceDir * 100f, Color.blue, 1f, false);

        
        bool isGoingToLeft = false;
        // Debug.Log("DOT FWDNYA ADALA" + dotFwd);
    
        if(_isFrontBehind)
        {
            float dotFwd = Vector3.Dot(_chosenWallToTakeCover.transform.right, wallMovementDir);
            if(_isFront)
            {
                if(dotFwd <= 0)isGoingToLeft = false;
                else isGoingToLeft = true;
            }
            else
            {
                if(dotFwd >= 0)isGoingToLeft = false;
                else isGoingToLeft = true;
            }
            // Debug.Log(dotFwd + "dottt forward TakeCover muvment"+ isGoingToLeft);
        }
        else
        {
            float dotRight = Vector3.Dot(_chosenWallToTakeCover.transform.forward, wallMovementDir);
            if(!_isRight)
            {
                if(dotRight <= 0)isGoingToLeft = false;
                else isGoingToLeft = true;
            }
            else
            {
                if(dotRight >= 0)isGoingToLeft = false;
                else isGoingToLeft = true;
            }
            // Debug.Log(dotRight + "dottt right TakeCover muvment" + isGoingToLeft);
        }
        if(direction != Vector3.zero)
        {
            if(isGoingToLeft)CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_TAKECOVERPOS, 1);
            else CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_TAKECOVERPOS, -1);
            
        }
        

        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, wallMovementDir, out hit, _playerToWallMinDistance/2, _wallLayerMask))
        {
            if(hit.collider != _chosenWallToTakeCover)
            {
                SetTakeCoverWallData(hit);
                TakeCoverAtWall();
            }
        }
        else if(Physics.Raycast(transform.position + wallMovementDir * _takeCoverWallCheckerMultiplierBuffer, -_takeCoverDirection, out hit, _playerToWallMinDistance, _wallLayerMask))
        {
            // Debug.Log(hit.collider + " collider skrg adalahh");
            if(hit.collider != _chosenWallToTakeCover)
            {
                SetTakeCoverWallData(hit);
                TakeCoverAtWall();
            }
            else
            {
                // 
                if(hit.normal != _takeCoverDirection) //inikalo dinding di kanannya itu msh sambungan wall ini dan collider sama
                {
                    SetTakeCoverWallData(hit);
                    TakeCoverAtWall();
                }
            }
        }
        else
        {
            Vector3 wallOtherSideDir = Vector3.zero;
            if(!isGoingToLeft)wallOtherSideDir = Quaternion.Euler (0, -_takeCoverWallCheckerAngleBuffer,0) * -_takeCoverDirection;
            else wallOtherSideDir = Quaternion.Euler (0, _takeCoverWallCheckerAngleBuffer,0) * -_takeCoverDirection;
            // Debug.DrawRay(transform.position + wallMovementDir * 1.1f, wallOtherSideDir * _playerToWallMinDistance, Color.black, 1f, false);
            RaycastHit otherSideWall;
            if(Physics.Raycast(transform.position + wallMovementDir * (1+_takeCoverWallCheckerMultiplierBuffer), wallOtherSideDir, out otherSideWall, _playerToWallMinDistance, _wallLayerMask))
            {
                if(otherSideWall.normal != _takeCoverDirection) //inikalo dinding di kanannya itu msh sambungan wall ini dan collider sama
                {
                    SetTakeCoverWallData(otherSideWall);
                    TakeCoverAtWall();
                }

            }
            else
            {
                wallMovementDir = Vector3.zero;
            }
        }


        CC.SimpleMove(wallMovementDir * _currSpeed);

        // CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_HORIZONTAL, direction.x);
        // CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_VERTICAL, direction.z);

        _charaGameObject.forward = Vector3.Slerp(_charaGameObject.forward, _takeCoverDirection.normalized, Time.deltaTime * _rotateSpeed);
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
    public void RotateWhileReviving()
    {
        if(!_getPlayableCharacterIdentity.IsReviving)return;
        // Vector3 flatForward = new Vector3(_playableLookTarget.forward.x, 0, _playableLookTarget.forward.z).normalized;
        Vector3 dir = (_getPlayableCharacterIdentity.GetFriendBeingRevivedPos.position - _charaGameObject.position).normalized;
        dir = new Vector3(dir.x, 0, dir.z);
        Vector3 newDir = Quaternion.Euler (0, _takeCoverWallCheckerAngleBuffer,0) * dir;

        // Debug.DrawRay(transform.position, dir, Color.red, 2f, false);
        // Debug.DrawRay(transform.position, newDir, Color.black, 2f, false);

        if(!IsAIInput)
        {
            
            RotatePlayableChara(newDir);

            
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    private void ForceStopPlayable()
    {
        InputMovement = Vector3.zero;
        // if(PlayableCharacterManager.IsSwitchingCharacter)SetCharaGameObjRotationToNormal();

        IsWalking = false;
        IsRunning = false;

        // IsCrouching = false; //biar kalo lg crouch di tmpt ga tb tb berdiri
    }
    public void SetCharaGameObjRotationToNormal()
    {
        if(_getPlayableCharacterIdentity.IsSilentKilling)
        {
            transform.rotation = _charaGameObject.rotation;
            _charaGameObject.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            _charaGameObject.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    #endregion

    #region Movement Speed

    public override void InitializeMovementSpeed(float speed)
    {
        base.InitializeMovementSpeed(speed);
        
        _crawlSpeed = _walkSpeed * _crawlSpeedMultiplier;
    }
    #endregion
    private void CharaIdentity_OnIsPlayerInputChange(bool isPlayerInput)
    {
        _isAIInput = !isPlayerInput;
        if(_isAIInput)
        {
            if(IsTakingCoverAtWall)
            {
                ExitTakeCover();
            }
        }
    }

    private void SetCharaConHeight(CharaControllerData data)
    {
        _cc.center = data.center;
        _cc.radius = data.radius;
        _cc.height = data.height;
    }

    #region TakeCoverAtWall
    public void NearWallTakeCoverHandler()
    {
        if(Physics.Raycast(transform.position, _charaGameObject.forward.normalized, out _wallTakeCoverHit, _playerToWallMinDistance, _wallLayerMask))
        {
            
        }
        KeybindUIHandler.OnShowTakeCoverKeybind(_wallTakeCoverHit.collider ? IsTakingCoverAtWall ? false : true : false);
    }
    public bool CanTakeCover()
    {
        if(_wallTakeCoverHit.collider != null)
        {
            SetTakeCoverWallData(_wallTakeCoverHit);
            
            return true;
        }
        return false;
    }
    public void TakeCoverAtWall()
    {
        if(_isGoingToTakeCover)return;
        IsTakingCoverAtWall = true;
        _isGoingToTakeCover = true;
        _isAtTakingCoverPos = false;

        _isAIInput = true;
        if(!IsWallTallerThanChara)
        {
            IsCrouching = true;
            PlayableCharacterCameraManager.Instance.SetCameraCrouchHeight();
        }
        
        SetAITargetToLook(-_takeCoverDirection, true);
        AllowLookTarget = true;
        SetAIDirection(_takeCoverPosition);
    }
    public void ExitTakeCover()
    {
        AgentNavMesh.ResetPath();
        _isAIInput = false;
        IsTakingCoverAtWall = false;
        CharaAnimator?.SetBool(ANIMATION_MOVE_PARAMETER_TAKECOVER, false);
        _isAtTakingCoverPos = false;
        _isGoingToTakeCover = false;
        AllowLookTarget = false;
        _animateCharaTransform.localPosition = new Vector3(0, _animateCharaTransform.localPosition.y, 0);
    }
    private void GoingToTakeCover()
    {
        if(IsTakingCoverAtWall &&_isGoingToTakeCover && !PlayableCharacterManager.IsSwitchingCharacter)
        {
            _charaGameObject.forward = Vector3.Slerp(_charaGameObject.forward, _takeCoverDirection.normalized, Time.deltaTime * _rotateSpeed);
            float dot = Vector3.Dot(_charaGameObject.forward, _takeCoverDirection.normalized);

            if(dot >= 0.99f && _isAtTakingCoverPos)
            {

                _animateCharaTransform.localPosition = new Vector3(0, _animateCharaTransform.localPosition.y, 0);
                _animateCharaTransform.localPosition -= _tempAnimateCharaTransform.InverseTransformDirection(_takeCoverDirection) * _takeCoverAnimateCharaPosMultiplierBuffer;
                
                AgentNavMesh.ResetPath();
                if(CharaAnimator?.GetBool(ANIMATION_MOVE_PARAMETER_TAKECOVER) == false)
                {
                    CharaAnimator?.SetFloat(ANIMATION_MOVE_PARAMETER_TAKECOVERPOS, 0);
                    // Debug.Log("eh ? ga masuk sini?");
                    CharaAnimator?.SetBool(ANIMATION_MOVE_PARAMETER_TAKECOVER, true);
                }
                AllowLookTarget = false;
                _isGoingToTakeCover = false;
                _isAtTakingCoverPos = false;
                _isAIInput = false;
            }
        }
    }
    private void OnIsTheSamePositionTakeCover(Vector3 vector)
    {
        if(IsTakingCoverAtWall && _isGoingToTakeCover)
        {
            if(vector == _takeCoverPosition)
            {
                _isAtTakingCoverPos = true;
            }
        }
    }
    #endregion
    public Vector3 GetCharaGameObjectFaceDir()
    {
        return _charaGameObject.transform.forward;
    }
    private void SetTakeCoverWallData(RaycastHit hit)
    {
        _isWallTallerThanChara = hit.collider.bounds.max.y > _charaHeadColl.bounds.max.y + _charaHeightBuffer;
        _chosenWallToTakeCover = hit.collider;
        _takeCoverPosition = hit.point;
        _takeCoverDirection = hit.normal;
        _wallWidth = _chosenWallToTakeCover.transform.lossyScale.x;
        _wallLength = _chosenWallToTakeCover.transform.lossyScale.z;

        float dotForward = Vector3.Dot(_takeCoverDirection, _chosenWallToTakeCover.transform.forward);
        if(Mathf.Abs(dotForward) >= 1)
        {
            _isFrontBehind = true;
            _isRight = false;
            if(dotForward >= 1)_isFront = true;
            else if(dotForward >= -1)_isFront = false;
        }
        else
        {
            float dotRight = Vector3.Dot(_takeCoverDirection, _chosenWallToTakeCover.transform.right);
            _isFrontBehind = false;
            _isFront = false;
            if(dotRight >= 1)_isRight = true;
            else if(dotRight >= -1)_isRight = false;
        }
    }
    
    public override void CharaConDataToNormal()
    {
        base.CharaConDataToNormal();
        ChangeCharaConData(_normalHeightCharaCon);
    }
    public override void CharaConDataToCrouch()
    {
        base.CharaConDataToCrouch();
        ChangeCharaConData(_crouchHeightCharaCon);
    }
    public void CharaConDataToCrawl()
    {
        ChangeNavMeshData(_crawlHeightCharaCon);
        ChangeCharaConData(_crawlHeightCharaCon);
    }

    private void ChangeCharaConData(CharaControllerData newData)
    {
        // Debug.Log("lewat siniii");
        _cc.center = newData.center;
        _cc.radius = newData.radius;
        _cc.height = newData.height;
    }


    public override bool IsHeadHitWhenUnCrouch()
    {
        if(!IsAIInput)
        {
            // Debug.DrawRay(_feetBasePoint.position, _feetBasePoint.up * _normalHeightCharaCon.height, Color.black, 0.5f);
            if(Physics.Raycast(_feetBasePoint.position, _feetBasePoint.up, out RaycastHit hitHead, _normalHeightCharaCon.height, _crouchPlatformLayer))
            {
                return true;
            }
            return false;
        }
        else
        {
            return base.IsHeadHitWhenUnCrouch();
        }
    }

    public void UnsubscribeEvent()
    {
        _canReceivePlayerInput.OnIsPlayerInputChange -= CharaIdentity_OnIsPlayerInputChange;
        OnIsTheSamePosition -= OnIsTheSamePositionTakeCover;
    }
}

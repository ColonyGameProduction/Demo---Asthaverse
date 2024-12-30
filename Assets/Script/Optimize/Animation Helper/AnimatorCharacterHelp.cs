using System;

using UnityEngine;

public class AnimatorCharacterHelp : MonoBehaviour, IUnsubscribeEvent
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterIdentity _characterIdentity;
    [SerializeField] private PlayableCharacterIdentity _playableCharaIdentity;
    [SerializeField] private UseWeaponStateMachine _useWeaponStateMachine;
    [SerializeField] private PlayableUseWeaponStateMachine _playableUseWeaponStateMachine;
    [SerializeField] private WeaponGameObjectDataContainer _weaponGameObjectDataContainer;
    [SerializeField] private PlayableInteraction _playableInteraction;
    private bool wasDeath;

    #region FEET IK VARIABLE

    [Header("Feet IK Ground")]
    // public bool _enableIKFeature = true, _enableIKProFeature = true, _showDebug = true;
    [SerializeField][Range(0, 2f)] private float _distanceToGround = 1.14f;
    [SerializeField][Range(0, 2f)] private float _raycastDownDistance = 1.5f;
    [SerializeField][Range(0, 1f)] private float _leftRotationWeight = 1;
    [SerializeField][Range(0, 1f)] private float _rightRotationWeight = 1;
    [SerializeField] private LayerMask _placeToWalkLayer;
    private bool _isCrawl;
    // [SerializeField] private float _pelvisOffset = 0f;
    // [SerializeField][Range(0, 1f)] private float _pelvisUpDownSpeed = 0.28f;
    // [SerializeField][Range(0, 1f)] private float _feetToLKPosSpeed = 0.5f;

    private const string LEFTFOOTANIM_VARIABLENAME = "LeftIKRotation";
    private const string RIGHTTFOOTANIM_VARIABLENAME = "RightIKRotation";
    // private Vector3 _rightFootPos, _leftFootPos, _leftFootIKPos, _rightFootIKPos;
    // private Quaternion _leftFootIKRotation, _rightFootIKRotation;
    // private float _lastPelvisPositionY, _lastRightFootPosY, _lastLeftFootPosY;
    #endregion

    #region  Reload Weapon Variable
    [Header("Reload Weapon")]
    [SerializeField] private Transform _handReloadParent;
    private Transform _magCurrGunTransform;
    private Transform _magGunParent;
    private Vector3 _magOriginalLocalPos, _magOriginalLocalEulerAngles;
    [SerializeField] private Vector3 _magHandLocalPos, _magHandLocalEulerAngles;
    #endregion
    public GameObject target;
    public bool canDoIt;
    public GameObject _chara;
    
    private void Awake() 
    {
        _animator = GetComponent<Animator>();
        _useWeaponStateMachine = GetComponentInParent<UseWeaponStateMachine>();
        _useWeaponStateMachine.OnEnsuringReload += EnsuringReload;

        _playableUseWeaponStateMachine = _useWeaponStateMachine as PlayableUseWeaponStateMachine;
        _characterIdentity = GetComponentInParent<CharacterIdentity>();
        

        _playableCharaIdentity = _characterIdentity as PlayableCharacterIdentity;
        
        // Debug.Log(transform.name + "Apakah adayg namanya interaksi " + _playableInteraction);
        _weaponGameObjectDataContainer = GetComponentInChildren<WeaponGameObjectDataContainer>();
    }



    private void Start() 
    {
        if(_playableCharaIdentity != null) _playableCharaIdentity.GetPlayableMovementStateMachine.OnIsCrawlingChange += ChangeIsCrawling;

        if(_playableCharaIdentity) _playableInteraction = _playableCharaIdentity.GetPlayableInteraction;
    }
    private void Update() {
        Debug.Log(_characterIdentity.transform.name + "IK RIGHT HAND POS Di Update" + _animator.GetIKPosition(AvatarIKGoal.RightHand));
        if(target && canDoIt)
        {
            Vector3 dir = _chara.transform.forward;
            dir.y = 0;
            Vector3 pos = _animator.GetIKPosition(AvatarIKGoal.RightHand);
            target.transform.position = pos + 1.5f *dir;
        }
    }

    public void ChangeIsCrawling(bool value) => _isCrawl = value;


    public void ReloadWeaponFinish()
    {
        _useWeaponStateMachine.ReloadAnimationFinished();
    }
    public void AfterDeathAnim()
    {
        wasDeath = true;
        _characterIdentity.AfterFinishDeathAnimation();
    }
    public void AfterFinishReviveAnim()
    {
        Debug.Log("Halooo ??? revive???");
        _playableCharaIdentity.AfterFinishReviveAnimation();
    }
    public void StartKnockOutAnim()
    {
        if(wasDeath)
        {
            wasDeath = false;
            _playableCharaIdentity.AfterFinishKnockOutAnimation();
        }
    }

    #region FEET LK METHODS

    public void OnAnimatorIK(int layerIndex) 
    {
        Debug.Log(_characterIdentity.transform.name + "IK RIGHT HAND POS Di ON ANIMATOR IK" + _animator.GetIKPosition(AvatarIKGoal.RightHand));
        FeetHandGroundControl();


    }
    
    public void FeetHandGroundControl()
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

        if(!_isCrawl)
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _leftRotationWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _rightRotationWeight);
            
            // _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(LEFTFOOTANIM_VARIABLENAME));
            // _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat(RIGHTTFOOTANIM_VARIABLENAME));
        }
        else
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }
        

        Ray rayLeftFoot = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if(Physics.Raycast(rayLeftFoot, out RaycastHit hitLeft, _distanceToGround + _raycastDownDistance, _placeToWalkLayer))
        {
            Debug.Log("KAKI KIRIIIII");
            Vector3 newFootPos = hitLeft.point;
            newFootPos.y += _distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, newFootPos);
            
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitLeft.normal);
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward,hitLeft.normal)); 
            
            // _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hitLeft.normal) * transform.rotation); 
            // if(hitLeft.transform.CompareTag("Ground"))
            // {
            // }
        }

        Ray rayRightFoot = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if(Physics.Raycast(rayRightFoot, out RaycastHit hitRight, _distanceToGround + _raycastDownDistance, _placeToWalkLayer))
        {
            Debug.Log("KAKI KANNAAAN");
            Vector3 newFootPos = hitRight.point;
            newFootPos.y += _distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, newFootPos);
            
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitRight.normal);
            _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward,hitRight.normal));
            // _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hitRight.normal) * transform.rotation); 
            // if(hitRight.transform.CompareTag("Ground"))
            // {
            // }
        }

        if(_isCrawl)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftRotationWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rightRotationWeight);
            // _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(LEFTFOOTANIM_VARIABLENAME));
            // _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _animator.GetFloat(RIGHTTFOOTANIM_VARIABLENAME));

            Ray rayLeftHand = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftHand) + Vector3.up, Vector3.down);
            if(Physics.Raycast(rayLeftHand, out RaycastHit hitLeftH, _distanceToGround + _raycastDownDistance, _placeToWalkLayer))
            {
                if(hitLeftH.transform.CompareTag("Ground"))
                {
                    Debug.Log("KAKI KIRIIIII");
                    Vector3 newHandPos = hitLeftH.point;
                    newHandPos.y += _distanceToGround;
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, newHandPos);
                    
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitLeftH.normal);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(forward,hitLeftH.normal)); 
                    
                    // _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hitLeft.normal) * transform.rotation); 
                }
            }

            Ray rayRightHand = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightHand) + Vector3.up, Vector3.down);
            if(Physics.Raycast(rayRightHand, out RaycastHit hitRightH, _distanceToGround + _raycastDownDistance, _placeToWalkLayer))
            {
                if(hitRightH.transform.CompareTag("Ground"))
                {
                    Debug.Log("KAKI KANNAAAN");
                    Vector3 newHandPos = hitRightH.point;
                    newHandPos.y += _distanceToGround;
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, newHandPos);
                    
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitRightH.normal);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(forward,hitRightH.normal));
                    // _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hitRight.normal) * transform.rotation); 
                }
            }

        }
        else
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }
    }
    // public void OnAnimatorIK(int layerIndex) 
    // {
    //     if(!_enableIKFeature || _animator == null) return;

    //     // MovePelvisHeight();

    //     _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
    //     _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);

    //     if(_enableIKProFeature)
    //     {
    //         _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat(RIGHTTFOOTANIM_VARIABLENAME));
    //         _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(LEFTFOOTANIM_VARIABLENAME));
    //     }

    //     MoveFeetToIKPoint(AvatarIKGoal.RightFoot, _rightFootIKPos, _rightFootIKRotation, ref _lastRightFootPosY);
    //     MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, _leftFootIKPos, _leftFootIKRotation, ref _lastLeftFootPosY);

    // }

    // private void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 posIKHolder, Quaternion rotationIKHolder, ref float lastFootPosY)
    // {
    //     Vector3 targetIKPos = _animator.GetIKPosition(foot);
    //     if(posIKHolder != Vector3.zero)
    //     {
    //         targetIKPos = transform.InverseTransformPoint(targetIKPos);
    //         posIKHolder = transform.InverseTransformPoint(posIKHolder);

    //         float newYPoint = Mathf.Lerp(lastFootPosY, posIKHolder.y, _feetToLKPosSpeed);

    //         targetIKPos.y += newYPoint;
    //         lastFootPosY = newYPoint;
    //         targetIKPos = transform.TransformPoint(targetIKPos);

    //         _animator.SetIKRotation(foot, rotationIKHolder);
    //     }

    //     _animator.SetIKPosition(foot, targetIKPos);
    // }
    // private void MovePelvisHeight()
    // {
    //     if(_rightFootIKPos == Vector3.zero || _leftFootIKPos == Vector3.zero || _lastPelvisPositionY == 0)
    //     {
    //         _lastPelvisPositionY = _animator.bodyPosition.y;
    //         return;
    //     }

    //     float leftOffsetPos = _leftFootIKPos.y - transform.position.y;
    //     float rightOffsetPos = _rightFootIKPos.y - transform.position.y;

    //     float totalOffest = (leftOffsetPos < rightOffsetPos) ? leftOffsetPos : rightOffsetPos;

    //     Vector2 newPelvisPos = _animator.bodyPosition + Vector3.up * totalOffest;
    //     newPelvisPos.y = Mathf.Lerp(_lastPelvisPositionY, newPelvisPos.y, _pelvisUpDownSpeed);
    //     _animator.bodyPosition = newPelvisPos;
    //     _lastPelvisPositionY = _animator.bodyPosition.y;
    // }
    // private void FeetPosSolver(Vector3 fromSkyPos, ref Vector3 feetIKPos, ref Quaternion feetIKRotation)
    // {
    //     //raycast
    //     RaycastHit feetHit;

    //     if(_showDebug)Debug.DrawRay(fromSkyPos, Vector3.down * (_raycastDownDistance + _distanceToGround), Color.red);

    //     if(Physics.Raycast(fromSkyPos, Vector3.down, out feetHit, _raycastDownDistance + _distanceToGround, _placeToWalkLayer))
    //     {
    //         feetIKPos = fromSkyPos;
    //         feetIKPos.y = feetHit.point.y + _pelvisOffset;
    //         feetIKRotation = Quaternion.FromToRotation(Vector3.up, feetHit.normal) * transform.rotation;
    //         return;
    //     }
    //     feetIKPos = Vector3.zero;
    // }
    // private void AdjustFeetTarget(ref Vector3 feetPos, HumanBodyBones foot)
    // {
    //     feetPos = _animator.GetBoneTransform(foot).position;
    //     feetPos.y = transform.position.y + _distanceToGround;
    // }
    #endregion

    #region Reload Weapon Methods

    public void ChangeMagParentToNormal()
    {
        _magOriginalLocalPos = _weaponGameObjectDataContainer.GetCurrWeaponOriLocalPos();
        _magOriginalLocalEulerAngles = _weaponGameObjectDataContainer.GetCurrWeaponOriEulerAngles();
        _magGunParent = _weaponGameObjectDataContainer.GetCurrWeaponMagParent();
        ChangeMagParent(_magGunParent, _magOriginalLocalPos, _magOriginalLocalEulerAngles);
    }
    public void ChangeMagParentToHand()
    {
        _magCurrGunTransform = _weaponGameObjectDataContainer.GetCurrWeaponMagTransform();
        if(_magCurrGunTransform == null)return;
        
        
        ChangeMagParent(_handReloadParent, _magHandLocalPos, _magHandLocalEulerAngles);
    }
    private void ChangeMagParent(Transform parent, Vector3 newLocalPos, Vector3 newLocalEulerAngles)
    {
        if(_magCurrGunTransform == null || parent == null)return;
        _magCurrGunTransform.parent = parent;
        _magCurrGunTransform.localPosition = newLocalPos;
        _magCurrGunTransform.localEulerAngles = newLocalEulerAngles;
    }

    public void HideMag()
    {
        if(_magCurrGunTransform == null)return;
        // Debug.Log("Hide Mag");
        _magCurrGunTransform.gameObject.SetActive(false);
    } 
    public void ShowMag()
    {
        if(_magCurrGunTransform == null)return;
        // Debug.Log("Show Mag");
        _magCurrGunTransform.gameObject.SetActive(true);
    } 

    private void EnsuringReload()
    {
        ChangeMagParentToNormal();
        ShowMag();
    }
    #endregion

    #region Switch Weapon Methods
    public void HideCurrWeapon()
    {
        _weaponGameObjectDataContainer.HideCurrWeapon();
    }
    public void SwitchingWeapon()
    {
        _playableCharaIdentity.SwitchWeapon();
    }
    public void SwitchWeaponFinished()
    {
        _playableUseWeaponStateMachine.SwitchWeaponAnimationFinished();
    }

    #endregion

    #region Picking Up Item
    public void PickUpItem()
    {
        _playableInteraction.AddHeldObject();
    }
    public void ThrowItem()
    {
        _playableInteraction.ThrowHeldObject();
    }
    public void ExitThrowAnim()
    {
        _playableCharaIdentity.ExitThrowInteractionAnimation();
    }
    #endregion

    public void UnsubscribeEvent()
    {
        _useWeaponStateMachine.OnEnsuringReload -= EnsuringReload;
        if(_playableCharaIdentity != null) _playableCharaIdentity.GetPlayableMovementStateMachine.OnIsCrawlingChange -= ChangeIsCrawling;
    }

}


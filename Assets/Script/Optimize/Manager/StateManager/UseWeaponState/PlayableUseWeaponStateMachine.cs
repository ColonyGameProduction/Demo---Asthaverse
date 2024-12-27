using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class PlayableUseWeaponStateMachine : UseWeaponStateMachine, IAdvancedUseWeaponData, IPlayableUseWeaponDataNeeded, IUnsubscribeEvent
{
    #region Normal Variable
    [Header ("Playable Character Variable")]

    protected PlayableCharacterCameraManager _charaCameraManager;
    [Space(2)]
    [Header("State Bool - Advanced use")]
    [ReadOnly(true), SerializeField] protected bool _isSwitchingWeapon;
    protected bool _canSwitchWeapon = true;
    [SerializeField] protected float _switchWeaponDuration; // ini sementara ampe ada animasi
    [ReadOnly(true), SerializeField] protected bool _isSilentKill;
    protected bool _canSilentKill = true;
    [SerializeField] protected float _silentKillDuration; // ini sementara ampe ada animasi
    private EnemyIdentity _silentKilledEnemy;

    [Space(1)]
    [Header("Additional Animation Component")]
    [SerializeField] protected Rig _rigControllerPistol;
    [SerializeField] protected Rig _rigHandFollowRifle, _rigHandFollowPistol;
    [SerializeField] private MultiAimConstraint _bodyRigConstraintDataRifle;
    [SerializeField] private MultiAimConstraint _aimRigConstraintDataRifle;
    [SerializeField] private MultiAimConstraint _bodyRigConstraintDataPistol;
    [SerializeField] private MultiAimConstraint _aimRigConstraintDataPistol;
    private bool _isChangeInUpdate;

    [Space(1)]
    [Header("Saving other component data")]
    protected IReceiveInputFromPlayer _getCanInputPlayer;
    protected ICanSwitchWeapon _getCanSwitchWeapon;
    protected Camera _mainCamera;

    [Space(1)]
    [Header("More Weapon Logic Data")]
    protected float _charaFriendAimAccuracy;

    protected PlayableCharacterIdentity _getPlayableCharacterIdentity;
    protected PlayerGunCollide _getPlayerGunCollide;
    [Header("Recoil Data Advanced")]
    [SerializeField] protected float _movingMaxRecoilOnScopeNotIdleBuffer = 0.5f, _movingMaxRecoilNotOnScopeIdleCrouch = 0.5f;
    [SerializeField] protected float _currRecoilModBufferOnScopeNotIdle = 0.25f, _currRecoilModBufferNotOnScopeNotIdle = 0.5f, _currRecoildModNotOnScopeIdleCrouch = 0.25f;
    [SerializeField] protected float _recoildAddMultiplierOnScopeNotIdle = 1.5f, _recoildAddMultiplierNotOnScopeNotIdle = 3f, _recoilAddMultiplierNotOnScopeIdleCrouch = 1.5f;
    protected float _movingMaxRecoil, _currRecoilMod, _recoilAddMultiplier;


    public const string ANIMATION_MOVE_PARAMETER_SWITCHWEAPON= "SwitchWeapon";
    public const string ANIMATION_GUN_COUNTER_PARAMETER = "GunCounter"; 
    #endregion
    #region GETTERSETTER Variable
    public override float CharaAimAccuracy
    {
        get {
            if(!IsAIInput)return _charaAimAccuracy;
            else return _charaFriendAimAccuracy;
        }
    }
    public bool IsSwitchingWeapon
    {
        get {return _isSwitchingWeapon;} 
        set{ 
            if(value)
            {
                if(_canSwitchWeapon)_isSwitchingWeapon = value;
            }
            else _isSwitchingWeapon = value;
        }
    }
    public bool CanSwitchWeapon { get {return _canSwitchWeapon;} set {_canSwitchWeapon = value;}}
    public float SwitchWeaponDuration { get {return _switchWeaponDuration;}}
    public bool IsSilentKill
    {
        get {return _isSilentKill;} 
        set{ 
            if(value)
            {
                if(_canSilentKill)_isSilentKill = value;
            }
            else _isSilentKill = value;
        }
    }
    public bool CanSilentKill { get {return _canSilentKill;} set {_canSilentKill = value;}}
    public float SilentKillDuration { get {return _silentKillDuration;}}
    public PlayableCharacterIdentity GetPlayableCharacterIdentity{ get {return _getPlayableCharacterIdentity;}}
    public PlayerGunCollide GetPlayerGunCollider {get {return _getPlayerGunCollide;} set { _getPlayerGunCollide = value;}} 

    #endregion
    public event Action OnTurningOffScope;// ini dipanggil kalo misal lg input player dan reload - yg subs adalah playablecharamanager
    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;
        CharaIdentity_OnIsPlayerInputChange(!IsAIInput);
        _getCanSwitchWeapon = GetComponent<ICanSwitchWeapon>();
        _getPlayableCharacterIdentity = _charaIdentity as PlayableCharacterIdentity;
        _getCanInputPlayer = GetComponent<IReceiveInputFromPlayer>();
        _isAIInput = !_getCanInputPlayer.IsPlayerInput;
        _getCanInputPlayer.OnIsPlayerInputChange += CharaIdentity_OnIsPlayerInputChange; // Ditaro di sini biar ga ketinggalan sebelah, krn sebelah diubah di start

    }
    protected override void Start()
    {
        _charaCameraManager = PlayableCharacterCameraManager.Instance;
        base.Start();
    }
    protected override void Update()
    {
        base.Update();

        if(_isChangeInUpdate)
        {
            _isChangeInUpdate = false;
            SetRigHandFollow();
        }
    }
    #region  Weapon
    public void SilentKill()
    {
        StartCoroutine(SilentKillActive(_silentKillDuration));
    }
    protected IEnumerator SilentKillActive(float reloadTime)
    {
        //DoAnimation
        yield return new WaitForSeconds(reloadTime);
        _silentKilledEnemy.GotSilentKilled();
        CanSilentKill = false;
        IsSilentKill = false;
        
    }
    public void CanSilentKill_Coroutine()
    {
        StartCoroutine(CanSilentKillNow());
    }
    protected IEnumerator CanSilentKillNow()
    {
        yield return new WaitForSeconds(_notAimSpamClickDelayTime);
        CanSilentKill = true;
    }

    public void SwitchWeapon()
    {
        StartCoroutine(SwitchWeaponActive(_switchWeaponDuration));
    }
    protected IEnumerator SwitchWeaponActive(float reloadTime)
    {
        //DoAnimation
        yield return new WaitForSeconds(reloadTime);
        _getCanSwitchWeapon.SwitchWeapon();
        SetCurrWeapon();

        CanSwitchWeapon = false;
        IsSwitchingWeapon = false;
        
    }
    public void SwitchWeaponAnimationFinished()
    {
        if(!IsSwitchingWeapon) return;
        _animator.SetBool(ANIMATION_MOVE_PARAMETER_SWITCHWEAPON, false);
        _isResetAnimTime = true;
        
        // _getCanSwitchWeapon.SwitchWeapon();
        if(_currWeapon == _charaIdentity.CurrWeapon)
        {
            _getCanSwitchWeapon.SwitchWeapon();
        }
        _getPlayableCharacterIdentity.SwitchAnimatorGunCounterToCurr();
        SetCurrWeapon();
        

        CanSwitchWeapon = false;
        IsSwitchingWeapon = false;
    }
    public void CanSwitchWeapon_Coroutine()
    {
        StartCoroutine(CanSwitchWeaponNow());
    }
    protected IEnumerator CanSwitchWeaponNow()
    {
        yield return new WaitForSeconds(_notAimSpamClickDelayTime);
        CanSwitchWeapon = true;
    }
    public override void ForceStopUseWeapon()
    {
        if(_currState == null)return;
        if(_currState == _states.AimWeaponState() || _currState == _states.UsingWeaponState())
        {
            if(IsSilentKill)IsSilentKill = false;
            if(IsSwitchingWeapon)IsSwitchingWeapon = false;
            if(IsReloading)IsReloading = false;
        }

        if(IsUsingWeapon)IsUsingWeapon = false;
        if(IsAiming)IsAiming = false;

        if(_currState == _states.ReloadWeaponState() && IsReloading && CanReload)
        {
            if(IsSilentKill)IsSilentKill = false;
            if(IsSwitchingWeapon)IsSwitchingWeapon = false;
        }
        else if(_currState == _states.SilentKillState() && IsSilentKill && CanSilentKill)
        {
            if(IsReloading)IsReloading = false;
            if(IsSwitchingWeapon)IsSwitchingWeapon = false;
        }
        else if(_currState == _states.SwitchingWeaponState() && IsSwitchingWeapon && CanSwitchWeapon)
        {
            if(IsSilentKill)IsSilentKill = false;
            if(IsReloading)IsReloading = false;
        }
    }
    
    protected override void SetShootPosition()
    {
        if(!IsAIInput)
        {
            _originShootPosition = CurrOriginShootPoint.position;
            _directionShootPosition = CurrDirectionShootPoint.forward.normalized;
            _gunOriginShootPosition = _gunOriginShootPoint.position;

            isGunInsideWall = _getPlayerGunCollide.IsInsideWall();
        }
        else
        {
            base.SetShootPosition();
            isGunInsideWall = false;
        }
        
    }
    
    #endregion
    private void CharaIdentity_OnIsPlayerInputChange(bool obj)
    {
        // Debug.Log("AAA" + transform.name);
        _isAIInput = !obj;
        if(!IsAIInput)
        {
            _currOriginShootPoint = _mainCamera.transform;
            _currDirectionShootPoint = _mainCamera.transform;

            SetConstraintData();
        }
        else
        {
            _currOriginShootPoint = _originShootPoint_AIContainer;
            _currDirectionShootPoint = _currChosenTarget;

            SetConstraintData();
        }
    }
    private void SetConstraintData()
    {
        var sourceObjectsData =_aimRigConstraintDataRifle.data.sourceObjects;
        sourceObjectsData.SetWeight(0, IsAIInput ? 0 : 1);
        sourceObjectsData.SetWeight(1, IsAIInput ? 1 : 0);

        _aimRigConstraintDataRifle.data.sourceObjects = sourceObjectsData;
        _bodyRigConstraintDataRifle.data.sourceObjects = sourceObjectsData;
        if(_aimRigConstraintDataPistol)_aimRigConstraintDataPistol.data.sourceObjects = sourceObjectsData;
        if(_bodyRigConstraintDataPistol)_bodyRigConstraintDataPistol.data.sourceObjects = sourceObjectsData;
    }
    public override void SetCurrWeapon()
    {
        base.SetCurrWeapon();
        SetRigHandFollow();
        _isChangeInUpdate = true;
        
    }
    private void SetRigHandFollow()
    {
        Debug.Log(transform.name + "Masuk sini " + _getPlayableCharacterIdentity.CurrWeaponIdx);
        if(_getPlayableCharacterIdentity.CurrWeaponIdx == 0)
        {
            if(_rigHandFollowRifle) _rigHandFollowRifle.weight = 1;
            if(_rigHandFollowPistol) _rigHandFollowPistol.weight = 0;
        }
        else
        {
            if(_rigHandFollowRifle) _rigHandFollowRifle.weight = 0;
            if(_rigHandFollowPistol) _rigHandFollowPistol.weight = 1;
        }
    }

    public override void ActivateRigAim()
    {
        if(_getPlayableCharacterIdentity.CurrWeaponIdx == 0)
        {
            base.ActivateRigAim();
            if(_rigControllerPistol)_rigControllerPistol.weight = 0;
        }
        else
        {
            base.DeactivateRigAim();
            if(_rigControllerPistol)_rigControllerPistol.weight = 1;
        }
    }
    public override void DeactivateRigAim()
    {
        base.DeactivateRigAim();
        if(_rigControllerPistol)_rigControllerPistol.weight = 0;
    }

    public void TellToTurnOffScope()
    {
        OnTurningOffScope?.Invoke();
    }
    public void SetCharaFriendAccuracy(float newAccuracy)
    {
        _charaFriendAimAccuracy = newAccuracy;
    }

    public void SetSilentKilledEnemy(EnemyIdentity enemy)
    {
        _silentKilledEnemy = enemy;
    }
    #region Recoil

    protected void MovingRecoilCount()
    {
        if(!_getPlayableCharacterIdentity.GetPlayableMovementStateMachine.IsIdle)
        {
            if(_charaCameraManager.IsScope)
            {
                _movingMaxRecoil = _currWeapon.weaponStatSO.recoil + _currWeapon.weaponStatSO.recoil * _movingMaxRecoilOnScopeNotIdleBuffer;
                _currRecoilMod = _currWeapon.weaponStatSO.recoil * _currRecoilModBufferOnScopeNotIdle;
                _recoilAddMultiplier = _recoildAddMultiplierOnScopeNotIdle;
            }
            else
            {
                _movingMaxRecoil = _currWeapon.weaponStatSO.recoil + _currWeapon.weaponStatSO.recoil;
                _currRecoilMod = _currWeapon.weaponStatSO.recoil * _currRecoilModBufferNotOnScopeNotIdle;
                _recoilAddMultiplier = _recoildAddMultiplierNotOnScopeNotIdle;
            }
        }
        else
        {
            if(_getPlayableCharacterIdentity.GetPlayableMovementStateMachine.IsCrouching)
            {
                if(_charaCameraManager.IsScope)
                {
                    _movingMaxRecoil = _currWeapon.weaponStatSO.recoil;
                    _currRecoilMod = _currWeapon.weaponStatSO.recoil;
                    _recoilAddMultiplier = 0f;
                }
                else
                {
                    _movingMaxRecoil = _currWeapon.weaponStatSO.recoil + _currWeapon.weaponStatSO.recoil * _movingMaxRecoilNotOnScopeIdleCrouch;
                    _currRecoilMod = _currWeapon.weaponStatSO.recoil * _currRecoildModNotOnScopeIdleCrouch;
                    _recoilAddMultiplier = _recoilAddMultiplierNotOnScopeIdleCrouch;
                }
            }
            else
            {
                _movingMaxRecoil = 0;
                _currRecoilMod = 0;
                _recoilAddMultiplier = 0;
            }
        }
    }
    
    protected override void RecoilHandler()
    {
        if(!IsAIInput)
        {   
            MovingRecoilCount();
            _recoilCoolDown = _currWeapon.weaponStatSO.fireRate + (_currWeapon.weaponStatSO.fireRate * _recoilCoolDownBuffer);
            _maxRecoil = _currWeapon.weaponStatSO.recoil + _movingMaxRecoil;
            _finalCountRecoil = _currRecoil + _currRecoilMod;
        }
        else base.RecoilHandler();
        
    }
    protected override void ComplexRecoil()
    {
        if(IsAIInput)base.ComplexRecoil();
        else
        {
            if (_recoilCoolDown > 0)
            {
                _recoilCoolDown -= Time.deltaTime * _currWeapon.weaponStatSO.fireRate;
                if (_currRecoil <= _maxRecoil)
                {
                    if(_recoilAddMultiplier != 0)
                    {
                        _currRecoil += Time.deltaTime * _currWeapon.weaponStatSO.recoil * _recoilAddMultiplier;
                    }
                    else
                    {
                        _currRecoil += Time.deltaTime * _currWeapon.weaponStatSO.recoil;
                    }
                }
            }
            else
            {
                _currRecoil = _currWeapon.weaponStatSO.recoil;;
            }

        }
    }
    #endregion

    public void UnsubscribeEvent()
    {
        _getCanInputPlayer.OnIsPlayerInputChange -= CharaIdentity_OnIsPlayerInputChange;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayableUseWeaponStateMachine : UseWeaponStateMachine, IAdvancedUseWeaponData, IPlayableUseWeaponDataNeeded
{
    #region Normal Variable
    [Header ("Playable Character Variable")]

    protected PlayableCharacterCameraManager _charaCameraManager;
    [Space(2)]
    [Header("State Bool - Advanced use")]
    [SerializeField] protected bool _isSwitchingWeapon;
    protected bool _canSwitchWeapon = true;
    [SerializeField] protected float _switchWeaponDuration; // ini sementara ampe ada animasi
    [SerializeField] protected bool _isSilentKill;
    protected bool _canSilentKill = true;
    [SerializeField] protected float _silentKillDuration; // ini sementara ampe ada animasi
    private EnemyIdentity _silentKilledEnemy;

    [Space(1)]
    [Header("Additional Animation Component")]
    [SerializeField] private MultiAimConstraint _bodyRigConstraintData;
    [SerializeField] private MultiAimConstraint _aimRigConstraintData;

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
    protected float _movingMaxRecoil, _currRecoilMod, _recoilAddMultiplier;

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
    

    #endregion
    public event Action OnTurningOffScope;// ini dipanggil kalo misal lg input player dan reload - yg subs adalah playablecharamanager
    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;
        CharaIdentity_OnIsPlayerInputChange(!IsAIInput);
        _getCanSwitchWeapon = GetComponent<ICanSwitchWeapon>();
        _getPlayableCharacterIdentity = _charaIdentity as PlayableCharacterIdentity;
        _getPlayerGunCollide = GetComponentInChildren<PlayerGunCollide>();
        _getCanInputPlayer = GetComponent<IReceiveInputFromPlayer>();
        _isAIInput = !_getCanInputPlayer.IsPlayerInput;
        _getCanInputPlayer.OnIsPlayerInputChange += CharaIdentity_OnIsPlayerInputChange; // Ditaro di sini biar ga ketinggalan sebelah, krn sebelah diubah di start

    }
    protected override void Start()
    {
        _charaCameraManager = PlayableCharacterCameraManager.Instance;
        base.Start();
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
            if(IsSwitchingWeapon)IsSwitchingWeapon = false;
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

            var sourceObjectsData =_aimRigConstraintData.data.sourceObjects;
            sourceObjectsData.SetWeight(0, 1f);
            sourceObjectsData.SetWeight(1, 0f);
            _aimRigConstraintData.data.sourceObjects = sourceObjectsData;
            _bodyRigConstraintData.data.sourceObjects = sourceObjectsData;
        }
        else
        {
            _currOriginShootPoint = _originShootPoint_AIContainer;
            _currDirectionShootPoint = _currChosenTarget;

            var sourceObjectsData =_aimRigConstraintData.data.sourceObjects;
            sourceObjectsData.SetWeight(0, 0f);
            sourceObjectsData.SetWeight(1, 1f);
            _aimRigConstraintData.data.sourceObjects = sourceObjectsData;
            _bodyRigConstraintData.data.sourceObjects = sourceObjectsData;
        }
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
        if(!_getPlayableCharacterIdentity.GetPlayableMovementData.IsIdle)
        {
            if(_charaCameraManager.IsScope)
            {
                _movingMaxRecoil = _currWeapon.weaponStatSO.recoil + _currWeapon.weaponStatSO.recoil * 0.5f;
                _currRecoilMod = _currWeapon.weaponStatSO.recoil * 0.25f;
                _recoilAddMultiplier = 1.5f;
            }
            else
            {
                _movingMaxRecoil = _currWeapon.weaponStatSO.recoil + _currWeapon.weaponStatSO.recoil;
                _currRecoilMod = _currWeapon.weaponStatSO.recoil * 0.5f;
                _recoilAddMultiplier = 3f;
            }
        }
        else
        {
            if(_getPlayableCharacterIdentity.GetPlayableMovementData.IsCrouching)
            {
                if(_charaCameraManager.IsScope)
                {
                    _movingMaxRecoil = _currWeapon.weaponStatSO.recoil;
                    _currRecoilMod = _currWeapon.weaponStatSO.recoil;
                    _recoilAddMultiplier = 0f;
                }
                else
                {
                    _movingMaxRecoil = _currWeapon.weaponStatSO.recoil + _currWeapon.weaponStatSO.recoil * 0.5f;
                    _currRecoilMod = _currWeapon.weaponStatSO.recoil * 0.25f;
                    _recoilAddMultiplier = 1.5f;
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
            _recoilCoolDown = _currWeapon.weaponStatSO.fireRate + (_currWeapon.weaponStatSO.fireRate * 0.1f);
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
                _currRecoil = 0;
            }

        }
    }
    #endregion
}

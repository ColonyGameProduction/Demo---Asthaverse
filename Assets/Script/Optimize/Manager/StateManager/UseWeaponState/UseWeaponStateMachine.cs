using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class UseWeaponStateMachine : CharacterStateMachine, IUseWeapon, INormalUseWeaponData
{
    #region Normal Variable
    protected GameManager _gm;
    [Space(1)]
    [Header("Use Weapon States - Normal")]
    [ReadOnly(true), SerializeField] protected bool _isIdle = true;
    [ReadOnly(true), SerializeField] protected bool _isAiming;
    [ReadOnly(true), SerializeField] protected bool _isUsingWeapon;
    [ReadOnly(true), SerializeField] protected bool _isReloading;
    protected UseWeaponStateFactory _states;
    protected UseWeaponState _currState;

    [Header("Animator Component")]
    [FormerlySerializedAs("_rigController")][SerializeField] protected Rig _rigControllerRifle;
    
    protected int _currActiveAnimLayer;
    protected float _currAnimTIme;
    protected bool _isResetAnimTime;


    [Space(1)]
    [Header("Spam Weapon State Change Delay Time")]
    [SerializeField] protected float _notAimSpamClickDelayTime = 0.1f;
    protected bool _canReload = true;
    protected bool _isfireRateOn;

    [Space(1)]
    [Header("Weapon Logic Component")]
    protected float _charaAimAccuracy;
    protected Transform _currChosenTarget;
    protected WeaponData _currWeapon;

    [Space(1)]
    [Header("ShootPoint - AI")]
    [Tooltip("Kalo AI isinya FOV, kalo player gausa isi krn ud lsg diisi camera.main")]
    protected Transform _originShootPoint_AIContainer; 
    protected Transform _currOriginShootPoint, _currDirectionShootPoint;
    protected Vector3 _originShootPosition, _directionShootPosition, _gunOriginShootPosition;
    protected bool isGunInsideWall = false;
    protected Transform _FOVPoint;
    protected Transform _gunOriginShootPoint;

    [Space(1)]
    [Header("Enemy Character Layermask")]
    [SerializeField] protected LayerMask _charaEnemyMask;

    [Space(1)]
    [Header("Saving other component data")]

    protected WeaponLogicManager _weaponLogicManager;
    protected WeaponShootVFX _weaponShootVFX;

    public Action OnWasUsingGun;
    public Action OnEnsuringReload;

    [Header("Recoil Data")]
    [SerializeField]protected float _recoilCoolDownBuffer = 0.1f;
    [ReadOnly(false), SerializeField] protected float _currRecoil, _maxRecoil, _recoilCoolDown;
    [ReadOnly(false), SerializeField] protected float _finalCountRecoil;
    
    public const string ANIMATION_MOVE_PARAMETER_RELOAD = "Reload";
    
    #endregion
    
    #region GETTERSETTER Variable
    public bool IsIdle { get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsAiming { get {return _isAiming;} set{ _isAiming = value;} }
    public bool IsUsingWeapon 
    { 
        get {return _isUsingWeapon;} 
        set{ 
            if(value) 
            {
                if(CurrWeapon.currBullet > 0)_isUsingWeapon = value;
            }
            
            else _isUsingWeapon = value;
            
        } 
    }
    public bool IsReloading 
    { 
        get {return _isReloading;} 
        set{ 
            if(value)
            {
                if(CurrWeapon.totalBullet > 0)
                {
                    if(_canReload)_isReloading = value;
                }
            }
            else _isReloading = value;
        }
    }
    
    public bool CanReload { get {return _canReload;} set {_canReload = value;}}
    public bool IsFireRateOn { get{return _isfireRateOn;} set {_isfireRateOn = value;}}
    public bool HasNoMoreBullets 
    {
        get {
            if(_currWeapon.totalBullet == 0 && _currWeapon.currBullet == 0)return true;
            return false;
        }
    }

    public WeaponData CurrWeapon{get{return _currWeapon;} }
    public Transform ChosenTarget { get {return _currChosenTarget;}}
    public virtual float CharaAimAccuracy { get { return _charaAimAccuracy; }}

    public Transform CurrOriginShootPoint { get{return _currOriginShootPoint;}}
    public Transform CurrDirectionShootPoint { get{return _currDirectionShootPoint;}}
    public Transform GunOriginShootPoint { get{return _gunOriginShootPoint;} set{_gunOriginShootPoint = value;}} 
    public int CurrActiveAnimLayer { get {return _currActiveAnimLayer;}}
    public float CurrAnimTime {get {return _currAnimTIme;}set {_currAnimTIme = value;}}
    public bool IsResetAnimTime {get {return _isResetAnimTime;} set {_isResetAnimTime = value;}}
    public LayerMask CharaEnemyMask {get{return _charaEnemyMask;}}

    #endregion

    protected override void Awake()
    {
        base.Awake();

        if(_originShootPoint_AIContainer == null)_originShootPoint_AIContainer = GetComponent<FOVMachine>().GetFOVPoint;
        
        if(IsAIInput)
        {
            _currOriginShootPoint = _originShootPoint_AIContainer;
            _currDirectionShootPoint = _currChosenTarget;
        }
        _states = new UseWeaponStateFactory(this);
    }
    protected virtual void Start() 
    {
        _gm = GameManager.instance;

        _weaponShootVFX = _charaIdentity.GetWeaponShootVFX;
        _weaponLogicManager = WeaponLogicManager.Instance;

        SetCurrWeapon();
        
        SwitchState(_states.IdleWeaponState());
    }
    protected virtual void Update()
    {
        if(!_gm.IsGamePlaying()) return;

        _currState?.UpdateState();
    }
    private void FixedUpdate() 
    {
        if(!_gm.IsGamePlaying()) return;

        ComplexRecoil();
        _currState?.PhysicsLogicUpdateState();
    }

    public override void SwitchState(BaseState newState)
    {
        if(_currState != null)
        {
            _currState?.ExitState();
        }
        _currState = newState as UseWeaponState;
        _currState?.EnterState();
    }
    
    #region UseWeapon
    public virtual void UseWeapon()
    {
        Shoot();
    }

    protected void Shoot()
    {
        if(CurrWeapon.currBullet > 0 && !_isfireRateOn)
        {
            RecoilHandler();
            SetShootPosition();
            _weaponLogicManager.ShootingPerformed(this.transform, _originShootPosition, _directionShootPosition, CharaAimAccuracy, CurrWeapon.weaponStatSO, _charaEnemyMask, _finalCountRecoil, _gunOriginShootPosition, IsAIInput, isGunInsideWall, _weaponShootVFX);
            _charaMakeSFX.PlayShootSFX();

            UseBullet();
            if(!CurrWeapon.weaponStatSO.allowHoldDownButton)IsUsingWeapon = false;
            StartCoroutine(FireRate(CurrWeapon.weaponStatSO.fireRate));
            if (CurrWeapon.currBullet == 0)
            {
                if(CurrWeapon.totalBullet > 0)
                {
                    if(!CanReload)CanReload = true;
                    IsReloading = true;
                }
                else 
                {
                    IsUsingWeapon = false;
                }
                
            }
            
        }
    }
    protected virtual void UseBullet()
    {
        CurrWeapon.currBullet -= 1;
    }
    protected virtual void SetShootPosition()
    {
        _originShootPosition = CurrOriginShootPoint.position;
        // _directionShootPosition = (CurrDirectionShootPoint.position - transform.position).normalized;

        _directionShootPosition = (CurrDirectionShootPoint.position - CurrOriginShootPoint.position).normalized;
        _gunOriginShootPosition = _gunOriginShootPoint.position;

        // Debug.Log("Shoot direction " + _directionShootPosition);
    }

    protected IEnumerator FireRate(float fireRateTime)
    {
        _isfireRateOn = true;

        yield return new WaitForSeconds(fireRateTime);

        _isfireRateOn = false;
    }

    public void ReloadWeapon()
    {
        StartCoroutine(ReloadWeaponActive(CurrWeapon.weaponStatSO.reloadTime));
    }
    protected IEnumerator ReloadWeaponActive(float reloadTime)
    {
        //DoAnimation
        yield return new WaitForSeconds(reloadTime);
        ReloadAnimationFinished();
    }
    public void ReloadAnimationFinished()
    {
        if(!_isReloading) return;
        _animator.SetBool(ANIMATION_MOVE_PARAMETER_RELOAD, false);
        _isResetAnimTime = true;
        _charaIdentity.ReloadWeapon();
        CanReload = false;
        IsReloading = false;
    }
    public void EnsureReloadWeapon()
    {
        _charaIdentity.ReloadWeapon();
        OnEnsuringReload?.Invoke();
    }
    
    public void CanReloadWeapon_Coroutine()
    {
        StartCoroutine(CanReloadWeapon());
    }
    protected IEnumerator CanReloadWeapon()
    {
        yield return new WaitForSeconds(_notAimSpamClickDelayTime);
        CanReload = true;
    }

    public virtual void ForceStopUseWeapon()
    {
        if(_currState == _states.AimWeaponState() || _currState == _states.UsingWeaponState())
        {
            if(IsReloading)IsReloading = false;
        }
        if(IsUsingWeapon)IsUsingWeapon = false;
        if(IsAiming)IsAiming = false;
    }
    public void GiveChosenTarget(Transform chosenTarget)
    {
        _currChosenTarget = chosenTarget;
        _currDirectionShootPoint = _currChosenTarget;
    }

    #endregion

    public virtual void SetCurrWeapon() => _currWeapon = _charaIdentity.CurrWeapon;// pas ganti weapon, ini dipanggil
    public virtual void ActivateRigAim() => _rigControllerRifle.weight = 1;
    public virtual void DeactivateRigAim() => _rigControllerRifle.weight = 0;

    public void SetCharaAimAccuracy(float newAccuracy) => _charaAimAccuracy = newAccuracy;

    #region Recoil
    protected virtual void RecoilHandler()
    {
        _recoilCoolDown = _currWeapon.weaponStatSO.fireRate + (_currWeapon.weaponStatSO.fireRate * _recoilCoolDownBuffer);
        _maxRecoil = _currWeapon.weaponStatSO.recoil;
        _finalCountRecoil = _currRecoil;
    }
    protected virtual void ComplexRecoil()
    {
        if (_recoilCoolDown > 0)
        {
            _recoilCoolDown -= Time.deltaTime;
            if (_currRecoil <= _maxRecoil) // Ini batasnya sampai maxrecoil ato lebihin max recoil ?
            {
                _currRecoil += Time.deltaTime * _currWeapon.weaponStatSO.recoil;
            }
            // else if(_currRecoil > _maxRecoil)_currRecoil = 
        }
        else
        {
            if(_currWeapon.weaponStatSO.bulletPerTap > 1) _currRecoil = _currWeapon.weaponStatSO.recoil;
            else _currRecoil = 0;
        }
    }
    #endregion
}

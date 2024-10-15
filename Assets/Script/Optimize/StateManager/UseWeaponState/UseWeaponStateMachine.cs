using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseWeaponStateMachine : CharacterStateMachine, IUseWeapon, INormalUseWeaponData
{
    #region Normal Variable
    [Header("Testing")]
    public bool isGo;
    public bool isStop;
    public GameObject target;

    [Space(1)]
    [Header("Use Weapon States - Normal")]
    [SerializeField] protected bool _isIdle;
    [SerializeField] protected bool _isAiming;
    [SerializeField] protected bool _isUsingWeapon;
    [SerializeField] protected bool _isReloading;
    protected UseWeaponStateFactory _states;
    protected UseWeaponState _currState;

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
    [SerializeField] protected Transform _originShootPoint_AIContainer; 
    protected Transform _currOriginShootPoint, _currDirectionShootPoint;
    protected Vector3 _originShootPosition, _directionShootPosition;

    [Space(1)]
    [Header("Enemy Character Layermask")]
    [SerializeField] protected LayerMask _charaEnemyMask;

    [Space(1)]
    [Header("Saving other component data")]
    protected CharacterIdentity _charaIdentity;
    protected WeaponLogicManager _weaponLogicManager;
    
    #endregion
    
    #region GETTERSETTER Variable
    public bool IsIdle { get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsAiming { get {return _isAiming;} set{ _isAiming = value;} }
    public bool IsUsingWeapon { get {return _isUsingWeapon;} set{ _isUsingWeapon = value;} }
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

    public WeaponData CurrWeapon{get{return _currWeapon;} }
    public Transform ChosenTarget { get {return _currChosenTarget;}}
    public float CharaAimAccuracy { get { return _charaAimAccuracy;}set { _charaAimAccuracy = value;}}

    public Transform CurrOriginShootPoint { get{return _currOriginShootPoint;}}
    public Transform CurrDirectionShootPoint { get{return _currDirectionShootPoint;}}

    #endregion

    protected override void Awake()
    {
        base.Awake();

        if(_originShootPoint_AIContainer == null)_originShootPoint_AIContainer = GetComponent<FOVMachine>().GetFOVPoint;
        
        _charaIdentity = GetComponent<CharacterIdentity>();
        if(!IsInputPlayer)
        {
            _currOriginShootPoint = _originShootPoint_AIContainer;
            _currDirectionShootPoint = _currChosenTarget;
        }
        
        if(_originShootPoint_AIContainer == null)_originShootPoint_AIContainer = GetComponent<FOVMachine>().GetFOVPoint;

        _states = new UseWeaponStateFactory(this);
    }
    private void Start() 
    {
        _weaponLogicManager = WeaponLogicManager.Instance;

        SetCurrWeapon();
        
        SwitchState(_states.IdleWeaponState());
    }
    protected virtual void Update()
    {
        _currState?.UpdateState();
    }
    private void FixedUpdate() {
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
            SetShootPosition();
            _weaponLogicManager.ShootingPerformed(_originShootPosition, _directionShootPosition, CharaAimAccuracy, CurrWeapon.weaponStatSO, _charaEnemyMask);
            CurrWeapon.currBullet -= 1;
            if(!CurrWeapon.weaponStatSO.allowHoldDownButton)IsUsingWeapon = false;
            StartCoroutine(FireRate(CurrWeapon.weaponStatSO.fireRate));
            if (CurrWeapon.currBullet == 0 && CurrWeapon.totalBullet > 0)
            {
                if(!CanReload)CanReload = true;
                IsReloading = true;
            }
        }
    }
    protected virtual void SetShootPosition()
    {
        _originShootPosition = CurrOriginShootPoint.position;
        _directionShootPosition = CurrDirectionShootPoint.position;
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
        _charaIdentity.ReloadWeapon();
        CanReload = false;
        IsReloading = false;
        
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

    protected void SetCurrWeapon()
    {
        _currWeapon = _charaIdentity.CurrWeapon;
        // pas ganti weapon, ini dipanggil
    }
}

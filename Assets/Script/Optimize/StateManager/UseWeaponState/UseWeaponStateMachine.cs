using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseWeaponStateMachine : CharacterStateMachine, IUseWeapon, INormalUseWeaponData
{
    #region Normal Variable
    [Header("Test")]
    [Header("Testing")]
    public bool isGo;
    public bool isStop;
    public GameObject target;
    [Space(5)]
    [Header("Other Component Variable")]
    protected UseWeaponStateFactory _states;
    protected CharacterIdentity _charaIdentity;

    [Header("State Bool")]
    [SerializeField] protected bool _isIdle;
    [SerializeField] protected bool _isAiming;
    [SerializeField] protected bool _isUsingWeapon;
    [SerializeField] protected bool _isReloading;
    [SerializeField] protected float _notAimSpamClickDelayTime = 0.1f;
    protected bool _canReload = true;
    protected bool _isfireRateOn;

    [Space(5)]
    [Header("No Inspector Variable")]
    protected UseWeaponState _currState;
    protected Transform _currChosenTarget;
    protected WeaponData _currWeapon;
    protected Transform _currOriginShootPoint, _currDirectionShootPoint;
    protected Vector3 _originShootPosition, _directionShootPosition;

    [Space(5)]
    [Header("Weapon Logic & Layermask")]
    [SerializeField] protected LayerMask _charaEnemyMask;
    [Tooltip("Kalo AI isinya FOV, kalo player gausa isi krn ud lsg diisi camera.main")]
    [SerializeField] protected Transform _originShootPoint_AIContainer; 

    protected WeaponLogicHandler _weaponLogicHandler;
    

    #endregion
    #region GETTERSETTER Variable
    public WeaponData CurrWeapon{get{return _currWeapon;} }
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
    public Transform CurrOriginShootPoint { get{return _currOriginShootPoint;}}
    public Transform CurrDirectionShootPoint { get{return _currDirectionShootPoint;}}
    public Transform ChosenTarget { get {return _currChosenTarget;}}


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
        
        //nyari fov jg bs disini, kalo fov null

        _weaponLogicHandler = new WeaponLogicHandler();
        _states = new UseWeaponStateFactory(this);
    }
    private void Start() 
    {
        SetCurrWeapon();
        SwitchState(_states.IdleWeaponState());
    }
    protected virtual void Update()
    {
        if(!IsInputPlayer)
        {
            if(isGo && target != null)
            {
                isGo = false;
                GiveChosenTarget(target.transform);
            }
            if(isStop)
            {
                isStop = false;
                _currChosenTarget = null;
            }
        }
        _currState?.UpdateState();
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
            _weaponLogicHandler.ShootingPerformed(_originShootPosition, _directionShootPosition, CurrWeapon.weaponStatSO, _charaEnemyMask);
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

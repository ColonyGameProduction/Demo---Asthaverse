using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableUseWeaponStateMachine : UseWeaponStateMachine, IAdvancedUseWeaponData, IPlayableUseWeaponDataNeeded
{
    #region Normal Variable
    [Header ("Playable Character Variable")]

    [Space(2)]
    [Header("State Bool - Advanced use")]
    [SerializeField] protected bool _isSwitchingWeapon;
    protected bool _canSwitchWeapon = true;
    [SerializeField] protected float _switchWeaponDuration; // ini sementara ampe ada animasi
    [SerializeField] protected bool _isSilentKill;
    protected bool _canSilentKill = true;
    [SerializeField] protected float _silentKillDuration; // ini sementara ampe ada animasi

    [Space(1)]
    [Header("Saving other component data")]
    protected ICanInputPlayer _getCanInputPlayer;
    protected ICanSwitchWeapon _getCanSwitchWeapon;

    #endregion
    #region GETTERSETTER Variable
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

    #endregion
    public event Action OnTurningOffScope;// ini dipanggil kalo misal lg input player dan reload - yg subs adalah playablecharamanager
    protected override void Awake()
    {
        base.Awake();

        _getCanSwitchWeapon = GetComponent<ICanSwitchWeapon>();

        _getCanInputPlayer = GetComponent<ICanInputPlayer>();
        _isInputPlayer = _getCanInputPlayer.IsInputPlayer;
        _getCanInputPlayer.OnInputPlayerChange += CharaIdentity_OnInputPlayerChange; // Ditaro di sini biar ga ketinggalan sebelah, krn sebelah diubah di start

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
        if(_isInputPlayer)
        {
            _originShootPosition = CurrOriginShootPoint.position;
            _directionShootPosition = CurrDirectionShootPoint.forward;
        }
        else
        {
            base.SetShootPosition();
        }
        
    }
    
    #endregion
    private void CharaIdentity_OnInputPlayerChange(bool obj)
    {
        _isInputPlayer = obj;
        if(_isInputPlayer)
        {
            _currOriginShootPoint = Camera.main.transform;
            _currDirectionShootPoint = Camera.main.transform;
        }
        else
        {
            _currOriginShootPoint = _originShootPoint_AIContainer;
            _currDirectionShootPoint = _currChosenTarget;
        }
    }

    public void TellToTurnOffScope()
    {
        OnTurningOffScope?.Invoke();
    }
}

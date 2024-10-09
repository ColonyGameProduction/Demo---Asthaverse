using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseWeaponStateMachine : CharacterStateMachine, IUseWeapon, INormalUseWeaponData
{
    #region Normal Variable
    [Space(5)]
    [Header("Other Component Variable")]
    UseWeaponStateFactory _states;
    protected CharacterIdentity _charaIdentity;

    [Header("State Bool")]
    [SerializeField] protected bool _isIdle;
    [SerializeField] protected bool _isAiming;
    [SerializeField] protected bool _isUsingWeapon;
    [SerializeField] protected bool _isReloading;
    protected bool _canReload = true;

    [Space(5)]
    [Header("No Inspector Variable")]
    UseWeaponState _currState;
    protected bool _isInputPlayer;
    protected Transform _currChosenTarget;

    #endregion
    #region GETTERSETTER Variable
    public WeaponData _currWeapon{get{return _charaIdentity.CurrWeapon;}}
    public bool IsIdle { get {return _isIdle;} set{ _isIdle = value;} }
    public bool IsAiming { get {return _isAiming;} set{ _isAiming = value;} }
    public bool IsUsingWeapon { get {return _isUsingWeapon;} set{ _isUsingWeapon = value;} }
    public bool IsReloading 
    { 
        get {return _isReloading;} 
        set{ 
            if(value)
            {
                if(_canReload)_isReloading = value;
            }
            else _isReloading = value;
        }
    }
    
    public bool CanReload { get {return _canReload;} set {_canReload = value;}  }
    public Transform ChosenTarget { get {return _currChosenTarget;}}
    public bool IsInputPlayer {get {return _isInputPlayer;}}

    #endregion

    protected override void Awake()
    {
        base.Awake();
        if(_charaIdentity == null)_charaIdentity = GetComponent<CharacterIdentity>();
        _isInputPlayer = _charaIdentity.IsInputPlayer;
        _charaIdentity.OnInputPlayerChange += CharaIdentity_OnInputPlayerChange;

        _states = new UseWeaponStateFactory(this);
    }
    private void Start() 
    {
        SwitchState(_states.IdleWeaponState());
    }
    protected virtual void Update()
    {
        _currState?.UpdateState();
    }
    public override void SwitchState(BaseState newState)
    {
        if(_currState != null)
        {
            _currState?.ExiState();
        }
        _currState = newState as UseWeaponState;
        _currState?.EnterState();
    }
    #region UseWeapon
    public virtual void UseWeapon()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ForceStopUseWeapon()
    {
        throw new System.NotImplementedException();
    }
    public void GiveChosenTarget(Transform chosenTarget)
    {
        _currChosenTarget = chosenTarget;
    }

    #endregion
    private void CharaIdentity_OnInputPlayerChange(bool obj)
    {
        _isInputPlayer = obj;
    }

}

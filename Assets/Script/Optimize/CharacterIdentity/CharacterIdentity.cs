using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The characterIdentity
/// Juga Tempat ambil data-data universal yg tidak berhubungan dgn si statemachine 
/// </summary>
public abstract class CharacterIdentity : MonoBehaviour, IHealth, IHaveWeapon, IUnsubscribeEvent
{
    [Header("test")]
    public bool immortalized;

    #region Normal Variable
    [Header("CHARACTER SCRIPTABLE OBJECT STAT")]
    [SerializeField] protected EntityStatSO _characterStatSO;

    [Header("Manager/Machine")]
    //Untuk ambil bool
    protected MovementStateMachine _moveStateMachine;
    protected UseWeaponStateMachine _useWeaponStateMachine;
    protected AIBehaviourStateMachine _aiStateMachine;
    protected WeaponShootVFX _weaponShootVFX;
    protected WeaponGameObjectDataContainer _weaponGameObjectDataContainer;
    protected FOVMachine _fovMachine;
    protected GameManager _gm;
    protected Animator _animator;

    #region CHARACTER STATS
    [Space(1)]
    [Header("Character Stats")]
    [SerializeField] private string _charaName;

    [Space(1)]
    [Header("   Health")]
    [ReadOnly(false), SerializeField] protected float _totalHealth;
    [ReadOnly(true), SerializeField] protected float _currHealth;
    [ReadOnly(false), SerializeField] protected bool _isDead;

    [Space(1)]
    [Header("   Armour")]
    protected armourType _armourType;
    protected float _armour;

    [Space(1)]
    [Header("   Weapon")]
    [ReadOnly(true), SerializeField] protected List<WeaponData> _weaponLists = new List<WeaponData>();
    [SerializeField] protected int _currWeaponIdx;


    [Space(1)]
    [Header("   Stealth")]
    [ReadOnly(true), SerializeField] protected float _stealthStats;
    #endregion
    [Header("Regen Stats")]
    [SerializeField] protected float _regenScale = 0.7f;
    [SerializeField] protected float _regenTimerMax = 0.5f;
    [ReadOnly(false), SerializeField] protected float _regenCDTimer;

    [Header("Rigging")]
    public Action<bool, bool> OnToggleLeftHandRig;
    #endregion

    #region const
    public const string ANIMATION_DEATH_PARAMETER = "Death"; 
    public const string ANIMATION_DEATH_TRIGGER_PARAMETER = "DeathTrigger";
    public const string ANIMATION_IsBoyChara_PARAMETER = "IsBoyChara";
    #endregion

    #region GETTERSETTER Variable
    [HideInInspector]

    public MovementStateMachine GetMovementStateMachine {get { return _moveStateMachine;}}
    public UseWeaponStateMachine GetUseWeaponStateMachine {get { return _useWeaponStateMachine;}}
    public AIBehaviourStateMachine GetAIStateMachine {get { return _aiStateMachine;}}
    public WeaponShootVFX GetWeaponShootVFX {get {return _weaponShootVFX;}}
    public WeaponGameObjectDataContainer GetWeaponGameObjectDataContainer {get{return _weaponGameObjectDataContainer;}}

    public virtual float TotalHealth {get { return _totalHealth; } }
    public virtual float CurrHealth {get {return _currHealth; } set { _currHealth = value; } }
    public virtual bool IsHalfHealthOrLower {get {return _currHealth <= _totalHealth/2; }} 
    public bool IsDead {get { return _isDead;}}

    public armourType GetCharaArmourType {get {return _armourType;}}
    public virtual float StealthStat { get{ return _stealthStats; }}

    public List<WeaponData> WeaponLists {get { return _weaponLists; } }
    public WeaponData CurrWeapon {get { return _weaponLists[_currWeaponIdx]; } }
    public EntityStatSO GetCharaStatSO {get {return _characterStatSO;}}

    #endregion
    protected virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        _moveStateMachine = GetComponent<MovementStateMachine>();
        _useWeaponStateMachine = GetComponent<UseWeaponStateMachine>();
        _useWeaponStateMachine.OnWasUsingGun += UseWeapon_OnWasUsingGun;

        _aiStateMachine = GetComponent<AIBehaviourStateMachine>();
        _weaponShootVFX = GetComponent<WeaponShootVFX>();
        _fovMachine = GetComponent<FOVMachine>();
        _weaponGameObjectDataContainer = GetComponentInChildren<WeaponGameObjectDataContainer>();

        InitializeCharacter();
        InitializeWeapon();
    }
    protected virtual void Start() 
    {
        _gm = GameManager.instance;
    }

    private void UseWeapon_OnWasUsingGun()
    {
        _moveStateMachine.WasCharacterAiming = true;
    }

    #region Health
    public virtual void Hurt(float Damage)
    {
        if(CurrHealth <= 0)return;
        
        _regenCDTimer = _regenTimerMax;
        CurrHealth -= Damage;
        if(CurrHealth <= 0)
        {
            CurrHealth = 0;
            if(!immortalized) Death();
        }
    }
    public virtual void Heal(float Healing)
    {
        if(CurrHealth == TotalHealth) return;

        CurrHealth += Healing;
        if(CurrHealth >= TotalHealth) CurrHealth = TotalHealth;
    }

    public virtual void Death()
    {
        if(_isDead)return;
        OnToggleLeftHandRig?.Invoke(false, false);

        _isDead = true;
        _regenCDTimer = 0f;
        _animator.SetBool(ANIMATION_DEATH_PARAMETER, true);
        _animator.SetTrigger(ANIMATION_DEATH_TRIGGER_PARAMETER);
        _useWeaponStateMachine.ForceStopUseWeapon();
        _moveStateMachine.ForceStopMoving();
        
        if(_fovMachine.enabled) _fovMachine.StopFOVMachine();
        _fovMachine.enabled = false;
    }
    public abstract void AfterFinishDeathAnimation();
    
    public virtual void InitializeCharacter()
    {
        if(_characterStatSO == null) 
        {
            _currHealth = TotalHealth;
            return;
        }

        _charaName = _characterStatSO.entityName;
        _animator.SetFloat(ANIMATION_IsBoyChara_PARAMETER, !_characterStatSO.isCharBoy? 0 : 1);

        _totalHealth = _characterStatSO.health;
        _currHealth = _totalHealth;

        _moveStateMachine.InitializeMovementSpeed(_characterStatSO.speed);

        _armourType = _characterStatSO.armourType;
        _armour = _characterStatSO.armor;

        _useWeaponStateMachine.SetCharaAimAccuracy(_characterStatSO.acuracy);
        _stealthStats = _characterStatSO.stealth;

        _fovMachine.viewRadius = _characterStatSO.FOVRadius;
        
        if(_weaponLists.Count > 0) _weaponLists.Clear();
        foreach(WeaponStatSO weaponStat in _characterStatSO.weaponStat)
        {
            WeaponData newWeapData = new WeaponData(weaponStat);
            _weaponLists.Add(newWeapData);
        }
        
    }

    #endregion
    
    #region Weapon
    public virtual void InitializeWeapon() // ntr kalo ada save baca dr save
    {
        foreach(WeaponData weapon in WeaponLists)
        {
            weapon.totalBullet = weapon.weaponStatSO.magSize * weapon.weaponStatSO.magSpare;
            weapon.currBullet = weapon.weaponStatSO.magSize;
        }

        _currWeaponIdx = 0;
        SetWeaponGameObjectDataContainer();

        _weaponShootVFX.CurrWeaponIdx = _currWeaponIdx;

        foreach(WeaponStatSO weaponStat in _characterStatSO.weaponStat)
        {
            _weaponShootVFX.SpawnTrail((int)(weaponStat.magSize * weaponStat.bulletPerTap), _useWeaponStateMachine.GunOriginShootPoint.position, weaponStat.bulletTrailPrefab, weaponStat.gunFlashPrefab);
        }

    }
    public virtual void SetWeaponGameObjectDataContainer()
    {
        _weaponGameObjectDataContainer.GetCurrWeaponGameObjectData(_currWeaponIdx);
        _useWeaponStateMachine.GunOriginShootPoint = _weaponGameObjectDataContainer.GetCurrShootPlacement();
    }
    public abstract void ReloadWeapon();
    #endregion

    #region StateMachine Command
    public virtual void Run(bool isRunning)
    {
        if(isRunning)
        {
            
            if(_moveStateMachine.IsCrouching)
            {   
                if(!_moveStateMachine.IsAtCrouchPlatform)
                {
                    if(_moveStateMachine.AllowLookTarget) _moveStateMachine.AllowLookTarget = false;
                    _useWeaponStateMachine.ForceStopUseWeapon();
                    _moveStateMachine.IsCrouching = false;
                    _moveStateMachine.IsRunning = isRunning;
                }
            }
            else
            {
                if(_moveStateMachine.AllowLookTarget) _moveStateMachine.AllowLookTarget = false;
                _useWeaponStateMachine.ForceStopUseWeapon();
                _moveStateMachine.IsRunning = isRunning;
            }
        }
        else
        {
            _moveStateMachine.IsRunning = isRunning;
        }
    }
    public virtual void Crouch(bool isCrouching)
    {
        if(isCrouching)
        {
            if(_moveStateMachine.IsRunning)
            {
                _moveStateMachine.IsRunning = false;
            }
            _moveStateMachine.IsCrouching = isCrouching;
        }
        else
        {
            if(!_moveStateMachine.IsAtCrouchPlatform) _moveStateMachine.IsCrouching = isCrouching;
        }
        
    }
    public virtual void Aiming(bool isAiming)
    {
        if(isAiming)
        {
            if(_moveStateMachine.IsRunning)
            {
                _moveStateMachine.IsRunning = false;
            }
        }
        _useWeaponStateMachine.IsAiming = isAiming;
    }
    public virtual void Shooting(bool isShooting)
    {
        if(isShooting)
        {
            Aiming(true);
        }
        _useWeaponStateMachine.IsUsingWeapon = isShooting;
    }
    #endregion

    public virtual void UnsubscribeEvent()
    {
        _useWeaponStateMachine.OnWasUsingGun -= UseWeapon_OnWasUsingGun;
    }
}

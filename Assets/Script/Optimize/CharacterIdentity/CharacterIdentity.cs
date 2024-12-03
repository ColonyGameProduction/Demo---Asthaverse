using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The characterIdentity
/// Juga Tempat ambil data-data universal yg tidak berhubungan dgn si statemachine 
/// </summary>
public abstract class CharacterIdentity : MonoBehaviour, IHealth, IHaveWeapon
{
    [Header("test")]
    public bool immortalized;

    #region Normal Variable
    [Header("CHARACTER SCRIPTABLE OBJECT STAT")]
    [SerializeField] protected EntityStatSO _characterStatSO;

    [Header("Manager/Machine")]
    //Untuk ambil bool
    [SerializeField] protected MovementStateMachine _moveStateMachine;
    [SerializeField] protected UseWeaponStateMachine _useWeaponStateMachine;
    protected WeaponShootVFX _weaponShootVFX;
    protected FOVMachine _fovMachine;
    protected GameManager _gm;
    [SerializeField]protected Animator _animator;

    #region CHARACTER STATS
    [Space(1)]
    [Header("Character Stats")]
    [SerializeField] private string _charaName;

    [Space(1)]
    [Header("   Health")]
    [SerializeField] protected float _totalHealth;
    protected float _currHealth;
    protected bool _isDead;

    [Space(1)]
    [Header("   Armour")]
    protected armourType _armourType;
    protected float _armour;

    [Space(1)]
    [Header("   Weapon")]
    [SerializeField] protected List<WeaponData> _weaponLists = new List<WeaponData>();
    [SerializeField] protected int _currWeaponIdx;


    [Space(1)]
    [Header("   Stealth")]
    protected float _stealthStats;
    #endregion
    [Header("Regen Stats")]
    [SerializeField] protected float _regenScale = 0.7f;
    protected float _regenCDTimer;
    [SerializeField]protected float _regenTimerMax = 0.5f;
    #endregion

    #region GETTERSETTER Variable
    [HideInInspector]
    //getter setter
    public virtual float StealthStat { get{ return _stealthStats; }}
    public virtual float TotalHealth {get { return _totalHealth; } }
    public virtual float CurrHealth {get {return _currHealth; } set { _currHealth = value; } }
    public virtual bool IsHalfHealthOrLower {get {return _currHealth <= _totalHealth/2; }} 
    public armourType GetCharaArmourType {get {return _armourType;}}
    public bool IsDead {get { return _isDead;}}
    public List<WeaponData> WeaponLists {get { return _weaponLists; } }
    public WeaponData CurrWeapon {get { return _weaponLists[_currWeaponIdx]; } }
    public MovementStateMachine MovementStateMachine {get { return _moveStateMachine;}}
    public UseWeaponStateMachine UseWeaponStateMachine {get { return _useWeaponStateMachine;}}

    #endregion
    protected virtual void Awake()
    {
        if(_animator == null)_animator = GetComponent<Animator>();
        if(_moveStateMachine == null) _moveStateMachine = GetComponent<MovementStateMachine>();
        if(_useWeaponStateMachine == null) _useWeaponStateMachine = GetComponent<UseWeaponStateMachine>();
        // Debug.Log(_useWeaponStateMachine + " tidak null");
        if(_useWeaponStateMachine != null)_useWeaponStateMachine.OnWasUsinghGun += UseWeapon_OnWasUsinghGun;
        _weaponShootVFX = GetComponent<WeaponShootVFX>();
        _fovMachine = GetComponent<FOVMachine>();

        InitializeCharacter();
        InitializeWeapon();
    }
    protected virtual void Start() 
    {
        _gm = GameManager.instance;
    }

    protected virtual void Update()
    {
        RegenerationTimer();
    }
    private void UseWeapon_OnWasUsinghGun()
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
            if(!immortalized)Death();
        }
    }
    public virtual void Heal(float Healing)
    {
        if(CurrHealth == TotalHealth)return;

        CurrHealth += Healing;
        if(CurrHealth >= TotalHealth) CurrHealth = TotalHealth;
    }
    protected virtual void Regeneration()
    {
        Heal(TotalHealth * _regenScale * Time.deltaTime);
    }
    protected void RegenerationTimer()
    {
        if(CurrHealth <= TotalHealth && !IsDead)
        {
            if(_regenCDTimer > 0)_regenCDTimer -= Time.deltaTime;
            else
            {
                _regenCDTimer = 0;
                Regeneration();
            }
        }
    }

    public virtual void Death()
    {
        if(_isDead)return;
        _regenCDTimer = 0f;
        _animator.SetBool("Death", true);
        _animator.SetTrigger("DeathTrigger");
        _isDead = true;
        _useWeaponStateMachine.ForceStopUseWeapon();
        _moveStateMachine.ForceStopMoving();
        if(_fovMachine.enabled)_fovMachine.StopFOVMachine();
        _fovMachine.enabled = false;
    }
    public virtual void AfterFinishDeathAnimation(){}
    

    public virtual void InitializeCharacter()
    {
        if(_characterStatSO == null) 
        {
            _currHealth = TotalHealth;
            return;
        }
        _charaName = _characterStatSO.entityName;

        _totalHealth = _characterStatSO.health;
        _currHealth = _totalHealth;

        MovementStateMachine.InitializeMovementSpeed(_characterStatSO.speed);

        _armourType = _characterStatSO.armourType;
        _armour = _characterStatSO.armor;

        _useWeaponStateMachine.SetCharaAimAccuracy(_characterStatSO.acuracy);
        _stealthStats = _characterStatSO.stealth;

        _fovMachine.viewRadius = _characterStatSO.FOVRadius;
        
        if(_weaponLists.Count > 0)_weaponLists.Clear();
        foreach(WeaponStatSO weaponStat in _characterStatSO.weaponStat)
        {
            WeaponData newWeapData = new WeaponData(weaponStat);
            _weaponLists.Add(newWeapData);
            // Debug.Log(weaponStat.gunShootPoint + " AAAAAAAAAA"+ weaponStat.gunShootPoint.position);
            _weaponShootVFX.SpawnTrail((int)(weaponStat.magSize * weaponStat.bulletPerTap), _useWeaponStateMachine.GunOriginShootPoint.position, weaponStat.bulletTrailPrefab, weaponStat.gunFlashPrefab);
            // _weaponShootVFX.SpawnTrail((int)(weaponStat.magSize * weaponStat.bulletPerTap * 2), weaponStat.gunShootPoint.position, weaponStat.bulletTrailPrefab, weaponStat.gunFlashPrefab);
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
        _weaponShootVFX.CurrWeaponIdx = _currWeaponIdx;

    }
    public abstract void ReloadWeapon();
    #endregion

    #region StateMachine Command
    public virtual void Run(bool isRunning)
    {
        if(isRunning)
        {
            if(MovementStateMachine.AllowLookTarget)MovementStateMachine.AllowLookTarget = false;
            UseWeaponStateMachine.ForceStopUseWeapon();
            if(MovementStateMachine.IsCrouching)
            {
                MovementStateMachine.IsCrouching = false;
            }
        }
        MovementStateMachine.IsRunning = isRunning;
    }
    public virtual void Crouch(bool isCrouching)
    {
        if(isCrouching)
        {
            if(MovementStateMachine.IsRunning)
            {
                MovementStateMachine.IsRunning = false;
            }
        }
        MovementStateMachine.IsCrouching = isCrouching;
    }
    public virtual void Aiming(bool isAiming)
    {
        if(isAiming)
        {
            if(MovementStateMachine.IsRunning)
            {
                MovementStateMachine.IsRunning = false;
            }
        }
        UseWeaponStateMachine.IsAiming = isAiming;
    }
    public virtual void Shooting(bool isShooting)
    {
        if(isShooting)
        {
            Aiming(true);
        }
        UseWeaponStateMachine.IsUsingWeapon = isShooting;
    }
    #endregion
}

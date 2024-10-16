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
    #region Normal Variable
    [Header("CHARACTER SCRIPTABLE OBJECT STAT")]
    [SerializeField] protected EntityStatSO _characterStatSO;

    [Header("Manager/Machine")]
    //Untuk ambil bool
    [SerializeField] protected MovementStateMachine _moveStateMachine;
    [SerializeField] protected UseWeaponStateMachine _useWeaponStateMachine;
    protected FOVMachine _fovMachine;
    protected GameManager _gm;

    #region CHARACTER STATS
    [Space(1)]
    [Header("Character Stats")]
    [SerializeField] private string _charaName;

    [Space(1)]
    [Header("   Health")]
    [SerializeField] protected float _totalHealth;
    protected float _currhealth;
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

    [Space(5)]
    [Header("No Inspector Variable")]
    //Thing needed to getcomponent
    //Get Standmovement bool -> isIdle, isWalking, isRunning
    protected IMovement _getMoveFunction;
    protected IStandMovementData _getStandMovementData;
    protected IUseWeapon _getUseWeaponFunction;
    protected INormalUseWeaponData _getNormalUseWeaponData;
    #endregion

    #region GETTERSETTER Variable
    [HideInInspector]
    //getter setter
    public float StealthStat { get{ return _stealthStats; }}
    public float TotalHealth {get { return _totalHealth; } }
    public float HealthNow {get {return _currhealth; } }
    public bool IsDead {get { return _isDead;}}
    public List<WeaponData> WeaponLists {get { return _weaponLists; } }
    public WeaponData CurrWeapon {get { return _weaponLists[_currWeaponIdx]; } }
    public MovementStateMachine MovementStateMachine {get { return _moveStateMachine;}}
    public IStandMovementData GetStandMovementData {get { return _getStandMovementData;}}
    public IMovement GetMoveFunction {get { return _getMoveFunction;}}
    public UseWeaponStateMachine UseWeaponStateMachine {get { return _useWeaponStateMachine;}}
    public IUseWeapon GetUseWeaponFunction {get { return _getUseWeaponFunction;}}
    public INormalUseWeaponData GetNormalUseWeaponData {get { return _getNormalUseWeaponData;}}
    #endregion
    protected virtual void Awake()
    {
        if(_moveStateMachine == null) _moveStateMachine = GetComponent<MovementStateMachine>();
        _getMoveFunction = GetComponent<IMovement>();
        _getStandMovementData = GetComponent<IStandMovementData>();

        _getUseWeaponFunction = GetComponent<IUseWeapon>();
        _getNormalUseWeaponData = GetComponent<INormalUseWeaponData>();
        _fovMachine = GetComponent<FOVMachine>();

        InitializeCharacter();
        InitializeWeapon();
    }
    protected virtual void Start() 
    {
        _gm = GameManager.instance;
    }

    #region Health
    public virtual void Hurt(float Damage)
    {
        if(HealthNow <= 0)return;

        _currhealth -= Damage;
        if(HealthNow <= 0)
        {
            _currhealth = 0;
            Death();
        }
    }
    public virtual void Heal(float Healing)
    {
        if(HealthNow == TotalHealth)return;

        _currhealth += Healing;
        if(HealthNow >= TotalHealth) _currhealth = TotalHealth;
    }

    public virtual void Death()
    {
        _isDead = true;
    }

    public virtual void InitializeCharacter()
    {
        if(_characterStatSO == null) 
        {
            _currhealth = TotalHealth;
            return;
        }
        _charaName = _characterStatSO.entityName;

        _totalHealth = _characterStatSO.health;
        _currhealth = _totalHealth;

        MovementStateMachine.WalkSpeed = _characterStatSO.speed;

        _armourType = _characterStatSO.armourType;
        _armour = _characterStatSO.armor;

        _useWeaponStateMachine.CharaAimAccuracy = _characterStatSO.acuracy;
        _stealthStats = _characterStatSO.stealth;

        _fovMachine.viewRadius = _characterStatSO.FOVRadius;
        
        if(_weaponLists.Count > 0)_weaponLists.Clear();
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

    }
    public abstract void ReloadWeapon();
    #endregion
}

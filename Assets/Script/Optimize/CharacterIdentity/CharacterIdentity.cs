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
    [Header("Manager/Machine")]
    //Untuk ambil bool
    [SerializeField] protected MovementStateMachine _moveStateMachine;
    [SerializeField] protected UseWeaponStateMachine _useWeaponStateMachine;
    protected GameManager _gm;

    [Space(1)]
    [Header("Health")]
    [SerializeField] protected float _totalHealth;
    protected float _health;

    [Space(1)]
    [Header("Weapon")]
    [SerializeField] protected List<WeaponData> _weaponLists;
    [SerializeField] protected int _currWeaponIdx;

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
    public float TotalHealth {get { return _totalHealth; } }
    public float HealthNow {get {return _health; } }
    public List<WeaponData> WeaponLists {get { return _weaponLists; } }
    public WeaponData CurrWeapon {get { return _weaponLists[_currWeaponIdx]; } }
    public MovementStateMachine MovementStateMachine {get { return _moveStateMachine;}}
    public IStandMovementData GetStandMovementData {get { return _getStandMovementData;}}
    public IMovement GetMoveFunction {get { return _getMoveFunction;}}
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
        InitializeHealth();
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

        _health -= Damage;
        if(HealthNow <= 0)
        {
            _health = 0;
            Death();
        }
    }
    public virtual void Heal(float Healing)
    {
        if(HealthNow == TotalHealth)return;

        _health += Healing;
        if(HealthNow >= TotalHealth) _health = TotalHealth;
    }

    public virtual void Death()
    {
        //Do something
    }
    public virtual void InitializeHealth()
    {
        _health = TotalHealth;
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

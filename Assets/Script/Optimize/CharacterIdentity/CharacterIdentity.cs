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
    protected GameManager _gm;
    [Header("Manager/Machine")]
    //Untuk ambil bool
    [SerializeField] protected MovementStateMachine _moveStateMachine;
    [Space(1)]
    [Header("Input Control Now")]
    [SerializeField]protected bool _isInputPlayer;
    public Action<bool> OnInputPlayerChange;
    [Space(1)]
    [Header("Health")]
    [SerializeField] protected float _totalHealth;
    protected float _health;
    [Space(1)]
    [Header("Weapon")]
    [SerializeField] protected List<WeaponData> _weaponLists;
    [SerializeField] protected int _currWeaponIdx;

    //Thing needed to getcomponent
    //Get Standmovement bool -> isIdle, isWalking, isRunning
    protected IMovement _getMoveFunction;
    protected IStandMovement _getStandMovementBool;

    [HideInInspector]
    //getter setter
    public bool IsInputPlayer 
    { 
        get { return _isInputPlayer; } 
        set
        { 
            if(IsInputPlayer != value)
            {
                _isInputPlayer = value;
                OnInputPlayerChange?.Invoke(_isInputPlayer);
            }
        } 
    }
    public float TotalHealth {get { return _totalHealth; } }
    public float HealthNow {get {return _health; } }
    public List<WeaponData> WeaponLists {get { return _weaponLists; } }
    public WeaponData CurrWeapon {get { return _weaponLists[_currWeaponIdx]; } }
    public MovementStateMachine MovementStateMachine {get { return _moveStateMachine;}}
    public IStandMovement GetStandMovementBool {get { return _getStandMovementBool;}}
    public IMovement GetMoveFunction {get { return _getMoveFunction;}}
    protected virtual void Awake()
    {
        if(_moveStateMachine == null) _moveStateMachine = GetComponent<MovementStateMachine>();
        _getMoveFunction = GetComponent<IMovement>();
        _getStandMovementBool = GetComponent<IStandMovement>();
    }
    protected virtual void Start() 
    {
        _gm = GameManager.instance;
        InitializeHealth();
        InitializeWeapon();
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
    #endregion
}

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
    public bool ded;
    public bool reviv;

    #region Normal Variable
    [Header("CHARACTER SCRIPTABLE OBJECT STAT")]
    [SerializeField] protected EntityStatSO _characterStatSO;

    [Header("Manager/Machine")]
    //Untuk ambil bool
    [SerializeField] protected MovementStateMachine _moveStateMachine;
    [SerializeField] protected UseWeaponStateMachine _useWeaponStateMachine;
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

    #endregion

    #region GETTERSETTER Variable
    [HideInInspector]
    //getter setter
    public virtual float StealthStat { get{ return _stealthStats; }}
    public virtual float TotalHealth {get { return _totalHealth; } }
    public virtual float CurrHealth {get {return _currHealth; } set { _currHealth = value; } }
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
        if(ded)
        {
            ded = false;
            Hurt (CurrHealth);
        }
    }
    private void UseWeapon_OnWasUsinghGun()
    {
        _moveStateMachine.WasCharacterAiming = true;
    }
    #region Health
    public virtual void Hurt(float Damage)
    {
        if(CurrHealth <= 0)return;

        CurrHealth -= Damage;
        if(CurrHealth <= 0)
        {
            CurrHealth = 0;
            Death();
        }
    }
    public virtual void Heal(float Healing)
    {
        if(CurrHealth == TotalHealth)return;

        CurrHealth += Healing;
        if(CurrHealth >= TotalHealth) CurrHealth = TotalHealth;
    }

    public virtual void Death()
    {
        _animator.SetBool("Death", true);
        _animator.SetTrigger("DeathTrigger");
        _isDead = true;
        _useWeaponStateMachine.ForceStopUseWeapon();
        _moveStateMachine.ForceStopMoving();
        if(_fovMachine.enabled)_fovMachine.StopFOVMachine();
        _fovMachine.enabled = false;
    }
    

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

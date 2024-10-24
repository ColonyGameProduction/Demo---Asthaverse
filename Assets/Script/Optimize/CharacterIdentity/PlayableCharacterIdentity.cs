using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterIdentity : CharacterIdentity, IPlayableFriendDataHelper, IReceiveInputFromPlayer, ICanSwitchWeapon
{
    
    [Space(1)]
    [Header("Input Control Now")]
    [SerializeField]protected bool _isPlayerInput;
    public event Action<bool> OnIsPlayerInputChange;

    [Header("Friend Data Helper")]
    [SerializeField] protected GameObject[] _friendsNormalPosition;
    protected int _friendID;
    [SerializeField] private EntityStatSO _friendStatSO;
        #region Friend STATS

    [Space(1)]
    [Header("   Health")]
    [SerializeField] protected float _totalHealthFriend;
    protected float _currHealthFriend;

    [Space(1)]
    [Header("   Armour")]
    protected armourType _armourTypeFriend;
    protected float _armourFriend;

    [Space(1)]
    [Header("   Stealth")]
    protected float _stealthStatsFriend;
        #endregion
    
    [Space(5)]
    [Header("No Inspector Variable")]
    //Thing needed to getcomponent
    protected PlayableMovementStateMachine _getPlayableMovementStateData;
    protected PlayableUseWeaponStateMachine _getPlayableUseWeaponStateData;
    protected PlayableCamera _getPlayableCamera;

    protected FriendAIBehaviourStateMachine _friendAIStateMachine;
    

    [Header("Death Animation Component")]
    private bool _isAnimatingOtherAnimation;
    #region GETTERSETTER Variable

    public override float StealthStat 
    { 
        get{ 
            if(IsPlayerInput)return _stealthStats; 
            else return _stealthStatsFriend; 
        }
    }
    public override float TotalHealth
    { 
        get{ 
            if(IsPlayerInput)return _totalHealth; 
            else return _totalHealthFriend; 
        }
    }
    public override float CurrHealth
    { 
        get
        { 
            if(IsPlayerInput)return _currHealth; 
            else return _currHealthFriend; 
        }
        set
        {
            if(IsPlayerInput) _currHealth = value; 
            else _currHealthFriend = value; 
        }
    }

    [Header("Event")]
    public Action OnPlayableDeath;
    [HideInInspector]
    //Getter Setter
    public bool IsPlayerInput 
    { 
        get { return _isPlayerInput; } 
        set
        { 
            if(_isPlayerInput != value)
            {
                _isPlayerInput = value;
                OnIsPlayerInputChange?.Invoke(_isPlayerInput);
            }
        } 
    }
    public int FriendID {get { return _friendID;} set { _friendID = value; }}
    public GameObject[] GetFriendsNormalPosition { get {return _friendsNormalPosition;}}

    public PlayableMovementStateMachine GetPlayableMovementData {get { return _getPlayableMovementStateData;}}
    public PlayableUseWeaponStateMachine GetPlayableUseWeaponData {get { return _getPlayableUseWeaponStateData;}}
    public PlayableCamera GetPlayableCamera {get {return _getPlayableCamera;}}

    public FriendAIBehaviourStateMachine FriendAIStateMachine {get { return _friendAIStateMachine;}}
    public FOVMachine FOVMachine{get { return _fovMachine;}}

    public bool IsAnimatingOtherAnimation {get {return _isAnimatingOtherAnimation;}}
    
    #endregion
    protected override void Awake()
    {
        base.Awake();
        _getPlayableMovementStateData = MovementStateMachine as PlayableMovementStateMachine;
        _getPlayableUseWeaponStateData = UseWeaponStateMachine as PlayableUseWeaponStateMachine;
        _getPlayableCamera = GetComponent<PlayableCamera>();
        InitializeFriend();


        _friendAIStateMachine = GetComponent<FriendAIBehaviourStateMachine>();
        
    }
    protected override void Update() 
    {
        base.Update();
        if(reviv)
        {
            reviv = false;
            Revive();
        }
    }

    public void InitializeFriend()
    {
        if(_friendStatSO == null) 
        {
            _currHealthFriend = _totalHealthFriend;
            return;
        }

        _totalHealthFriend = _friendStatSO.health;
        _currHealthFriend = _totalHealthFriend;

        _armourTypeFriend = _friendStatSO.armourType;
        _armourFriend = _friendStatSO.armor;

        GetPlayableUseWeaponData.SetCharaFriendAccuracy(_friendStatSO.acuracy);
        _stealthStatsFriend = _friendStatSO.stealth;

    }

    public override void ReloadWeapon()
    {
        float bulletNeed = CurrWeapon.weaponStatSO.magSize - CurrWeapon.currBullet;
        if (CurrWeapon.totalBullet >= bulletNeed)
        {
            CurrWeapon.currBullet = CurrWeapon.weaponStatSO.magSize;
            CurrWeapon.totalBullet -= bulletNeed;
        }
        else if (CurrWeapon.totalBullet > 0)
        {
            CurrWeapon.currBullet += CurrWeapon.totalBullet;
            CurrWeapon.totalBullet = 0;
        } 
    }
    public void SwitchWeapon()
    {
        _currWeaponIdx++;
        if(_currWeaponIdx == WeaponLists.Count)
        {
            _currWeaponIdx = 0;
        }
    }

    public override void Death()
    {
        base.Death();
        _isAnimatingOtherAnimation = true;
        StartCoroutine(DeathANim());
    }
    public IEnumerator DeathANim()
    {
        yield return new WaitForSeconds(2f); // ded anim
        if(IsPlayerInput)OnPlayableDeath?.Invoke();
        _getPlayableMovementStateData.SetCharaGameObjRotationToNormal();
        _isAnimatingOtherAnimation = false;
        if(!_getPlayableMovementStateData.IsCrawling)
        {
            _getPlayableMovementStateData.IsCrawling = true;
        }
        //state rangkak on
        
    }

    public void Revive()
    {
        _useWeaponStateMachine.ForceStopUseWeapon();
        _moveStateMachine.ForceStopMoving();
        _isAnimatingOtherAnimation = true;
        StartCoroutine(Reviving());
    }
    private IEnumerator Reviving()
    {
        Debug.Log("reviving");
        yield return new WaitForSeconds(2f);
        _animator.SetTrigger("ReviveTrigger");
        _fovMachine.enabled = true;

        if(_getPlayableMovementStateData.IsCrawling)_getPlayableMovementStateData.IsCrawling = false;
        ResetHealth();

        _isDead = false;
        _isAnimatingOtherAnimation = false;
        Debug.Log("reviving Done");
    }
    
    public void TurnOnOffFriendAI(bool isTurnOn)
    {
        FriendAIStateMachine.enabled = isTurnOn;
        if(isTurnOn)
        {
            if(!IsDead)FOVMachine.enabled = isTurnOn;
        }
        else 
        {
            if(FOVMachine.enabled) FOVMachine.enabled = isTurnOn;
        }
    }

    public void ResetHealth()
    {
        _currHealth = _totalHealth;
        _currHealthFriend = _totalHealthFriend;
    }


}

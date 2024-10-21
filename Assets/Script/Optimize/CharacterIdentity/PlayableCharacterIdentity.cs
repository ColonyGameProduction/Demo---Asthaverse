using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterIdentity : CharacterIdentity, IPlayableFriendDataHelper, ICanInputPlayer, ICanSwitchWeapon
{
    
    [Space(1)]
    [Header("Input Control Now")]
    [SerializeField]protected bool _isInputPlayer;
    public event Action<bool> OnInputPlayerChange;

    [Header("Friend Data Helper")]
    [SerializeField] protected GameObject[] _friendsNormalPosition;
    protected int _friendID;
    
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

    [Header("Evenet")]
    public Action OnPlayableDeath;
    [HideInInspector]
    //Getter Setter
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
        if(IsInputPlayer)OnPlayableDeath?.Invoke();
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
        _fovMachine.enabled = true;

        if(_getPlayableMovementStateData.IsCrawling)_getPlayableMovementStateData.IsCrawling = false;
        _currhealth = _totalHealth;

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
}

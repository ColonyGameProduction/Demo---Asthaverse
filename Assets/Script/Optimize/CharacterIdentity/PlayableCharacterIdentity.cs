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
    protected ICrouchMovementData _getCrouchMovementData;
    protected IPlayableMovementDataNeeded _getPlayableMovementData;
    protected PlayableCamera _getPlayableCamera;
    protected IAdvancedUseWeaponData _getAdvancedUseWeaponData;
    protected IPlayableUseWeaponDataNeeded _getPlayableUseWeaponData;

    protected FriendAIBehaviourStateMachine _friendAIStateMachine;
    


    #region GETTERSETTER Variable

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
    public ICrouchMovementData GetCrouchMovementData {get { return _getCrouchMovementData;}}
    public IPlayableMovementDataNeeded GetPlayableMovementData {get { return _getPlayableMovementData;}}
    public int FriendID {get { return _friendID;} set { _friendID = value; }}
    public GameObject[] GetFriendsNormalPosition { get {return _friendsNormalPosition;}}
    public PlayableCamera GetPlayableCamera {get {return _getPlayableCamera;}}
    public IAdvancedUseWeaponData GetAdvancedUseWeaponData {get { return _getAdvancedUseWeaponData;}}
    public IPlayableUseWeaponDataNeeded GetPlayableUseWeaponData {get { return _getPlayableUseWeaponData;}}

    public FriendAIBehaviourStateMachine FriendAIStateMachine {get { return _friendAIStateMachine;}}
    public FOVMachine FOVMachine{get { return _fovMachine;}}
    
    #endregion
    protected override void Awake()
    {
        base.Awake();
        _getCrouchMovementData = GetComponent<ICrouchMovementData>();
        _getPlayableMovementData = GetComponent<IPlayableMovementDataNeeded>();
        _getPlayableCamera = GetComponent<PlayableCamera>();

        _getAdvancedUseWeaponData = GetComponent<IAdvancedUseWeaponData>();
        _getPlayableUseWeaponData = GetComponent<IPlayableUseWeaponDataNeeded>();

        _friendAIStateMachine = GetComponent<FriendAIBehaviourStateMachine>();
        
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
    
}

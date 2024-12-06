using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class PlayableCharacterIdentity : CharacterIdentity, IPlayableFriendDataHelper, IReceiveInputFromPlayer, ICanSwitchWeapon, IInteractable
{
    
    [Space(1)]
    [Header("Input Control Now")]
    [SerializeField]protected bool _isPlayerInput;
    public event Action<bool> OnIsPlayerInputChange;

    [Header("Friend Data Helper")]
    // [SerializeField] protected GameObject[] _friendsNormalPosition;
    protected int _friendID;
    [SerializeField] private EntityStatSO _friendStatSO;
        #region Friend STATS

    [Space(1)]
    [Header("   Health")]
    [SerializeField] protected float _totalHealthFriend;
    protected float _currHealthFriend;
    private bool _isReviving;

    [SerializeField] protected GameObject _deadColl;

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
    protected PlayableInteraction _getPlayableInteraction;
    protected PlayableSkill _getPlayableSkill;
    protected PlayableMakeSFX _getPlayableMakeSFX;

    protected FriendAIBehaviourStateMachine _friendAIStateMachine;
    
    
    protected IEnumerator _revCoroutine;
    protected bool _isBeingRevived;
    protected PlayableCharacterIdentity _characterIdentityWhoReviving, _characterIdentityWhoBeingRevived;
    [Header("Death Animation Component")]
    private bool _isAnimatingOtherAnimation;

    public Action OnIsCrawlingChange;
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
    public override bool IsHalfHealthOrLower 
    {
        get
        {
            if(IsPlayerInput)return _currHealth <= _totalHealth/2;
            else return _currHealthFriend <= _totalHealthFriend/2;
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
    // public GameObject[] GetFriendsNormalPosition { get {return _friendsNormalPosition;}}

    public PlayableMovementStateMachine GetPlayableMovementData {get { return _getPlayableMovementStateData;}}
    public PlayableUseWeaponStateMachine GetPlayableUseWeaponData {get { return _getPlayableUseWeaponStateData;}}
    public PlayableCamera GetPlayableCamera {get {return _getPlayableCamera;}}
    public PlayableInteraction GetPlayableInteraction {get {return _getPlayableInteraction;}}
    public PlayableSkill GetPlayableSkill {get {return _getPlayableSkill;}}
    public PlayableMakeSFX GetPlayableMakeSFX {get {return _getPlayableMakeSFX;}}

    public FriendAIBehaviourStateMachine FriendAIStateMachine {get { return _friendAIStateMachine;}}
    public FOVMachine FOVMachine{get { return _fovMachine;}}

    public bool IsAnimatingOtherAnimation {get {return _isAnimatingOtherAnimation;}}

    public Transform InteractableTransform {get{return transform;}}
    public bool CanInteract {get{return IsDead && !IsAnimatingOtherAnimation;}}
    public bool IsReviving {get {return _isReviving;} set { _isReviving = value;}}
    public bool IsSilentKilling {get {return _getPlayableUseWeaponStateData.IsSilentKill;} set { _getPlayableUseWeaponStateData.IsSilentKill = value;}}

    public Transform FriendBeingRevivedPos {get {return _characterIdentityWhoBeingRevived.transform;}}
    

    #endregion
    protected override void Awake()
    {
        base.Awake();
        
        if(_deadColl.activeSelf)_deadColl.gameObject.SetActive(false);
        _getPlayableMovementStateData = MovementStateMachine as PlayableMovementStateMachine;
        _getPlayableUseWeaponStateData = UseWeaponStateMachine as PlayableUseWeaponStateMachine;
        _getPlayableCamera = GetComponent<PlayableCamera>();
        _getPlayableInteraction = GetComponentInChildren<PlayableInteraction>();
        _getPlayableSkill = GetComponent<PlayableSkill>();
        _getPlayableMakeSFX = GetComponentInChildren<PlayableMakeSFX>();
        InitializeFriend();


        _friendAIStateMachine = _aiStateMachine as FriendAIBehaviourStateMachine;
        
    }
    protected override void Start() 
    {
        base.Start();
        EnemyAIManager.Instance.OnEnemyDead += DeleteEnemyFromList;
    }



    protected void Update() 
    {
        RegenerationTimer();
        
        if(!IsPlayerInput)
        {
            if(!_moveStateMachine.IsAtCrouchPlatform && !FriendAIStateMachine.IsAIEngage && !FriendAIStateMachine.GotDetectedbyEnemy)
            {
                if(_getPlayableMovementStateData.IsAskedToCrouchByPlayable && !_getPlayableMovementStateData.IsCrouching)Crouch(true);
                else if(_getPlayableMovementStateData.IsAskedToRunByPlayable && !_getPlayableMovementStateData.IsRunning)Run(true);
            }
        }


        // if(_isBeingRevived)
        // {
        //     AnimatorTransitionInfo currentState = _animator.GetAnimatorTransitionInfo(0);
        //     if(_animator.IsInTransition(0))
        //     {
                
        //         Debug.Log(currentState.normalizedTime + " waktunya animasi");
        //         if (currentState.normalizedTime < 0.9f)
        //         {
        //             Debug.Log("Animasi masih otw2");
        //         }
        //         else
        //         {
        //             Debug.Log("Animasi done");
        //             AfterFinishReviveAnimation();
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log(currentState.normalizedTime + " waktunya animasi2");
        //     }
        // }
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
        _weaponShootVFX.CurrWeaponIdx = _currWeaponIdx;
    }
    protected void Regeneration()
    {
        Heal(TotalHealth * _regenScale * Time.deltaTime);
    }
    protected void RegenerationTimer()
    {
        if(CurrHealth <= TotalHealth && !IsDead)
        {
            if(_friendAIStateMachine.EnemyWhoSawAIList.Count > 0)
            {
                _regenCDTimer = _regenTimerMax;
            }
            else
            {
                if(_regenCDTimer > 0)_regenCDTimer -= Time.deltaTime;
                else
                {
                    _regenCDTimer = 0;
                    Regeneration();
                }
            }
        }
    }

    public override void Death()
    {
        // if(_revCoroutine != null)
        // {
        //     StopCoroutine(_revCoroutine);
        // }
        // if(_isBeingRevived)
        // {
        //     _isBeingRevived = false;
        //     _animator.Play("Rifle_Knock_Out");
        // }

        if(IsReviving)
        {
            if(_characterIdentityWhoBeingRevived)_characterIdentityWhoBeingRevived.CutOutFromBeingRevived();
            StopRevivingFriend();
        }
        base.Death();
        _isAnimatingOtherAnimation = true;
    }

    public override void AfterFinishDeathAnimation()
    {
        if(IsPlayerInput)OnPlayableDeath?.Invoke();
        _getPlayableMovementStateData.SetCharaGameObjRotationToNormal();
        _moveStateMachine.IsCrouching = false;
        _animator.SetTrigger("KnockTrigger");
        
    }
    public void AfterFinishKnockOutAnimation()
    {
        _isAnimatingOtherAnimation = false;
        _deadColl.SetActive(true);
        if(!_getPlayableMovementStateData.IsCrawling)
        {
            _getPlayableMovementStateData.IsCrawling = true;
        }
    }
    

    public void Revive(PlayableCharacterIdentity characterIdentityWhoReviving)
    {
        ForceStopAllStateMachine();
        _deadColl.SetActive(false);
        _isAnimatingOtherAnimation = true;

        _animator.SetBool("BeingRevived", true);

        _characterIdentityWhoReviving = characterIdentityWhoReviving;
        _isBeingRevived = true;
        // _revCoroutine = Reviving(characterIdentityWhoReviving);
        // StartCoroutine(_revCoroutine);
        // AfterFinishReviveAnimation();
    }
    private IEnumerator Reviving(PlayableCharacterIdentity characterIdentityWhoReviving)
    {
        Debug.Log("reviving");
        
        yield return new WaitForSeconds(2f);
        _animator.SetTrigger("ReviveTrigger");
        _fovMachine.enabled = true;

        if(_getPlayableMovementStateData.IsCrawling)_getPlayableMovementStateData.IsCrawling = false;
        ResetHealth();

        _isDead = false;
        characterIdentityWhoReviving.IsReviving = false;
        _isAnimatingOtherAnimation = false;
        
        _revCoroutine = null;
        Debug.Log("reviving Done");
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Death", false);
    }
    public void AfterFinishReviveAnimation()
    {
        // _animator.GetAnimatorTransitionInfo().
        _isBeingRevived = false;
        _fovMachine.enabled = true;

        if(_getPlayableMovementStateData.IsCrawling)_getPlayableMovementStateData.IsCrawling = false;
        ResetHealth();
        _characterIdentityWhoReviving.StopRevivingFriend();
        _characterIdentityWhoReviving = null;
        _isDead = false;
        _isAnimatingOtherAnimation = false;
        _animator.SetBool("BeingRevived", false);
        _animator.SetBool("Death", false);
    }
    public void CutOutFromBeingRevived()
    {
        if(!_isBeingRevived)return;
        _isBeingRevived = false;
        _isAnimatingOtherAnimation = false;
        _animator.SetBool("BeingRevived", false);
        _animator.Play("Rifle_Knock_Out");
        _isDead = true;
        _fovMachine.enabled = false;
        _animator.SetBool("Death", true);
        
        _deadColl.SetActive(true);
        if(!_getPlayableMovementStateData.IsCrawling)
        {
            _getPlayableMovementStateData.IsCrawling = true;
        }

    }
    
    
    public void TurnOnOffFriendAI(bool isTurnOn)
    {
        // FriendAIStateMachine.enabled = isTurnOn;
        if(!isTurnOn)
        {
            FriendAIStateMachine.IsAIEngage = false;
            FriendAIStateMachine.IsAIIdle = true;
            FriendAIStateMachine.NotDetectedAnymore();
            FriendAIStateMachine.IsAtTakingCoverHidingPlace = false;
            FriendAIStateMachine.IsAtTakingCoverHidingPlace = false;
        }
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


    public void Interact(PlayableCharacterIdentity characterIdentityWhoReviving)
    {
        if(_revCoroutine != null || _isBeingRevived)return;
        if(characterIdentityWhoReviving.GetPlayableInteraction.IsHeldingObject)
        {
            characterIdentityWhoReviving.GetPlayableInteraction.RemoveHeldObject();
        }

        characterIdentityWhoReviving.RevivingFriend(this);

        Revive(characterIdentityWhoReviving);
    }
    public void ForceStopAllStateMachine()
    {
        GetPlayableUseWeaponData.ForceStopUseWeapon();
        GetPlayableMovementData.ForceStopMoving();
    }

    #region StateMachine Command
    public override void Run(bool isRunning)
    {
        if(IsPlayerInput)
        {
            if(isRunning)
            {
                if(GetPlayableMovementData.IsMustLookForward)GetPlayableMovementData.IsMustLookForward = false;
                UseWeaponStateMachine.ForceStopUseWeapon();
                // PlayableCharacterCameraManager.Instance.ResetScope();
                if(MovementStateMachine.IsCrouching)
                {
                    MovementStateMachine.IsCrouching = false;
                    // PlayableCharacterCameraManager.Instance.ResetCameraHeight();
                }
            }
            MovementStateMachine.IsRunning = isRunning;
        }
        else
        {
            
            base.Run(isRunning);
        }
        
    }
    public override void Crouch(bool isCrouching)
    {
        if(IsPlayerInput)
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
        else
        {
           if(isCrouching)
            {
                if(MovementStateMachine.IsRunning)
                {
                    MovementStateMachine.IsRunning = false;
                }
                MovementStateMachine.IsCrouching = isCrouching;
            }
            else
            {
                if(!MovementStateMachine.IsAtCrouchPlatform)MovementStateMachine.IsCrouching = isCrouching;
            }
        }
    }
    public override void Aiming(bool isAiming)
    {
        
        if(IsPlayerInput)
        {
            base.Aiming(isAiming);
            GetPlayableMovementData.IsMustLookForward = isAiming;
        }
        else
        {
            base.Aiming(isAiming);
        }
    }
    #endregion
    private void DeleteEnemyFromList(Transform transform)
    {
        _getPlayableInteraction.DeleteKilledEnemyFromList(transform);
        if(_fovMachine.VisibleTargets.Contains(transform))_fovMachine.VisibleTargets.Remove(transform);
        _friendAIStateMachine.DeleteKilledEnemyFromList(transform);
    }


    public void RevivingFriend(PlayableCharacterIdentity characterIdentityWhoBeingRevived)
    {
        if(GetPlayableInteraction.IsHeldingObject)
        {
            GetPlayableInteraction.RemoveHeldObject();
        }
        IsReviving = true;
        _characterIdentityWhoBeingRevived = characterIdentityWhoBeingRevived;
        _animator.SetBool("Reviving", true);
        GetPlayableUseWeaponData.TellToTurnOffScope();
        ForceStopAllStateMachine();
    }
    public void StopRevivingFriend()
    {
        IsReviving = false;
        _characterIdentityWhoBeingRevived = null;
        _animator.SetBool("Reviving", false);
    }

}

using System;
using UnityEngine;


public class PlayableCharacterIdentity : CharacterIdentity, IPlayableFriendDataHelper, IReceiveInputFromPlayer, ICanSwitchWeapon, IInteractable
{
    [Space(1)]
    [Header("Input Control Now")]
    [ReadOnly(false), SerializeField] protected bool _isPlayerInput;
    public event Action<bool> OnIsPlayerInputChange;

    [Header("Friend Data Helper")]
    protected int _friendID;
    [SerializeField] private EntityStatSO _friendStatSO;

        #region Friend STATS

    [Space(1)]
    [Header("   Health")]
    [ReadOnly(false), SerializeField] protected float _totalHealthFriend;
    [ReadOnly(true), SerializeField] protected float _currHealthFriend;
    [ReadOnly(false), SerializeField] private bool _isReviving;

    [SerializeField] protected GameObject _deadColl;

    [Space(1)]
    [Header("   Armour")]
    protected armourType _armourTypeFriend;
    protected float _armourFriend;

    [Space(1)]
    [Header("   Stealth")]
    protected float _stealthStatsFriend;
        #endregion
    
    [Space(1)]
    [Header("Hold Interaction")]
    [ReadOnly(false), SerializeField] private bool _isHoldingInteraction;

    [Space(5)]
    [Header("No Inspector Variable")]
    //Thing needed to getcomponent
    protected PlayableMovementStateMachine _playableMovementStateMachine;
    protected PlayableUseWeaponStateMachine _playableUseWeaponStateMachine;
    protected PlayableCamera _playableCamera;
    protected PlayableInteraction _playableInteraction;
    protected PlayableSkill _playableSkill;
    protected PlayableMakeSFX _playableMakeSFX;
    protected PlayableMinimapSymbolHandler _playableMinimapSymbolHandler;

    protected FriendAIBehaviourStateMachine _friendAIStateMachine;
    
    
    [ReadOnly(false), SerializeField] protected bool _isBeingRevived;
    [ReadOnly(false), SerializeField] protected PlayableCharacterIdentity _characterIdentityWhoReviving, _characterIdentityWhoBeingRevived;
    [Header("Death Animation Component")]
    [ReadOnly(false), SerializeField] private bool _isAnimatingOtherAnimation;

    #region const
    public const string ANIMATION_RIFLE_KNOCK_OUT_NAME = "Rifle_Knock_Out"; 
    public const string ANIMATION_KNOCK_TRIGGER_PARAMETER = "KnockTrigger"; 
    public const string ANIMATION_REVIVING_PARAMETER = "Reviving"; 
    public const string ANIMATION_BEING_REVIVED_PARAMETER = "BeingRevived"; 
    public const string ANIMATION_GUN_COUNTER_PARAMETER = "GunCounter"; 
    public const string ANIMATION_IS_NORMALINTERACTION_PARAMETER = "IsNormalInteraction"; 
    public const string ANIMATION_IS_PICKUPINTERACTION_PARAMETER = "IsPickUpInteraction"; 
    public const string ANIMATION_NORMALINTERACTION_TRIGGER_PARAMETER = "NormalInteractionTrigger";
    public const string ANIMATION_STAND_TAKE_ITEM_CLIP = "Stand_Take_Item";
    public const string ANIMATION_IS_HOLDING_ITEM_PARAMETER = "IsHoldingItem";
    public const string ANIMATION_IS_THROWING_PARAMETER = "IsThrowing";
    #endregion

    private bool _isHoldSwitchState;
    [Space(1)]
    [Header("Event")]
    public Action OnPlayableDeath;
    public Action OnIsCrawlingChange;
    public Action<float, float> OnPlayableHealthChange;
    public Action OnSwitchWeapon;
    public Action OnCancelInteractionButton;

    #region GETTERSETTER Variable

    public override float StealthStat 
    { 
        get{ 
            if(IsPlayerInput) return _stealthStats; 
            else return _stealthStatsFriend; 
        }
    }
    public override float TotalHealth
    { 
        get{ 
            if(IsPlayerInput) return _totalHealth; 
            else return _totalHealthFriend; 
        }
    }
    public override float CurrHealth
    { 
        get
        { 
            if(IsPlayerInput) return _currHealth; 
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
            if(IsPlayerInput) return _currHealth <= _totalHealth/2;
            else return _currHealthFriend <= _totalHealthFriend/2;
        }
    }


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

    public PlayableMovementStateMachine GetPlayableMovementStateMachine {get { return _playableMovementStateMachine;}}
    public PlayableUseWeaponStateMachine GetPlayableUseWeaponStateMachine {get { return _playableUseWeaponStateMachine;}}
    public PlayableCamera GetPlayableCamera {get {return _playableCamera;}}
    public PlayableInteraction GetPlayableInteraction {get {return _playableInteraction;}}
    public PlayableSkill GetPlayableSkill {get {return _playableSkill;}}
    public PlayableMakeSFX GetPlayableMakeSFX {get {return _playableMakeSFX;}}
    public PlayableMinimapSymbolHandler GetPlayableMinimapSymbolHandler {get {return _playableMinimapSymbolHandler;}}

    public FriendAIBehaviourStateMachine GetFriendAIStateMachine {get { return _friendAIStateMachine;}}
    public FOVMachine GetFOVMachine{get { return _fovMachine;}}

    public bool IsAnimatingOtherAnimation {get {return _isAnimatingOtherAnimation;}}

    public Transform GetInteractableTransform {get{return transform;}}
    public bool CanInteract {get{return IsDead && !IsAnimatingOtherAnimation;}}
    public bool IsReviving {get {return _isReviving;} set { _isReviving = value;}}
    public bool IsSilentKilling {get {return _playableUseWeaponStateMachine.IsSilentKill;} set { _playableUseWeaponStateMachine.IsSilentKill = value;}}

    public Transform GetFriendBeingRevivedPos {get {return _characterIdentityWhoBeingRevived.transform;}}

    public bool IsHoldingInteraction 
    {
        get {return _isHoldingInteraction;}
        set 
        {
            if(_isHoldingInteraction != value && !value)
            {
                OnCancelInteractionButton?.Invoke();
            }
            _isHoldingInteraction = value;
        }
    }
    public int CurrWeaponIdx {get{return _currWeaponIdx;}}
    public bool IsHoldSwitchState {get {return _isHoldSwitchState;} set {_isHoldSwitchState = value;}}

    #endregion
    protected override void Awake()
    {
        base.Awake();
        SwitchAnimatorGunCounterToCurr();
        
        if(_deadColl.activeSelf)_deadColl.gameObject.SetActive(false);
        _playableMovementStateMachine = _moveStateMachine as PlayableMovementStateMachine;
        _playableUseWeaponStateMachine = _useWeaponStateMachine as PlayableUseWeaponStateMachine;
        
        if(_playableUseWeaponStateMachine.GetPlayerGunCollider != null) _playableUseWeaponStateMachine.GetPlayerGunCollider.ResetCollider();
        _playableUseWeaponStateMachine.GetPlayerGunCollider = _weaponGameObjectDataContainer.GetPlayerGunCollide();
        

        _playableCamera = GetComponent<PlayableCamera>();
        _playableInteraction = GetComponentInChildren<PlayableInteraction>();
        _playableSkill = GetComponent<PlayableSkill>();
        _playableMakeSFX = GetComponentInChildren<PlayableMakeSFX>();
        _playableMinimapSymbolHandler = GetComponentInChildren<PlayableMinimapSymbolHandler>();
        InitializeFriend();


        _friendAIStateMachine = _aiStateMachine as FriendAIBehaviourStateMachine;
        
    }
    protected override void Start() 
    {
        base.Start();
        _playableUseWeaponStateMachine.GetPlayerGunCollider = _weaponGameObjectDataContainer.GetPlayerGunCollide();
        EnemyAIManager.Instance.OnEnemyDead += DeleteEnemyFromList;
    }



    protected void Update() 
    {
        if(!_gm.IsGamePlaying()) return;
        
        RegenerationTimer();
        
        if(!_isPlayerInput)
        {
            if(!_moveStateMachine.IsAtCrouchPlatform && !_friendAIStateMachine.IsAIEngage && !_friendAIStateMachine.GotDetectedbyEnemy)
            {
                if(_playableMovementStateMachine.IsAskedToCrouchByPlayable && !_playableMovementStateMachine.IsCrouching) Crouch(true);
                else if(_playableMovementStateMachine.IsAskedToRunByPlayable && !_playableMovementStateMachine.IsRunning) Run(true);
            }
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

        _playableUseWeaponStateMachine.SetCharaFriendAccuracy(_friendStatSO.acuracy);
        _stealthStatsFriend = _friendStatSO.stealth;

    }

    public override void SetWeaponGameObjectDataContainer()
    {
        base.SetWeaponGameObjectDataContainer();

        
        // Debug.Log("KOK INI GA KEPANGGIL ????? + " + _animator.GetFloat(ANIMATION_GUN_COUNTER_PARAMETER));
        if(_playableUseWeaponStateMachine != null)
        {
            if(_playableUseWeaponStateMachine.GetPlayerGunCollider != null) _playableUseWeaponStateMachine.GetPlayerGunCollider.ResetCollider();
            _playableUseWeaponStateMachine.GetPlayerGunCollider = _weaponGameObjectDataContainer.GetPlayerGunCollide();

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
        SetWeaponGameObjectDataContainer();
        
        _weaponShootVFX.CurrWeaponIdx = _currWeaponIdx;
        OnSwitchWeapon?.Invoke();
    }
    public void EnsureSwitchWeapon(int oldIdx)
    {
        if(oldIdx == _currWeaponIdx)
        {
            SwitchWeapon();
            SwitchAnimatorGunCounterToCurr();
            GetPlayableUseWeaponStateMachine.SetCurrWeapon();
        }
        else
        {
            SetWeaponGameObjectDataContainer();
            // OnSwitchWeapon?.Invoke();
        }
    }
    public void SwitchAnimatorGunCounterToCurr()
    {
        _animator.SetFloat(ANIMATION_GUN_COUNTER_PARAMETER, (float)_currWeaponIdx);
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
                if(_regenCDTimer > 0) _regenCDTimer -= Time.deltaTime;
                else
                {
                    _regenCDTimer = 0;
                    Regeneration();
                }
            }
        }
    }

    public override void Hurt(float Damage)
    {
        if(CurrHealth <= 0 || !_gm.IsNormalGamePlayMode())return;
        
        _regenCDTimer = _regenTimerMax;
        CurrHealth -= Damage;
        
        OnPlayableHealthChange?.Invoke(CurrHealth, TotalHealth);

        if(CurrHealth <= 0)
        {
            CurrHealth = 0;
            
            if(!immortalized) Death();
        }
    }
    public override void Heal(float Healing)
    {
        base.Heal(Healing);

        OnPlayableHealthChange?.Invoke(CurrHealth, TotalHealth);
    }
    public override void Death()
    {
        if(IsReviving)
        {
            if(_characterIdentityWhoBeingRevived) _characterIdentityWhoBeingRevived.CutOutFromBeingRevived();
            StopRevivingFriend();
        }
        _isHoldSwitchState = false;
        base.Death();

        if(IsHoldingInteraction) IsHoldingInteraction = false;
        if(_playableInteraction.IsHeldingObject) _playableInteraction.RemoveHeldObject();
        StopNormalInteractionAnimation();
        StopPickUpInteractionAnimation();
        StopThrowInteractionAnimation();

        _isAnimatingOtherAnimation = true;
    }

    public override void AfterFinishDeathAnimation()
    {
        if(_isPlayerInput) OnPlayableDeath?.Invoke();
        _playableMovementStateMachine.SetCharaGameObjRotationToNormal();
        _moveStateMachine.IsCrouching = false;
        _animator.SetTrigger(ANIMATION_KNOCK_TRIGGER_PARAMETER);
    }

    public void AfterFinishKnockOutAnimation()
    {
        _isAnimatingOtherAnimation = false;
        _deadColl.SetActive(true);
        if(!_playableMovementStateMachine.IsCrawling)
        {
            _playableMovementStateMachine.IsCrawling = true;
        }
    }

    public void Revive(PlayableCharacterIdentity characterIdentityWhoReviving)
    {
        ForceStopAllStateMachine();
        _deadColl.SetActive(false);
        _isAnimatingOtherAnimation = true;

        _animator.SetBool(ANIMATION_BEING_REVIVED_PARAMETER, true);

        _characterIdentityWhoReviving = characterIdentityWhoReviving;
        _isBeingRevived = true;
    }

    public void AfterFinishReviveAnimation()
    {
        _isBeingRevived = false;
        _fovMachine.enabled = true;

        OnToggleLeftHandRig?.Invoke(true, true);

        if(_playableMovementStateMachine.IsCrawling) _playableMovementStateMachine.IsCrawling = false;
        ResetHealth();
        _characterIdentityWhoReviving.StopRevivingFriend();
        _characterIdentityWhoReviving = null;
        _isDead = false;

        OnPlayableHealthChange?.Invoke(CurrHealth, TotalHealth);
        _isAnimatingOtherAnimation = false;
        _animator.SetBool(ANIMATION_BEING_REVIVED_PARAMETER, false);
        _animator.SetBool(ANIMATION_DEATH_PARAMETER, false);
    }
    public void CutOutFromBeingRevived()
    {
        if(!_isBeingRevived)return;

        _isBeingRevived = false;
        _isAnimatingOtherAnimation = false;
        _animator.SetBool(ANIMATION_BEING_REVIVED_PARAMETER, false);
        _animator.Play(ANIMATION_RIFLE_KNOCK_OUT_NAME);
        _isDead = true;
        _fovMachine.enabled = false;
        _animator.SetBool(ANIMATION_DEATH_PARAMETER, true);
        
        _deadColl.SetActive(true);
        if(!_playableMovementStateMachine.IsCrawling)
        {
            _playableMovementStateMachine.IsCrawling = true;
        }

    }
    
    
    public void TurnOnOffFriendAI(bool isTurnOn)
    {
        if(!isTurnOn)
        {
            _friendAIStateMachine.IsAIEngage = false;
            _friendAIStateMachine.IsAIIdle = true;
            _friendAIStateMachine.NotDetectedAnymore();
            _friendAIStateMachine.IsAtTakingCoverHidingPlace = false;
            _friendAIStateMachine.IsAtTakingCoverHidingPlace = false;
        }
        if(isTurnOn)
        {
            if(!IsDead) _fovMachine.enabled = isTurnOn;
        }
        else 
        {
            if(_fovMachine.enabled) _fovMachine.enabled = isTurnOn;
        }
    }

    public void ResetHealth()
    {
        _currHealth = _totalHealth;
        _currHealthFriend = _totalHealthFriend;
    }


    public void Interact(PlayableCharacterIdentity characterIdentityWhoReviving)
    {
        if(_isBeingRevived) return;

        if(characterIdentityWhoReviving.GetPlayableInteraction.IsHeldingObject)
        {
            characterIdentityWhoReviving.GetPlayableInteraction.RemoveHeldObject();
        }

        characterIdentityWhoReviving.RevivingFriend(this);

        Revive(characterIdentityWhoReviving);
    }
    public void ForceStopAllStateMachine()
    {
        _useWeaponStateMachine.ForceStopUseWeapon();
        _moveStateMachine.ForceStopMoving();
    }

    #region StateMachine Command
    public override void Run(bool isRunning)
    {
        if(IsPlayerInput)
        {
            if(isRunning)
            {
                // if(_playableMovementStateMachine.IsMustLookForward) _playableMovementStateMachine.IsMustLookForward = false;
                // _useWeaponStateMachine.ForceStopUseWeapon();

                if(_moveStateMachine.IsCrouching)
                {
                    _moveStateMachine.IsCrouching = false;
                }
            }
            _moveStateMachine.IsRunning = isRunning;
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
                if(_moveStateMachine.IsRunning)
                {
                    _moveStateMachine.IsRunning = false;
                }
            }
            _moveStateMachine.IsCrouching = isCrouching;
        }
        else
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
    }
    public override void Aiming(bool isAiming)
    {
        
        if(IsPlayerInput)
        {
            base.Aiming(isAiming);
            _playableMovementStateMachine.IsMustLookForward = isAiming;
        }
        else
        {
            base.Aiming(isAiming);
        }
    }
    #endregion
    private void DeleteEnemyFromList(Transform transform)
    {
        _playableInteraction.DeleteKilledEnemyFromList(transform);
        if(_fovMachine.VisibleTargets.Contains(transform)) _fovMachine.VisibleTargets.Remove(transform);
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

        OnToggleLeftHandRig?.Invoke(false, false);
        _animator.SetBool(ANIMATION_REVIVING_PARAMETER, true);
        _playableUseWeaponStateMachine.TellToTurnOffScope();
        ForceStopAllStateMachine();
    }
    public void StopRevivingFriend()
    {
        IsReviving = false;
        _characterIdentityWhoBeingRevived = null;
        _animator.SetBool(ANIMATION_REVIVING_PARAMETER, false);
        
        if(!_playableInteraction.IsHeldingObject) OnToggleLeftHandRig?.Invoke(true, true);
    }

    #region Interaction animation
    public void DoNormalInteractionAnimation()
    {
        OnToggleLeftHandRig?.Invoke(false, false);
        _animator.SetBool(ANIMATION_IS_NORMALINTERACTION_PARAMETER, true);
        _animator.SetTrigger(ANIMATION_NORMALINTERACTION_TRIGGER_PARAMETER);
    }
    public void StopNormalInteractionAnimation()
    {
        if(!_playableInteraction.IsHeldingObject && !_isDead) OnToggleLeftHandRig?.Invoke(true, false);
        _animator.SetBool(ANIMATION_IS_NORMALINTERACTION_PARAMETER, false);
    }
    public void DoPickUpInteractionAnimation()
    {
        OnToggleLeftHandRig?.Invoke(false, false);
        _animator.SetBool(ANIMATION_IS_PICKUPINTERACTION_PARAMETER, true);
        _animator.Play(ANIMATION_STAND_TAKE_ITEM_CLIP);
    }
    public void StopPickUpInteractionAnimation()
    {
        if(!_playableInteraction.IsHeldingObject && !_isDead) OnToggleLeftHandRig?.Invoke(true, true);
        _animator.SetBool(ANIMATION_IS_PICKUPINTERACTION_PARAMETER, false);
    }
    public void ChangeIsHoldingItemAnimationValue(float value)
    {
        _animator.SetFloat(ANIMATION_IS_HOLDING_ITEM_PARAMETER, value);
    }
    public void DoThrowInteractionAnimation()
    {
        _animator.SetBool(ANIMATION_IS_THROWING_PARAMETER, true);
    }
    public void StopThrowInteractionAnimation()
    {
        _animator.SetBool(ANIMATION_IS_THROWING_PARAMETER, false);
    }
    public void ExitThrowInteractionAnimation()
    {
        if(!_playableInteraction.IsHeldingObject && !_isDead) OnToggleLeftHandRig?.Invoke(true, true);
        _playableInteraction.IsAnimatingThrowingItem = false;
    }

    protected override void OnCharaStartRunning()
    {
        if(IsPlayerInput)
        {
            if(_playableMovementStateMachine.IsMustLookForward) _playableMovementStateMachine.IsMustLookForward = false;
        }
        else
        {
            if(_moveStateMachine.AllowLookTarget) _moveStateMachine.AllowLookTarget = false;
        }
        _useWeaponStateMachine.ForceStopUseWeapon();
    }
    #endregion

    public override void UnsubscribeEvent()
    {
        base.UnsubscribeEvent();
        EnemyAIManager.Instance.OnEnemyDead -= DeleteEnemyFromList;
    }
}

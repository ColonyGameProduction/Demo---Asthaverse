using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayableCharacterManager : MonoBehaviour, IUnsubscribeEvent
{
    public static PlayableCharacterManager Instance {get; private set;}
    #region Normal Variable
    [Header("Test")]
    public PlayableCharacterIdentity _chose;

    [Header("Manager Variable")]
    private GameManager _gm;
    private GameInputManager _gameInputManager;
    private AudioManager _am;

    [Header("Character")]
    [SerializeField] private List<PlayableCharacterIdentity> _charaIdentities = new List<PlayableCharacterIdentity>();
    private bool _isFirstTimeSwitch = true;
    private static bool _isSwitchingCharacter;
    private static bool _isAddingRemovingCharacter;
    [SerializeField] private int _totalFriendInControl;
    
    [Space(1)]
    [Header("Command Variable")]
    [SerializeField] protected GameObject[] _friendsCommandPosition;
    private static bool _isCommandingFriend;
    private static bool _isHoldInPlaceFriend;
    // private bool _isLeftMouseClicked;

    [Space(1)]
    [Header("Camera Effect")]
    [SerializeField] private PlayableCharacterCameraManager _playableCharacterCameraManager;

    [Space(1)]
    [Header("Switch Player Variable")]
    [SerializeField] private float _cameraDelayDuration = 0.1f;
    [SerializeField] private float _switchDelayDuration = 1f;

    [Space(1)]
    [Header("Damage UI Handler")]
    private WeaponLogicManager _weaponLogicManager;
    private PlayableCharacterUIManager _playableCharacterUIManager;

    //Saving all curr variable..
    [Header("Curr Playable StateMachine & other data")]
    private int _currCharaidx, _oldIdx;
    //Movement Save
    private PlayableMovementStateMachine _currPlayableMoveStateMachine;
    private PlayableUseWeaponStateMachine _currPlayableUseWeaponStateMachine;
    //Get Standmovement bool -> isIdle, isWalking, isRunning
    private PlayableCamera _currPlayableCamera;
    private PlayableInteraction _currPlayableInteraction;
    private PlayableSkill _currPlayableSkill;
    private PlayableMakeSFX _currPlayableMakeSFX;
    private PlayableMinimapSymbolHandler _currPlayableMinimapSymbolHandler;
    private WorldSoundManager _worldSoundManager;

    [Header("Control Settings Option")]
    [ReadOnly(false), SerializeField] private bool _isScopeModeHold;
    [ReadOnly(false), SerializeField] private bool _isRunModeHold, _isCrouchModeHold;
    private bool _isCanCancelRun_ModeToggle, _isCanCancelScope_ModeToggle, _isCanCancelCrouch_ModeToggle;

    [Header("Events")]
    public Action<bool, int> OnCommandingBoolChange;
    public Action OnRegroupOneFriendInput, OnCommandUnHoldInput;
    public Action<Transform> OnPlayerSwitch;
    #endregion

    #region GETTERSETTER Variable
    //Getter Setter
    public static bool IsSwitchingCharacter { get { return _isSwitchingCharacter;}}
    public static bool IsAddingRemovingCharacter { get { return _isAddingRemovingCharacter;}}
    public static bool IsCommandingFriend { get { return _isCommandingFriend;}}
    public static bool IsHoldInPlaceFriend { get { return _isHoldInPlaceFriend;}}

    public PlayableCharacterIdentity CurrPlayableChara { get { return _charaIdentities[_currCharaidx];}}
    public void SetCurrPlayableSkill(PlayableSkill skill) => _currPlayableSkill = skill;
    public bool IsScopeModeHold 
    {
        get {return _isScopeModeHold;} 
        set 
        {
            if(_isScopeModeHold != value && value)
            {
                _isScopeModeHold = value;
                if(_playableCharacterCameraManager.IsScope)
                {
                    GameInput_OnScopeCanceled();
                }
                _isCanCancelScope_ModeToggle = false;
            }
            else _isScopeModeHold = value;
        }
    }
    public bool IsRunModeHold
    {
        get {return _isRunModeHold;} 
        set 
        {
            if(_isRunModeHold != value && value)
            {
                _isRunModeHold = value;
                if(_currPlayableMoveStateMachine != null && _currPlayableMoveStateMachine.IsRunning)
                {
                    GameInput_OnRunCanceled();
                }
                _isCanCancelRun_ModeToggle = false;
            }
            else _isRunModeHold = value;
        }
    }
    public bool IsCrouchModeHold
    {
        get {return _isCrouchModeHold;} 
        set 
        {
            if(_isCrouchModeHold != value && value)
            {
                _isCrouchModeHold = value;
                if(_currPlayableMoveStateMachine != null && _currPlayableMoveStateMachine.IsCrouching)
                {
                    GameInput_OnCrouchCanceled();
                }
                _isCanCancelCrouch_ModeToggle = false;
            }
            else _isCrouchModeHold = value;
        }
    }

    #endregion
    private void Awake() 
    {
        Instance = this;

        if(_playableCharacterCameraManager == null) _playableCharacterCameraManager = GetComponent<PlayableCharacterCameraManager>();

        _playableCharacterUIManager = GetComponent<PlayableCharacterUIManager>();
    }
    void Start()
    {
        _am = AudioManager.Instance;

        _gm = GameManager.instance;
        _gm.OnPlayerPause += GameManager_OnPlayerPause;
        _gm.OnChangeGamePlayModeToEvent += GameManager_OnChangeGamePlayModeToEvent;
        _gm.OnChangeGamePlayModeToNormal += GameManager_OnChangeGamePlayModeToNormal;

        _worldSoundManager = WorldSoundManager.Instance;

        foreach(GameObject player in _gm.playerGameObject)
        {
            PlayableCharacterIdentity charaIdentity = player.GetComponent<PlayableCharacterIdentity>();
            _charaIdentities.Add(charaIdentity);
            _playableCharacterUIManager.GetCharacterProfileUIHandler.AssignCharaProfileUI(charaIdentity);
        }

        //jd kalo misal ada save save bs lwt sini
        _currCharaidx = 0;
        SetAllCurr();
        SwitchCharacter(_currCharaidx);
        //jd kalo misal ada save save bs lwt sini
        _gameInputManager = GameInputManager.Instance;
        SubscribeToGameInputManager();

    }

    private void GameManager_OnChangeGamePlayModeToNormal()
    {
        foreach(GameObject player in _gm.playerGameObject)
        {
            player.SetActive(true);
        }
    }

    private void GameManager_OnChangeGamePlayModeToEvent()
    {
        GameInput_OnExitCommandPerformed();
        GameInput_OnInteractCanceled();

        ForceStopAllCharacterState();
        
        _playableCharacterCameraManager.ResetScope();
        _playableCharacterCameraManager.ResetNightVision();

        StartCoroutine(CameraDelay());

        foreach(GameObject player in _gm.playerGameObject)
        {
            player.SetActive(false);
        }
    }

    private void GameManager_OnPlayerPause(bool obj)
    {
        if(obj)
        {
            if(_isCommandingFriend) GameInput_OnExitCommandPerformed();
        }
    }

    // Update is called once per frame
    public void Update()
    {
        // if(Input.GetKeyDown(KeyCode.P) && _chose != null && !IsAddingRemovingCharacter)
        // {

        //     AddPlayableCharacter(_chose);
        // }
        // if(Input.GetKeyDown(KeyCode.O) && _chose != null && !IsAddingRemovingCharacter)
        // {
        //     RemovePlayableCharacter(_chose);
        // }
        GameInput_Movement();
    }


    #region character
    #region Character Switching
    // Ganti Karakter
    //     Logic 'Switch Character'
    private void SwitchCharacter(int newIdx)
    {
        
        if(newIdx == _charaIdentities.Count)
        {
            newIdx = 0;
        }
        if(_charaIdentities[newIdx].IsDead)
        {
            for(int i=0; i < _charaIdentities.Count; i++)
            {
                newIdx++;
                if(newIdx == _charaIdentities.Count)
                {
                    newIdx = 0;
                }
                if(!_charaIdentities[newIdx].IsDead)break;
            }
        }

        if(_charaIdentities[newIdx].IsDead)
        {
            _gm.GameOver();
            return;
        }
        

        if(newIdx == _currCharaidx && !_isFirstTimeSwitch) return; //Kalo balik lg ke karakter awal yauda gausa ganti

        _playableCharacterUIManager.GetCharacterProfileUIHandler.SwitchingCharaProfileUI(newIdx);
        
        _isFirstTimeSwitch = false;
        _isSwitchingCharacter = true;
        //Matikan semua pergerakan dan aim dan lainnya - in state machine and player identities
        
        ForceStopAllCharacterState();
        if(_currPlayableInteraction.IsHeldingObject)_currPlayableInteraction.RemoveHeldObject();
        CurrPlayableChara.OnPlayableDeath -= PlayableChara_OnPlayableDeath;
        _currPlayableUseWeaponStateMachine.OnTurningOffScope -= UseWeaponData_OnTurningOffScope;

        CurrPlayableChara.IsPlayerInput = false;
        if(_currPlayableMoveStateMachine.IsTakingCoverAtWall)_currPlayableMoveStateMachine.ExitTakeCover();

        _playableCharacterUIManager.DisconnectUIHandler();
        
        //Kategori kamera
        _playableCharacterCameraManager.ResetCameraHeight(); //hmmmm
        _playableCharacterCameraManager.ResetScope();
        _playableCharacterCameraManager.ResetNightVision();
        _currPlayableCamera.GetFollowCamera.Priority = 1;//Prioritas kamera yg diikuti diturunkan
        StartCoroutine(CameraDelay());

        //kategori untuk friendsAI
        // PlayableCharaNow.FriendID = 1;

        //Time to change Chara
        _oldIdx = _currCharaidx;
        _currCharaidx = newIdx;
        SetActiveCharacter();

        StartCoroutine(Switching());
    }


    // Logic 'Mengaktifkan karakter ketika di switch'
    private void SetActiveCharacter()
    {   
        //Ini uda ganti chara angkanya
        //Turn Off the AI Brain
        CurrPlayableChara.TurnOnOffFriendAI(false);

        //Command dihapuskan
        if(CurrPlayableChara.GetFriendAIStateMachine.IsToldHold) CurrPlayableChara.GetFriendAIStateMachine.IsToldHold = false;

        //Kembalikan semua control utk playable
        CurrPlayableChara.IsPlayerInput = true;
        CurrPlayableChara.ResetHealth();
        SetAllCurr();

        OnPlayerSwitch?.Invoke(CurrPlayableChara.transform); //Kasihtau breadcrumbs player barunya

        CurrPlayableChara.OnPlayableDeath += PlayableChara_OnPlayableDeath;
        _currPlayableUseWeaponStateMachine.OnTurningOffScope += UseWeaponData_OnTurningOffScope;

        //kategori kamera
        _currPlayableCamera.GetFollowCamera.Priority = 2;
        
        //kategori untuk friendsAI - updating move direction etc
        CurrPlayableChara.FriendID = 0;
        _currPlayableMinimapSymbolHandler.ChangeSymbolColorToPlayable();

        int nextCharaidx = _currCharaidx + 1;
        for(int i=1; i <= _charaIdentities.Count - 1; i++)
        {
            if(nextCharaidx == _charaIdentities.Count)nextCharaidx = 0;
            
            _charaIdentities[nextCharaidx].FriendID = i;
            _charaIdentities[nextCharaidx].GetPlayableMinimapSymbolHandler.ChangeSymbolColorToFriend();
            //Di sini nanti jg taro di AI controllernya, posisi update mereka yang biasa
            // Debug.Log(CurrPlayableChara + " AAAAAA" + _currCharaidx);
            _charaIdentities[nextCharaidx].GetFriendAIStateMachine.GiveUpdateFriendDirection(CurrPlayableChara, _friendsCommandPosition[i-1].transform);
            if(!_charaIdentities[nextCharaidx].IsDead)_charaIdentities[nextCharaidx].ResetHealth();
            if(_charaIdentities[nextCharaidx].GetFriendAIStateMachine.IsToldHold)
            {
                _friendsCommandPosition[i-1].transform.position = _charaIdentities[nextCharaidx].transform.position;
            }

            ++nextCharaidx;
        }
        if(_currCharaidx != _oldIdx)_charaIdentities[_oldIdx].TurnOnOffFriendAI(true);
        
    }

    // delay untuk switch karakter
    private IEnumerator Switching()
    {
        yield return new WaitForSeconds(_switchDelayDuration);
        _isSwitchingCharacter = false;
        // Debug.Log(IsSwitchingCharacter);
    }

    // delay untuk perpindahan kamera
    private IEnumerator CameraDelay()
    {
        yield return new WaitForSeconds(_cameraDelayDuration);

        //Pastiin perputaran kameranya br ke awal 
        _currPlayableCamera.GetFollowTarget.eulerAngles = Vector3.zero;
    }
    private void SetAllCurr()
    {
        _playableCharacterUIManager.ConnectUIHandler(CurrPlayableChara);

        //Dapetin semua class data dr CurrPlayableChara jdnya ga ngebebanin pas getter setter terus
        _currPlayableMoveStateMachine = CurrPlayableChara.GetPlayableMovementStateMachine;
        _currPlayableUseWeaponStateMachine = CurrPlayableChara.GetPlayableUseWeaponStateMachine;
        _currPlayableInteraction = CurrPlayableChara.GetPlayableInteraction;
        _currPlayableCamera = CurrPlayableChara.GetPlayableCamera;
        _playableCharacterCameraManager.SetCurrPlayableCamera(_currPlayableCamera);

        _currPlayableSkill = CurrPlayableChara.GetPlayableSkill;
        _currPlayableMakeSFX = CurrPlayableChara.GetPlayableMakeSFX;
        _currPlayableMinimapSymbolHandler = CurrPlayableChara.GetPlayableMinimapSymbolHandler;

    }
    #endregion

    #region Character Add or Remove
    public void AddPlayableCharacter(PlayableCharacterIdentity _chosenChara)
    {
        if(_charaIdentities.Contains(_chosenChara) || (_chosenChara == CurrPlayableChara)) return;

        _isAddingRemovingCharacter = true;
        ForceStopAllCharacterState();

        _charaIdentities.Add(_chosenChara);

        _chosenChara.TurnOnOffFriendAI(true);

        _charaIdentities = _charaIdentities.OrderBy(chara => chara.gameObject.name).ToList();
        SettingCharacterFriends_AddingRemoving();
        _isAddingRemovingCharacter = false;
    }
    
    public void RemovePlayableCharacter(PlayableCharacterIdentity _chosenChara)
    {
        if(!_charaIdentities.Contains(_chosenChara) || (_chosenChara == CurrPlayableChara)) return;
        //ntr yg ini diseeting lg si, bs aja playablechara nya ntr diganti dulu wkwk


        _isAddingRemovingCharacter = true;
        ForceStopAllCharacterState();

        _chosenChara.TurnOnOffFriendAI(false);
        if(_chosenChara.GetFriendAIStateMachine.IsToldHold) _chosenChara.GetFriendAIStateMachine.IsToldHold = false;


        _charaIdentities.Remove(_chosenChara);

        SettingCharacterFriends_AddingRemoving();
        _isAddingRemovingCharacter = false;
    }

    private void SettingCharacterFriends_AddingRemoving()
    {

        int nextCharaidx = _currCharaidx + 1;
        for(int i=1; i <= _charaIdentities.Count - 1; i++)
        {

            if(nextCharaidx == _charaIdentities.Count)nextCharaidx = 0;
            
            _charaIdentities[nextCharaidx].FriendID = i;
            //Di sini nanti jg taro di AI controllernya, posisi update mereka yang biasa
            _charaIdentities[nextCharaidx].GetFriendAIStateMachine.GiveUpdateFriendDirection(CurrPlayableChara, _friendsCommandPosition[i-1].transform);

            if(_charaIdentities[nextCharaidx].GetFriendAIStateMachine.IsToldHold)
            {
                _friendsCommandPosition[i-1].transform.position = _charaIdentities[nextCharaidx].transform.position;
            }

            ++nextCharaidx;
        }

    }
    #endregion
    private void ForceStopAllCharacterState()
    {
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            CurrPlayableChara.IsHoldingInteraction = false;
            chara.GetMovementStateMachine?.ForceStopMoving();
            chara.GetUseWeaponStateMachine?.ForceStopUseWeapon();

        }
    }

    private void PlayableChara_OnPlayableDeath()
    {
        if(IsCommandingFriend)GameInput_OnExitCommandPerformed();
        if(!IsSwitchingCharacter)SwitchCharacter(_currCharaidx + 1);
    }
    #endregion

    #region  Weapon Data
    private void UseWeaponData_OnTurningOffScope()
    {
        if(!_playableCharacterCameraManager.IsScope) return;
        CurrPlayableChara.Aiming(false);
        _playableCharacterCameraManager.ResetScope();
    }
    #endregion

    #region Command Friend
    public void ChangeFriendCommandPosition(int friendID, Vector3 newPos)
    {
        // if(friendID >= _charaIdentities.Count) return; // krn karakter nya mungkin cuma 2, tp kok friend id ada 2
        _friendsCommandPosition[friendID - 1].transform.position = newPos;
        ChangeHoldCommandFriend(true, friendID);
    }
    public void ChangeHoldCommandFriend(bool change, int friendID)
    {
        // if(friendID < 1 || friendID >= _charaIdentities.Count) return;
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            // Debug.Log(chara.FriendID + " " + friendID);
            if(chara.FriendID == friendID)
            {
                if(chara.GetFriendAIStateMachine.IsToldHold != change)chara.GetFriendAIStateMachine.IsToldHold = change;
                if(change == false)
                {
                    _friendsCommandPosition[friendID - 1].transform.position = CurrPlayableChara.transform.position;
                }
                break;
            }
        }
    }
    public void RegroupAllFriendFromCommandHold()
    {
        for(int i=1; i<= _charaIdentities.Count-1;i++)
        {
            ChangeHoldCommandFriend(false, i);
        }
    }
    #endregion

    #region GameInput Validate Function
    public bool CanDoThisFunction()
    {
        if(IsSwitchingCharacter)return false;
        if(IsCommandingFriend)return false;
        if(IsAddingRemovingCharacter)return false;
        return true;
    }
    #endregion

    #region GameInput Event
    private void SubscribeToGameInputManager()
    {
        _gameInputManager.OnRunPerformed += GameInput_OnRunPerformed;
        _gameInputManager.OnRunCanceled += GameInput_OnRunCanceled;
        _gameInputManager.OnCrouchPerformed += GameInput_OnCrouchPerformed;
        _gameInputManager.OnCrouchCanceled += GameInput_OnCrouchCanceled;

        _gameInputManager.OnChangePlayerPerformed += GameInput_OnChangePlayerPerformed;
        _gameInputManager.OnChangeWeaponPerformed += GameInput_OnChangeWeaponPerformed;

        // _gameInputManager.OnCommandPerformed += GameInput_OnCommandPerformed;
        _gameInputManager.OnCommandFriendPerformed += GameInput_OnCommandPerformed;
        _gameInputManager.OnExitCommandPerformed += GameInput_OnExitCommandPerformed;
        _gameInputManager.OnRegroupFriendPerformed += GameInput_OnRegroupFriendPerformed;

        _gameInputManager.OnSilentKillPerformed += GameInput_OnSilentKillPerformed;
        _gameInputManager.OnShootingPerformed += GameInput_OnShootingPerformed;
        _gameInputManager.OnShootingCanceled += GameInput_OnShootingCanceled;
        _gameInputManager.OnScopePerformed += GameInput_OnScopePerformed;
        _gameInputManager.OnScopeCanceled += GameInput_OnScopeCanceled;
        _gameInputManager.OnReloadPerformed += GameInput_OnReloadPerformed;

        _gameInputManager.OnInteractPerformed += GameInput_OnInteractPerformed;
        _gameInputManager.OnInteractCanceled += GameInput_OnInteractCanceled;
        _gameInputManager.OnNightVisionPerformed += GameInput_OnNightVisionPerformed;
        _gameInputManager.OnSkillPerformed += GameInput_OnSkillPerformed;
        _gameInputManager.OnWhistlePerformed += GameInput_OnWhistlePerformed;
        _gameInputManager.OnThrowPerformed += GameInput_OnThrowPerformed;
        _gameInputManager.OnTakeCoverPerformed += GameInput_OnTakeCoverPerformed;
        _gameInputManager.OnExitTakeCoverPerformed += GameInput_OnExitTakeCoverPerformed;
    }

    private void GameInput_OnTakeCoverPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableUseWeaponStateMachine.IsReloading && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && _currPlayableMoveStateMachine.IsNearWall() && !CurrPlayableChara.IsHoldingInteraction)
        {
            if(_currPlayableInteraction.IsHeldingObject) _currPlayableInteraction.RemoveHeldObject();
            if(_playableCharacterCameraManager.IsScope) _playableCharacterCameraManager.ResetScope();
            CurrPlayableChara.ForceStopAllStateMachine();
            _currPlayableMoveStateMachine.TakeCoverAtWall();
            
        }
    }
    private void GameInput_OnExitTakeCoverPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && _currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction)
        {
            _currPlayableMoveStateMachine.ExitTakeCover();
        }   
    }


    private void GameInput_OnThrowPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && _currPlayableInteraction.IsHeldingObject && !CurrPlayableChara.IsHoldingInteraction)
        {
            _currPlayableInteraction.ThrowObject();
        }
    }

    private void GameInput_OnWhistlePerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !CurrPlayableChara.IsHoldingInteraction)
        {
            _worldSoundManager.MakeSound(WorldSoundName.Whistle, CurrPlayableChara.transform.position, CurrPlayableChara.GetFOVMachine.CharaEnemyMask);
            _currPlayableMakeSFX.PlayWhistleSFX();
        }
    }

    private void GameInput_OnInteractPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;
        //Ntr kasi syarat lain
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableUseWeaponStateMachine.IsReloading && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction) _currPlayableInteraction.Interact();
    }
    private void GameInput_OnInteractCanceled()
    {
        if(CurrPlayableChara.IsHoldingInteraction)
        {
            CurrPlayableChara.IsHoldingInteraction = false;
        }
    }

    private void GameInput_OnNightVisionPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead)
        {
            if(_playableCharacterCameraManager.IsNightVision)
            {
                _playableCharacterCameraManager.ResetNightVision();
            }
            else
            {
                _playableCharacterCameraManager.NightVision();
            }

            _am.PlayNighVision(_playableCharacterCameraManager.IsNightVision);
        }
    }
    private void GameInput_OnSkillPerformed()
    {
        if(!_gm.IsGamePlaying()) return;

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && _currPlayableSkill != null && !_currPlayableSkill.IsSkillOnGoing)
        {
            _currPlayableSkill.UseSkill();
        }   
    }


    private void GameInput_Movement()
    {
        if(!_gm.IsGamePlaying()) return;

        if(!_gm.IsNormalGamePlayMode())
        {
            _currPlayableMoveStateMachine.InputMovement = Vector2.zero;
            return;
        }
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !CurrPlayableChara.IsHoldingInteraction)_currPlayableMoveStateMachine.InputMovement = _gameInputManager.Movement();
    }
    private void GameInput_OnRunPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(!_isRunModeHold)
        {
            if(_currPlayableMoveStateMachine.IsRunning)
            {
                _isCanCancelRun_ModeToggle = true;
                GameInput_OnRunCanceled();
                return;
            }
        }

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction )
        {   
            if(_currPlayableMoveStateMachine.IsCrouching)
            {
                if(!_currPlayableMoveStateMachine.IsHeadHitWhenUnCrouch())
                {
                    if(_playableCharacterCameraManager.IsScope)_playableCharacterCameraManager.ResetScope();
                    // _playableCharacterCameraManager.ResetCameraHeight();
                    CurrPlayableChara.Run(true);
                }
            }
            else
            {
                if(_playableCharacterCameraManager.IsScope)_playableCharacterCameraManager.ResetScope();
                CurrPlayableChara.Run(true);
            }
            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                if(chara == CurrPlayableChara)continue;
                chara.GetPlayableMovementStateMachine.IsAskedToRunByPlayable = true;
                chara.GetPlayableMovementStateMachine.IsAskedToCrouchByPlayable = false;
                if(chara.GetFriendAIStateMachine.IsAIEngage || chara.GetFriendAIStateMachine.GotDetectedbyEnemy)continue;
                chara.Run(true);
            }
            
        }
    }

    private void GameInput_OnRunCanceled()
    {
        if(!_isRunModeHold)
        {
            if(_gm.IsGamePlaying())
            {
                if(_isCanCancelRun_ModeToggle)
                {
                    _isCanCancelRun_ModeToggle = false;
                }
                else return;
            }
            else
            {
                _isCanCancelRun_ModeToggle = false;
                return;
            }
        }


        CurrPlayableChara.Run(false);
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            if(chara == CurrPlayableChara)continue;
            chara.GetPlayableMovementStateMachine.IsAskedToRunByPlayable = false;
            if(!chara.GetFriendAIStateMachine.GotDetectedbyEnemy && !chara.GetFriendAIStateMachine.IsAIEngage)chara.Run(false);
        }
    }

    private void GameInput_OnCrouchPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(!_isCrouchModeHold)
        {
            if(_currPlayableMoveStateMachine.IsCrouching)
            {
                _isCanCancelCrouch_ModeToggle = true;
                GameInput_OnCrouchCanceled();
                return;
            }
        }

        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !CurrPlayableChara.IsHoldingInteraction)
        {

            CurrPlayableChara.Crouch(true);
            // _playableCharacterCameraManager.SetCameraCrouchHeight();
            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                if(chara == CurrPlayableChara)continue;
                chara.GetPlayableMovementStateMachine.IsAskedToRunByPlayable = false;
                chara.GetPlayableMovementStateMachine.IsAskedToCrouchByPlayable = true;
                if(chara.GetFriendAIStateMachine.IsAIEngage || chara.GetFriendAIStateMachine.GotDetectedbyEnemy)continue;
                chara.Crouch(true);
            }

        }
    }

    private void GameInput_OnCrouchCanceled()
    {
        if(!_isCrouchModeHold)
        {
            if(_gm.IsGamePlaying())
            {
                if(_isCanCancelCrouch_ModeToggle)
                {
                    _isCanCancelCrouch_ModeToggle = false;
                }
                else return;
            }
            else
            {
                _isCanCancelCrouch_ModeToggle = false;
                return;
            }
        }

        if(!_currPlayableMoveStateMachine.IsTakingCoverAtWall)
        {
            if(!_currPlayableMoveStateMachine.IsHeadHitWhenUnCrouch())
            {
                CurrPlayableChara.Crouch(false);
                // _playableCharacterCameraManager.ResetCameraHeight();
            }
            
        }
        else
        {
            if(_currPlayableMoveStateMachine.IsWallTallerThanChara)
            {
                CurrPlayableChara.Crouch(false);
                // _playableCharacterCameraManager.ResetCameraHeight();
            }
        }
        
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            if(chara == CurrPlayableChara)continue;
            chara.GetPlayableMovementStateMachine.IsAskedToCrouchByPlayable = false;
            if(!chara.GetFriendAIStateMachine.IsAIEngage)chara.Crouch(false);
        }
    }

    private void GameInput_OnChangePlayerPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        //pas silent kill gabole ganti?
        if(CanDoThisFunction())SwitchCharacter(_currCharaidx + 1);
    }
    private void GameInput_OnChangeWeaponPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction)
        {
            if(_currPlayableInteraction.IsHeldingObject)_currPlayableInteraction.RemoveHeldObject();
            _currPlayableUseWeaponStateMachine.IsSwitchingWeapon = true;
        }
    }

    private void GameInput_OnCommandPerformed(int friendID)
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(EnemyAIManager.Instance.IsEnemyEngaging)return;
        PlayableCharacterIdentity chosenFriend = null;
        for(int i=0;i < _charaIdentities.Count;i++)
        {
            if(_charaIdentities[i] == CurrPlayableChara)continue;
            if(_charaIdentities[i].FriendID == friendID)
            {
                chosenFriend = _charaIdentities[i];
                break;
            }  
        }
        if(chosenFriend.IsDead || chosenFriend == null)return;

        if(IsCommandingFriend)OnCommandingBoolChange?.Invoke(true, friendID);

        if(!CanDoThisFunction() || _playableCharacterCameraManager.IsScope || CurrPlayableChara.IsDead || CurrPlayableChara.IsReviving || _currPlayableUseWeaponStateMachine.IsSilentKill || _currPlayableUseWeaponStateMachine.IsReloading || _currPlayableUseWeaponStateMachine.IsSwitchingWeapon || _currPlayableMoveStateMachine.IsTakingCoverAtWall || CurrPlayableChara.IsHoldingInteraction)return;

        
        _currPlayableMoveStateMachine.ForceStopMoving();
        _currPlayableUseWeaponStateMachine.ForceStopUseWeapon();
        Time.timeScale = 0.2f;

        if(chosenFriend.GetFriendAIStateMachine.IsToldHold)
        {
            // _friendsCommandPosition[friendID - 1].transform.position = chosenFriend.transform.position;
        }
        else
        {
            _friendsCommandPosition[friendID - 1].transform.position = CurrPlayableChara.transform.position;
        }

        _isCommandingFriend = true;
        OnCommandingBoolChange?.Invoke(true, friendID);
    }
    private void GameInput_OnExitCommandPerformed()
    {
        if(_isCommandingFriend)
        {   
            Time.timeScale = 1f;
            _isCommandingFriend = false;
            OnCommandingBoolChange?.Invoke(false, -1);
        }
    }
    private void GameInput_OnRegroupFriendPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(_isCommandingFriend)
        {
            OnRegroupOneFriendInput?.Invoke();
        }
        else
        {
            if(!CanDoThisFunction() || _playableCharacterCameraManager.IsScope || CurrPlayableChara.IsDead)return;
            RegroupAllFriendFromCommandHold();
        }
    }

    private void GameInput_OnSilentKillPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsSilentKill && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableInteraction.IsHeldingObject && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction)
        {
            _currPlayableInteraction.SilentKill();
        }
    }
    private void GameInput_OnShootingPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !_currPlayableMoveStateMachine.IsRunning && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableInteraction.IsHeldingObject && !_currPlayableMoveStateMachine.IsTakingCoverAtWall)
        {   
            if(CurrPlayableChara.IsHoldingInteraction) GameInput_OnInteractCanceled();
            CurrPlayableChara.Shooting(true);
            // if(!_currPlayableUseWeaponStateMachine.IsAiming)_currPlayableUseWeaponStateMachine.IsAiming = true;
            // _currPlayableUseWeaponStateMachine.IsUsingWeapon = true;
            // if(!_currPlayableMoveStateMachine.IsMustLookForward)_currPlayableMoveStateMachine.IsMustLookForward = true;
        }
    }
    private void GameInput_OnShootingCanceled()
    {
        if(CanDoThisFunction())
        {
            CurrPlayableChara.Shooting(false);
            _currPlayableUseWeaponStateMachine.IsUsingWeapon = false;
            if(!_playableCharacterCameraManager.IsScope)
            {
                // _currPlayableMoveStateMachine.IsMustLookForward = false;
                // _currPlayableUseWeaponStateMachine.IsAiming = false;
                CurrPlayableChara.Aiming(false);
            }

        }
    }
    private void GameInput_OnScopePerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;
        
        if(!_isScopeModeHold)
        {
            if(_playableCharacterCameraManager.IsScope)
            {
                _isCanCancelScope_ModeToggle = true;
                GameInput_OnScopeCanceled();
                return;
            }
        }

        if(CanDoThisFunction() && !_currPlayableMoveStateMachine.IsRunning && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !_currPlayableUseWeaponStateMachine.IsReloading && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableInteraction.IsHeldingObject && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction)
        {
            //KALO LAGI mo ngescope, tp blm aim, lsg aim nya nyalain jg

            //tp kalo unscope, dan
            CurrPlayableChara.Aiming(true);
            _playableCharacterCameraManager.ScopeCamera();

        }
    }
    private void GameInput_OnScopeCanceled()
    {
        if(!_isScopeModeHold)
        {
            if(_gm.IsGamePlaying())
            {
                if(_isCanCancelScope_ModeToggle)
                {
                    _isCanCancelScope_ModeToggle = false;
                }
                else return;
            }
            else
            {
                _isCanCancelScope_ModeToggle = false;
                return;
            }
        }

        CurrPlayableChara.Aiming(false);
        _playableCharacterCameraManager.ResetScope();
    }
    private void GameInput_OnReloadPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsNormalGamePlayMode()) return;

        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsReloading && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableMoveStateMachine.IsTakingCoverAtWall && !CurrPlayableChara.IsHoldingInteraction)
        {
            if(_currPlayableInteraction.IsHeldingObject)_currPlayableInteraction.RemoveHeldObject();
            _currPlayableUseWeaponStateMachine.IsReloading = true;
        }
    }

    #endregion

    public void UnsubscribeEvent()
    {
        _gm.OnPlayerPause -= GameManager_OnPlayerPause;
        _gm.OnChangeGamePlayModeToEvent -= GameManager_OnChangeGamePlayModeToEvent;
        _gm.OnChangeGamePlayModeToNormal -= GameManager_OnChangeGamePlayModeToNormal;
        CurrPlayableChara.OnPlayableDeath -= PlayableChara_OnPlayableDeath;
        _currPlayableUseWeaponStateMachine.OnTurningOffScope -= UseWeaponData_OnTurningOffScope;


        _gameInputManager.OnRunPerformed -= GameInput_OnRunPerformed;
        _gameInputManager.OnRunCanceled -= GameInput_OnRunCanceled;
        _gameInputManager.OnCrouchPerformed -= GameInput_OnCrouchPerformed;
        _gameInputManager.OnCrouchCanceled -= GameInput_OnCrouchCanceled;

        _gameInputManager.OnChangePlayerPerformed -= GameInput_OnChangePlayerPerformed;
        _gameInputManager.OnChangeWeaponPerformed -= GameInput_OnChangeWeaponPerformed;

        // _gameInputManager.OnCommandPerformed += GameInput_OnCommandPerformed;
        _gameInputManager.OnCommandFriendPerformed -= GameInput_OnCommandPerformed;
        _gameInputManager.OnExitCommandPerformed -= GameInput_OnExitCommandPerformed;
        _gameInputManager.OnRegroupFriendPerformed -= GameInput_OnRegroupFriendPerformed;

        _gameInputManager.OnSilentKillPerformed -= GameInput_OnSilentKillPerformed;
        _gameInputManager.OnShootingPerformed -= GameInput_OnShootingPerformed;
        _gameInputManager.OnShootingCanceled -= GameInput_OnShootingCanceled;
        _gameInputManager.OnScopePerformed -= GameInput_OnScopePerformed;
        _gameInputManager.OnScopeCanceled -= GameInput_OnScopeCanceled;
        _gameInputManager.OnReloadPerformed -= GameInput_OnReloadPerformed;

        _gameInputManager.OnInteractPerformed -= GameInput_OnInteractPerformed;
        _gameInputManager.OnInteractCanceled -= GameInput_OnInteractCanceled;
        _gameInputManager.OnNightVisionPerformed -= GameInput_OnNightVisionPerformed;
        _gameInputManager.OnSkillPerformed -= GameInput_OnSkillPerformed;
        _gameInputManager.OnWhistlePerformed -= GameInput_OnWhistlePerformed;
        _gameInputManager.OnThrowPerformed -= GameInput_OnThrowPerformed;
        _gameInputManager.OnTakeCoverPerformed -= GameInput_OnTakeCoverPerformed;
        _gameInputManager.OnExitTakeCoverPerformed -= GameInput_OnExitTakeCoverPerformed;
    }
}

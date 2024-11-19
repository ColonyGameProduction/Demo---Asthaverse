using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PlayableCharacterManager : MonoBehaviour
{
    #region Normal Variable
    [Header("Test")]
    public PlayableCharacterIdentity _chose;

    [Header("Manager Variable")]
    GameManager _gm;
    GameInputManager _gameInputManager;

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
    private WorldSoundManager _worldSoundManager;


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

    #endregion
    private void Awake() 
    {
        if(_playableCharacterCameraManager == null) _playableCharacterCameraManager = GetComponent<PlayableCharacterCameraManager>();
    }
    void Start()
    {
        _gm = GameManager.instance;
        _worldSoundManager = WorldSoundManager.Instance;

        foreach(GameObject player in _gm.playerGameObject)
        {
            _charaIdentities.Add(player.GetComponent<PlayableCharacterIdentity>());
            
        }

        //jd kalo misal ada save save bs lwt sini
        _currCharaidx = 0;
        SetAllCurr();
        SwitchCharacter(_currCharaidx);
        //jd kalo misal ada save save bs lwt sini
        _gameInputManager = GameInputManager.Instance;
        SubscribeToGameInputManager();

    }

    // Update is called once per frame
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && _chose != null && !IsAddingRemovingCharacter)
        {

            AddPlayableCharacter(_chose);
        }
        if(Input.GetKeyDown(KeyCode.O) && _chose != null && !IsAddingRemovingCharacter)
        {
            RemovePlayableCharacter(_chose);
        }
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
        

        if(newIdx == _currCharaidx && !_isFirstTimeSwitch) return; //Kalo balik lg ke karakter awal yauda gausa ganti

        _isFirstTimeSwitch = false;
        _isSwitchingCharacter = true;
        //Matikan semua pergerakan dan aim dan lainnya - in state machine and player identities

        ForceStopAllCharacterState();
        CurrPlayableChara.OnPlayableDeath -= PlayableChara_OnPlayableDeath;
        _currPlayableUseWeaponStateMachine.OnTurningOffScope -= UseWeaponData_OnTurningOffScope;

        CurrPlayableChara.IsPlayerInput = false;
        
        //Kategori kamera
        _playableCharacterCameraManager.ResetCameraHeight();
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
        if(CurrPlayableChara.FriendAIStateMachine.IsToldHold) CurrPlayableChara.FriendAIStateMachine.IsToldHold = false;

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
        int nextCharaidx = _currCharaidx + 1;
        for(int i=1; i <= _charaIdentities.Count - 1; i++)
        {
            if(nextCharaidx == _charaIdentities.Count)nextCharaidx = 0;
            
            _charaIdentities[nextCharaidx].FriendID = i;
            //Di sini nanti jg taro di AI controllernya, posisi update mereka yang biasa
            _charaIdentities[nextCharaidx].FriendAIStateMachine.GiveUpdateFriendDirection(CurrPlayableChara.transform, CurrPlayableChara.GetFriendsNormalPosition[i-1].transform, _friendsCommandPosition[i-1].transform);
            _charaIdentities[nextCharaidx].ResetHealth();
            if(_charaIdentities[nextCharaidx].FriendAIStateMachine.IsToldHold)
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
        //Dapetin semua class data dr CurrPlayableChara jdnya ga ngebebanin pas getter setter terus
        _currPlayableMoveStateMachine = CurrPlayableChara.GetPlayableMovementData;
        _currPlayableUseWeaponStateMachine = CurrPlayableChara.GetPlayableUseWeaponData;
        _currPlayableInteraction = CurrPlayableChara.GetPlayableInteraction;
        _currPlayableCamera = CurrPlayableChara.GetPlayableCamera;
        _playableCharacterCameraManager.SetCurrPlayableCamera(_currPlayableCamera);

        _currPlayableSkill = CurrPlayableChara.GetPlayableSkill;

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
        if(_chosenChara.FriendAIStateMachine.IsToldHold) _chosenChara.FriendAIStateMachine.IsToldHold = false;


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
            _charaIdentities[nextCharaidx].FriendAIStateMachine.GiveUpdateFriendDirection(CurrPlayableChara.transform, CurrPlayableChara.GetFriendsNormalPosition[i-1].transform, _friendsCommandPosition[i-1].transform);

            if(_charaIdentities[nextCharaidx].FriendAIStateMachine.IsToldHold)
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
            chara.MovementStateMachine?.ForceStopMoving();
            chara.UseWeaponStateMachine?.ForceStopUseWeapon();

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
        if(_playableCharacterCameraManager.IsScope)_playableCharacterCameraManager.ResetScope();
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
                if(chara.FriendAIStateMachine.IsToldHold != change)chara.FriendAIStateMachine.IsToldHold = change;
                if(change == false)
                {
                    _friendsCommandPosition[friendID - 1].transform.position = CurrPlayableChara.GetFriendsNormalPosition[friendID - 1].transform.position;
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
        _gameInputManager.OnReloadPerformed += GameInput_OnReloadPerformed;

        _gameInputManager.OnInteractPerformed += GameInput_OnInteractPerformed;
        _gameInputManager.OnNightVisionPerformed += GameInput_OnNightVisionPerformed;
        _gameInputManager.OnSkillPerformed += GameInput_OnSkillPerformed;
        _gameInputManager.OnWhistlePerformed += GameInput_OnWhistlePerformed;
    }

    private void GameInput_OnWhistlePerformed()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill)
        {
            _worldSoundManager.MakeSound(WorldSoundName.Whistle, CurrPlayableChara.transform.position, CurrPlayableChara.FOVMachine.CharaEnemyMask);
        }
    }

    private void GameInput_OnInteractPerformed()
    {
        //Ntr kasi syarat lain
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill)_currPlayableInteraction.Interact();
    }

    private void GameInput_OnNightVisionPerformed()
    {
        _playableCharacterCameraManager.NightVision();
    }
    private void GameInput_OnSkillPerformed()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill && _currPlayableSkill != null && !_currPlayableSkill.IsSkillOnGoing)
        {
            _currPlayableSkill.UseSkill();
        }   
    }


    private void GameInput_Movement()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill)_currPlayableMoveStateMachine.InputMovement = _gameInputManager.Movement();
    }
    private void GameInput_OnRunPerformed()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill)
        {

            if(_currPlayableMoveStateMachine.IsCrouching)_playableCharacterCameraManager.ResetCameraHeight();
            if(_playableCharacterCameraManager.IsScope)_playableCharacterCameraManager.ResetScope();
            CurrPlayableChara.Run(true);
            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                if(chara == CurrPlayableChara)continue;
                if(chara.FriendAIStateMachine.IsAIEngage || chara.FriendAIStateMachine.GotDetectedbyEnemy)continue;
                chara.Run(true);
            }
            
        }
    }

    private void GameInput_OnRunCanceled()
    {
        CurrPlayableChara.Run(false);
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            if(chara == CurrPlayableChara)continue;
            if(!chara.FriendAIStateMachine.GotDetectedbyEnemy && !chara.FriendAIStateMachine.IsAIEngage)chara.Run(false);
        }
    }

    private void GameInput_OnCrouchPerformed()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill)
        {

            CurrPlayableChara.Crouch(true);
            _playableCharacterCameraManager.SetCameraCrouchHeight();
            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                if(chara == CurrPlayableChara)continue;
                if(chara.FriendAIStateMachine.IsAIEngage || chara.FriendAIStateMachine.GotDetectedbyEnemy)continue;
                chara.Crouch(true);
            }

        }
    }

    private void GameInput_OnCrouchCanceled()
    {
        CurrPlayableChara.Crouch(false);
        _playableCharacterCameraManager.ResetCameraHeight();
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            if(chara == CurrPlayableChara)continue;
            if(!chara.FriendAIStateMachine.IsAIEngage)chara.Crouch(false);
        }
    }

    private void GameInput_OnChangePlayerPerformed()
    {
        //pas silent kill gabole ganti?
        if(CanDoThisFunction())SwitchCharacter(_currCharaidx + 1);
    }
    private void GameInput_OnChangeWeaponPerformed()
    {

        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving && !_currPlayableUseWeaponStateMachine.IsSilentKill)
        {
            _currPlayableUseWeaponStateMachine.IsSwitchingWeapon = true;
        }
    }

    private void GameInput_OnCommandPerformed(int friendID)
    {
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
        if(chosenFriend.IsDead)return;

        if(IsCommandingFriend)OnCommandingBoolChange?.Invoke(true, friendID);

        if(!CanDoThisFunction() || _playableCharacterCameraManager.IsScope || CurrPlayableChara.IsDead || CurrPlayableChara.IsReviving || _currPlayableUseWeaponStateMachine.IsSilentKill)return;
        _currPlayableMoveStateMachine.ForceStopMoving();
        _currPlayableUseWeaponStateMachine.ForceStopUseWeapon();
        Time.timeScale = 0.2f;

        if(chosenFriend.FriendAIStateMachine.IsToldHold)
        {
            _friendsCommandPosition[friendID - 1].transform.position = CurrPlayableChara.GetFriendsNormalPosition[friendID - 1].transform.position;
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
        if(_isCommandingFriend)
        {
            OnRegroupOneFriendInput?.Invoke();
        }
        else
        {
            if(!CanDoThisFunction() || _playableCharacterCameraManager.IsScope || CurrPlayableChara.IsDead)return;RegroupAllFriendFromCommandHold();
        }
    }

    private void GameInput_OnSilentKillPerformed()
    {
        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsSilentKill && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving)
        {
            _currPlayableInteraction.SilentKill();
        }
    }
    private void GameInput_OnShootingPerformed()
    {
        if(CanDoThisFunction() && !_currPlayableMoveStateMachine.IsRunning && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving)
        {   
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
        if(CanDoThisFunction() && !_currPlayableMoveStateMachine.IsRunning && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !_currPlayableUseWeaponStateMachine.IsReloading && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving)
        {
            //KALO LAGI mo ngescope, tp blm aim, lsg aim nya nyalain jg

            //tp kalo unscope, dan
            if(!_playableCharacterCameraManager.IsScope)
            {
                CurrPlayableChara.Aiming(true);
                _playableCharacterCameraManager.ScopeCamera();
            }
            else
            {
                CurrPlayableChara.Aiming(false);
                _playableCharacterCameraManager.ResetScope();
            }

        }
    }
    private void GameInput_OnReloadPerformed()
    {
        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsReloading && !CurrPlayableChara.IsDead && !CurrPlayableChara.IsReviving)
        {
            _currPlayableUseWeaponStateMachine.IsReloading = true;
        }
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PlayableCharacterManager : MonoBehaviour, IPlayableCameraEffect
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
    [SerializeField] private float _normalFOV = 40, _scopeFOV = 60;
    [SerializeField]
    private bool _isScope;
    private bool _isNightVision;

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


    [Header("Events")]
    public Action<bool> OnCommandingBoolChange;
    public Action OnCommandHoldInput, OnCommandUnHoldInput;
    public Action<Transform> OnPlayerSwitch;
    #endregion

    #region GETTERSETTER Variable
    //Getter Setter
    public static bool IsSwitchingCharacter { get { return _isSwitchingCharacter;}}
    public static bool IsAddingRemovingCharacter { get { return _isAddingRemovingCharacter;}}
    public static bool IsCommandingFriend { get { return _isCommandingFriend;}}
    public static bool IsHoldInPlaceFriend { get { return _isHoldInPlaceFriend;}}

    public PlayableCharacterIdentity CurrPlayableChara { get { return _charaIdentities[_currCharaidx];}}
    public bool IsScope {get { return _isScope;}}
    public bool IsNightVision {get { return _isNightVision;}}

    #endregion
    void Start()
    {
        _gm = GameManager.instance;

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
        while(_charaIdentities[newIdx].IsDead)
        {
            newIdx++;
            if(newIdx == _charaIdentities.Count)
            {
                newIdx = 0;
            }
        }
        if(newIdx == _currCharaidx && !_isFirstTimeSwitch) return; //Kalo balik lg ke karakter awal yauda gausa ganti

        _isFirstTimeSwitch = false;
        _isSwitchingCharacter = true;
        //Matikan semua pergerakan dan aim dan lainnya - in state machine and player identities

        ForceStopAllCharacterState();
        CurrPlayableChara.OnPlayableDeath -= PlayableChara_OnPlayableDeath;
        _currPlayableUseWeaponStateMachine.OnTurningOffScope -= UseWeaponData_OnTurningOffScope;

        CurrPlayableChara.IsInputPlayer = false;
        
        //Kategori kamera
        ResetScope(_currCharaidx);
        ResetNightVision(_currCharaidx);
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
        CurrPlayableChara.IsInputPlayer = true;
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
        Debug.Log(IsSwitchingCharacter);
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

        _currPlayableCamera = CurrPlayableChara.GetPlayableCamera;

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
        if(IsCommandingFriend)GameInput_OnUnCommandPerformed();
        if(!IsSwitchingCharacter)SwitchCharacter(_currCharaidx + 1);
    }
    #endregion

    #region Camera Effect
    public void ScopeCamera(int charaIdx)
    {
        _isScope = true;
        _charaIdentities[charaIdx].GetPlayableCamera?.ChangeCameraFOV(_normalFOV);
    }

    public void ResetScope(int charaIdx)
    {
        _isScope = false;
        _charaIdentities[charaIdx].GetPlayableCamera?.ChangeCameraFOV(_scopeFOV);
    }

    public void NightVision(int charaIdx)
    {
        _isNightVision = true;
        //do smth with camera
    }

    public void ResetNightVision(int charaIdx)
    {
        _isNightVision = false;
        //do smth with camera
    }
    #endregion
    #region  Weapon Data
    private void UseWeaponData_OnTurningOffScope()
    {
        if(IsScope)ResetScope(_currCharaidx);
    }
    #endregion

    #region Command Friend
    public void ChangeFriendCommandPosition(int friendID, Vector3 newPos)
    {
        // if(friendID >= _charaIdentities.Count) return; // krn karakter nya mungkin cuma 2, tp kok friend id ada 2
        _friendsCommandPosition[friendID - 1].transform.position = newPos;
    }
    public void ChangeHoldInput(bool change, int friendID)
    {
        // if(friendID < 1 || friendID >= _charaIdentities.Count) return;
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            Debug.Log(chara.FriendID + " " + friendID);
            if(chara.FriendID == friendID)
            {
                chara.FriendAIStateMachine.IsToldHold = change;
                break;
            }
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

        _gameInputManager.OnCommandPerformed += GameInput_OnCommandPerformed;
        _gameInputManager.OnUnCommandPerformed += GameInput_OnUnCommandPerformed;
        _gameInputManager.OnHoldPosPerformed += GameInput_OnHoldPosPerformed;
        _gameInputManager.OnUnHoldPosPerformed += GameInput_OnUnHoldPosPerformed;

        _gameInputManager.OnSilentKillPerformed += GameInput_OnSilentKillPerformed;
        _gameInputManager.OnShootingPerformed += GameInput_OnShootingPerformed;
        _gameInputManager.OnShootingCanceled += GameInput_OnShootingCanceled;
        _gameInputManager.OnScopePerformed += GameInput_OnScopePerformed;
        _gameInputManager.OnReloadPerformed += GameInput_OnReloadPerformed;
    }
    private void GameInput_Movement()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead)_currPlayableMoveStateMachine.InputMovement = _gameInputManager.Movement();
    }
    private void GameInput_OnRunPerformed()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead)
        {
            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                chara.UseWeaponStateMachine.ForceStopUseWeapon();
            }

            // _currUseWeaponFunction.ForceStopUseWeapon();
            if(_currPlayableMoveStateMachine.IsMustLookForward)_currPlayableMoveStateMachine.IsMustLookForward = false;

            if(IsScope)ResetScope(_currCharaidx);
            if(_currPlayableMoveStateMachine.IsCrouching)_currPlayableMoveStateMachine.IsCrouching = false;
            _currPlayableMoveStateMachine.IsRunning = true;

            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                if(chara == CurrPlayableChara)continue;
                if(chara.GetPlayableMovementData.IsCrouching)chara.GetPlayableMovementData.IsCrouching = false;
                chara.MovementStateMachine.IsRunning = true;
            }
            
        }
    }

    private void GameInput_OnRunCanceled()
    {
         _currPlayableMoveStateMachine.IsRunning = false;
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            if(chara == CurrPlayableChara)continue;
            chara.MovementStateMachine.IsRunning = false;
        }
    }

    private void GameInput_OnCrouchPerformed()
    {
        if(CanDoThisFunction() && !CurrPlayableChara.IsDead)
        {
            if(_currPlayableMoveStateMachine.IsRunning)_currPlayableMoveStateMachine.IsRunning = false;
            _currPlayableMoveStateMachine.IsCrouching = true;

            foreach(PlayableCharacterIdentity chara in _charaIdentities)
            {
                if(chara == CurrPlayableChara)continue;
                if(chara.MovementStateMachine.IsRunning)chara.MovementStateMachine.IsRunning = false;
                chara.GetPlayableMovementData.IsCrouching = true;
            }
        }
    }

    private void GameInput_OnCrouchCanceled()
    {
        _currPlayableMoveStateMachine.IsCrouching = false;
        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            if(chara == CurrPlayableChara)continue;
            chara.GetPlayableMovementData.IsCrouching = false;
        }
    }

    private void GameInput_OnChangePlayerPerformed()
    {
        //pas silent kill gabole ganti?
        if(CanDoThisFunction())SwitchCharacter(_currCharaidx + 1);
    }
    private void GameInput_OnChangeWeaponPerformed()
    {

        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !CurrPlayableChara.IsDead)
        {
            _currPlayableUseWeaponStateMachine.IsSwitchingWeapon = true;
        }
    }

    private void GameInput_OnCommandPerformed()
    {
        if(!CanDoThisFunction() || IsScope || CurrPlayableChara.IsDead)return;
        _currPlayableMoveStateMachine.ForceStopMoving();
        _currPlayableUseWeaponStateMachine.ForceStopUseWeapon();

        for(int i=0;i < _charaIdentities.Count;i++)
        {
            if(_charaIdentities[i] == CurrPlayableChara)continue;
            if(!_charaIdentities[i].FriendAIStateMachine.IsToldHold)
            {
                int friendID = _charaIdentities[i].FriendID - 1;
                _friendsCommandPosition[friendID].transform.position = CurrPlayableChara.GetFriendsNormalPosition[friendID].transform.position;
            }
            // _friendsCommandPosition[i].transform.position = CurrPlayableChara.GetFriendsNormalPosition[i].transform.position;
        }

        _isCommandingFriend = true;
        OnCommandingBoolChange?.Invoke(true);
    }
    private void GameInput_OnUnCommandPerformed()
    {
        if(_isCommandingFriend)
        {
            _isCommandingFriend = false;
            OnCommandingBoolChange?.Invoke(false);
        }
    }
    private void GameInput_OnHoldPosPerformed()
    {
        if(_isCommandingFriend)
        {
            OnCommandHoldInput?.Invoke();
        }
    }
    private void GameInput_OnUnHoldPosPerformed()
    {
        if(_isCommandingFriend)
        {
            OnCommandUnHoldInput?.Invoke();
        }
    }

    private void GameInput_OnSilentKillPerformed()
    {
        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsSilentKill && !CurrPlayableChara.IsDead)
        {
            _currPlayableUseWeaponStateMachine.IsSilentKill = true;
        }
    }
    private void GameInput_OnShootingPerformed()
    {
        if(CanDoThisFunction() && !_currPlayableMoveStateMachine.IsRunning && !CurrPlayableChara.IsDead)
        {   
            if(!_currPlayableUseWeaponStateMachine.IsAiming)_currPlayableUseWeaponStateMachine.IsAiming = true;
            _currPlayableUseWeaponStateMachine.IsUsingWeapon = true;
            _currPlayableMoveStateMachine.IsMustLookForward = true;
        }
    }
    private void GameInput_OnShootingCanceled()
    {
        if(CanDoThisFunction())
        {
            _currPlayableUseWeaponStateMachine.IsUsingWeapon = false;
            _currPlayableMoveStateMachine.IsMustLookForward = false;
            if(!IsScope)_currPlayableUseWeaponStateMachine.IsAiming = false;
        }
    }
    private void GameInput_OnScopePerformed()
    {
        if(CanDoThisFunction() && !_currPlayableMoveStateMachine.IsRunning && !_currPlayableUseWeaponStateMachine.IsSilentKill && !_currPlayableUseWeaponStateMachine.IsSwitchingWeapon && !_currPlayableUseWeaponStateMachine.IsReloading && !CurrPlayableChara.IsDead)
        {
            //KALO LAGI mo ngescope, tp blm aim, lsg aim nya nyalain jg

            //tp kalo unscope, dan
            if(!IsScope)
            {
                _currPlayableUseWeaponStateMachine.IsAiming = true;
                _currPlayableMoveStateMachine.IsMustLookForward = true;
                ScopeCamera(_currCharaidx);
            }
            else
            {
                _currPlayableUseWeaponStateMachine.IsAiming = false;
                _currPlayableMoveStateMachine.IsMustLookForward = false;
                ResetScope(_currCharaidx);
            }

        }
    }
    private void GameInput_OnReloadPerformed()
    {
        if(CanDoThisFunction() && !_currPlayableUseWeaponStateMachine.IsReloading && !CurrPlayableChara.IsDead)
        {
            _currPlayableUseWeaponStateMachine.IsReloading = true;
        }
    }

    #endregion
}

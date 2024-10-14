using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayableCharacterManager : MonoBehaviour, IPlayableCameraEffect
{
    #region Normal Variable
    [Header("Manager Variable")]
    GameManager _gm;
    GameInputManager _gameInputManager;

    [Header("Character")]
    [SerializeField] private List<PlayableCharacterIdentity> _charaIdentities = new List<PlayableCharacterIdentity>();
    private static bool _isSwitchingCharacter;
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
    [Header("No Inspector Variable - Saving all Current Data")]
    private int _currCharaidx;
    //Movement Save
    private MovementStateMachine _currMoveStateMachine;
    //Get Standmovement bool -> isIdle, isWalking, isRunning
    private IMovement _currMoveFunction;
    private IStandMovementData _currStandMovementData;
    private ICrouchMovementData _currCrouchMovementData;
    private IPlayableMovementDataNeeded _currPlayableMovementData;
    private PlayableCamera _currPlayableCamera;

    private IUseWeapon _currUseWeaponFunction;
    private INormalUseWeaponData _currNormalUseWeaponData;
    private IAdvancedUseWeaponData _currAdvancedUseWeaponData;
    private IPlayableUseWeaponDataNeeded _currPlayableUseWeaponData;
    
    


    [Header("Events")]
    public Action<bool> OnCommandingBoolChange;
    #endregion

    #region GETTERSETTER Variable
    //Getter Setter
    public static bool IsSwitchingCharacter { get { return _isSwitchingCharacter;}}
    public static bool IsCommandingFriend { get { return _isCommandingFriend;}}
    public static bool IsHoldInPlaceFriend { get { return _isHoldInPlaceFriend;}}
    public PlayableCharacterIdentity PlayableCharaNow { get { return _charaIdentities[_currCharaidx];}}
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
        GameInput_Movement();
    }

    #region Character Switching
    // Ganti Karakter
    //     Logic 'Switch Character'
    private void SwitchCharacter(int newIdx)
    {
        _isSwitchingCharacter = true;
        //Matikan semua pergerakan dan aim dan lainnya - in state machine and player identities

        foreach(PlayableCharacterIdentity chara in _charaIdentities)
        {
            chara.GetMoveFunction?.ForceStopMoving();
            chara.GetUseWeaponFunction?.ForceStopUseWeapon();
        }
        _currPlayableUseWeaponData.OnTurningOffScope -= UseWeaponData_OnTurningOffScope;

        PlayableCharaNow.IsInputPlayer = false;
        
        //Kategori kamera
        ResetScope(_currCharaidx);
        ResetNightVision(_currCharaidx);
        _currPlayableCamera.GetFollowCamera.Priority = 1;//Prioritas kamera yg diikuti diturunkan
        StartCoroutine(CameraDelay());

        //kategori untuk friendsAI
        // PlayableCharaNow.FriendID = 1;


        //Time to change Chara
        if(newIdx == _charaIdentities.Count)
        {
            newIdx = 0;
        }
        _currCharaidx = newIdx;
        SetActiveCharacter();

        StartCoroutine(Switching());
    }


    // Logic 'Mengaktifkan karakter ketika di switch'
    private void SetActiveCharacter()
    {   
        //Ini uda ganti chara angkanya
        //Turn Off the AI Brain
        PlayableCharaNow.FriendAIStateMachine.enabled = false;
        PlayableCharaNow.FOVMachine.enabled = false;

        PlayableCharaNow.IsInputPlayer = true;
        SetAllCurr();
        _currPlayableUseWeaponData.OnTurningOffScope += UseWeaponData_OnTurningOffScope;
        //kategori kamera
        _currPlayableCamera.GetFollowCamera.Priority = 2;

        //kategori untuk friendsAI
        int nextCharaidx = _currCharaidx + 1;
        for(int i=1; i <= _charaIdentities.Count - 1; i++)
        {
            if(nextCharaidx == _charaIdentities.Count)nextCharaidx = 0;
            
            _charaIdentities[nextCharaidx].FriendID = i;
            //Di sini nanti jg taro di AI controllernya, posisi update mereka yang biasa
            _charaIdentities[nextCharaidx].FriendAIStateMachine.GiveUpdateFriendDirection(PlayableCharaNow.GetFriendsNormalPosition[i-1].transform, _friendsCommandPosition[i-1].transform);

            if(IsHoldInPlaceFriend)
            {
                _friendsCommandPosition[i-1].transform.position = _charaIdentities[nextCharaidx].transform.position;
            }

            if(i == _charaIdentities.Count - 1)
            {
                _charaIdentities[nextCharaidx].FriendAIStateMachine.enabled = true;
                _charaIdentities[nextCharaidx].FOVMachine.enabled = true;
            }
            // _charaIdentities[nextCharaidx].MovementStateMachine.GiveAIDirection();
            // Debug.Log(nextCharaidx + " aa"+ i);
            ++nextCharaidx;
        }
        
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
        //Dapetin semua class data dr playablecharanow jdnya ga ngebebanin pas getter setter terus
        _currMoveStateMachine = PlayableCharaNow.MovementStateMachine;
        _currMoveFunction = PlayableCharaNow.GetMoveFunction;
        _currStandMovementData = PlayableCharaNow.GetStandMovementData;
        _currCrouchMovementData = PlayableCharaNow.GetCrouchMovementData;
        _currPlayableMovementData = PlayableCharaNow.GetPlayableMovementData;
        _currPlayableCamera = PlayableCharaNow.GetPlayableCamera;

        _currUseWeaponFunction = PlayableCharaNow.GetUseWeaponFunction;
        _currNormalUseWeaponData = PlayableCharaNow.GetNormalUseWeaponData;
        _currAdvancedUseWeaponData = PlayableCharaNow.GetAdvancedUseWeaponData;
        _currPlayableUseWeaponData = PlayableCharaNow.GetPlayableUseWeaponData;
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
        _friendsCommandPosition[friendID - 1].transform.position = newPos;
    }
    #endregion

    #region GameInput Validate Function
    public bool CanDoThisFunction()
    {
        if(IsSwitchingCharacter)return false;
        if(IsCommandingFriend)return false;
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
        if(CanDoThisFunction())_currPlayableMovementData.InputMovement = _gameInputManager.Movement();
    }
    private void GameInput_OnRunPerformed()
    {
        if(CanDoThisFunction())
        {
            _currUseWeaponFunction.ForceStopUseWeapon();
            if(_currPlayableMovementData.IsMustLookForward)_currPlayableMovementData.IsMustLookForward = false;

            if(IsScope)ResetScope(_currCharaidx);
            if(_currCrouchMovementData.IsCrouching)_currCrouchMovementData.IsCrouching = false;
            _currStandMovementData.IsRunning = true;
        }
    }

    private void GameInput_OnRunCanceled()
    {
         _currStandMovementData.IsRunning = false;
    }

    private void GameInput_OnCrouchPerformed()
    {
        if(CanDoThisFunction())
        {
            if(_currStandMovementData.IsRunning)_currStandMovementData.IsRunning = false;
            _currCrouchMovementData.IsCrouching = true;
        }
    }

    private void GameInput_OnCrouchCanceled()
    {
        _currCrouchMovementData.IsCrouching = false;
    }

    private void GameInput_OnChangePlayerPerformed()
    {
        //pas silent kill gabole ganti?
        if(CanDoThisFunction())SwitchCharacter(_currCharaidx + 1);
    }
    private void GameInput_OnChangeWeaponPerformed()
    {

        if(CanDoThisFunction() && !_currAdvancedUseWeaponData.IsSwitchingWeapon)
        {
            _currAdvancedUseWeaponData.IsSwitchingWeapon = true;
        }
    }

    private void GameInput_OnCommandPerformed()
    {
        if(!CanDoThisFunction() || IsScope)return;
        _currMoveFunction.ForceStopMoving();
        _currUseWeaponFunction.ForceStopUseWeapon();
        if(!_isHoldInPlaceFriend)
        {
            for(int i=0;i < _charaIdentities.Count-1;i++)
            {
                _friendsCommandPosition[i].transform.position = PlayableCharaNow.GetFriendsNormalPosition[i].transform.position;
            }
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
        if(_isCommandingFriend)_isHoldInPlaceFriend = true;
    }
    private void GameInput_OnUnHoldPosPerformed()
    {
        if(_isCommandingFriend)_isHoldInPlaceFriend = false;
    }

    private void GameInput_OnSilentKillPerformed()
    {
        if(CanDoThisFunction() && !_currAdvancedUseWeaponData.IsSilentKill)
        {
            _currAdvancedUseWeaponData.IsSilentKill = true;
        }
    }
    private void GameInput_OnShootingPerformed()
    {
        if(CanDoThisFunction() && !_currStandMovementData.IsRunning)
        {   
            if(!_currNormalUseWeaponData.IsAiming)_currNormalUseWeaponData.IsAiming = true;
            _currNormalUseWeaponData.IsUsingWeapon = true;
            _currPlayableMovementData.IsMustLookForward = true;
        }
    }
    private void GameInput_OnShootingCanceled()
    {
        if(CanDoThisFunction())
        {
            _currNormalUseWeaponData.IsUsingWeapon = false;
            _currPlayableMovementData.IsMustLookForward = false;
            if(!IsScope)_currNormalUseWeaponData.IsAiming = false;
        }
    }
    private void GameInput_OnScopePerformed()
    {
        if(CanDoThisFunction() && !_currStandMovementData.IsRunning && !_currAdvancedUseWeaponData.IsSilentKill && !_currAdvancedUseWeaponData.IsSwitchingWeapon && !_currNormalUseWeaponData.IsReloading)
        {
            //KALO LAGI mo ngescope, tp blm aim, lsg aim nya nyalain jg

            //tp kalo unscope, dan
            if(!IsScope)
            {
                _currNormalUseWeaponData.IsAiming = true;
                _currPlayableMovementData.IsMustLookForward = true;
                ScopeCamera(_currCharaidx);
            }
            else
            {
                _currNormalUseWeaponData.IsAiming = false;
                _currPlayableMovementData.IsMustLookForward = false;
                ResetScope(_currCharaidx);
            }

        }
    }
    private void GameInput_OnReloadPerformed()
    {
        if(CanDoThisFunction() && !_currNormalUseWeaponData.IsReloading)
        {
            _currNormalUseWeaponData.IsReloading = true;
        }
    }

    #endregion
}

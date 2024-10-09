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
    [SerializeField] private PlayableCharacterIdentity[] _charaIdentities;
    private static bool _isSwitchingCharacter;
    private static bool _isSwitchingWeapon;
    private static bool _isCommanding;

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
    private IStandMovement _currStandMovementBool;
    private ICrouch _currCrouchMovementBool;
    private IPlayableMovementDataNeeded _currPlayableMovementData;
    private PlayableCamera _currPlayableCamera;
    #endregion
    
    #region GETTERSETTER Variable
    //Getter Setter
    public static bool IsSwitchingCharacter { get { return _isSwitchingCharacter;}}
    public static bool IsSwitchingWeapon { get { return _isSwitchingWeapon;}}
    public PlayableCharacterIdentity PlayableCharaNow { get { return _charaIdentities[_currCharaidx];}}
    public bool IsScope {get { return _isScope;}}
    public bool IsNightVision {get { return _isNightVision;}}
    #endregion
    void Start()
    {
        _gm = GameManager.instance;
        _charaIdentities = new PlayableCharacterIdentity[_gm.playerGameObject.Length];
        for(int i = 0; i < _gm.playerGameObject.Length; i++)
        {
            _charaIdentities[i] = _gm.playerGameObject[i].GetComponent<PlayableCharacterIdentity>();
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
        }
        PlayableCharaNow.IsInputPlayer = false;
        
        //Kategori kamera
        ResetScope(_currCharaidx);
        ResetNightVision(_currCharaidx);
        _currPlayableCamera.GetFollowCamera.Priority = 1;//Prioritas kamera yg diikuti diturunkan
        StartCoroutine(CameraDelay());

        //kategori untuk friendsAI
        // PlayableCharaNow.FriendID = 1;


        //Time to change Chara
        if(newIdx == _charaIdentities.Length)
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
        PlayableCharaNow.IsInputPlayer = true;
        SetAllCurr();
        //kategori kamera
        _currPlayableCamera.GetFollowCamera.Priority = 2;

        //kategori untuk friendsAI
        int nextCharaidx = _currCharaidx + 1;
        for(int i=1; i <= _charaIdentities.Length - 1; i++)
        {
            if(nextCharaidx == _charaIdentities.Length)nextCharaidx = 0;

            _charaIdentities[nextCharaidx].FriendID = i;
            //Di sini nanti jg taro di AI controllernya, posisi update mereka yang biasa
            _charaIdentities[nextCharaidx].MovementStateMachine.GiveAIDirection(PlayableCharaNow.GetFriendsNormalPosition[i-1].transform);
            Debug.Log(nextCharaidx + " aa"+ i);
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
        _currStandMovementBool = PlayableCharaNow.GetStandMovementBool;
        _currCrouchMovementBool = PlayableCharaNow.GetCrouchMovementBool;
        _currPlayableMovementData = PlayableCharaNow.GetPlayableMovementData;
        _currPlayableCamera = PlayableCharaNow.GetPlayableCamera;
    }
    #endregion

    #region Weapon Switching
    private void SwitchWeapon()
    {
        
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
        if(!IsSwitchingCharacter)_currPlayableMovementData.InputMovement = _gameInputManager.Movement();
    }
    private void GameInput_OnRunPerformed()
    {
        if(!IsSwitchingCharacter)
        {
            if(_currCrouchMovementBool.IsCrouching)_currCrouchMovementBool.IsCrouching = false;
            _currStandMovementBool.IsRunning = true;
        }
    }

    private void GameInput_OnRunCanceled()
    {
         _currStandMovementBool.IsRunning = false;
    }

    private void GameInput_OnCrouchPerformed()
    {
        if(!IsSwitchingCharacter)
        {
            if(_currStandMovementBool.IsRunning)_currStandMovementBool.IsRunning = false;
            _currCrouchMovementBool.IsCrouching = true;
        }
    }

    private void GameInput_OnCrouchCanceled()
    {
        _currCrouchMovementBool.IsCrouching = false;
    }

    private void GameInput_OnChangePlayerPerformed()
    {
        if(!IsSwitchingCharacter)SwitchCharacter(_currCharaidx + 1);
    }
    private void GameInput_OnChangeWeaponPerformed()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnCommandPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnUnCommandPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnHoldPosPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnUnHoldPosPerformed()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnSilentKillPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnShootingPerformed()
    {
        if(!IsSwitchingCharacter)
        {
            _currPlayableMovementData.IsMustLookForward = true;
        }
    }
    private void GameInput_OnShootingCanceled()
    {
        if(!IsSwitchingCharacter)
        {
            _currPlayableMovementData.IsMustLookForward = false;
            if(IsScope)
            {
                ResetScope(_currCharaidx);
            }
        }
    }
    private void GameInput_OnScopePerformed()
    {
        if(!IsSwitchingCharacter)
        {
            _currPlayableMovementData.IsMustLookForward = true;
            if(!IsScope)
            {
                ScopeCamera(_currCharaidx);
            }
            else
            {
                ResetScope(_currCharaidx);
            }

        }
    }
    private void GameInput_OnReloadPerformed()
    {
        
    }

    #endregion
}

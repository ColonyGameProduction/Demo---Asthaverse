using System;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityEngine.Rendering;

public class PlayableCharacterCameraManager : MonoBehaviour, IPlayableCameraEffect, IUnsubscribeEvent
{
    public static PlayableCharacterCameraManager Instance { get;private set; }
    
    private GameManager _gm;
    private PlayableCamera _currPlayableCamera;
    [Header("Camera Effect Variable")]
    [Header("Camera Sensitivity")]
    [SerializeField] private ControlsManager _controlsManager;
    private List<PlayableCamera> _playableCameraList = new List<PlayableCamera>();
    [Header("Camera Scope")]
    [SerializeField] private float _normalFOV = 40, _scopeFOV = 60;
    [SerializeField] private bool _isScope;

    [Header("Camera X-Ray")]
    [Header("Camera Night Vision")]
    [SerializeField] private Volume _nightVisionPostProcessVolume;
    [SerializeField] private float _fullWeight = 0.879f;
    [SerializeField] private bool _isNightVision;
    [SerializeField] private float _changeNightVisionSpeed = 5f;
    private bool _isChangingNightVision;
    private float _nightVisionWeightTarget;


    [Header("Camera Height")]
    [SerializeField] private float _normalHeight = -0.018f;
    [SerializeField] private float _crouchHeight = -0.4f;
    private bool _isNormalHeight = true;

    #region Event
    public static Action OnResetCameraHeight, OnCrouchCameraHeight;
    #endregion

    #region GETTER SETTER VARIABLE
    public bool IsScope {get { return _isScope;}}
    public bool IsNightVision {get { return _isNightVision;}}
    public bool IsNormalHeight {get { return _isNormalHeight;}}
    #endregion
    const float EPSILON = 0.0001f;
    private void Awake() 
    {
        Instance = this;
        _playableCameraList = FindObjectsOfType<PlayableCamera>().ToList();
        _controlsManager.OnSensValueChange += SetAllPlayableCameraSensitivityMultiplier;

        OnResetCameraHeight += ResetCameraHeight;
        OnCrouchCameraHeight += SetCameraCrouchHeight;
        
    }
    private void Start() 
    {
        _gm = GameManager.instance;
    }

    private void Update() 
    {
        if(!_gm.IsGamePlaying()) return;
        
        ChangeNightVisionWeight();
    }
    public void ScopeCamera()
    {
        _isScope = true;
        _currPlayableCamera?.ChangeCameraFOV(_normalFOV);
        MainUICamHandler.OnMainCamChangeFOV(_normalFOV);
    }

    public void ResetScope()
    {
        _isScope = false;
        _currPlayableCamera?.ChangeCameraFOV(_scopeFOV);
        MainUICamHandler.OnMainCamChangeFOV(_scopeFOV);
    }

    public void NightVision()
    {
        _isNightVision = true;
        _isChangingNightVision = true;
        _nightVisionWeightTarget = _fullWeight;
    }

    public void ResetNightVision()
    {
        _isNightVision = false;
        _isChangingNightVision = true;
        _nightVisionWeightTarget = 0f;
    }

    private void ChangeNightVisionWeight()
    {
        if(_isChangingNightVision)
        {
            if(_nightVisionWeightTarget > _nightVisionPostProcessVolume.weight || _nightVisionWeightTarget < _nightVisionPostProcessVolume.weight)
            {
                float newWeight = Mathf.MoveTowards(_nightVisionPostProcessVolume.weight, _nightVisionWeightTarget, Time.deltaTime * _changeNightVisionSpeed);
                if( Mathf.Abs(_nightVisionWeightTarget - newWeight) < EPSILON)
                {
                    newWeight = _nightVisionWeightTarget;
                    _isChangingNightVision = false;
                }
                _nightVisionPostProcessVolume.weight = newWeight;
            }
        }
    }

    public void ResetCameraHeight()
    {
        _isNormalHeight = true;
        _currPlayableCamera.SetCameraHeight(_normalHeight);
    }
    public void SetCameraCrouchHeight()
    {
        _isNormalHeight = false;
        _currPlayableCamera.SetCameraHeight(_crouchHeight);
    }
    public void SetCurrPlayableCamera(PlayableCamera curr)
    {
        _currPlayableCamera = curr;
    }

    public void SetAllPlayableCameraSensitivityMultiplier(float value)
    {
        foreach(PlayableCamera camera in _playableCameraList)
        {
            camera.SetCameraRotationMultiplier = value;
        }
    }

    public void UnsubscribeEvent()
    {
        _controlsManager.OnSensValueChange -= SetAllPlayableCameraSensitivityMultiplier;
        OnResetCameraHeight -= ResetCameraHeight;
        OnCrouchCameraHeight -= SetCameraCrouchHeight;
    }
}

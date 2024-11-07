using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayableCharacterCameraManager : MonoBehaviour, IPlayableCameraEffect
{
    public static PlayableCharacterCameraManager Instance { get;private set; }
    private PlayableCamera _currPlayableCamera;
    [Header("Camera Effect Variable")]
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

    #region GETTER SETTER VARIABLE
    public bool IsScope {get { return _isScope;}}
    public bool IsNightVision {get { return _isNightVision;}}
    public bool IsNormalHeight {get { return _isNormalHeight;}}
    #endregion
    const float EPSILON = 0.0001f;
    private void Awake() 
    {
        Instance = this;
    }

    private void Update() 
    {
        ChangeNightVisionWeight();
    }
    public void ScopeCamera()
    {
        _isScope = true;
        _currPlayableCamera?.ChangeCameraFOV(_normalFOV);
    }

    public void ResetScope()
    {
        _isScope = false;
        _currPlayableCamera?.ChangeCameraFOV(_scopeFOV);
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
}

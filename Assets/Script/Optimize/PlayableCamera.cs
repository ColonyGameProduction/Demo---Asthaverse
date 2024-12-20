using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayableCamera : MonoBehaviour
{
    private GameManager _gm;

    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineVirtualCamera _followCamera;
    [Header("Reference to Follow Target")]
    [SerializeField] private Transform _followTarget;

    [Header("Adjust Camera Rotation Speed")]
    [SerializeField] private float _cameraRotationSpeed = 200f;
    private float _cameraRotationMultiplier = 1f;
    

    [Header("Camera Height Variable")]
    [SerializeField] private float _changeCameraHeightSpeed = 2f;
    private bool _isChangingCameraHeight;
    private float _currTargetHeight;
    const float EPSILON = 0.0001f;
    #region GETTER SETTER VARIABLE

    public CinemachineVirtualCamera GetFollowCamera {get { return _followCamera;}}
    public Transform GetFollowTarget {get{ return _followTarget;}}
    public float SetCameraRotationMultiplier {set {_cameraRotationMultiplier = value;}}
    #endregion

    private void Start() 
    {
        _gm = GameManager.instance;
    }
    private void Update()
    {
        if(!_gm.IsGamePlaying())return;
        
        HandleCameraMovement();
        ChangingHeight();
    }

    private void HandleCameraMovement()
    {
        // mouse input declaration
        float mouseX = Input.GetAxis("Mouse X") * (_cameraRotationSpeed * _cameraRotationMultiplier) * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * (_cameraRotationSpeed * _cameraRotationMultiplier) * Time.deltaTime;

        // move the camera x and y axis with rotating follow target from player
        _followTarget.rotation *= Quaternion.AngleAxis(mouseX, Vector3.up);
        _followTarget.rotation *= Quaternion.AngleAxis(-mouseY, Vector3.right);
        
        

        // prevent camera moving out of bounds
        Vector3 angles = _followTarget.localEulerAngles;
        angles.z = 0f;

        float angle = _followTarget.localEulerAngles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        _followTarget.localEulerAngles = angles;
        

    }

    public void ChangeCameraFOV(float newFOV)
    {
        _followCamera.m_Lens.FieldOfView = newFOV;
    }
    public void SetCameraHeight(float chosenHeight)
    {
        _isChangingCameraHeight = true;
        _currTargetHeight = chosenHeight;
        // _followTarget.localPosition = new Vector3(_followTarget.localPosition.x, chosenHeight, _followTarget.localPosition.z);
        
    }

    private void ChangingHeight()
    {
        if(_isChangingCameraHeight)
        {
            if(_currTargetHeight > _followTarget.localPosition.y || _currTargetHeight < _followTarget.localPosition.y)
            {
                float newHeight = Mathf.MoveTowards(_followTarget.localPosition.y, _currTargetHeight, Time.deltaTime * _changeCameraHeightSpeed);
                if( Mathf.Abs(_currTargetHeight - newHeight) < EPSILON)
                {
                    newHeight = _currTargetHeight;
                    _isChangingCameraHeight = false;
                }
                _followTarget.localPosition = new Vector3(_followTarget.localPosition.x, newHeight, _followTarget.localPosition.z);
            }
        }
    }

}

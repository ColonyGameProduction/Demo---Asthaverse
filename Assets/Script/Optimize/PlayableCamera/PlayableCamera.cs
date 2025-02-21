
using Cinemachine;
using UnityEngine;

public class PlayableCamera : MonoBehaviour
{
    protected GameManager _gm;

    [Header("Cinemachine Camera")]
    [SerializeField] protected CinemachineVirtualCamera _followCamera;
    [Header("Reference to Follow Target")]
    [SerializeField] protected Transform _followTarget;

    [Header("Adjust Camera Rotation Speed")]
    [SerializeField] protected float _cameraRotationSpeed = 200f;
    protected float _cameraRotationMultiplier = 1f;
    
    [Header("Camera Height Variable")]
    protected float _currCamFOV;
    protected float _currTargetCamFOV;
    [SerializeField] private float _changeCameraFOVSpeed = 0.2f;
    private int _leanTweenChangeFOVID;

    [Header("Camera Height Variable")]
    [SerializeField] protected float _changeCameraHeightSpeed = 2f;
    protected bool _isChangingCameraHeight;
    protected float _currTargetHeight;
    const float EPSILON = 0.0001f;

    [Header("Camera View Pos")]
    [ReadOnly(false), SerializeField] private bool _isLeft;
    [SerializeField] private float _moveCameraViewSpeed = 0.2f;
    private int _leanTweenChangeViewID;
    // private float _newView;
    private Cinemachine3rdPersonFollow _cinemachine3rdPersonFollow;
    private float _startPos;

    [Header("Camera Clamp X Variable")]
    [SerializeField] protected float _cameraRotateXClamp = 40;
    [ReadOnly(true), SerializeField] protected float _startCameraRotateX = 0f;
    [ReadOnly(true), SerializeField] protected float _maxUpCameraRotateX = 340, _maxDownCameraRotateX = 40;

    #region GETTER SETTER VARIABLE

    public CinemachineVirtualCamera GetFollowCamera {get { return _followCamera;}}
    public Transform GetFollowTarget {get{ return _followTarget;}}
    public float SetCameraRotationMultiplier {set {_cameraRotationMultiplier = value;}}
    #endregion

    protected virtual void Start() 
    {
        _currCamFOV = _followCamera.m_Lens.FieldOfView;
        SetCameraToLookAtX();
        _gm = GameManager.instance;
        _cinemachine3rdPersonFollow = _followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        _startPos = _cinemachine3rdPersonFollow.ShoulderOffset.x;
    }
    protected virtual void Update()
    {
        if(!_gm.IsGamePlaying())return;
        
        HandleCameraMovement();
        ChangingHeight();
    }

    protected virtual void HandleCameraMovement()
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

        float angleX = _followTarget.localEulerAngles.x;

        if(_maxUpCameraRotateX > _startCameraRotateX && _startCameraRotateX > _maxDownCameraRotateX)
        {
            if(angleX > _maxDownCameraRotateX && angleX < _maxUpCameraRotateX)
            {
                // Debug.Log("Masuk atas Sniper camera X");
                if(angleX <= _startCameraRotateX) angles.x = _maxDownCameraRotateX;
                else angles.x = _maxUpCameraRotateX;
            }
        }
        else
        {
            if((angleX > _maxUpCameraRotateX && angleX > _maxDownCameraRotateX) || (angleX < _maxUpCameraRotateX && angleX < _maxDownCameraRotateX)) 
            {
                // Debug.Log("Masuk bwh Sniper camera X");
                if(_maxUpCameraRotateX > _startCameraRotateX)
                {
                    if(angleX > _maxDownCameraRotateX || (angleX < _maxUpCameraRotateX && angleX < _startCameraRotateX)) angles.x = _maxDownCameraRotateX;
                    else if(angleX < _maxUpCameraRotateX && angleX >= _startCameraRotateX) angles.x = _maxUpCameraRotateX;
                }
                else
                {
                    if(angleX > _maxDownCameraRotateX && angleX < _startCameraRotateX) angles.x = _maxDownCameraRotateX;
                    else angles.x = _maxUpCameraRotateX;
                }
            }
        }

        _followTarget.localEulerAngles = angles;
        

    }

    public void ChangeCameraFOV(float newFOV)
    {
        float startFOV = _followCamera.m_Lens.FieldOfView;
        _currTargetCamFOV = newFOV;

        LeanTween.cancel(_leanTweenChangeFOVID);
        _leanTweenChangeFOVID = LeanTween.value(startFOV, _currTargetCamFOV, _moveCameraViewSpeed).setOnUpdate((float value)=>
            {
                _followCamera.m_Lens.FieldOfView = value;
            }
        ).setOnComplete(()=> {_currCamFOV = _currTargetCamFOV;}).id;
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
    public void ChangeCameraView()
    {
        _isLeft = !_isLeft;
        float oldView = _cinemachine3rdPersonFollow.ShoulderOffset.x;
        float _newView = _isLeft ? -_startPos : _startPos;

        LeanTween.cancel(_leanTweenChangeViewID);
        _leanTweenChangeViewID = LeanTween.value(oldView, _newView, _moveCameraViewSpeed).setOnUpdate((float value)=>
            {
                _cinemachine3rdPersonFollow.ShoulderOffset.x = value;
            }
        ).id;
    }
    private void SetCameraToLookAtX()
    {
        _maxUpCameraRotateX = _startCameraRotateX - _cameraRotateXClamp;
        if(_maxUpCameraRotateX < 0)
        {
            _maxUpCameraRotateX = 360 + _maxUpCameraRotateX;
        }

        _maxDownCameraRotateX = _startCameraRotateX + _cameraRotateXClamp;
        if(_maxDownCameraRotateX > 360)
        {
            _maxDownCameraRotateX = _maxDownCameraRotateX - 360;
        }

        _startCameraRotateX = Mathf.Abs(180 - _startCameraRotateX);
    }

}

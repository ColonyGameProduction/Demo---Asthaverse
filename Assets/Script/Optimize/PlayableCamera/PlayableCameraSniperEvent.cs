
using UnityEngine;

public class PlayableCameraSniperEvent : PlayableCamera
{
    private SniperShootingEvent _sniperShootingEvent;
    private float recoilX, recoilY;
    private bool _goback;
    [SerializeField] private float _recoilDelayMax = 0.2f, _gobackDelayMax = 0.25f;
    [SerializeField] private float _cameraRotateYClamp = 45f;
    [ReadOnly(false), SerializeField]private float _startCameraRotateY, _maxLeftCameraRotateY, _maxRightCameraRotateY;
    private float _recoilDelay, _gobackDelay;
    private float _oldFOV;
    
    private void Awake() 
    {
        _sniperShootingEvent = GetComponent<SniperShootingEvent>();
        
    }
    protected override void Update()
    {
        if(!_gm.IsEventGamePlayMode()) return;
        base.Update();
    }
    public void GiveRecoilToCamera()
    {
        float recoilMod = _sniperShootingEvent.FinalCountRecoil + ((100 - _sniperShootingEvent.AimAccuracy) * _sniperShootingEvent.FinalCountRecoil / 100);
        recoilX = UnityEngine.Random.Range(-recoilMod * 0.25F, recoilMod * 0.25F);
        recoilY = UnityEngine.Random.Range(-recoilMod, recoilMod);
        _recoilDelay = 0;
        _gobackDelay = 0;
        _goback = true;
        HandleCameraMovement();
    }
    private void DelayRecoil()
    {
        if(_recoilDelay < _recoilDelayMax)
        {
            _recoilDelay += Time.deltaTime;
        }
        else
        {
            if(_goback)
            {
                _goback = false;
                recoilX = -recoilX;
                recoilY = -recoilY;
            }
            if(_gobackDelay < _gobackDelayMax)
            {
                _gobackDelay += Time.deltaTime;
            }
            else
            {
                recoilX = 0;
                recoilY = 0;
            }
            
        }
    }
    protected override void HandleCameraMovement()
    {
        // mouse input declaration
        float mouseX = Input.GetAxis("Mouse X") * (_cameraRotationSpeed * _cameraRotationMultiplier) * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * (_cameraRotationSpeed * _cameraRotationMultiplier) * Time.deltaTime;

        mouseX += recoilX;
        mouseY += recoilY;

        DelayRecoil();
        // move the camera x and y axis with rotating follow target from player
        _followTarget.rotation *= Quaternion.AngleAxis(mouseX, Vector3.up);
        _followTarget.rotation *= Quaternion.AngleAxis(-mouseY, Vector3.right);
        
        

        // prevent camera moving out of bounds
        Vector3 angles = _followTarget.localEulerAngles;
        angles.z = 0f;

        float angleX = _followTarget.localEulerAngles.x;
        float angleY = _followTarget.localEulerAngles.y;

        if (angleX > 180 && angleX < 340)
        {
            angles.x = 340;
        }
        else if (angleX < 180 && angleX > 40)
        {
            angles.x = 40;
        }

        // if(angleY > _startCameraRotateY && angleY < _maxLeftCameraRotateY)
        // {
        //     angles.y = _maxLeftCameraRotateY;
        // }
        // else if(angleY < _startCameraRotateY && angleY > _maxRightCameraRotateY)
        // {
        //     angles.y = _maxRightCameraRotateY;
        // }
        // Debug.Log("Sniper camera angle before" + angles);
        if(_maxLeftCameraRotateY > _startCameraRotateY && _startCameraRotateY > _maxRightCameraRotateY)
        {
            if(angleY > _maxRightCameraRotateY && angleY < _maxLeftCameraRotateY)
            {
                // Debug.Log("Masuk atas Sniper camera");
                if(angleY <= _startCameraRotateY) angles.y = _maxRightCameraRotateY;
                else angles.y = _maxLeftCameraRotateY;
            }
        }
        else
        {
            if((angleY > _maxLeftCameraRotateY && angleY > _maxRightCameraRotateY) || (angleY < _maxLeftCameraRotateY && angleY < _maxRightCameraRotateY)) 
            {
                // Debug.Log("Masuk bwh Sniper camera");
                if(_maxLeftCameraRotateY > _startCameraRotateY)
                {
                    if(angleY > _maxRightCameraRotateY || (angleY < _maxLeftCameraRotateY && angleY < _startCameraRotateY)) angles.y = _maxRightCameraRotateY;
                    else if(angleY < _maxLeftCameraRotateY && angleY >= _startCameraRotateY) angles.y = _maxLeftCameraRotateY;
                }
                else
                {
                    if(angleY > _maxRightCameraRotateY && angleY < _startCameraRotateY) angles.y = _maxRightCameraRotateY;
                    else angles.y = _maxLeftCameraRotateY;
                }
            }
        }
        
        
        // Debug.Log("Sniper camera angle now" + angles);
        _followTarget.localEulerAngles = angles;
        

    }

    public void SetCameraToLookAt(Transform newPos)
    {
        // isChangingPlaceToFace = true;
        Quaternion lookRot = Quaternion.LookRotation(newPos.position - _followTarget.position, Vector3.up);
        _followTarget.rotation = lookRot;
        // Debug.Log("Start Sniper camera angle now" + lookRot);
        _startCameraRotateY = _followTarget.eulerAngles.y;
        _maxRightCameraRotateY = _startCameraRotateY + _cameraRotateYClamp;
        if(_maxRightCameraRotateY > 360)
        {
            _maxRightCameraRotateY = _maxRightCameraRotateY - 360;
        }

        _maxLeftCameraRotateY = _startCameraRotateY - _cameraRotateYClamp;
        if(_maxLeftCameraRotateY < 0)
        {
            _maxLeftCameraRotateY = 360 + _maxLeftCameraRotateY;
        }

        _startCameraRotateY = Mathf.Abs(180 - _startCameraRotateY);
        
    }
    public void SetUICameraSnipingFOV()
    {
        // Debug.Log("FOV now" + Camera.main.fieldOfView);
        _oldFOV = Camera.main.fieldOfView;
        if(MainUICamHandler.OnMainCamChangeFOV != null) MainUICamHandler.OnMainCamChangeFOV(1.5f);
        
    }
    public void SetUICameraNormalFOV()
    {
        if(MainUICamHandler.OnMainCamChangeFOV != null) MainUICamHandler.OnMainCamChangeFOV(_oldFOV);
    }
}

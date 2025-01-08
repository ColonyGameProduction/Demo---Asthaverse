using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCameraSniperEvent : PlayableCamera
{
    private SniperShootingEvent _sniperShootingEvent;
    private float recoilX, recoilY;
    [SerializeField] private float _recoilDelayMax = 0.2f, _recoilDelay;
    
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
        recoilX = UnityEngine.Random.Range(-recoilMod*0.9F, recoilMod*0.9F);
        recoilY = UnityEngine.Random.Range(-recoilMod, recoilMod);
        _recoilDelay = 0;
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
            recoilX = 0;
            recoilY = 0;
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
}

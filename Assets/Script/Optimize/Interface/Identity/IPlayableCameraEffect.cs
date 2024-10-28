using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayableCameraEffect
{
    public bool IsScope{ get;}
    public bool IsNightVision { get;}
    public bool IsNormalHeight {get;}
    void ScopeCamera();
    void ResetScope();
    void NightVision();
    void ResetNightVision();
    void ResetCameraHeight();
    void SetCameraCrouchHeight();

}

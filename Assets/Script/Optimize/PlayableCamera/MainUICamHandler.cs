using System;

using UnityEngine;

public class MainUICamHandler : MonoBehaviour, IUnsubscribeEvent
{
    public static Action<float> OnMainCamChangeFOV;
    private Camera _uiCam;


    private void Awake() 
    {
        _uiCam = GetComponent<Camera>();
        OnMainCamChangeFOV += ChangeUICamFOV;
    }

    private void ChangeUICamFOV(float fov)
    {
        _uiCam.fieldOfView = fov;
    }
    public void UnsubscribeEvent()
    {
        OnMainCamChangeFOV -= ChangeUICamFOV;
    }
}

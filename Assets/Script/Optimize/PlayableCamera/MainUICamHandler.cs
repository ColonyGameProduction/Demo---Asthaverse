using System;

using UnityEngine;

public class MainUICamHandler : MonoBehaviour, IUnsubscribeEvent
{
    // public static Action<float> OnMainCamChangeFOV;
    private Camera _uiCam;
    [SerializeField] private Camera _mainCam;


    private void Awake() 
    {
        // _mainCam = GetComponentInParent<Camera>();
        _uiCam = GetComponent<Camera>();
        // OnMainCamChangeFOV += ChangeUICamFOV;
    }
    private void Update()
    {
        ChangeUICamFOV(_mainCam.fieldOfView);
    }

    private void ChangeUICamFOV(float fov)
    {
        _uiCam.fieldOfView = fov;
    }
    public void UnsubscribeEvent()
    {
        // OnMainCamChangeFOV -= ChangeUICamFOV;
    }
}

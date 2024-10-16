using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayableCamera : MonoBehaviour
{
    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineVirtualCamera _followCamera;
    [Header("Reference to Follow Target")]
    [SerializeField] private Transform _followTarget;

    [Header("Adjust Camera Rotation Speed")]
    [SerializeField] private float _cameraRotationSpeed = 200f;
    //getter setter
    public CinemachineVirtualCamera GetFollowCamera {get { return _followCamera;}}
    public Transform GetFollowTarget {get{ return _followTarget;}}

    private void Start()
    {
        // hide mouse cursor when game start

        //Urus cursor nanti
        
    }

    private void Update()
    {
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        // mouse input declaration
        float mouseX = Input.GetAxis("Mouse X") * _cameraRotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _cameraRotationSpeed * Time.deltaTime;

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
}

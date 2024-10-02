using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Reference to Follow Target")]
    public Transform followTarget;

    [Header("Adjust Camera Rotation Speed")]
    public float cameraRotationSpeed = 200f;

    private PlayerAction playerAction;

    private void Awake()
    {
        playerAction = GetComponent<PlayerAction>();
    }

    private void Start()
    {
        HideMouseCursor();
    }

    private void Update()
    {
        HandleCameraMovement();
    }

    private void HideMouseCursor()
    {
        // hide mouse cursor when game start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleCameraMovement()
    {
        // mouse input declaration
        float mouseX = Input.GetAxis("Mouse X") * cameraRotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraRotationSpeed * Time.deltaTime;

        // move the camera x and y axis with rotating follow target from player
        followTarget.rotation *= Quaternion.AngleAxis(mouseX, Vector3.up);
        followTarget.rotation *= Quaternion.AngleAxis(-mouseY, Vector3.right);

        // prevent camera moving out of bounds
        Vector3 angles = followTarget.localEulerAngles;
        angles.z = 0f;

        float angle = followTarget.localEulerAngles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTarget.localEulerAngles = angles;

    }
}

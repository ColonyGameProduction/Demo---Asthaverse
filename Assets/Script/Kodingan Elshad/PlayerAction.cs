using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

//kelas untuk player action seperti attacking, scope, dan Silent kill

public class PlayerAction : ExecuteLogic
{
    private PlayerActionInput inputActions;

    private bool isShooting = false;

    [SerializeField]
    private Transform playerGameObject;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private GameObject[] friendsDestination;

    [SerializeField]
    private float rotateSpeed = 10f;

    [SerializeField]
    private float moveSpeed = 5f;
    private CharacterController CC;

    [SerializeField]
    private GameObject command;

    //supaya input action bisa digunakan
    private void Awake()
    {
        inputActions = new PlayerActionInput();
    }

    private void Start()
    {
        //membuat event untuk menjalankan aksi yang dipakai oleh player
        inputActions.InputPlayerAction.Shooting.performed += Shooting_Performed;
        inputActions.InputPlayerAction.Shooting.canceled += Shooting_canceled;
        inputActions.InputPlayerAction.SilentKill.performed += SilentKill_performed;
        inputActions.InputPlayerAction.ChangePlayer.performed += ChangePlayer_performed;
        inputActions.InputPlayerAction.Scope.performed += Scope_performed;

        CC = GetComponent<CharacterController>();

        StartingSetup();
    }

    

    private bool Run()
    {
        if (inputActions.InputPlayerAction.Run.ReadValue<float>() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Crouch()
    {
        if (inputActions.InputPlayerAction.Crouch.ReadValue<float>() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Scope_performed(InputAction.CallbackContext context)
    {
        Scope();
    }

    private void ChangePlayer_performed(InputAction.CallbackContext context)
    {
        GameManager gm = GameManager.instance;

        if (gm.canSwitch)
        {
            SwitchCharacter();
        }
    }


    //event ketika 'SilentKill' dilakukan
    private void SilentKill_performed(InputAction.CallbackContext context)
    {
        SilentKill();
    }

    //event ketika 'Shoot' dilakukan
    private void Shooting_Performed(InputAction.CallbackContext context)
    {
        isShooting = true;
    }

    private void Shooting_canceled(InputAction.CallbackContext obj)
    {
        isShooting = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            command.SetActive(true);
        }
        else
        {
            command.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if(isShooting)
        {
            Shoot();
        }
        Movement();
    }

    //movement
    private void Movement()
    {       
        Vector2 move = new Vector2(inputActions.InputPlayerAction.Movement.ReadValue<Vector2>().x, inputActions.InputPlayerAction.Movement.ReadValue<Vector2>().y);
        Vector3 movement = new Vector3(move.x, 0, move.y).normalized;

        Vector3 flatForward = new Vector3(followTarget.forward.x, 0, followTarget.forward.z).normalized;
        Vector3 direction = flatForward * movement.z + followTarget.right * movement.x;        

        if (Crouch())
        {
            CC.SimpleMove(direction * (moveSpeed - 2));
        }
        else if (Run())
        {
            CC.SimpleMove(direction * (moveSpeed + 2));
        }
        else
        {
            CC.SimpleMove(direction * moveSpeed);
        }   
        Rotation(direction);
    }

    private void Rotation(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            playerGameObject.forward = Vector3.Slerp(playerGameObject.forward, direction.normalized, Time.deltaTime * rotateSpeed);
        }
    }

    //untuk mendapatkan refrensi player action input
    public PlayerActionInput GetPlayerActionInput()
    {
        return inputActions;
    }

    public GameObject[] GetDestinationGameObject()
    {
        return friendsDestination;
    }

    public Transform GetFollowTargetTransform()
    {
        return followTarget;
    }

    private void OnEnable()
    {
        inputActions.InputPlayerAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.InputPlayerAction.Disable();
    }
}

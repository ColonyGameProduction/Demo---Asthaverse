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
    GameManager gm;
    private PlayerActionInput inputActions;
    private bool isShooting = false;
    private bool isReloading = false;
    private bool fireRateOn = false;

    [Header("Untuk Movement dan Kamera")]
    [SerializeField]
    private Transform playerGameObject;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private GameObject[] friendsDestination;
    [SerializeField]
    private GameObject[] GoToTargetPosition;

    [SerializeField]
    private float rotateSpeed = 10f;
    [SerializeField]
    private float moveSpeed = 5f;
    private CharacterController CC;

    [Header("Weaponry")]
    [SerializeField]
    private LayerMask enemyMask;
    [SerializeField]
    private WeaponStatSO[] weaponStat;
    private WeaponStatSO activeWeapon;

    [SerializeField]
    private EntityStatSO siapaSih;

    [SerializeField]
    private GameObject crosshairPoint;

    private AnimationTestScript testAnimation;
    
    private int curWeapon;

    [SerializeField]
    // private GameObject command;
    public GameObject command;

    //command variables
    public bool isCommandActive = false;
    public bool isHoldPosition = false;
    private int selectedFriendID = -1;

    //supaya input action bisa digunakan
    private void Awake()
    {
        inputActions = new PlayerActionInput();
    }

    private void Start()
    {
        gm = GameManager.instance;
        testAnimation = GetComponent<AnimationTestScript>();

        //membuat event untuk menjalankan aksi yang dipakai oleh player
        inputActions.InputPlayerAction.Shooting.performed += Shooting_Performed;
        inputActions.InputPlayerAction.Shooting.canceled += Shooting_canceled;

        inputActions.InputPlayerAction.SilentKill.performed += SilentKill_performed;
        inputActions.InputPlayerAction.ChangePlayer.performed += ChangePlayer_performed;
        inputActions.InputPlayerAction.ChangingWeapon.performed += ChangingWeapon_performed;
        inputActions.InputPlayerAction.Scope.performed += Scope_performed;
        inputActions.InputPlayerAction.Reload.performed += Reload_performed;

        inputActions.InputPlayerAction.Command.performed += Command_performed;
        inputActions.InputPlayerAction.UnCommand.performed += UnCommand_performed;
        inputActions.InputPlayerAction.HoldPosition.performed += HoldPosition_performed;
        inputActions.InputPlayerAction.UnHoldPosition.performed += UnHoldPosition_performed;

        CC = GetComponent<CharacterController>();

        weaponStat = siapaSih.weaponStat;

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

    private void Reload_performed(InputAction.CallbackContext obj)
    {
        Reload(activeWeapon);
        StartCoroutine(ReloadTime(ReloadFlag, activeWeapon.reloadTime));
    }

    private void ChangingWeapon_performed(InputAction.CallbackContext context)
    {
        ChangeWeapon(this, weaponStat, curWeapon);
    }

    private void Scope_performed(InputAction.CallbackContext context)
    {
        Scope();
        if(gm.scope)
        {
            testAnimation?.animator.SetBool("Scope", true);
        }
        else
        {
            testAnimation?.animator.SetBool("Scope", false);
        }
    }

    private void Command_performed(InputAction.CallbackContext context)
    {
        Command();
    }

    private void UnCommand_performed(InputAction.CallbackContext context)
    {
        UnCommand();
    }

    private void HoldPosition_performed(InputAction.CallbackContext context)
    {
        HoldPosition();
    }

    private void UnHoldPosition_performed(InputAction.CallbackContext context)
    {
        UnHoldPosition();
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
        testAnimation?.animator.SetBool("Scope", true);
        isShooting = true;
        //only once
        if (!activeWeapon.allowHoldDownButton && isShooting && activeWeapon.currBullet > 0 && !isReloading && !fireRateOn)
        {
            Shoot(Camera.main.transform.position, followTarget.transform.position, activeWeapon, enemyMask);
            StartCoroutine(FireRate(FireRateFlag, activeWeapon.fireRate));
            isShooting = false;
            if (activeWeapon.currBullet == 0 && activeWeapon.totalBullet > 0)
            {
                isReloading = true;
                Reload(activeWeapon);
                StartCoroutine(ReloadTime(ReloadFlag, activeWeapon.reloadTime));
            }
        }
    }

    private void Shooting_canceled(InputAction.CallbackContext obj)
    {
        testAnimation?.animator.SetBool("Scope", false);
        isShooting = false;
    }

    private void Update()
    {
        // Input yang ini itu sementara aja

        // Select the AI friend by pressing keys 1, 2, etc.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedFriendID = 1; // Select FriendAI with ID 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedFriendID = 2; // Select FriendAI with ID 2
        }

        // If command is active and a friend is selected, detect mouse click
        if (isCommandActive && selectedFriendID != -1 && Mouse.current.leftButton.wasPressedThisFrame)
        {

            Vector3 rayOrigin = crosshairPoint.transform.position;
            Vector3 rayDirection = crosshairPoint.transform.forward;

            Debug.DrawRay(rayOrigin, rayDirection, Color.red);

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit))
            {
                // Set the destination for the selected friend based on the mouse click
                GoToTargetPosition[selectedFriendID - 1].transform.position = hit.point;

                Debug.Log(hit.point);
            }
        }
    }

    private void FixedUpdate()
    {
        //continous Shoot
        if(isShooting && activeWeapon.allowHoldDownButton && activeWeapon.currBullet > 0 && !isReloading && !fireRateOn)
        {
            if(activeWeapon != null)
            {

                Shoot(Camera.main.transform.position, followTarget.transform.position, activeWeapon, enemyMask);
                StartCoroutine(FireRate(FireRateFlag, activeWeapon.fireRate));
                if (activeWeapon.currBullet == 0)
                {
                    isReloading = true;
                    Reload(activeWeapon);
                    StartCoroutine(ReloadTime(ReloadFlag, activeWeapon.reloadTime));
                }
            }
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

        if(move == Vector2.zero)
        {
            testAnimation?.animator.SetBool("Move", false);
        }
        else
        {
            testAnimation?.animator.SetBool("Move", true);
        }

        testAnimation?.WalkAnimation(move);
        if (!isShooting && !gm.scope)
        {
            
            Rotation(direction);
        }
        else if(isShooting || gm.scope)
        {
            testAnimation?.animator.SetBool("Move", false);
            Rotation(flatForward);
        }
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

    public void SetCurrentWeapon(WeaponStatSO weaponStat, int curWeaponIndex)
    {
        activeWeapon = weaponStat;
        curWeapon = curWeaponIndex;
    }

    public GameObject[] GetPositionGameObject()
    {
        return GoToTargetPosition;
    }

    private void OnEnable()
    {
        curWeapon = 0;
        activeWeapon = weaponStat[curWeapon];
        inputActions.InputPlayerAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.InputPlayerAction.Disable();
    }

    private void ReloadFlag(bool value)
    {
        isReloading = value;
    }

    private void FireRateFlag(bool value)
    {
        fireRateOn = value;
    }

    

}

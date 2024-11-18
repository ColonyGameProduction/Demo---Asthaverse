using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;
using UnityEngine.Rendering;

//kelas untuk player action seperti attacking, scope, dan Silent kill

public class PlayerAction : ExecuteLogic
{
    [Header("TRest")]
    [SerializeField]PlayableMovementStateMachine stateMachine;
    GameManager gm;
    private PlayerActionInput inputActions;
    private bool isShooting = false;
    private bool isReloading = false;
    private bool fireRateOn = false;
    private bool isRun = false;
    private bool IsCrouching = false;
    private bool isMoving = false;

    [Header("Untuk Movement dan Kamera")]
    [SerializeField]
    private Transform playerGameObject;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private Transform aim;
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

    [Header("Audio")]
    public AudioSource footstepsSource;
    public AudioSource whistleSource;

    [SerializeField]
    private EntityStatSO character;
    private float playerHP;

    private AnimationTestScript testAnimation;

    private int curWeapon;

    [SerializeField]
    // private GameObject command;
    public GameObject command;

    //command variables
    public bool isCommandActive = false;
    public bool isHoldPosition = false;
    private int selectedFriendID = -1;

    private int currBreadCrumbs;

    [Header("Object Interaction")]
    public Transform holdPoint;
    public GameObject heldObject;

    private float curRecoil = 0;
    private float curRecoilMod = 0;
    private float maxRecoil = 0;
    private float movingMaxRecoil = 0;
    private float recoilCooldown = 0;
    private float recoilAddMultiplier = 0;

    //supaya input action bisa digunakan
    private void Awake()
    {
        inputActions = new PlayerActionInput();
    }

    private void Start()
    {

        StartCoroutine("BreadCrumbsDrop", .3f);

        gm = GameManager.instance;
        testAnimation = GetComponent<AnimationTestScript>();
        playerHP = character.health;

        //membuat event untuk menjalankan aksi yang dipakai oleh player
        inputActions.InputPlayerAction.Run.performed += Run_performed;
        inputActions.InputPlayerAction.Run.canceled += Run_canceled;

        inputActions.InputPlayerAction.Crouch.performed += Crouch_performed;
        inputActions.InputPlayerAction.Crouch.canceled += Crouch_canceled;

        inputActions.InputPlayerAction.Shooting.performed += Shooting_Performed;
        inputActions.InputPlayerAction.Shooting.canceled += Shooting_canceled;

        inputActions.InputPlayerAction.SilentKill.performed += SilentKill_performed;
        inputActions.InputPlayerAction.ChangePlayer.performed += ChangePlayer_performed;
        inputActions.InputPlayerAction.ChangingWeapon.performed += ChangingWeapon_performed;
        inputActions.InputPlayerAction.Scope.performed += Scope_performed;
        inputActions.InputPlayerAction.Reload.performed += Reload_performed;
        inputActions.InputPlayerAction.Interact.performed += Interact_performed;
        inputActions.InputPlayerAction.Throw.performed += Throw_performed;
        inputActions.InputPlayerAction.NightVision.performed += NightVision_performed;

        inputActions.InputPlayerAction.Command.performed += Command_performed;
        inputActions.InputPlayerAction.UnCommand.performed += UnCommand_performed;
        inputActions.InputPlayerAction.RegroupFriend.performed += HoldPosition_performed;
        inputActions.InputPlayerAction.UnHoldPosition.performed += UnHoldPosition_performed;
        inputActions.InputPlayerAction.Whistle.performed += Whistle_Performed;

        CC = GetComponent<CharacterController>();

        weaponStat = character.weaponStat;

        StartingSetup();
    }

    private void NightVision_performed(InputAction.CallbackContext context)
    {
        Volume nightVision = Camera.main.GetComponent<Volume>();
        if(nightVision.enabled)
        {
            nightVision.enabled = false;
        }
        else
        {
            nightVision.enabled = true;
        }
    }

    private void Crouch_canceled(InputAction.CallbackContext context)
    {
        IsCrouching = false;
    }

    private void Crouch_performed(InputAction.CallbackContext context)
    {
        if(isRun)isRun = false;
        IsCrouching = true;
    }

    private void Run_canceled(InputAction.CallbackContext context)
    {
        isRun = false;
    }

    private void Run_performed(InputAction.CallbackContext context)
    {
        if(IsCrouching)IsCrouching = false;
        isRun = true;
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

        if (isHoldPosition == false) // pas command aktif dan keadaannya KAGA HOLD POSITION
        {
            for (int i = 0; i < GoToTargetPosition.Length; i++)
            {
                GoToTargetPosition[i].transform.position = friendsDestination[i].transform.position; // posisi si friend ini bakal stay dibelakang sesuai posisi dari friendDestination;
            }
        }
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

    private void Interact_performed(InputAction.CallbackContext context)
    {
        Interact();
    }

    private void Throw_performed(InputAction.CallbackContext context)
    {
        if (heldObject != null)
        {
            ThrowObject();
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
        if (!activeWeapon.allowHoldDownButton && isShooting && activeWeapon.currBullet > 0 && !isReloading && !fireRateOn && !isRun)
        {
            if (isMoving)
            {
                if (gm.scope)
                {
                    movingMaxRecoil = activeWeapon.recoil + activeWeapon.recoil * .5f;
                    curRecoilMod = activeWeapon.recoil * .25f;
                    recoilAddMultiplier = 1.5f;
                }
                else
                {
                    movingMaxRecoil = activeWeapon.recoil + activeWeapon.recoil;
                    curRecoilMod = activeWeapon.recoil * .5f;
                    recoilAddMultiplier = 3;
                }
            }
            else if (IsCrouching)
            {
                if (gm.scope)
                {
                    movingMaxRecoil = activeWeapon.recoil;
                    curRecoilMod = activeWeapon.recoil;
                    recoilAddMultiplier = 0f;
                }
                else
                {
                    movingMaxRecoil = activeWeapon.recoil + activeWeapon.recoil * .5f;
                    curRecoilMod = activeWeapon.recoil * .25f;
                    recoilAddMultiplier = 1.5f;
                }
            }
            else
            {
                movingMaxRecoil = 0;
                curRecoilMod = 0;
                recoilAddMultiplier = 0;
            }

            RecoilHandler();
            Shoot(Camera.main.transform.position, Camera.main.transform.forward, character, activeWeapon, enemyMask, curRecoil + curRecoilMod);
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

    //event ketika selesai shoot
    private void Shooting_canceled(InputAction.CallbackContext obj)
    {
        if(!gm.scope)
        {
            testAnimation?.animator.SetBool("Scope", false);

        }
        isShooting = false;
    }

    // event ketika 'Whistle' dilakukan
    private void Whistle_Performed(InputAction.CallbackContext context)
    {
        PlayWhistleSound(whistleSource);
    }

    private void Update()
    {
        // Input yang ini itu sementara aja

        // ketika player move maka sound footsteps bakal aktif
        if (isMoving)
        {
            //PlayFootstepsSound(footstepsSource);
        }

        // ketika player crouch maka volume sound footsteps bakal berkurang
        if (IsCrouching)
        {
            //footstepsSource.volume = 0.2f;
        }
        else
        {
            //footstepsSource.volume = 1.0f;
        }

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
            Vector3 rayOrigin = Camera.main.transform.position;
            Vector3 rayDirection = Camera.main.transform.forward.normalized;

            Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.red, 2f);

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                GoToTargetPosition[selectedFriendID - 1].transform.position = hit.point;
            }
            else if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hithit, 100f, LayerMask.GetMask("Wall")))
            {
                Debug.Log(hithit.point);
                Vector3 movePos = hithit.point;
                Vector3 movePosWithoutY = new Vector3();

                if (Physics.Raycast(hithit.point, Vector3.down, out RaycastHit hit2, 100f, LayerMask.GetMask("Ground")))
                {
                    movePosWithoutY = new Vector3(movePos.x, hit2.point.y, movePos.z);
                }

                // Set the destination for the selected friend based on the mouse click
                GoToTargetPosition[selectedFriendID - 1].transform.position = movePosWithoutY;
            }
        }
        // Vector2 move = new Vector2(inputActions.InputPlayerAction.Movement.ReadValue<Vector2>().x, inputActions.InputPlayerAction.Movement.ReadValue<Vector2>().y);
        // dataMovement.InputMovement = move;
    }

    private void FixedUpdate()
    {
        ComplexRecoil(ref curRecoil);
        //continous Shoot
        if (isShooting && activeWeapon.allowHoldDownButton && activeWeapon.currBullet > 0 && !isReloading && !fireRateOn && !isRun)
        {
            if(activeWeapon != null)
            {
                if (isMoving)
                {
                    if(gm.scope)
                    {
                        movingMaxRecoil = activeWeapon.recoil + activeWeapon.recoil * .5f;
                        curRecoilMod = activeWeapon.recoil * .25f;
                        recoilAddMultiplier = 1.5f;
                    }
                    else
                    {
                        movingMaxRecoil = activeWeapon.recoil + activeWeapon.recoil;
                        curRecoilMod = activeWeapon.recoil * .5f;
                        recoilAddMultiplier = 3;
                    }
                }
                else if (IsCrouching)
                {
                    if(gm.scope)
                    {
                        movingMaxRecoil = activeWeapon.recoil;
                        curRecoilMod = activeWeapon.recoil;
                        recoilAddMultiplier = 0f;
                    }
                    else
                    {
                        movingMaxRecoil = activeWeapon.recoil + activeWeapon.recoil * .5f;
                        curRecoilMod = activeWeapon.recoil * .25f;
                        recoilAddMultiplier = 1.5f;
                    }
                }
                else
                {
                    movingMaxRecoil = 0;
                    curRecoilMod = 0;
                    recoilAddMultiplier = 0;
                }
                RecoilHandler();
                Shoot(Camera.main.transform.position, Camera.main.transform.forward, character, activeWeapon, enemyMask, curRecoil + curRecoilMod);
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

        if (move == Vector2.zero)
        {
            testAnimation?.animator.SetBool("Move", false);
            isMoving = false;
        }
        else
        {
            testAnimation?.animator.SetBool("Move", true);
            isMoving = true;
        }

        if (IsCrouching)
        {
            CC.SimpleMove(direction * (moveSpeed - 2));
        }
        else if (isRun)
        {
            CC.SimpleMove(direction * (moveSpeed + 2));
        }
        else
        {
            CC.SimpleMove(direction * moveSpeed);
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

    private void RecoilHandler()
    {
        recoilCooldown = activeWeapon.fireRate + (activeWeapon.fireRate * .1f);
        maxRecoil = activeWeapon.recoil + movingMaxRecoil;
    }

    private void ComplexRecoil(ref float curRecoil)
    {
        if (recoilCooldown > 0)
        {
            recoilCooldown -= Time.deltaTime * activeWeapon.fireRate;
            if (curRecoil <= maxRecoil)
            {
                if(recoilAddMultiplier != 0)
                {
                    curRecoil += Time.deltaTime * activeWeapon.recoil * recoilAddMultiplier;
                }
                else
                {
                    curRecoil += Time.deltaTime * activeWeapon.recoil;
                }
            }
            else
            {
                curRecoil = maxRecoil;
            }
        }
        else if(recoilCooldown <= 0)
        {
            curRecoil = 0;
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

    public float GetPlayerHP()
    {
        return playerHP + character.armor;
    }

    public void SetPlayerHP(float HP)
    {
        playerHP = HP;
    }

    private IEnumerator BreadCrumbsDrop(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            BreadcrumbsFollowPlayer(this, ref currBreadCrumbs);
        }
    }

    public EntityStatSO GetPlayerStat()
    {
        return character;
    }

}

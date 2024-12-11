using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

//kelas untuk player action seperti attacking, scope, dan Silent kill

public class ArrowData
{
    public GameObject arrow;
    public GameObject whoShootMe;
    public float stayTime;
    public float fadeTime;
}

public class PlayerAction : ExecuteLogic
{


    [Header("TRest")]
    [SerializeField] PlayableMovementStateMachine stateMachine;
    GameManager gm;
    public DamageCanvasHandler damageCanvasHandler;
    public InGameUIHandler inGameUIHandler;
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
    public WeaponStatSO activeWeapon;

    [Header("Audio")]
    public AudioSource footstepsSource;
    public AudioSource whistleSource;

    public EntityStatSO character;
    public float playerHP;
    public float maxPlayerHP;

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

    public List<ArrowData> arrowList = new List<ArrowData>();
    public List<ArrowData> tempArrowList = new List<ArrowData>();

    // Take Cover by raden
    [SerializeField]
    private Transform highCoverDetection;
    [SerializeField]
    private Transform rightCoverDetection;
    [SerializeField]
    private Transform leftCoverDetection;

    private Vector3 coverHitPoint;
    public Vector3 autoMoverTargetPos;

    public bool isHighCover;
    public bool autoMoverActive;

    private Vector3 coverSurfaceDirection;

    [SerializeField]
    private float coverMoveSpeed = 2f;

    private bool isCover;
    public Vector3 inCoverMoveDirection;
    public Vector3 inCoverProhibitedDirection;

    private Transform coverTransform;
    private Vector3 coverDirection;

    public Transform playerComponentParent;
    public Transform playerObjParent;
    public Transform coverRaycastTransformParent;

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


        inputActions.InputPlayerAction.TakeCover.performed += TakeCover_performed;
        inputActions.InputPlayerAction.ExitTakeCover.performed += ExitCover_performed;

        CC = GetComponent<CharacterController>();

        weaponStat = character.weaponStat;

        StartingSetup();
    }

    private void TakeCover_performed(InputAction.CallbackContext context)
    {
        if (IsNearWall())
        {
            SetCoverType();
            MoveCharacterToCover();
        }
    }

    private void ExitCover_performed(InputAction.CallbackContext context)
    {
        if (isCover)
        {
            isCover = false;
            coverRaycastTransformParent.eulerAngles = Vector3.zero;
        }
    }

    private bool IsNearWall()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, playerGameObject.forward, out hit, 2f, LayerMask.GetMask("WallTakeCover")))
        {
            // lakukan sesuatu pada hit
            coverHitPoint = hit.point;

            coverTransform = hit.transform;
            coverDirection = coverTransform.right;

            coverSurfaceDirection = GetCoverSurfaceDirection(hit.normal);

            Vector3 toPlayer = (transform.position - hit.point).normalized;
            if (Vector3.Dot(toPlayer, coverTransform.forward) > 0)
            {
                coverDirection = -coverDirection; // Balik arah jika di sisi belakang
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetCoverType()
    {
        if (Physics.Raycast(highCoverDetection.position, highCoverDetection.forward, 2f, LayerMask.GetMask("WallTakeCover")))
        {
            Debug.Log("High Cover!");
            isHighCover = true;
        }
        else
        {
            Debug.Log("Low Cover!");
            isHighCover = false;
        }
    }

    private void MoveCharacterToCover()
    {
        isCover = true;
        BeginMoveToCover(coverHitPoint);
    }

    private void MoveToCover()
    {
        Vector3 moveDirection = (autoMoverTargetPos - transform.position).normalized;

        if (Vector3.Distance(transform.position, autoMoverTargetPos) > 0.5f)
        {
            GetComponent<CharacterController>().Move(moveDirection * 2f * Time.deltaTime);
        }
        else
        {
            autoMoverActive = false;
            autoMoverTargetPos = Vector3.zero;

            EnableControls();
        }
    }

    public void BeginMoveToCover(Vector3 targetPos)
    {
        DisableControls();

        isCover = true;
        autoMoverActive = true;
        autoMoverTargetPos = targetPos;
    }

    public void DisableControls()
    {
        inputActions.Disable();
    }

    public void EnableControls()
    {
        inputActions.Enable();
    }
    private void OnDrawGizmos()
    {
        if (rightCoverDetection != null && leftCoverDetection != null)
        {
            Gizmos.color = Color.red;

            // Gambar raycast dari rightCoverDetection
            Gizmos.DrawLine(rightCoverDetection.position, rightCoverDetection.position + rightCoverDetection.forward * 2f);

            // Gambar raycast dari leftCoverDetection
            Gizmos.DrawLine(leftCoverDetection.position, leftCoverDetection.position + leftCoverDetection.forward * 2f);

            // Deteksi hit dengan LayerMask WallTakeCover
            bool didRightCoverDetectorHit = Physics.Raycast(rightCoverDetection.position, rightCoverDetection.forward, 2f, LayerMask.GetMask("WallTakeCover"));
            bool didLeftCoverDetectorHit = Physics.Raycast(leftCoverDetection.position, leftCoverDetection.forward, 2f, LayerMask.GetMask("WallTakeCover"));

            // Gambar titik hit
            if (didRightCoverDetectorHit)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rightCoverDetection.position + rightCoverDetection.forward * 2f, 0.1f);
            }
            if (didLeftCoverDetectorHit)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(leftCoverDetection.position + leftCoverDetection.forward * 2f, 0.1f);
            }
        }
    }

    private void InCoverMovementRestrictor()
    {
        bool didRightCoverDetectorHit = Physics.Raycast(rightCoverDetection.position, rightCoverDetection.forward, 2f, LayerMask.GetMask("WallTakeCover"));
        bool didLeftCoverDetectorHit = Physics.Raycast(leftCoverDetection.position, leftCoverDetection.forward, 2f, LayerMask.GetMask("WallTakeCover"));

        if (!didLeftCoverDetectorHit || !didRightCoverDetectorHit)
        {
            if (!didLeftCoverDetectorHit)
            {
                SetCharacterMoverCoverDirection(coverSurfaceDirection, -coverSurfaceDirection);
            }
            else
            {
                SetCharacterMoverCoverDirection(coverSurfaceDirection, coverSurfaceDirection);
            }
        }
        else
        {
            SetCharacterMoverCoverDirection(coverSurfaceDirection, Vector3.up);
        }
    }

    private void SetCharacterMoverCoverDirection(Vector3 moveDirection, Vector3 directionToProhibit)
    {
        inCoverMoveDirection = moveDirection;
        inCoverProhibitedDirection = directionToProhibit;
    }

    private Vector3 GetCoverSurfaceDirection(Vector3 hitNormal)
    {
        return Vector3.Cross(hitNormal, Vector3.up).normalized;
    }


    private void NightVision_performed(InputAction.CallbackContext context)
    {
        Volume nightVision = Camera.main.GetComponent<Volume>();
        if (nightVision.enabled)
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
        if (isRun) isRun = false;
        IsCrouching = true;
    }

    private void Run_canceled(InputAction.CallbackContext context)
    {
        isRun = false;
    }

    private void Run_performed(InputAction.CallbackContext context)
    {
        if (IsCrouching) IsCrouching = false;
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
        if (gm.scope)
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
            inGameUIHandler.SwitchingUICharacter();
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
            Shoot(this.gameObject, Camera.main.transform.position, Camera.main.transform.forward, character, activeWeapon, enemyMask, curRecoil + curRecoilMod);
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
        if (!gm.scope)
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
        tempArrowList = arrowList;
        foreach(ArrowData arrow in tempArrowList)
        {
            damageCanvasHandler.DamageArrow(arrow);
            if(arrow.stayTime > 0)
            {
                arrow.stayTime -= Time.deltaTime;
            }
            else
            {
                
                if(arrow.fadeTime > 0)
                {
                    arrow.fadeTime -= Time.deltaTime;

                    Color color = arrow.arrow.GetComponentInChildren<Image>().color;
                    color.a = arrow.fadeTime / damageCanvasHandler.fadeTime;
                    arrow.arrow.GetComponentInChildren<Image>().color = color;

                }
                else
                {                    
                    Destroy(arrow.arrow); 
                    arrowList.Remove(arrow);
                }
            }
        }


        // Input yang ini itu sementara aja
        Debug.DrawRay(transform.position, playerGameObject.forward.normalized * 2f, Color.red);

        if (isCover && autoMoverActive)
        {
            MoveToCover();
        }

        if (isCover)
        {
            SetCoverType();
            InCoverMovementRestrictor();

            coverRaycastTransformParent.SetParent(playerComponentParent);
        }
        else
        {
            coverRaycastTransformParent.SetParent(playerObjParent);
            coverRaycastTransformParent.localPosition = Vector3.zero;
            coverRaycastTransformParent.localRotation = Quaternion.identity;
            coverRaycastTransformParent.localScale = Vector3.one;
        }

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
            else if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hithit, 100f, LayerMask.GetMask("WallTakeCover")))
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
            if (activeWeapon != null)
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
                Shoot(this.gameObject, Camera.main.transform.position, Camera.main.transform.forward, character, activeWeapon, enemyMask, curRecoil + curRecoilMod);
                StartCoroutine(FireRate(FireRateFlag, activeWeapon.fireRate));
                if (activeWeapon.currBullet == 0)
                {
                    isReloading = true;
                    Reload(activeWeapon);
                    StartCoroutine(ReloadTime(ReloadFlag, activeWeapon.reloadTime));
                }
            }
        }
        if (isCover)
        {
            HandleCoverMovement();
        }
        else
        {
            Movement();
        }
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
        else if (isShooting || gm.scope)
        {
            testAnimation?.animator.SetBool("Move", false);
            Rotation(flatForward);
        }
    }

    void HandleCoverMovement()
    {
        InCoverMovementRestrictor();

        Vector2 move = new Vector2(inputActions.InputPlayerAction.Movement.ReadValue<Vector2>().x, inputActions.InputPlayerAction.Movement.ReadValue<Vector2>().y);
        Vector3 movement = new Vector3(move.x, 0, move.y).normalized;

        Vector3 flatForward = new Vector3(followTarget.forward.x, 0, followTarget.forward.z).normalized;
        Vector3 direction = flatForward * movement.z + followTarget.right * movement.x;

        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraForward = Vector3.Cross(Vector3.up, -cameraRight);

        Vector3 inputDirection = (move.x * coverDirection) + (move.y * cameraForward);
        inputDirection = Vector3.Project(inputDirection, inCoverMoveDirection);

        if (Vector3.Dot(inputDirection, inCoverProhibitedDirection) > 0)
        {
            inputDirection = Vector3.zero; // Tidak boleh bergerak ke arah yang dilarang
        }

        transform.position += inputDirection.normalized * coverMoveSpeed * Time.fixedDeltaTime;

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

        testAnimation?.WalkAnimation(move);
        if (!isShooting && !gm.scope)
        {

            Rotation(direction);
        }
        else if (isShooting || gm.scope)
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
                if (recoilAddMultiplier != 0)
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
        else if (recoilCooldown <= 0)
        {
            curRecoil = 0;
        }
    }

    public void InstantiateArrowDamage(GameObject whoShoot)
    {
        GameObject arrowGameObject = null;
        ArrowData currArrowData = null;

        foreach(ArrowData arrowData in arrowList)
        {
            if(arrowData.whoShootMe == whoShoot)
            {
                arrowGameObject = arrowData.arrow;
                currArrowData = arrowData;
                break;
            }
        }

        if (arrowGameObject == null)
        {
            arrowGameObject = Instantiate(damageCanvasHandler.ArrowIndicator, damageCanvasHandler.damageCanvas.transform);
            currArrowData = new ArrowData();
            currArrowData.arrow = arrowGameObject;
            currArrowData.whoShootMe = whoShoot;
            arrowList.Add(currArrowData);
        }
        if(currArrowData != null)
        {
            Color color = currArrowData.arrow.GetComponentInChildren<Image>().color;
            color.a = 1f;
            currArrowData.arrow.GetComponentInChildren<Image>().color = color;
            damageCanvasHandler.DamageDirectionArrowIndicator(currArrowData);
        }
    }

    private void Rotation(Vector3 direction)
    {
        if (direction != Vector3.zero)
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
        playerHP = character.health + character.armor;
        maxPlayerHP = playerHP;
        curWeapon = 0;
        activeWeapon = weaponStat[curWeapon];
        inputActions.InputPlayerAction.Enable();
        damageCanvasHandler.maxHP = maxPlayerHP;
        damageCanvasHandler.currHP = playerHP;
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
        return playerHP;
    }

    public void SetPlayerHP(float HP)
    {
        playerHP = HP;
        damageCanvasHandler.currHP = playerHP;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourLungFPS : ExecuteLogic
{
    public PlayerActionInput inputActions;
    public EntityStatSO FourLungStat;
    public WeaponStatSO weaponStat;
    public LayerMask enemyMask;

    bool isShooting = false;
    bool isReloading = false;
    bool fireRateOn = false;

    public float sensX;
    public float sensY;

    float rotationX;
    float rotationY;

    public float recoilCooldown;
    public float maxRecoil;
    public float currRecoil;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weaponStat = FourLungStat.weaponStat[0];
        inputActions = new PlayerActionInput();
        inputActions.InputPlayerAction.Enable();

        inputActions.InputPlayerAction.Shooting.performed += Shooting_performed;
    }

    private void Shooting_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isShooting = true;
        if (!weaponStat.allowHoldDownButton && isShooting && weaponStat.currBullet > 0 && !isReloading && !fireRateOn)
        {
            RecoilHandler();
            Shooting();
            StartCoroutine(FireRate(FireRateFlag, weaponStat.fireRate));
            isShooting = false;
            if (weaponStat.currBullet == 0 && weaponStat.totalBullet > 0)
            {
                isReloading = true;
                Reload(weaponStat);
                StartCoroutine(ReloadTime(ReloadFlag, weaponStat.reloadTime));
            }
        }
    }

    private void OnDisable()
    {
        inputActions.InputPlayerAction.Disable();
    }

    private void Update()
    {
        ComplexRecoil(ref currRecoil);

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    public void Shooting()
    {
        Shoot(this.gameObject, transform.position, transform.forward, FourLungStat, weaponStat, enemyMask, currRecoil);
    }

    private void RecoilHandler()
    {
        recoilCooldown = weaponStat.fireRate + (weaponStat.fireRate * .1f);
        maxRecoil = weaponStat.recoil;
    }

    private void ComplexRecoil(ref float curRecoil)
    {
        if (recoilCooldown > 0)
        {
            recoilCooldown -= Time.deltaTime;
            if (curRecoil <= maxRecoil)
            {
                curRecoil += Time.deltaTime * weaponStat.recoil;
            }
        }
        else
        {
            curRecoil = 0;
        }
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

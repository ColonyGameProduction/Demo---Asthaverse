using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  

//kelas untuk player action seperti attacking, scope, dan Silent kill

public class PlayerAction : ExecuteLogic
{
    private PlayerActionInput inputActions;

    //supaya input action bisa digunakan
    private void Awake()
    {
        inputActions = new PlayerActionInput();
        inputActions.InputPlayerAction.Shooting.Enable();
        inputActions.InputPlayerAction.SilentKill.Enable();
        inputActions.InputPlayerAction.ChangingWeapon.Enable();
        inputActions.InputPlayerAction.ChangePlayer.Enable();
        inputActions.InputPlayerAction.Scope.Enable();
    }

    private void Start()
    {
        //membuat event untuk menjalankan aksi yang dipakai oleh player
        inputActions.InputPlayerAction.Shooting.performed += Shooting_Performed;
        inputActions.InputPlayerAction.SilentKill.performed += SilentKill_performed;
        inputActions.InputPlayerAction.ChangingWeapon.performed += ChangingWeapon_performed;
        inputActions.InputPlayerAction.ChangePlayer.performed += ChangePlayer_performed;
        inputActions.InputPlayerAction.Scope.performed += Scope_performed;
    }

    private void Scope_performed(InputAction.CallbackContext context)
    {
        Scope();
    }

    private void ChangePlayer_performed(InputAction.CallbackContext context)
    {
        GameManager gm = GameManager.instance;

        if(gm.canSwitch)
        {
            SwitchCharacter();
        }
    }

    private void ChangingWeapon_performed(InputAction.CallbackContext context)
    {
        ChangingWeapon();
    }

    //event ketika 'SilentKill' dilakukan
    private void SilentKill_performed(InputAction.CallbackContext context)
    {
        SilentKill();
    }


    //event ketika 'Shoot' dilakukan
    private void Shooting_Performed(InputAction.CallbackContext context)
    {
        Shoot();        
    }    

    public PlayerActionInput GetPlayerActionInput()
    {
        return inputActions;
    }
    
}

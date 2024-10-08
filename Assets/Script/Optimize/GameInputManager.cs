using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance {get; private set;}
    GameManager _gm;
    private PlayerActionInput _inputActions;

    public Action OnRunPerformed, OnRunCanceled, OnCrouchPerformed, OnCrouchCanceled,
                  OnChangePlayerPerformed, OnChangeWeaponPerformed, 
                  OnCommandPerformed, OnUnCommandPerformed, OnHoldPosPerformed, OnUnHoldPosPerformed,
                  OnSilentKillPerformed, OnShootingPerformed, OnShootingCanceled, OnScopePerformed, OnReloadPerformed;
    private void Awake() 
    {
        Instance = this;
        _inputActions = new PlayerActionInput();   
    }
    private void Start() 
    {
        _gm = GameManager.instance;

        //membuat event untuk menjalankan aksi yang dipakai oleh player
        _inputActions.InputPlayerAction.Run.performed += Run_performed;
        _inputActions.InputPlayerAction.Run.canceled += Run_canceled;

        _inputActions.InputPlayerAction.Crouch.performed += Crouch_performed;
        _inputActions.InputPlayerAction.Crouch.canceled += Crouch_canceled;

        _inputActions.InputPlayerAction.ChangePlayer.performed += ChangePlayer_performed;
        _inputActions.InputPlayerAction.ChangingWeapon.performed += ChangingWeapon_performed;

        _inputActions.InputPlayerAction.Command.performed += Command_performed;
        _inputActions.InputPlayerAction.UnCommand.performed += UnCommand_performed;
        _inputActions.InputPlayerAction.HoldPosition.performed += HoldPosition_performed;
        _inputActions.InputPlayerAction.UnHoldPosition.performed += UnHoldPosition_performed;

        _inputActions.InputPlayerAction.SilentKill.performed += SilentKill_performed;

        _inputActions.InputPlayerAction.Shooting.performed += Shooting_Performed;
        _inputActions.InputPlayerAction.Shooting.canceled += Shooting_canceled;
        _inputActions.InputPlayerAction.Scope.performed += Scope_performed;
        _inputActions.InputPlayerAction.Reload.performed += Reload_performed;

    }

    private void Run_performed(InputAction.CallbackContext context) => OnRunPerformed?.Invoke();
    private void Run_canceled(InputAction.CallbackContext context) => OnRunCanceled?.Invoke();
    private void Crouch_performed(InputAction.CallbackContext context) => OnCrouchPerformed?.Invoke();
    private void Crouch_canceled(InputAction.CallbackContext context) => OnCrouchCanceled?.Invoke();

    private void ChangePlayer_performed(InputAction.CallbackContext context)=> OnChangePlayerPerformed?.Invoke();
    private void ChangingWeapon_performed(InputAction.CallbackContext context)=> OnChangeWeaponPerformed?.Invoke();

    private void Command_performed(InputAction.CallbackContext context)=> OnCommandPerformed?.Invoke();
    private void UnCommand_performed(InputAction.CallbackContext context)=> OnUnCommandPerformed?.Invoke();
    private void HoldPosition_performed(InputAction.CallbackContext context)=> OnHoldPosPerformed?.Invoke();
    private void UnHoldPosition_performed(InputAction.CallbackContext context)=> OnUnHoldPosPerformed?.Invoke();

    private void SilentKill_performed(InputAction.CallbackContext context)=> OnSilentKillPerformed?.Invoke();

    private void Shooting_Performed(InputAction.CallbackContext context)=> OnShootingPerformed?.Invoke();
    private void Shooting_canceled(InputAction.CallbackContext context)=> OnShootingCanceled?.Invoke();    
    private void Scope_performed(InputAction.CallbackContext context)=> OnScopePerformed?.Invoke();
    private void Reload_performed(InputAction.CallbackContext context)=> OnReloadPerformed?.Invoke();
    public Vector2 Movement()
    {
        Vector2 direction = Vector2.zero;
        direction = _inputActions.InputPlayerAction.Movement.ReadValue<Vector2>();
        return direction;
    }









}

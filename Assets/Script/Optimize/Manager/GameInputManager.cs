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
                  OnExitCommandPerformed, OnRegroupFriendPerformed,
                  OnSilentKillPerformed, OnShootingPerformed, OnShootingCanceled, OnScopePerformed, OnReloadPerformed,
                  OnInteractPerformed, OnNightVisionPerformed, OnSkillPerformed, OnWhistlePerformed, OnThrowPerformed,
                  OnTakeCoverPerformed, OnExitTakeCoverPerformed;
    public Action<int> OnCommandFriendPerformed;
    private void Awake() 
    {
        Instance = this;
        _inputActions = new PlayerActionInput();  
        //membuat event untuk menjalankan aksi yang dipakai oleh player
        _inputActions.InputPlayerAction.Run.performed += Run_performed;
        _inputActions.InputPlayerAction.Run.canceled += Run_canceled;

        _inputActions.InputPlayerAction.Crouch.performed += Crouch_performed;
        _inputActions.InputPlayerAction.Crouch.canceled += Crouch_canceled;

        _inputActions.InputPlayerAction.ChangePlayer.performed += ChangePlayer_performed;
        _inputActions.InputPlayerAction.ChangingWeapon.performed += ChangingWeapon_performed;

        _inputActions.InputPlayerAction.CommandFriend1.performed += Command1_performed;
        _inputActions.InputPlayerAction.CommandFriend2.performed += Command2_performed;
        _inputActions.InputPlayerAction.ExitCommand.performed += ExitCommand_performed;
        _inputActions.InputPlayerAction.RegroupFriend.performed += RegroupFriend_performed;

        _inputActions.InputPlayerAction.SilentKill.performed += SilentKill_performed;

        _inputActions.InputPlayerAction.Shooting.performed += Shooting_Performed;
        _inputActions.InputPlayerAction.Shooting.canceled += Shooting_canceled;
        _inputActions.InputPlayerAction.Scope.performed += Scope_performed;
        _inputActions.InputPlayerAction.Reload.performed += Reload_performed;

        _inputActions.InputPlayerAction.Interact.performed += Interact_performed;
        _inputActions.InputPlayerAction.NightVision.performed += NightVision_performed;
        _inputActions.InputPlayerAction.SkillButton.performed += SkillButton_performed;
        _inputActions.InputPlayerAction.Whistle.performed += Whistle_performed;
        _inputActions.InputPlayerAction.Throw.performed += Throw_performed;

        _inputActions.InputPlayerAction.TakeCover.performed += TakeCover_performed;
        _inputActions.InputPlayerAction.ExitTakeCover.performed += ExitTakeCover_performed;
        
        _inputActions.Enable();
    }





    private void Start() 
    {


        _gm = GameManager.instance;
    }

    private void Run_performed(InputAction.CallbackContext context) => OnRunPerformed?.Invoke();
    private void Run_canceled(InputAction.CallbackContext context) => OnRunCanceled?.Invoke();
    private void Crouch_performed(InputAction.CallbackContext context) => OnCrouchPerformed?.Invoke();
    private void Crouch_canceled(InputAction.CallbackContext context) => OnCrouchCanceled?.Invoke();

    private void ChangePlayer_performed(InputAction.CallbackContext context) => OnChangePlayerPerformed?.Invoke();
    private void ChangingWeapon_performed(InputAction.CallbackContext context)=> OnChangeWeaponPerformed?.Invoke();

    private void Command1_performed(InputAction.CallbackContext context)=> OnCommandFriendPerformed?.Invoke(1);
    private void Command2_performed(InputAction.CallbackContext context)=> OnCommandFriendPerformed?.Invoke(2);
    private void ExitCommand_performed(InputAction.CallbackContext context)=> OnExitCommandPerformed?.Invoke();
    private void RegroupFriend_performed(InputAction.CallbackContext context)=> OnRegroupFriendPerformed?.Invoke();

    private void SilentKill_performed(InputAction.CallbackContext context)=> OnSilentKillPerformed?.Invoke();

    private void Shooting_Performed(InputAction.CallbackContext context)=> OnShootingPerformed?.Invoke();
    private void Shooting_canceled(InputAction.CallbackContext context)=> OnShootingCanceled?.Invoke();    
    private void Scope_performed(InputAction.CallbackContext context)=> OnScopePerformed?.Invoke();
    private void Reload_performed(InputAction.CallbackContext context)=> OnReloadPerformed?.Invoke();
    private void Interact_performed(InputAction.CallbackContext context) => OnInteractPerformed?.Invoke();
    private void NightVision_performed(InputAction.CallbackContext context) => OnNightVisionPerformed?.Invoke();
    private void SkillButton_performed(InputAction.CallbackContext context) => OnSkillPerformed?.Invoke();

    private void Whistle_performed(InputAction.CallbackContext context) => OnWhistlePerformed?.Invoke();
    private void Throw_performed(InputAction.CallbackContext context) => OnThrowPerformed?.Invoke();
    private void TakeCover_performed(InputAction.CallbackContext context) => OnTakeCoverPerformed?.Invoke();
    private void ExitTakeCover_performed(InputAction.CallbackContext context) => OnExitTakeCoverPerformed?.Invoke();

    public Vector2 Movement()
    {
        Vector2 direction = Vector2.zero;
        direction = _inputActions.InputPlayerAction.Movement.ReadValue<Vector2>();
        return direction;
    }









}

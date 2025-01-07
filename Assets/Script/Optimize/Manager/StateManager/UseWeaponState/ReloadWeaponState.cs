using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeaponState : UseWeaponState
{
    bool isDoReloading;

    public ReloadWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {
        _activeStateAnimParamName = "Reload";
    }
    public override void EnterState()
    {
        // mainkan animasi
        // _stateMachine.CharaAnimator.
        isDoReloading = false;
        _sm.IsResetAnimTime = true;
        if(!_sm.IsAIInput)
        {
            _playableData.TellToTurnOffScope();
        }

        // Debug.Log("Reload Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        // _stateMachine.CharaAnimator.Play("Crouch", 1, _currAnimTIme);
        if(!_normalUse.IsAiming)SetAnimParamInactive("Aim");


        if(!isDoReloading && _normalUse.IsReloading)
        {
            isDoReloading = true;
            _sm.CharaIdentity.OnToggleLeftHandRig?.Invoke(false, false);
            base.EnterState();
        }
        else if(isDoReloading && !_normalUse.IsReloading)
        {
            // base.ExitState();
            if(_advancedUse != null && _advancedUse.IsSilentKill)
            {
                _sm.SwitchState(_factory.SilentKillState());
            }
            else if(_advancedUse != null && _advancedUse.IsSwitchingWeapon)
            {
                _sm.SwitchState(_factory.SwitchingWeaponState());
            }
            else if(_normalUse.IsAiming)
            {
                if(_normalUse.IsUsingWeapon)
                {
                    _sm.SwitchState(_factory.UsingWeaponState());
                }
                else
                {
                    _sm.SwitchState(_factory.AimWeaponState());
                }
            }
            else
            {
                _sm.SwitchState(_factory.IdleWeaponState());
            }

        }
        // if(isDoReloading)
        // {
            
            
        // }
    }
    public override void ExitState()
    {
        _sm.CharaIdentity.OnToggleLeftHandRig?.Invoke(true, false);
        _sm.EnsureReloadWeapon();

        _sm.CanReloadWeapon_Coroutine();
    }

}

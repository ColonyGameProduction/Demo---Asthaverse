using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeaponState : UseWeaponState
{
    bool isDoReloading;

    public ReloadWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {
        StateAnimationName = "Reload";
    }
    public override void EnterState()
    {
        // mainkan animasi
        // _stateMachine.CharaAnimator.
        isDoReloading = false;
        _stateMachine.CurrAnimTime = 0;
        if(_stateMachine.IsInputPlayer)
        {
            _playableData.TellToTurnOffScope();
        }

        // Debug.Log("Reload Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        // _stateMachine.CharaAnimator.Play("Crouch", 1, _currAnimTIme);
        if(!_normalUse.IsAiming)StateAnimationOff("Aim");


        


        if(!isDoReloading && _normalUse.IsReloading)
        {
            isDoReloading = true;
            base.EnterState();
        }
        else if(isDoReloading && !_normalUse.IsReloading)
        {
            // base.ExitState();
            if(_advancedUse != null && _advancedUse.IsSilentKill)
            {
                _stateMachine.SwitchState(_factory.SilentKillState());
            }
            else if(_advancedUse != null && _advancedUse.IsSwitchingWeapon)
            {
                _stateMachine.SwitchState(_factory.SwitchingWeaponState());
            }
            else if(_normalUse.IsAiming)
            {
                if(_normalUse.IsUsingWeapon)
                {
                    _stateMachine.SwitchState(_factory.UsingWeaponState());
                }
                else
                {
                    _stateMachine.SwitchState(_factory.AimWeaponState());
                }
            }
            else
            {
                _stateMachine.SwitchState(_factory.IdleWeaponState());
            }

        }
        // if(isDoReloading)
        // {
            
            
        // }
    }
    public override void ExitState()
    {
        _stateMachine.CanReloadWeapon_Coroutine();
    }

}

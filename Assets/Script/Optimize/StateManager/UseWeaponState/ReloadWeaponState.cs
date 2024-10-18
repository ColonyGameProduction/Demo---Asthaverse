using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeaponState : UseWeaponState
{
    bool isDoReloading;
    public ReloadWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {

    }
    public override void EnterState()
    {
        // base.EnterState(); mainkan animasi

        isDoReloading = false;
        
        if(_stateMachine.IsInputPlayer)
        {
            _playableData.TellToTurnOffScope();
        }

        // Debug.Log("Reload Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(!isDoReloading && _normalUse.IsReloading)
        {
            isDoReloading = true;
            _stateMachine.ReloadWeapon();
        }
        else if(isDoReloading && !_normalUse.IsReloading)
        {
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
    }
    public override void ExitState()
    {
        _stateMachine.CanReloadWeapon_Coroutine();
    }

}

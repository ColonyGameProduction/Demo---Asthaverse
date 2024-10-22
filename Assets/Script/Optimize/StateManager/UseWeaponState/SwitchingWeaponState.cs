using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingWeaponState : UseWeaponState
{
    bool isDoSwitch;
    public SwitchingWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {

    }
    public override void EnterState()
    {
        isDoSwitch = false;
        
        if(_stateMachine.IsInputPlayer)
        {
            _playableData.TellToTurnOffScope();
        }

        Debug.Log("Swithc" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(!isDoSwitch && _advancedUse.IsSwitchingWeapon)
        {
            isDoSwitch = true;
            _advancedUse.SwitchWeapon();
        }
        else if(isDoSwitch && !_advancedUse.IsSwitchingWeapon)
        {
            if(_stateMachine.CurrWeapon.currBullet == 0 && !_normalUse.IsReloading)
            {
                _normalUse.IsReloading = true;
            }
            else if (_stateMachine.CurrWeapon.currBullet > 0 && _normalUse.IsReloading)
            {
                _normalUse.IsReloading = false;
            }

            if(_advancedUse.IsSilentKill)
            {
                _stateMachine.SwitchState(_factory.SwitchingWeaponState());
            }
            else if(_normalUse.IsReloading)
            {
                _stateMachine.SwitchState(_factory.ReloadWeaponState());
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
        _advancedUse.CanSwitchWeapon_Coroutine();
    }
}

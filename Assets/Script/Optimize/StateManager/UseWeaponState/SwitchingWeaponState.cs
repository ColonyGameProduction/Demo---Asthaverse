using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingWeaponState : UseWeaponState
{
    bool isDoSwitch;
    public SwitchingWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {

    }
    public override void EnterState()
    {
        isDoSwitch = false;
        
        if(!_sm.IsAIInput)
        {
            _playableData.TellToTurnOffScope();
        }

        // Debug.Log("Swithc" + _stateMachine.gameObject.name);
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
            if(_sm.CurrWeapon.currBullet == 0 && !_normalUse.IsReloading)
            {
                _normalUse.IsReloading = true;
            }
            else if (_sm.CurrWeapon.currBullet > 0 && _normalUse.IsReloading)
            {
                _normalUse.IsReloading = false;
            }

            if(_advancedUse.IsSilentKill)
            {
                _sm.SwitchState(_factory.SwitchingWeaponState());
            }
            else if(_normalUse.IsReloading)
            {
                _sm.SwitchState(_factory.ReloadWeaponState());
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
    }
    public override void ExitState()
    {
        _advancedUse.CanSwitchWeapon_Coroutine();
    }
}

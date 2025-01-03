using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilentKillState : UseWeaponState
{
    bool isDoSilentKill;
    public SilentKillState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {

    }
    public override void EnterState()
    {
        // base.EnterState(); mainkan animasi

        isDoSilentKill = false;
        
        //hrsnya gaperlu krn gabisa silentkill pas scope
        // if(_stateMachine.IsInputPlayer)
        // {
        //     _playableData.TellToTurnOffScope();
        // }

        // Debug.Log("SilentKill" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(!isDoSilentKill && _advancedUse.IsSilentKill)
        {
            isDoSilentKill = true;
            _advancedUse.SilentKill();
        }
        else if(isDoSilentKill && !_advancedUse.IsSilentKill)
        {
            if(_sm.CurrWeapon.currBullet == 0 && !_normalUse.IsReloading)
            {
                _normalUse.IsReloading = true;
            }
            if(_advancedUse.IsSwitchingWeapon)
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
        _advancedUse.CanSilentKill_Coroutine();
    }
}

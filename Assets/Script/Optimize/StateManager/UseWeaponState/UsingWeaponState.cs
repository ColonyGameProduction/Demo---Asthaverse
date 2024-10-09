using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingWeaponState : UseWeaponState
{
    public UsingWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {

    }
    public override void EnterState()
    {
        // base.EnterState(); // Mainkan animasi
        Debug.Log("Use Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(_normalUse.IsUsingWeapon)
        {
            if(!_stateMachine.IsInputPlayer)
            {
                if(_stateMachine.ChosenTarget != null)Debug.Log("Shoot" + _stateMachine.ChosenTarget);
                else
                {
                    //we're gonna talk about this later
                    _normalUse.IsAiming = false;
                    _normalUse.IsUsingWeapon = false;
                    _stateMachine.SwitchState(_factory.IdleWeaponState());
                }
            }
            else
            {
                Debug.Log("SHOOOOTT");
            }
        }
        if(_normalUse.IsReloading)
        {
            _stateMachine.SwitchState(_factory.ReloadWeaponState());
        }
        else if(_advancedUse != null)
        {
            if(_advancedUse.IsSwitchingWeapon)
            {
                _stateMachine.SwitchState(_factory.SwitchingWeaponState());
            }
            else if(_advancedUse.IsSilentKill)
            {
                _stateMachine.SwitchState(_factory.SilentKillState());
            }
        }
        else if(!_normalUse.IsUsingWeapon)
        {
            if(_normalUse.IsAiming)
            {
                _stateMachine.SwitchState(_factory.AimWeaponState());
            }
            else
            {
                _stateMachine.SwitchState(_factory.IdleWeaponState());
            }
        }




    }
    public override void ExiState()
    {
        // base.ExiState(); // Matikan Animasi
    }
}

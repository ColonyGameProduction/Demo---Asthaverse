using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class AimWeaponState : UseWeaponState
{
    public AimWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {
        StateAnimationName = "Aim";
    }
    public override void EnterState()
    {
        base.EnterState(); //Do animation
        // Debug.Log("Aim Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(_normalUse.IsAiming && _normalUse.IsUsingWeapon)
        {
            if(!_stateMachine.IsInputPlayer)
            {
                if(_stateMachine.ChosenTarget != null)_stateMachine.SwitchState(_factory.UsingWeaponState());
                else
                {
                    _normalUse.IsAiming = false;
                    _normalUse.IsUsingWeapon = false;
                    _stateMachine.SwitchState(_factory.IdleWeaponState());
                }
            }
            else
            {
                _stateMachine.SwitchState(_factory.UsingWeaponState());
            }
        }
        else if(_normalUse.IsReloading)
        {
            _stateMachine.SwitchState(_factory.ReloadWeaponState());
        }
        else if(_advancedUse != null && _advancedUse.IsSilentKill)
        {
            _stateMachine.SwitchState(_factory.SilentKillState());
        }
        else if(_advancedUse != null && _advancedUse.IsSwitchingWeapon)
        {
            _stateMachine.SwitchState(_factory.SwitchingWeaponState());
        }
        else if(!_normalUse.IsAiming)
        {
            _stateMachine.SwitchState(_factory.IdleWeaponState());
        }
        // else if()
    }
    public override void ExitState()
    {
        if(!_normalUse.IsAiming)
        {
            base.ExitState();
        }
        // base.ExiState(); //Turn Off Aiming ANimation if isAiming is false
    }
}

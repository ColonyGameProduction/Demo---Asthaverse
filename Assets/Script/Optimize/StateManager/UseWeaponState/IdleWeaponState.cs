using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWeaponState : UseWeaponState
{
    public IdleWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {   

    }
    public override void EnterState()
    {
        Debug.Log("Idle Weapon" + _stateMachine.gameObject.name);
        _stateMachine.IsIdle = true;
    }
    public override void UpdateState()
    {
        if(PlayableCharacterManager.IsSwitchingCharacter)return;

        if(_normalUse.IsAiming)
        {
            _stateMachine.SwitchState(_factory.AimWeaponState());
        }
        else if(_normalUse.IsReloading)
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
    }
    public override void ExiState()
    {
        _stateMachine.IsIdle = false;
    }
}

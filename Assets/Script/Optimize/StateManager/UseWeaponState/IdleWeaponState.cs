using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWeaponState : UseWeaponState
{
    public IdleWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {   

    }
    public override void EnterState()
    {
        
        if(!_sm.IsIdle)
        {
            // Debug.Log("what");
            _sm.OnWasUsinghGun?.Invoke();
        }

        _sm.IsIdle = true;
    }
    public override void UpdateState()
    {
        if((_advancedUse != null && (PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter)) || _sm.IsCharacterDead)return;

        if(_normalUse.IsAiming)
        {
            _sm.SwitchState(_factory.AimWeaponState());
        }
        else if(_advancedUse != null && _advancedUse.IsSilentKill)
        {
            _sm.SwitchState(_factory.SilentKillState());
        }
        else if(_advancedUse != null && _advancedUse.IsSwitchingWeapon)
        {
            _sm.SwitchState(_factory.SwitchingWeaponState());
        }
        else if(_normalUse.IsReloading)
        {
            _sm.SwitchState(_factory.ReloadWeaponState());
        }
    }
    public override void ExitState()
    {
        _sm.IsIdle = false;
    }
}

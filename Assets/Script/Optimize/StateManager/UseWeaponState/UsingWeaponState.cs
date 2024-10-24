using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingWeaponState : UseWeaponState
{
    public UsingWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {
        _activeStateAnimParamName = "Fire";
    }
    public override void EnterState()
    {
        base.EnterState(); 
        _sm.ActivateRigAim();
        if(_normalUse.IsAiming && !_sm.CharaAnimator.GetBool("Aim")) SetAnimParamActive("Aim");
        // Mainkan animasi
        // Debug.Log("Use Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        
        if(_advancedUse != null && _advancedUse.IsSilentKill)
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
        else if(!_normalUse.IsUsingWeapon)
        {
            if(_normalUse.IsAiming)
            {
                _sm.SwitchState(_factory.AimWeaponState());
            }
            else
            {
                _sm.SwitchState(_factory.IdleWeaponState());
            }
        }




    }
    public override void ExitState()
    {
        base.ExitState(); // Matikan Animasi shooting
        if((_advancedUse != null && _advancedUse.IsSilentKill) || (_advancedUse != null && _advancedUse.IsSwitchingWeapon) || _normalUse.IsReloading || !_normalUse.IsAiming)
        {
            _sm.DeactivateRigAim();
        }


        //kalo aim jg off
        if(!_normalUse.IsAiming && _sm.CharaAnimator.GetBool("Aim")) SetAnimParamInactive("Aim");
    }
    public override void PhysicsLogicUpdateState()
    {
        if(_normalUse.IsUsingWeapon)
        {
            if(_sm.IsAIInput)
            {
                if(_sm.ChosenTarget != null && !_sm.ChosenTarget.GetComponent<CharacterIdentity>().IsDead)
                {
                    _sm.UseWeapon();
                }
                else 
                {
                    //we're gonna talk about this later
                    _normalUse.IsAiming = false;
                    _normalUse.IsUsingWeapon = false;
                    _sm.SwitchState(_factory.IdleWeaponState());
                }
            }
            else
            {
                _sm.UseWeapon();
            }
        }
    }
}

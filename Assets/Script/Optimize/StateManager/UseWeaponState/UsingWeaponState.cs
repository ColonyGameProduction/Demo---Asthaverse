using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingWeaponState : UseWeaponState
{
    public UsingWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {
        StateAnimationName = "Fire";
    }
    public override void EnterState()
    {
        base.EnterState(); 
        if(_normalUse.IsAiming && !_stateMachine.CharaAnimator.GetBool("Aim")) StateAnimationOn("Aim");
        // Mainkan animasi
        // Debug.Log("Use Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        
        if(_advancedUse != null && _advancedUse.IsSilentKill)
        {
            _stateMachine.SwitchState(_factory.SilentKillState());
        }
        else if(_advancedUse != null && _advancedUse.IsSwitchingWeapon)
        {
            _stateMachine.SwitchState(_factory.SwitchingWeaponState());
        }
        else if(_normalUse.IsReloading)
        {
            _stateMachine.SwitchState(_factory.ReloadWeaponState());
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
    public override void ExitState()
    {
        base.ExitState(); // Matikan Animasi shooting
        //kalo aim jg off
        if(!_normalUse.IsAiming && _stateMachine.CharaAnimator.GetBool("Aim")) StateAnimationOff("Aim");
    }
    public override void PhysicsLogicUpdateState()
    {
        if(_normalUse.IsUsingWeapon)
        {
            if(!_stateMachine.IsInputPlayer)
            {
                if(_stateMachine.ChosenTarget != null && !_stateMachine.ChosenTarget.GetComponent<CharacterIdentity>().IsDead)
                {
                    _stateMachine.UseWeapon();
                }
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
                _stateMachine.UseWeapon();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingWeaponState : UseWeaponState
{
    bool isDoSwitch;
    int oldIdx = 0;
    public SwitchingWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {
        _activeStateAnimParamName = "SwitchWeapon";
    }
    public override void EnterState()
    {
        isDoSwitch = false;
        _sm.IsResetAnimTime = true;
        if(!_sm.IsAIInput)
        {
            _playableData.TellToTurnOffScope();
        }
        oldIdx = _playableData.GetPlayableCharacterIdentity.CurrWeaponIdx;

        // Debug.Log("Swithc" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(!_normalUse.IsAiming)SetAnimParamInactive("Aim");

        if(!isDoSwitch && _advancedUse.IsSwitchingWeapon)
        {
            isDoSwitch = true;
            _sm.IsResetAnimTime = true;
            _sm.CharaIdentity.OnToggleLeftHandRig?.Invoke(false, false);

            _playableData.GetPlayableMakeSFX.PlaySwitchWeaponSFX();
            base.EnterState();
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
        _playableData.GetPlayableCharacterIdentity.EnsureSwitchWeapon(oldIdx);
        _sm.CharaIdentity.OnToggleLeftHandRig?.Invoke(true, false);

        _advancedUse.CanSwitchWeapon_Coroutine();
    }
}



public class AimWeaponState : UseWeaponState
{
    public AimWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {
        _activeStateAnimParamName = "Aim";
    }
    public override void EnterState()
    {
        base.EnterState(); 
        _sm.ActivateRigAim();
        //Do animation
        // Debug.Log("Aim Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(_sm.IsAIInput && _sm.CurrWeapon.currBullet == 0)
        {
            if(_sm.CurrWeapon.totalBullet > 0)
            {
                if(!_sm.CanReload)_sm.CanReload = true;
                _sm.IsReloading = true;
            }
            
        }
        if(_normalUse.IsAiming && _normalUse.IsUsingWeapon)
        {
            if(_sm.IsAIInput)
            {
                if(_sm.ChosenTarget != null)_sm.SwitchState(_factory.UsingWeaponState());
                else
                {
                    _normalUse.IsAiming = false;
                    _normalUse.IsUsingWeapon = false;
                    _sm.SwitchState(_factory.IdleWeaponState());
                }
            }
            else
            {
                _sm.SwitchState(_factory.UsingWeaponState());
            }
        }
        else if(_normalUse.IsReloading)
        {
            _sm.SwitchState(_factory.ReloadWeaponState());
        }
        else if(_advancedUse != null && _advancedUse.IsSilentKill)
        {
            _sm.SwitchState(_factory.SilentKillState());
        }
        else if(_advancedUse != null && _advancedUse.IsSwitchingWeapon)
        {
            _sm.SwitchState(_factory.SwitchingWeaponState());
        }
        else if(!_normalUse.IsAiming)
        {
            _sm.SwitchState(_factory.IdleWeaponState());
        }
        // else if()
    }
    public override void ExitState()
    {
        if(!(_normalUse.IsAiming && _normalUse.IsUsingWeapon))
        {
            _sm.DeactivateRigAim();
        }
        if(!_normalUse.IsAiming)
        {
            base.ExitState();
        }
        // base.ExiState(); //Turn Off Aiming ANimation if isAiming is false
    }
}

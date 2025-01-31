

public class SilentKillState : UseWeaponState
{
    bool isDoSilentKill;

    public SilentKillState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine, factory)
    {
        _activeStateAnimParamName = "SilentKill";
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
            _sm.CharaIdentity.OnToggleFollowHandRig?.Invoke(false, false);
            
            _playableData.GetPlayableMakeSFX.PlaySilentKillSFX(_sm.CharaIdentity.SilentKillIdx);
            base.EnterState();
            _playableData.SilentKilledEnemyAnimation();
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
        _sm.CharaIdentity.SilentKillIdx = _sm.CharaIdentity.SilentKillIdx == 1 ? 0 : 1;
        
        _sm.CharaIdentity.OnToggleFollowHandRig?.Invoke(true, false);
        _advancedUse.CanSilentKill_Coroutine();
    }
}

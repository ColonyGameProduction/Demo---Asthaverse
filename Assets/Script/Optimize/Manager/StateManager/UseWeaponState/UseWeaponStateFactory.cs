

public class UseWeaponStateFactory
{
    UseWeaponStateMachine _machine;
    UseWeaponState _idleWeapon, _aimWeapon, _usingWeapon, _reloadWeapon, _switchingWeapon, _silentKill;
    public UseWeaponStateFactory (UseWeaponStateMachine currStateMachine)
    {
        _machine = currStateMachine;
    }
    
    public UseWeaponState IdleWeaponState()
    {
        if(_idleWeapon == null)_idleWeapon = new IdleWeaponState(_machine, this);
        return _idleWeapon;
    }
    public UseWeaponState AimWeaponState()
    {
        if(_aimWeapon == null)_aimWeapon = new AimWeaponState(_machine, this);
        return _aimWeapon;
    }
    public UseWeaponState UsingWeaponState()
    {
        if(_usingWeapon == null)_usingWeapon = new UsingWeaponState(_machine, this);
        return _usingWeapon;
    }
    public UseWeaponState ReloadWeaponState()
    {
        if(_reloadWeapon == null)_reloadWeapon = new ReloadWeaponState(_machine, this);
        return _reloadWeapon;
    }
    public UseWeaponState SwitchingWeaponState()
    {
        if(_switchingWeapon == null)_switchingWeapon = new SwitchingWeaponState(_machine, this);
        return _switchingWeapon;
    }
    public UseWeaponState SilentKillState()
    {
        if(_silentKill == null)_silentKill = new SilentKillState(_machine, this);
        return _silentKill;
    }
}
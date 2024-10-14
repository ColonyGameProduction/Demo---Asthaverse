using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdvancedUseWeaponData
{
    public bool IsSwitchingWeapon { get; set;}
    public bool CanSwitchWeapon {get; set;}
    public float SwitchWeaponDuration {get;}
    public bool IsSilentKill { get; set;}
    public bool CanSilentKill {get; set;}
    public float SilentKillDuration {get;}
    void SwitchWeapon(); //Nanti nyambung ke charaidentities;
    void SilentKill(); // ntr nyambung ke... ?
    void CanSilentKill_Coroutine();
    void CanSwitchWeapon_Coroutine();
}

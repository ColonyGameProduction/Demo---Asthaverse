using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdvancedUseWeaponData
{
    public bool IsSwitchingWeapon { get; set;}
    public bool IsSilentKill { get; set;}
    public bool CanSilentKill {get; set;}
}

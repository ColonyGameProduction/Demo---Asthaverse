using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseWeapon
{
    public bool IsFireRateOn { get; set; }
    public Transform CurrOriginShootPoint { get;}
    public Transform CurrDirectionShootPoint { get;}
    void UseWeapon();
    void ReloadWeapon();
    void ForceStopUseWeapon();
}

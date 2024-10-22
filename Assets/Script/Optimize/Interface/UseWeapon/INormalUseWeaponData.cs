using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INormalUseWeaponData
{
    public bool IsIdle { get; set;}
    public bool IsAiming { get; set;}
    public bool IsUsingWeapon { get; set;}
    public bool IsReloading { get; set;}
    public bool CanReload { get; set;}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingWeaponState : UseWeaponState
{
    public SwitchingWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {

    }
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void UpdateState()
    {
        
    }
    public override void ExiState()
    {
        base.ExiState();
    }
}

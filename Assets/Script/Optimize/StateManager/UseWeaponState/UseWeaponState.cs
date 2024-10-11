using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseWeaponState : CharacterBaseState<UseWeaponStateMachine>
{
    protected UseWeaponStateFactory _factory;
    protected UseWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine)
    {
        _factory = factory;
    }
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExiState()
    {
        base.ExiState();
    }
    public virtual void PhysicsLogicUpdateState(){}
}

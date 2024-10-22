using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseWeaponState : CharacterBaseState<UseWeaponStateMachine>
{
    [Header("To Get the data from statemachine class and child class in a group")]
    protected UseWeaponStateFactory _factory;
    protected INormalUseWeaponData _normalUse;
    protected IAdvancedUseWeaponData _advancedUse;
    protected IPlayableUseWeaponDataNeeded _playableData;
    protected UseWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine)
    {
        _factory = factory;
        if(stateMachine is INormalUseWeaponData n)
        {
            _normalUse = n;
        }
        if(stateMachine is IAdvancedUseWeaponData a)
        {
            _advancedUse = a;
        }
        if(stateMachine is IPlayableUseWeaponDataNeeded p)
        {
            _playableData = p;
        }
    }
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
    }
    public virtual void PhysicsLogicUpdateState(){}
}

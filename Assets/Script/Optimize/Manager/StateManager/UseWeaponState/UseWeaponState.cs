

public abstract class UseWeaponState : CharacterBaseState<UseWeaponStateMachine>
{
    protected UseWeaponStateFactory _factory;
    protected INormalUseWeaponData _normalUse; //normal data -> reload, shoot etc
    protected IAdvancedUseWeaponData _advancedUse; //data kayak silentkill, switch weapon
    protected IPlayableUseWeaponDataNeeded _playableData; //data tambahan dr playable
    protected UseWeaponState(UseWeaponStateMachine currStateMachine, UseWeaponStateFactory factory) : base(currStateMachine)
    {
        _factory = factory;
        if(currStateMachine is INormalUseWeaponData n) _normalUse = n;

        if(currStateMachine is IAdvancedUseWeaponData a) _advancedUse = a;

        if(currStateMachine is IPlayableUseWeaponDataNeeded p) _playableData = p;

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

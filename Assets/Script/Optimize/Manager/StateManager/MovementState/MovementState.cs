

/// <summary>
/// Movement state -> semua state pergerakan: Idle, Walk, Run, Crouch
/// </summary>
public abstract class MovementState : CharacterBaseState<MovementStateMachine>
{
    protected MovementStateFactory _factory;
    protected IStandMovementData _standData; // Data berdiri -> isidle, iswalk dsb
    protected IGroundMovementData _groundData; // Data tanah -> crouch, crawl
    protected IPlayableMovementDataNeeded _playableData; // Data player tambahan

    protected const string ANIMATION_MOVE_PARAMETER_ISMOVING = "IsMoving";
    public MovementState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine) 
    {
        _factory = factory;
        if(currStateMachine is IStandMovementData s)_standData = s;

        if(currStateMachine is IGroundMovementData g)_groundData = g;

        if(currStateMachine is IPlayableMovementDataNeeded m)_playableData = m;

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

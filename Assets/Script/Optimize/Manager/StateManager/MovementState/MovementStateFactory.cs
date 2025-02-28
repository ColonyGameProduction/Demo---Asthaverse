

public class MovementStateFactory
{
    MovementStateMachine _machine;
    MovementState _idle, _walk, _run, _crouch, _crawl;
    public MovementStateFactory (MovementStateMachine machine)
    {
        _machine = machine;
    }
    
    public MovementState IdleState()
    {
        if(_idle == null)_idle = new IdleState(_machine, this);
        return _idle;
    }
    public MovementState WalkState()
    {
        if(_walk == null)_walk = new WalkState(_machine, this);
        return _walk;
    }
    public MovementState RunState()
    {
        if(_run == null)_run = new RunState(_machine, this);
        return _run;
    }
    public MovementState CrouchState()
    {
        if(_crouch == null)_crouch = new CrouchState(_machine, this);
        return _crouch;
    }
    public MovementState CrawlState()
    {
        if(_crawl == null)_crawl = new CrawlState(_machine, this);
        return _crawl;
    }
}

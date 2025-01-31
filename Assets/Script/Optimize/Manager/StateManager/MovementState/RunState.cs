

/// <summary>
/// State Run
/// </summary>
public class RunState : WalkState
{    
    public RunState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory) =>_activeStateAnimParamName = "Run";
    public override void EnterState()
    {
        _sm.OnStartRunning?.Invoke();
        SetAnimParamActive(_activeStateAnimParamName);

        _sm.ChangeCurrSpeed(_sm.RunSpeed);
    }

    public override void ExitState()
    {
        SetAnimParamInactive(_activeStateAnimParamName);
        // //Matikan state animasi Run -> mau state tombol masih true pun tetep matiin krn ga ada animasi idle d run
    }

    protected override void CheckStateWhileMoving()
    {
        if(!_standData.IsRunning)
        {
            if(_standData.IsCrouching) _sm.SwitchState(_factory.CrouchState());
            else _sm.SwitchState(_factory.WalkState());
        }
    }
}

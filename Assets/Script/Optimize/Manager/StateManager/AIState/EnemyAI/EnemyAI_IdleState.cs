

public class EnemyAI_IdleState : EnemyAIState
{
    public EnemyAI_IdleState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _sm.IsAIIdle = true;
        _sm.IsCheckingEnemyInHunt = false;
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
    }

    public override void UpdateState()
    {
        if(_sm.IsCharacterDead || _sm.EnemyIdentity.IsSilentKilled)
        {
            _sm.GetMoveStateMachine.IsIdleMustStayAlert = false;
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
            return;
        }

        _sm.GetFOVState.FOVStateHandler();
        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            _sm.GetMoveStateMachine.IsIdleMustStayAlert = true;
            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
            _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy.position, false);
            if(!_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = true;
            
            if(_sm.GetFOVState.CurrState == FOVDistState.middle)
            {
                _sm.AlertValue = _sm.MaxAlertValue / 2 + _sm.AlertValueCountMultiplier;
            }
            else if(_sm.GetFOVState.CurrState == FOVDistState.close)
            {
                _sm.AlertValue = _sm.MaxAlertValue + _sm.AlertValueCountMultiplier;
            }
        }

        if(_sm.AlertValue >= _sm.MaxAlertValue / 2 && _sm.AlertValue < _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_HuntedState());
            return;
        }
        else if(_sm.AlertValue >= _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            return;
        }

        if(_sm.GetFOVState.CurrState == FOVDistState.none) //no person
        {
            _sm.GetMoveStateMachine.IsIdleMustStayAlert = false;
            if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
            Patrol();
        }
    }
    public override void ExitState()
    {
        _sm.GetMoveStateMachine.IsIdleMustStayAlert = false;
        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
        // else 

        _sm.IsAIIdle = false;
    }

    public void Patrol()
    {
        if(_sm.GetMoveStateMachine.IsRunning) _sm.GetMoveStateMachine.IsRunning = false;
        if (_sm.PatrolPath.Length > 1)
        {
            _sm.GetMoveStateMachine.SetAIDirection(_sm.PatrolPath[_sm.CurrPath].position);
        }
        else
        {
            _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position);
        }
    }
    
}

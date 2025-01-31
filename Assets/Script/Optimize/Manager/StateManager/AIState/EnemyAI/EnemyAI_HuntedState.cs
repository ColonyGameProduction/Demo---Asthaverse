

public class EnemyAI_HuntedState : EnemyAIState
{
    float tempAlertValue = 0;
    public EnemyAI_HuntedState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        _sm.IsAIHunted = true;
        // if(_stateMachine.GetUseWeaponStateMachine.)_stateMachine.GetUseWeaponStateMachine.ForceStopUseWeapon();
        _sm.EnemyIdentity.Aiming(true);
        
    }

    public override void UpdateState()
    {
        _sm.GetFOVState.FOVStateHandler();
        if(_sm.GetFOVState.CurrState != FOVDistState.none)
        {
            _sm.AimAIPointLookAt(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
            if(_sm.GetFOVState.CurrState == FOVDistState.middle)
            {
                // _sm.MaxAlertValue *= 0.5f;
                tempAlertValue = _sm.MaxAlertValue / 2;
                if(_sm.AlertValue < tempAlertValue) _sm.AlertValue = tempAlertValue + _sm.AlertValueCountMultiplier;
            }
            else if(_sm.GetFOVState.CurrState == FOVDistState.close)
            {
                // _sm.MaxAlertValue = 0f;
                tempAlertValue = _sm.MaxAlertValue;
                if(_sm.AlertValue < tempAlertValue) _sm.AlertValue = tempAlertValue + _sm.AlertValueCountMultiplier;
            }
            // Debug.Log("HALOOO");
        }
        else
        {
            _sm.AimAIPointLookAt(null);
        }

        if(_sm.AlertValue < _sm.MaxAlertValue / 2 || _sm.IsCharacterDead || _sm.EnemyIdentity.IsSilentKilled)
        {
            _sm.SwitchState(_factory.AI_IdleState());
            return;
        }
        else if(_sm.AlertValue >= _sm.MaxAlertValue)
        {
            _sm.SwitchState(_factory.AI_EngageState());
            return;
        }


        _sm.RunningTowardsEnemy();
        if(_sm.CurrPOI == null)
        {
            if(_sm.GetFOVAdvancedData.HasToCheckEnemyLastSeenPosition) //meaning visible targets or other visible ada - krn kalo ga ada pasti ud di unchcek lwt _sm.runningtowardsenemy yg kalo ga ada visible atau other visible
            {
                _sm.EnemyAIManager.OnCaptainsStartHunting?.Invoke(_sm);
            }
            else
            {
                _sm.EnemyAIManager.EditEnemyCaptainList(_sm, false);
            }

        }


    }
    public override void ExitState()
    {
        _sm.IsAIHunted = false;
        if(_sm.CurrPOI != null) _sm.CurrPOI = null;
    }
    
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_IdleState : EnemyAIState
{
    public EnemyAI_IdleState(EnemyAIBehaviourStateMachine stateMachine, EnemyAIStateFactory factory) : base(stateMachine, factory)
    {
        
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        if(_stateMachine.IsCharacterDead)
        {
            if(_stateMachine.GetMoveStateMachine.CurrAIDirPos != _stateMachine.transform.position)_stateMachine.GetMoveStateMachine.ForceStopMoving();
            return;
        }
        
    }
    
}

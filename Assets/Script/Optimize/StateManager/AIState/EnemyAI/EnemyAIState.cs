using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIState : BaseState
{
    protected EnemyAIStateFactory _factory;
    protected EnemyAIBehaviourStateMachine _stateMachine;
    public EnemyAIState(EnemyAIBehaviourStateMachine stateMachine, EnemyAIStateFactory factory)
    {
        _factory = factory;
        _stateMachine = stateMachine;
    }
    public override void EnterState()
    {
        // throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        // throw new System.NotImplementedException();
    }

}

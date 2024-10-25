using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIState : BaseState
{
    protected EnemyAIStateFactory _factory;
    protected EnemyAIBehaviourStateMachine _sm;
    public EnemyAIState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory)
    {
        _factory = factory;
        _sm = currStateMachine;
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

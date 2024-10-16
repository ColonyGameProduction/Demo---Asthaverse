using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIStateFactory
{
    EnemyAIBehaviourStateMachine _machine;
    EnemyAIState _idle;
    public EnemyAIStateFactory (EnemyAIBehaviourStateMachine machine)
    {
        _machine = machine;
    }
    
    public EnemyAIState AI_IdleState()
    {
        if(_idle == null)_idle = new EnemyAIState(_machine, this);
        return _idle;
    }
}


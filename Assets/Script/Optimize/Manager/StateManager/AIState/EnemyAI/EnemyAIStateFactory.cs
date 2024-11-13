using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIStateFactory
{
    EnemyAIBehaviourStateMachine _machine;
    EnemyAIState _idle, _hunted, _engage, _takingCover;
    public EnemyAIStateFactory (EnemyAIBehaviourStateMachine currStateMachine)
    {
        _machine = currStateMachine;
    }
    
    public EnemyAIState AI_IdleState()
    {
        if(_idle == null)_idle = new EnemyAI_IdleState(_machine, this);
        return _idle;
    }
    public EnemyAIState AI_HuntedState()
    {
        if(_hunted == null)_hunted = new EnemyAI_HuntedState(_machine, this);
        return _hunted;
    }
    public EnemyAIState AI_EngageState()
    {
        if(_engage == null)_engage = new EnemyAI_EngageState(_machine, this);
        return _engage;
    }
    
    public EnemyAIState AI_TakingCoverState()
    {
        if(_takingCover == null)_takingCover = new EnemyAI_TakingCoverState(_machine, this);
        return _takingCover;
    }
}


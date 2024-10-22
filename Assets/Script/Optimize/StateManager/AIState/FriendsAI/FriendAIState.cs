using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIState : BaseState
{
    protected FriendAIStateFactory _factory;
    protected FriendAIBehaviourStateMachine _stateMachine;
    public FriendAIState(FriendAIBehaviourStateMachine stateMachine, FriendAIStateFactory factory)
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

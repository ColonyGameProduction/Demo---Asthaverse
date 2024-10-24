using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIState : BaseState
{
    protected FriendAIStateFactory _factory;
    protected FriendAIBehaviourStateMachine _sm;
    public FriendAIState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory)
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

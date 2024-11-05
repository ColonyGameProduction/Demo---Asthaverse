using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIStateFactory
{
    FriendAIBehaviourStateMachine _machine;
    FriendAIState _idle, _engage, _takeCover;
    public FriendAIStateFactory (FriendAIBehaviourStateMachine currStateMachine)
    {
        _machine = currStateMachine;
    }
    public FriendAIState AI_IdleState()
    {
        if(_idle == null)_idle = new FriendAI_IdleState(_machine, this);
        return _idle;
    }
    public FriendAIState AI_EngageState()
    {
        if(_engage == null)_engage = new FriendAI_EngageState(_machine, this);
        return _engage;
    }
    public FriendAIState AI_TakingCoverState()
    {
        if(_takeCover == null)_takeCover = new FriendAI_TakingCoverState(_machine, this);
        return _takeCover;
    }
}

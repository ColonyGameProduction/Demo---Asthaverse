using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAIStateFactory
{
    FriendAIBehaviourStateMachine _machine;
    FriendAIState _idle;
    public FriendAIStateFactory (FriendAIBehaviourStateMachine machine)
    {
        _machine = machine;
    }
}

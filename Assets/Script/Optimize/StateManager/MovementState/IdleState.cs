using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementState
{
    public IdleState()
    {
        StateAnimationName = "IdleAnimation";
    }

    public override void EnterState(MovementStateManager stateManager)
    {

    }

    public override void UpdateState(MovementStateManager stateManager)
    {
        throw new NotImplementedException();
    }
    public override void ExiState(MovementStateManager stateManager)
    {
        throw new NotImplementedException();
    }
}

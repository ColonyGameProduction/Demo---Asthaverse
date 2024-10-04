using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementState : CharacterBaseState<MovementStateManager>
{
    public override void EnterState(MovementStateManager stateManager)
    {
        base.EnterState(stateManager);
    }

    public override void ExiState(MovementStateManager stateManager)
    {
        base.EnterState(stateManager);
    }

}

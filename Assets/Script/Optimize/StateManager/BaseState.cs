using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T> where T : BaseStateManager
{
    public abstract void EnterState(T stateManager);
    public abstract void UpdateState(T stateManager);
    public abstract void ExiState(T stateManager);
}

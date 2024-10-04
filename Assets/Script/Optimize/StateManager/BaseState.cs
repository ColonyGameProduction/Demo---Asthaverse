using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T> where T : BaseStateManager
{
    //If Entering State do what
    public abstract void EnterState(T stateManager);
    //if in Update frame, do what
    public abstract void UpdateState(T stateManager);
    //If going to exit state, do what
    public abstract void ExiState(T stateManager);
}

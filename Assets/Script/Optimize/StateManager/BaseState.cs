using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base of All state
/// </summary>
public abstract class BaseState
{
    //If Entering State do what
    public abstract void EnterState();
    //if in Update frame, do what
    public abstract void UpdateState();
    //If going to exit state, do what
    public virtual void ExitState()
    {

    }
}

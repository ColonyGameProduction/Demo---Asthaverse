using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base of all State Manager
/// </summary>
public abstract class BaseStateMachine : MonoBehaviour
{
    //Switch to other state

    public abstract void SwitchState(BaseState newState);
}

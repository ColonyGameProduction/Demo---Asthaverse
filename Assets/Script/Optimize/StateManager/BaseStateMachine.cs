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
    protected bool _isInputPlayer;
    public bool IsInputPlayer {get {return _isInputPlayer;}}
    public abstract void SwitchState(BaseState newState);
}

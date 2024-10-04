using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base of all State Manager
/// </summary>
public abstract class BaseStateManager : MonoBehaviour
{
    //Switch to other state
    public abstract void SwitchState<T>(BaseState<T> newState) where T : BaseStateManager;
}

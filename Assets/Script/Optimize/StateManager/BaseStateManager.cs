using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateManager : MonoBehaviour
{
    public abstract void SwitchState<T>(BaseState<T> newState) where T : BaseStateManager;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base of all State Manager
/// </summary>
public abstract class BaseStateMachine : MonoBehaviour
{
    protected CharacterIdentity _charaIdentity;
    public bool IsCharacterDead { get {return _charaIdentity.IsDead;}}
    //Switch to other state
    protected virtual void Awake()
    {
        if(_charaIdentity == null)_charaIdentity = GetComponent<CharacterIdentity>();
    }
    public abstract void SwitchState(BaseState newState);
}

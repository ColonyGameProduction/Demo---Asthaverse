using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base State that connect with animation; connect
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class  CharacterBaseState<T> : BaseState where T : CharacterStateMachine
{
    protected T _stateMachine;
    protected string StateAnimationName{get;set;}
    public CharacterBaseState(T stateMachine)
    {
        _stateMachine = stateMachine;
    }
    public override void EnterState()
    {
        _stateMachine.CharaAnimator.SetBool(StateAnimationName, true);
    }

    public override void ExiState()
    {
        _stateMachine.CharaAnimator.SetBool(StateAnimationName, false);
    }
}

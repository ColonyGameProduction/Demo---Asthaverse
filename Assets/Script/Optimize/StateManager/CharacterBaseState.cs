using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base State that connect with animation; connect
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class  CharacterBaseState<T> : BaseState<T> where T : CharacterStateManager
{
    protected string StateAnimationName{get;set;}
    public override void EnterState(T stateManager)
    {
        stateManager.CharaAnimator.SetBool(StateAnimationName, true);
    }

    public override void ExiState(T stateManager)
    {
        stateManager.CharaAnimator.SetBool(StateAnimationName, false);
    }
}

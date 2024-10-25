using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base State that connect with animation in it's doing; 
/// StateAnimationName maksudnya nama bool parameter di animator
/// Di sini pakai T karena state machinenya yg penting anak dr characterstatemachine
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class  CharacterBaseState<T> : BaseState where T : CharacterStateMachine
{
    protected T _sm; // STATE MACHINE
    protected string _activeStateAnimParamName{get;set;} //Param -> Parameter
    public CharacterBaseState(T currStateMachine)
    {
        _sm = currStateMachine;
    }
    public override void EnterState()
    {
        SetAnimParamActive(_activeStateAnimParamName);
    }

    public override void ExitState()
    {
        SetAnimParamInactive(_activeStateAnimParamName);
    }


    protected void SetAnimParamActive(string animationName)
    {
        if(!_sm.CharaAnimator.GetBool(animationName))_sm.CharaAnimator?.SetBool(animationName, true);
    }
    protected void SetAnimParamInactive(string animationName)
    {
        if(_sm.CharaAnimator.GetBool(animationName))_sm.CharaAnimator?.SetBool(animationName, false);
    }
}

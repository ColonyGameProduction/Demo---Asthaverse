using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crouch
/// </summary>
public class CrouchState : WalkState
{
    public CrouchState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory) => _activeStateAnimParamName = "Crouch";
    public override void EnterState()
    {
        SetAnimParamActive(_activeStateAnimParamName);
        _sm.CharaConDataToCrouch();
        
        _sm.ChangeCurrSpeed(_standData.CrouchSpeed);
        if(!_sm.IsAIInput)PlayableCharacterCameraManager.OnCrouchCameraHeight?.Invoke();
    }

    public override void ExitState()
    {
        if(_sm.IsCharacterDead)
        {
            _standData.IsCrouching = false;
            if(!_sm.IsAIInput)PlayableCharacterCameraManager.OnResetCameraHeight?.Invoke();
        }
        
        if(!_standData.IsCrouching)
        {
            SetAnimParamInactive(_activeStateAnimParamName);
            if(!_sm.IsAIInput)PlayableCharacterCameraManager.OnResetCameraHeight?.Invoke();
        }
    }

    protected override void CheckStateWhileMoving()
    {
        if(!_standData.IsCrouching)
        {
            // if(_sm.IsHeadHitWhenUnCrouch()) return;

            _sm.CharaConDataToNormal();
            if(_standData.IsRunning) _sm.SwitchState(_factory.RunState());
            else _sm.SwitchState(_factory.WalkState());
        }
    }
}

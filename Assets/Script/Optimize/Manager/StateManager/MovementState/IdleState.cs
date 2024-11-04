using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// State idle, jalankan animasi idle saja dan menunggu apakah ada sesuatu yg dpt memicu masuk ke state lain
/// </summary>
public class IdleState : MovementState
{
    float _timeCounter, _currTargetTime, _nextIdleAnimIdxTarget;
    bool _isIdleAnimChanging;
    const float EPSILON = 0.0001f;
    const string ANIMATION_MOVE_PARAMETER_CROUCH = "Crouch";
    const float STAND_IDLE_ANIM_CYCLE_TOTAL = 3;

    public IdleState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory){}
    
    public override void EnterState()
    {   
        ResetIdleAnimCycle();
        
        _standData.IsIdle = true;

        StopMovementAnimation();
    }
    public override void UpdateState()
    {
        if(_playableData != null && (PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || _playableData.GetPlayableCharacterIdentity.IsReviving || _playableData.GetPlayableCharacterIdentity.IsSilentKilling))return;

        if((!_sm.IsAIInput && _playableData.InputMovement != Vector3.zero) || (_sm.IsAIInput && !_sm.IsAIAtDirPos()))
        {
            if(_groundData != null && _groundData.IsCrawling)_sm.SwitchState(_factory.CrawlState());
            else if(_groundData != null && _groundData.IsCrouching)_sm.SwitchState(_factory.CrouchState());
            else if(_standData.IsRunning)_sm.SwitchState(_factory.RunState());
            else _sm.SwitchState(_factory.WalkState());
        }
        
        CheckMustRotateWhileIdle();

        CheckingIdleAnimationCycle();
        if(_groundData != null) CheckIsCrouchWhileIdle();
    }
    public override void ExitState()
    {
        if((!_sm.IsAIInput && _playableData.InputMovement != Vector3.zero) || (_sm.IsAIInput && !_sm.IsAIAtDirPos())) _standData.IsIdle = false; //kyk gini krn bs aja keluar krn crouch state di atas
        // base.EnterState(); //Stop Idle Anim
        SetAnimParamActive(ANIMATION_MOVE_PARAMETER_ISMOVING);
        if(_sm.IdleAnimCycleIdx > 1)_sm.SetIdleAnimToNormal();
    }
    private void CheckMustRotateWhileIdle()
    {
        if(!_sm.IsAIInput && _playableData.IsMustLookForward)_playableData.RotateToAim_Idle();
        if(_sm.IsAIInput && _sm.IsAIAtDirPos())if(_sm.AllowLookTarget)_sm.RotateAIToTarget_Idle();
    }
    private void CheckIsCrouchWhileIdle()
    {
        if(_groundData.IsCrouching)
        {
            if(_sm.IdleAnimCycleIdx > 1) 
            {
                _sm.SetIdleAnimToNormal();
                _timeCounter = _sm.IdleAnimCycleTimeTarget[0];
                _currTargetTime = _sm.IdleAnimCycleTimeTarget[1];
            }
            SetAnimParamActive(ANIMATION_MOVE_PARAMETER_CROUCH);
        }
        else
        {
            SetAnimParamInactive(ANIMATION_MOVE_PARAMETER_CROUCH);
        }
    }
    private void StopMovementAnimation()
    {
        _sm.CharaAnimator?.SetFloat(MovementStateMachine.ANIMATION_MOVE_PARAMETER_HORIZONTAL, 0);
        _sm.CharaAnimator?.SetFloat(MovementStateMachine.ANIMATION_MOVE_PARAMETER_VERTICAL, 0);
        SetAnimParamInactive(ANIMATION_MOVE_PARAMETER_ISMOVING);
    }

    #region Idle Anim Cycle
    private void CheckingIdleAnimationCycle()
    {
        if(!_isIdleAnimChanging) IdleAnimCycleTimerUpdate();
        else if(_isIdleAnimChanging) ChangingIdleAnimation();
        if(_sm.WasCharacterAiming)
        {
            _isIdleAnimChanging = false;
            _sm.WasCharacterAiming = false;
            _sm.SetIdleAnimAfterAim();
            _timeCounter = 0;
            _currTargetTime = _sm.IdleAnimCycleTimeTarget[0];
        }
    }
    private void IdleAnimCycleTimerUpdate()
    {
        // Debug.Log("Time COUNTERR" + _timeCounter);
        if(_timeCounter < _currTargetTime)_timeCounter += Time.deltaTime;
        else if(_timeCounter >= _currTargetTime)
        {
            if(_sm.IdleAnimCycleIdx >= 0)
            {
                if(_sm.IdleAnimCycleIdx == 0 || _groundData == null || (_groundData != null && !_groundData.IsCrouching))
                {
                    if(_sm.IdleAnimCycleIdx < STAND_IDLE_ANIM_CYCLE_TOTAL)
                    {
                        float nextIdleAnimIdx = _sm.IdleAnimCycleIdx + 1;
                        if(nextIdleAnimIdx < STAND_IDLE_ANIM_CYCLE_TOTAL)_currTargetTime = _sm.IdleAnimCycleTimeTarget[(int)nextIdleAnimIdx];
                        _nextIdleAnimIdxTarget = nextIdleAnimIdx;
                        _isIdleAnimChanging = true;
                        // _stateMachine.ChangeIdleCounter(x);
                    }
                }
            }
            
        }
        
    }
    private void ChangingIdleAnimation()
    {   
        float tempCounter = Mathf.Lerp(_sm.IdleAnimCycleIdx, _nextIdleAnimIdxTarget, Time.deltaTime * _sm.IdleAnimCycleSpeed);
        if(_nextIdleAnimIdxTarget - tempCounter < EPSILON)tempCounter = _nextIdleAnimIdxTarget;
        _sm.SetIdleAnimCycleIdx(tempCounter);
        if(_sm.IdleAnimCycleIdx == _nextIdleAnimIdxTarget)
        {
            _isIdleAnimChanging = false;
            _sm.SetIdleAnimCycleIdx(_nextIdleAnimIdxTarget);
        }
    }

    private void ResetIdleAnimCycle()
    {
        _isIdleAnimChanging = false;

        if(_sm.WasCharacterAiming)
        {
            _sm.WasCharacterAiming = false;
            _sm.SetIdleAnimAfterAim();
            _timeCounter = 0;
            _currTargetTime = _sm.IdleAnimCycleTimeTarget[0];

        }
        
        else if(_sm.IdleAnimCycleIdx == 1)
        {
            _timeCounter = _sm.IdleAnimCycleTimeTarget[0];
            _currTargetTime = _sm.IdleAnimCycleTimeTarget[1];
        }
    }
    #endregion
        
}

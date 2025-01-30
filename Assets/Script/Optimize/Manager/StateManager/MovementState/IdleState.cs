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
        if(_playableData != null)_playableData.GetPlayableMakeSFX.StopMovementTypeSFX();
        ResetIdleAnimCycle();
        
        _standData.IsIdle = true;

        StopMovementAnimation();
    }
    public override void UpdateState()
    {
        _playableData?.RotateWhileReviving();
        if(_playableData != null && (PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || _playableData.GetPlayableCharacterIdentity.IsReviving || _playableData.GetPlayableCharacterIdentity.IsSilentKilling))return;

        if((!_sm.IsAIInput && _playableData.InputMovement != Vector3.zero) || (_sm.IsAIInput && !_sm.IsAIAtDirPos()))
        {
            if(_groundData != null && _groundData.IsCrawling)_sm.SwitchState(_factory.CrawlState());
            else if(_standData.IsCrouching)_sm.SwitchState(_factory.CrouchState());
            else if(_standData.IsRunning)_sm.SwitchState(_factory.RunState());
            else _sm.SwitchState(_factory.WalkState());
        }
        
        CheckMustRotateWhileIdle();

        CheckingIdleAnimationCycle();
        CheckIsCrouchWhileIdle();

        CheckCharaCon();
        
    }
    public override void ExitState()
    {
        if((!_sm.IsAIInput && _playableData.InputMovement != Vector3.zero) || (_sm.IsAIInput && !_sm.IsAIAtDirPos())) _standData.IsIdle = false; //kyk gini krn bs aja keluar krn crouch state di atas
        // base.EnterState(); //Stop Idle Anim
        SetAnimParamActive(ANIMATION_MOVE_PARAMETER_ISMOVING);
        if(_sm.IdleAnimCycleIdx > 1)_sm.SetIdleAnimToNormal();
    }
    private void CheckCharaCon()
    {   if(_standData.IsCrouching)
        {
            _sm.CharaConDataToCrouch();
        }
        else if(_groundData != null && _groundData.IsCrawling)
        {
            if(_groundData.IsCrawling)
            {
                _playableData?.CharaConDataToCrawl();
            }
        }
        else if(!_standData.IsCrouching && ((_groundData == null) || (_groundData != null && !_groundData.IsCrawling)))
        {
            _sm.CharaConDataToNormal();
        }
    }
    private void CheckMustRotateWhileIdle()
    {
        if(!_sm.IsAIInput && _playableData.IsMustLookForward)_playableData.RotateToAim_Idle();
        if(_sm.IsAIInput && _sm.IsAIAtDirPos())if(_sm.AllowLookTarget)_sm.RotateAIToTarget_Idle();
    }
    private void CheckIsCrouchWhileIdle()
    {
        if(_standData.IsCrouching)
        {
            
            if(_sm.IdleAnimCycleIdx > 1) 
            {
                _sm.SetIdleAnimToNormal();
                _timeCounter = _sm.IdleAnimCycleTimeTarget[0];
                _currTargetTime = _sm.IdleAnimCycleTimeTarget[1];
            }
            SetAnimParamActive(ANIMATION_MOVE_PARAMETER_CROUCH);
            if(!_sm.IsAIInput)PlayableCharacterCameraManager.OnCrouchCameraHeight?.Invoke();
            

            if(_sm.IsAtCrouchPlatform && _sm.IsAIInput)
            {
                if(!_sm.IsHeadHitWhenUnCrouch())
                {
                    _sm.IsAtCrouchPlatform = false;
                    _sm.IsCrouching = false;
                }

            }
        }
        else
        {
            SetAnimParamInactive(ANIMATION_MOVE_PARAMETER_CROUCH);
            if(!_sm.IsAIInput)PlayableCharacterCameraManager.OnResetCameraHeight?.Invoke();
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
        if(_sm.IsIdleMustStayAlert)
        {
            if(_sm.IdleAnimCycleIdx != 0)SetIdleAnimToAlert();
        }
        else
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

    }
    private void IdleAnimCycleTimerUpdate()
    {
        // Debug.Log("Time COUNTERR" + _timeCounter);
        if(_timeCounter < _currTargetTime)_timeCounter += Time.deltaTime;
        else if(_timeCounter >= _currTargetTime)
        {
            if(_sm.IdleAnimCycleIdx >= 0)
            {
                if(_sm.IdleAnimCycleIdx == 0 || !_standData.IsCrouching)
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
    private void SetIdleAnimToAlert()
    {
        _isIdleAnimChanging = false;
        _sm.SetIdleAnimAfterAim();
        _timeCounter = 0;
        _currTargetTime = _sm.IdleAnimCycleTimeTarget[0];
    }
    #endregion
        
}

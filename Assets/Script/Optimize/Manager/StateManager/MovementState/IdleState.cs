using UnityEngine;

/// <summary>
/// State idle, jalankan animasi idle saja dan menunggu apakah ada sesuatu yg dpt memicu masuk ke state lain
/// </summary>
public class IdleState : MovementState
{
    float _timeCounter, _currTargetTime, _nextIdleAnimIdxTarget;
    bool _isIdleAnimChanging;
    bool _isGoingBackToIdleRelax1 = false;
    private float _standIdleRifleAnimCycleTotal = 3;
    const float EPSILON = 0.0001f;
    const string ANIMATION_MOVE_PARAMETER_CROUCH = "Crouch";

    public IdleState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory)
    {
        _standIdleRifleAnimCycleTotal = _sm.IdleAnimCycleTimeTarget.Length;
    }
    
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
        _sm.AnimateMovement(Vector3.zero);
        if(_playableData != null && (PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || _playableData.GetPlayableCharacterIdentity.IsReviving || _playableData.GetPlayableCharacterIdentity.IsSilentKilling))return;

        // Debug.Log(_sm.transform.name + " hallooooo????");
        // if(_sm.IsAIInput)Debug.Log(_sm.transform.name + "Why is take cover still on" + "I go here even if no takecover");
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
        if((_timeCounter < _currTargetTime && !_isGoingBackToIdleRelax1) || (_timeCounter > _currTargetTime && _isGoingBackToIdleRelax1))
        {
            if(!_isGoingBackToIdleRelax1) _timeCounter += Time.deltaTime;
            else _timeCounter -= Time.deltaTime;

            // Debug.Log(_sm.transform.name + " _time" + _timeCounter + " target" + _currTargetTime + " " + _isGoingBackToIdleRelax1);
        }
        else if((_timeCounter >= _currTargetTime && !_isGoingBackToIdleRelax1) || (_timeCounter <= _currTargetTime && _isGoingBackToIdleRelax1))
        {
            if(_sm.IdleAnimCycleIdx >= 0)
            {

                if(_sm.IdleAnimCycleIdx == 0 || !_standData.IsCrouching)
                {
                    if(_sm.IdleAnimCycleIdx < _standIdleRifleAnimCycleTotal)
                    {
                        
                        float nextIdleAnimIdx = _sm.IdleAnimCycleIdx + 1;
                        if(nextIdleAnimIdx < _standIdleRifleAnimCycleTotal) _currTargetTime = _sm.IdleAnimCycleTimeTarget[(int)nextIdleAnimIdx];
                        if(nextIdleAnimIdx == _standIdleRifleAnimCycleTotal && _playableData != null) _playableData.GetPlayableCharacterIdentity.RandomAnimationIdleFinalIdx();
                        _nextIdleAnimIdxTarget = nextIdleAnimIdx;
                        _isIdleAnimChanging = true;
                        // _stateMachine.ChangeIdleCounter(x);
                        // Debug.Log(_sm.transform.name + " _time" + " masuk atas" + _timeCounter + " target" + _currTargetTime + " " + _isGoingBackToIdleRelax1 + " " + _sm.IdleAnimCycleIdx + " " + _nextIdleAnimIdxTarget + " " + _standIdleRifleAnimCycleTotal);
                    }
                    else if(_sm.IdleAnimCycleIdx == _standIdleRifleAnimCycleTotal)
                    {
                        
                        if(!_isGoingBackToIdleRelax1)
                        {
                            _isGoingBackToIdleRelax1 = true;
                            float nextIdleAnimIdx = _sm.IdleAnimCycleIdx - 1;
                            _currTargetTime = _sm.IdleAnimCycleTimeTarget[(int)nextIdleAnimIdx-1];
                            _nextIdleAnimIdxTarget = nextIdleAnimIdx;
                            // Debug.Log(_sm.transform.name + " _time" + " masuk sini dulu" + _timeCounter + " target" + _currTargetTime + " " + _isGoingBackToIdleRelax1 + " " + _sm.IdleAnimCycleIdx + " " + _nextIdleAnimIdxTarget + " " + _standIdleRifleAnimCycleTotal);
                        }
                        else
                        {
                            // Debug.Log(_sm.transform.name + " _time" + " masuk bawah" + _timeCounter + " target" + _currTargetTime + " " + _isGoingBackToIdleRelax1 + " " + _sm.IdleAnimCycleIdx + " " + _nextIdleAnimIdxTarget + " " + _standIdleRifleAnimCycleTotal);
                            _isGoingBackToIdleRelax1 = false;
                            
                            _currTargetTime = _sm.IdleAnimCycleTimeTarget[(int)_standIdleRifleAnimCycleTotal - 1];
                            _isIdleAnimChanging = true;
                        }
                    }
                }
            }
            
        }
        
    }
    private void ChangingIdleAnimation()
    {   
        float tempCounter = Mathf.Lerp(_sm.IdleAnimCycleIdx, _nextIdleAnimIdxTarget, Time.deltaTime * _sm.IdleAnimCycleSpeed);
        if(Mathf.Abs(_nextIdleAnimIdxTarget - tempCounter) < EPSILON)
        {
            tempCounter = _nextIdleAnimIdxTarget;
            tempCounter = Mathf.Floor(tempCounter);
        }
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
        _isGoingBackToIdleRelax1 = false;
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

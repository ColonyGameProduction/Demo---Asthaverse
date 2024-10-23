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
    float _timeCounter;
    float _currTargetTime;
    float _nextChangeTarget;
    bool _isIdleAnimChanging;
    const float epsilon = 0.0001f;
    const string ANIMATION_MOVE_PARAMETER_CROUCH = "Crouch";
    //Di idle state ini, walaupun misal isRun atau isCrouch masih nyala, tetap bisa ke state ini, yg penting inputnya tidak ada 
    // dan karena crouch ada idle animation, jd crouch tetap di posisi animasi crouch; sedangkan run tidak. Ketika isCrouch = false, maka animasinya akan dimatikan

    public IdleState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        
    }
    
    public override void EnterState()
    {   
        _isIdleAnimChanging = false;

        if(_stateMachine.WasAiming)
        {
            _stateMachine.WasAiming = false;
            _stateMachine.ChangeIdleCounterAfterAim();
            _timeCounter = 0;
            _currTargetTime = _stateMachine.IdleRelaxTargetTime[0];

        }
        
        else if(_stateMachine.IdleCounter == 1)
        {
            _timeCounter = _stateMachine.IdleRelaxTargetTime[0];
            _currTargetTime = _stateMachine.IdleRelaxTargetTime[1];
        }
        
        
        _standMovement.IsIdle = true;

        _stateMachine.CharaAnimator?.SetFloat(MovementStateMachine.ANIMATION_MOVE_PARAMETER_HORIZONTAL, 0);
        _stateMachine.CharaAnimator?.SetFloat(MovementStateMachine.ANIMATION_MOVE_PARAMETER_VERTICAL, 0);
        StateAnimationOff(ANIMATION_MOVE_PARAMETER_ISMOVING);

    }
    public override void UpdateState()
    {
        //Kalo lg switch kan semuanya di force balik idle, dn kalo lwt sini ya gabisa ngapa ngapain :D
        if(_groundMovement != null && (PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter))return;

        if(!_isIdleAnimChanging) IdleAnimationChanger();
        else if(_isIdleAnimChanging) IdleAnimationChanging();
        
        //If there's an input movement: dalam hal ini kalo inputnya player berarti input movement tidak sama dengan 0 ATAU kalau input dari AI berarti currAIDirectionnya itu ga null, maka kita akan masuk ke state selanjutnya tergantung syarat yg ada
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            if(_groundMovement != null && _groundMovement.IsCrawling)_stateMachine.SwitchState(_factory.CrawlState());
            else if(_groundMovement != null && _groundMovement.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState());
            else if(_standMovement.IsRunning)_stateMachine.SwitchState(_factory.RunState());
            else _stateMachine.SwitchState(_factory.WalkState());

        }
        
        if(_stateMachine.IsInputPlayer &&_playableData.IsMustLookForward)_playableData.Idle_RotateAim();

        if(!_stateMachine.IsInputPlayer && _stateMachine.IsTargetTheSamePositionAsTransform())
        {
            if(_stateMachine.AskAIToLookWhileIdle)_stateMachine.IdleAI_RotateToEnemy();
        }

        if(_stateMachine.WasAiming)
        {
            _isIdleAnimChanging = false;
            _stateMachine.WasAiming = false;
            _stateMachine.ChangeIdleCounterAfterAim();
            _timeCounter = 0;
            _currTargetTime = _stateMachine.IdleRelaxTargetTime[0];

        }
        if(_groundMovement != null && _groundMovement.IsCrouching)
        {
            if(_stateMachine.IdleCounter > 1) 
            {
                _stateMachine.ChangeIdleCounterNormal();
                _timeCounter = _stateMachine.IdleRelaxTargetTime[0];
                _currTargetTime = _stateMachine.IdleRelaxTargetTime[1];
            }
            StateAnimationOn(ANIMATION_MOVE_PARAMETER_CROUCH);
        }
        else if(_groundMovement != null && !_groundMovement.IsCrouching)
        {
            StateAnimationOff(ANIMATION_MOVE_PARAMETER_CROUCH);
        }
    }
    public override void ExitState()
    {
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform())) _standMovement.IsIdle = false; //kyk gini krn bs aja keluar krn crouch state di atas
        // base.EnterState(); //Stop Idle Anim
        StateAnimationOn(ANIMATION_MOVE_PARAMETER_ISMOVING);
        if(_stateMachine.IdleCounter > 1)_stateMachine.ChangeIdleCounterNormal();
    }

    public void IdleAnimationChanger()
    {
        // Debug.Log("Time COUNTERR" + _timeCounter);
        if(_timeCounter < _currTargetTime)_timeCounter += Time.deltaTime;
        else if(_timeCounter >= _currTargetTime)
        {
            if(_stateMachine.IdleCounter >= 0)
            {
                if(_stateMachine.IdleCounter == 0 || _groundMovement == null || (_groundMovement != null && !_groundMovement.IsCrouching))
                {
                    if(_stateMachine.IdleCounter < 3)
                    {
                        float x = _stateMachine.IdleCounter + 1;
                        if(x < 3)_currTargetTime = _stateMachine.IdleRelaxTargetTime[(int)x];
                        _nextChangeTarget = x;
                        _isIdleAnimChanging = true;
                        // _stateMachine.ChangeIdleCounter(x);
                    }
                }
            }
            
        }
        
    }
    public void IdleAnimationChanging()
    {   
        float tempCounter = Mathf.Lerp(_stateMachine.IdleCounter, _nextChangeTarget, Time.deltaTime * 2);
        if(_nextChangeTarget - tempCounter < epsilon)tempCounter = _nextChangeTarget;
        _stateMachine.ChangeIdleCounter(tempCounter);
        if(_stateMachine.IdleCounter == _nextChangeTarget)
        {
            _isIdleAnimChanging = false;
            _stateMachine.ChangeIdleCounter(_nextChangeTarget);
        }
    }
        
}

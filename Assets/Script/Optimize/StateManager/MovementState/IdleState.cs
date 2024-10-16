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
    bool wasCrouch;
    //Di idle state ini, walaupun misal isRun atau isCrouch masih nyala, tetap bisa ke state ini, yg penting inputnya tidak ada 
    // dan karena crouch ada idle animation, jd crouch tetap di posisi animasi crouch; sedangkan run tidak. Ketika isCrouch = false, maka animasinya akan dimatikan

    public IdleState(MovementStateMachine machine, MovementStateFactory factory) : base(machine, factory)
    {
        // StateAnimationName = "IdleAnimation";
    }
    
    public override void EnterState()
    {
        Debug.Log("Idle" + _stateMachine.gameObject.name);
        
        //Making sure that it's idle animation that plays
        if(_crouch != null && _crouch.IsCrouching)wasCrouch = true;
        _standMovement.IsIdle = true;
        _stateMachine.CharaAnimator?.SetFloat(MovementStateMachine.ANIMATION_MOVE_PARAMETER_HORIZONTAL, 0);
        _stateMachine.CharaAnimator?.SetFloat(MovementStateMachine.ANIMATION_MOVE_PARAMETER_VERTICAL, 0);
    }
    public override void UpdateState()
    {
        //Kalo lg switch kan semuanya di force balik idle, dn kalo lwt sini ya gabisa ngapa ngapain :D
        if(PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter)return;

        //If there's an input movement: dalam hal ini kalo inputnya player berarti input movement tidak sama dengan 0 ATAU kalau input dari AI berarti currAIDirectionnya itu ga null, maka kita akan masuk ke state selanjutnya tergantung syarat yg ada
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform()))
        {
            if(_crouch != null && _crouch.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState());
            else if(_standMovement.IsRunning)_stateMachine.SwitchState(_factory.RunState());
            else _stateMachine.SwitchState(_factory.WalkState());

        }
        
        if(_stateMachine.IsInputPlayer &&_playableData.IsMustLookForward)_playableData.Idle_RotateAim();
        //Ini emang sudah diam di tmpt
        ///Urusan animasi aja, soalnya kek kondisi crouch kan dia msh bs idle
        ///
        //Cek animasinya msh jalan ga, kalo ga jalan gausa pindah ke sana
        // if(_crouch != null)
        // {
        //     if(!_crouch.IsCrouching)_stateMachine.SwitchState(_factory.CrouchState()); //lwt bntr utk stop animasi
        // }

        if(wasCrouch && !_crouch.IsCrouching)
        {
            wasCrouch = false;
            _stateMachine.SwitchState(_factory.CrouchState());
        }
    }
    public override void ExitState()
    {
        if((_stateMachine.IsInputPlayer && _playableData.InputMovement != Vector3.zero) || (!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform())) _standMovement.IsIdle = false; //kyk gini krn bs aja keluar krn crouch state di atas
        // base.EnterState(); //Stop Idle Anim
    }
        
}

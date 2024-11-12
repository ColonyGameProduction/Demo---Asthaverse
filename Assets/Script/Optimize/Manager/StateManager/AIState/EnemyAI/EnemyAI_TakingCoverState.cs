using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_TakingCoverState : EnemyAIState
{
    float changeTimer;
    float changeTimerMax = 3f;
    public EnemyAI_TakingCoverState(EnemyAIBehaviourStateMachine currStateMachine, EnemyAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        _sm.IsHiding = true;
        _sm.IsChecking = false;
        changeTimer = changeTimerMax;
        _sm.HidingCheckDelayTimer = _sm.GetFOVMachine.FindDelayTimerNow;
        _sm.EnemyWhoSawAIList.Clear();

        _sm.EnemyIdentity.Aiming(true);
        _sm.SetAllowLookTarget(true, _sm.GetMoveStateMachine, _sm.DirToLookAtWhenTakingCover, true);

        // if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching && _sm.GetMoveStateMachine.IsIdle)_sm.GetPlayableCharaIdentity.Crouch(true);
        if(!_sm.isWallTallerThanChara && !_sm.GetMoveStateMachine.IsCrouching )_sm.EnemyIdentity.Crouch(true);
    }

    public override void UpdateState()
    {
        if(_sm.HidingCheckDelayTimer > 0 && _sm.IsAtTakingCoverHidingPlace && ((!_sm.isWallTallerThanChara && _sm.IsCrouchingBehindWall()) || _sm.isWallTallerThanChara)) _sm.HidingCheckDelayTimer -= Time.deltaTime;
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.IsCrouching)_sm.GetMoveStateMachine.IsCrouching = false;
        _sm.IsTakingCover = false;
        _sm.IsAtTakingCoverHidingPlace = false;
        _sm.IsAtTakingCoverCheckingPlace = false;
    }
}

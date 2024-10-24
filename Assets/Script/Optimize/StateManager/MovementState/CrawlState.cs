using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlState : MovementState
{
    public CrawlState(MovementStateMachine stateMachine, MovementStateFactory factory) : base(stateMachine, factory)
    {
        // StateAnimationName = "Crawl";
        StateAnimationName = "Move";
    }

    public override void EnterState()
    {
        base.EnterState(); // Jalankan animasi
        // Debug.Log("Crawling" + _stateMachine.gameObject.name);
        _stateMachine.ChangeCurrSpeed(_groundMovement.CrawlSpeed);

        //mungkin di sini bisa ditambah kalau masuknya zero atau masih idle dan iscrouching false, maka animasi dimatikan trus lsg ke exit
    }
    public override void UpdateState()
    {
        //satu-satunya ke sini adalah lwt ai, yg gerakin ai, input player ga mungkin
        if(!_stateMachine.IsInputPlayer && !_stateMachine.IsTargetTheSamePositionAsTransform())
        {
            _stateMachine.Move();
            if(!_stateMachine.IsCharacterDead)_stateMachine.SwitchState(_factory.IdleState());
        }
        else if(_stateMachine.IsTargetTheSamePositionAsTransform())
        {
            _stateMachine.SwitchState(_factory.IdleState());
        }
    }
    public override void ExitState()
    {
        // if(!_crouch.IsCrouching) //Matikan state animasi crouch
        Debug.Log(_stateMachine.IsCharacterDead);
        if(!_stateMachine.IsCharacterDead)_groundMovement.IsCrawling = false;
        
        base.ExitState();
    }
}

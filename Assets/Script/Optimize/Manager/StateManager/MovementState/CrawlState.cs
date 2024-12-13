using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlState : MovementState
{
    public CrawlState(MovementStateMachine currStateMachine, MovementStateFactory factory) : base(currStateMachine, factory)
    {
        // StateAnimationName = "Crawl";
        _activeStateAnimParamName = "Move";
    }

    public override void EnterState()
    {
        base.EnterState(); // Jalankan animasi
        // Debug.Log("Crawling" + _stateMachine.gameObject.name);
        _sm.ChangeCurrSpeed(_groundData.CrawlSpeed);
        _playableData?.CharaConDataToCrawl();

        //mungkin di sini bisa ditambah kalau masuknya zero atau masih idle dan iscrouching false, maka animasi dimatikan trus lsg ke exit
    }
    public override void UpdateState()
    {
        //satu-satunya ke sini adalah lwt ai, yg gerakin ai, input player ga mungkin
        if(_sm.IsAIInput && !_sm.IsAIAtDirPos())
        {
            _sm.Move();
            if(!_sm.IsCharacterDead)
            {
                _sm.CharaConDataToNormal();
                _sm.SwitchState(_factory.IdleState());
            }
        }
        else if(_sm.IsAIAtDirPos())
        {
            _sm.SwitchState(_factory.IdleState());
        }
    }
    public override void ExitState()
    {
        if(!_sm.IsCharacterDead)_groundData.IsCrawling = false;
        
        base.ExitState();
    }
}

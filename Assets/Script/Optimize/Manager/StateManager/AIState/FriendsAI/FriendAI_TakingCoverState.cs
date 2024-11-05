using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//runaway - includes running away or run to take cover
public class FriendAI_TakingCoverState : FriendAIState
{
    public FriendAI_TakingCoverState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        //I'm on my way to the take cover place
        if(_sm.GotDetectedbyEnemy)
        {
            if(_sm.EnemyWhoSawAIList.Count >= _sm.MinEnemyMakeCharaFeelOverwhelmed)
            {
                _sm.SwitchState(_factory.AI_EngageState());
                return;
            }
        }
        _sm.GetFOVMachine.GetClosestEnemy();
        _sm.GetClosestEnemyWhoSawAI();
    }
    public override void ExitState()
    {
        
    }
}

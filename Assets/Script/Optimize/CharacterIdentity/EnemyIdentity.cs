using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentity : CharacterIdentity
{
    protected EnemyAIBehaviourStateMachine _enemyAIStateMachine;
    
    protected override void Awake() {
        base.Awake();
        _enemyAIStateMachine = GetComponent<EnemyAIBehaviourStateMachine>();
    }
    public override void ReloadWeapon()
    {
        CurrWeapon.currBullet = CurrWeapon.weaponStatSO.magSize;
    }
    public override void Death()
    {
        base.Death();
        EnemyAIManager.Instance.EditEnemyCaptainList(_enemyAIStateMachine, false);
        EnemyAIManager.Instance.EditEnemyHearAnnouncementList(_enemyAIStateMachine, false);
        _enemyAIStateMachine.enabled = false;
    }
    
}

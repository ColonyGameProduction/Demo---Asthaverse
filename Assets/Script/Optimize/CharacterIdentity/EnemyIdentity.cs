using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentity : CharacterIdentity, ISilentKillAble
{
    protected EnemyAIBehaviourStateMachine _enemyAIStateMachine;
    private bool _canBeKill = true;
    private bool _isSilentKilled;

    public Transform SilentKillAbleTransform {get{return transform;}}

    public bool CanBeKill {get{return _canBeKill;}}
    public bool IsSilentKilled {get{return _isSilentKilled;}}

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
    public override void AfterFinishDeathAnimation()
    {
        _enemyAIStateMachine.UnsubscribeEvent();
        Destroy(this.gameObject, 0.5f);
    }

    public void GotSilentKill(PlayableCharacterIdentity characterIdentityWhoKilling)
    {
        characterIdentityWhoKilling.GetPlayableMovementData.ForceStopMoving();
        characterIdentityWhoKilling.GetPlayableUseWeaponData.SetSilentKilledEnemy(this);
        characterIdentityWhoKilling.IsSilentKilling = true;
        _isSilentKilled = true;
        _moveStateMachine.ForceStopMoving();
        _useWeaponStateMachine.ForceStopUseWeapon();
    }
    public void GotSilentKilled()
    {
        Hurt(CurrHealth);
    }
}

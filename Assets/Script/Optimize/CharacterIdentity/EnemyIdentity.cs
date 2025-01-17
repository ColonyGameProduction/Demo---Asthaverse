using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentity : CharacterIdentity, ISilentKillAble
{
    protected EnemyAIBehaviourStateMachine _enemyAIStateMachine;

    private bool _canBeKill = true;
    private bool _isSilentKilled;
    [SerializeField] private Transform _enemyGameObject;

    private const string ANIMATION_PARAMETER_SILENTKILLED = "SilentKilled";

    public Transform GetSilentKillAbleTransform {get{return transform;}}
    public bool CanBeKill {get{return _canBeKill;}}
    public bool IsSilentKilled {get{return _isSilentKilled;}}

    protected override void Awake() 
    {
        base.Awake();
        _enemyAIStateMachine = _aiStateMachine as EnemyAIBehaviourStateMachine;
        _enemyGameObject = _animator.gameObject.transform;
    }
    public override void ReloadWeapon()
    {
        CurrWeapon.currBullet = CurrWeapon.weaponStatSO.magSize;
    }
    protected override void HandleDeath()
    {
        base.HandleDeath();
        EnemyAIManager.Instance.EditEnemyCaptainList(_enemyAIStateMachine, false);
        EnemyAIManager.Instance.EditEnemyHearAnnouncementList(_enemyAIStateMachine, false);
        EnemyAIManager.Instance.OnEnemyDead?.Invoke(this.transform);
        _enemyAIStateMachine.enabled = false;
    }
    public override void AfterFinishDeathAnimation()
    {
        if(_isSilentKilled)
        {
            if(_fovMachine.enabled) _fovMachine.StopFOVMachine();
            _fovMachine.enabled = false;

            EnemyAIManager.Instance.EditEnemyCaptainList(_enemyAIStateMachine, false);
            EnemyAIManager.Instance.EditEnemyHearAnnouncementList(_enemyAIStateMachine, false);
            EnemyAIManager.Instance.OnEnemyDead?.Invoke(this.transform);
            _enemyAIStateMachine.enabled = false;
        }
        _enemyAIStateMachine.UnsubscribeEvent();
        Destroy(this.gameObject, 0.5f);
    }

    public void GotSilentKill(PlayableCharacterIdentity characterIdentityWhoKilling)
    {
        _silentKillAnimationIdx = characterIdentityWhoKilling.SilentKillIdx;        
        _animator.SetFloat(ANIMATION_PARAMETER_SILENTKILLCOUNTER, _silentKillAnimationIdx);

        characterIdentityWhoKilling.GetPlayableMovementStateMachine.ForceStopMoving();
        characterIdentityWhoKilling.GetPlayableUseWeaponStateMachine.SetSilentKilledEnemy(this);
        characterIdentityWhoKilling.IsSilentKilling = true;
        _isSilentKilled = true;
        _isDead = true;
        _moveStateMachine.ForceStopMoving();
        _useWeaponStateMachine.ForceStopUseWeapon();
    }
    public void GotSilentKilled()
    {
        Hurt(CurrHealth);
    }

    public override void Hurt(float Damage)
    {
        if(_isSilentKilled) return;
        base.Hurt(Damage);
    }
    public void StartSilentKilled(Transform silentKillerTransform)
    {
        _animator.SetBool(ANIMATION_PARAMETER_SILENTKILLED, true);

        _enemyGameObject.position = silentKillerTransform.position;
        _enemyGameObject.rotation = silentKillerTransform.rotation;
        
        OnToggleFollowHandRig?.Invoke(false, false);
    }
}

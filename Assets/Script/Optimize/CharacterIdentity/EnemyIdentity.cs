using UnityEngine;

public class EnemyIdentity : CharacterIdentity, ISilentKillAble
{
    protected EnemyAIBehaviourStateMachine _enemyAIStateMachine;

    private bool _canBeKill = true;
    private bool _isSilentKilled;
    private Transform _enemyGameObject;
    

    [Header("Dead Variable")]
    [SerializeField] private float _enemyDeathFadeDuration = 2f;
    private Body _enemyBody;
    [SerializeField] private GameObject[] _hideUIArrayWhenDead;

    private const string ANIMATION_PARAMETER_SILENTKILLED = "SilentKilled";
    

    public Transform GetSilentKillAbleTransform {get{return transform;}}
    public bool CanBeKill {get{return _canBeKill;}}
    public bool IsSilentKilled {get{return _isSilentKilled;}}

    protected override void Awake() 
    {
        base.Awake();
        SetIdleFinalAnimationAnimatorIdx();

        _enemyAIStateMachine = _aiStateMachine as EnemyAIBehaviourStateMachine;
        _enemyBody = GetComponent<Body>();
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

        foreach(GameObject ui in _hideUIArrayWhenDead)
        {
            ui.SetActive(false);
        }
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

            foreach(GameObject ui in _hideUIArrayWhenDead)
            {
                ui.SetActive(false);
            }
        }
        _enemyAIStateMachine.UnsubscribeEvent();
        _enemyBody.ToggleBodyPartsCollider(false);
        _enemyAIStateMachine.Agent.enabled = false;
        _charaMakeSFX.UnsubscribeEvent();
        // _enemyBody.MakeItTransparent(_enemyDeathFadeDuration);
        Destroy(this.gameObject, _enemyDeathFadeDuration + 0.1f);
    }

    public void GotSilentKill(PlayableCharacterIdentity characterIdentityWhoKilling)
    {
        if(_isSilentKilled) return;
        
        _silentKillAnimationIdx = characterIdentityWhoKilling.SilentKillIdx;        
        _animator.SetFloat(ANIMATION_PARAMETER_SILENTKILLCOUNTER, _silentKillAnimationIdx);

        characterIdentityWhoKilling.GetPlayableMovementStateMachine.ForceStopMoving();
        characterIdentityWhoKilling.GetPlayableUseWeaponStateMachine.SetSilentKilledEnemy(this);
        characterIdentityWhoKilling.IsSilentKilling = true;
        _isSilentKilled = true;
        _canBeKill = false;
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

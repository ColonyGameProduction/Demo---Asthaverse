using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterUIManager : MonoBehaviour
{
    [SerializeField] private DamageUIHandler _dmgUIHandler;
    private PlayableCharacterIdentity currPlayable;
    private WeaponLogicManager _weaponLogicManager;
    private PlayableCharacterManager _playableCharacterManager;
    private EnemyAIManager _enemyAIManager;

    private void Awake() 
    {
        _playableCharacterManager = GetComponent<PlayableCharacterManager>();
    }
    private void Start() 
    {
        _weaponLogicManager = WeaponLogicManager.Instance;
        _weaponLogicManager.OnCharacterGotHurt += WeaponLogicManager_OnCharacterGotHurt;

        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnEnemyDead += _dmgUIHandler.StoreArrowBasedOnShooter;
    }
    public void ConnectUIHandler(PlayableCharacterIdentity curr)
    {
        currPlayable = curr;

        currPlayable.OnPlayableHealthChange += _dmgUIHandler.SetDamageVisualPhase;
    }
    public void DisconnectUIHandler()
    {
        if(currPlayable == null)
        {
            ResetUI();
            return;
        }

        currPlayable.OnPlayableHealthChange -= _dmgUIHandler.SetDamageVisualPhase;

        currPlayable = null;
        ResetUI();
    }

    public void ResetUI()
    {
        _dmgUIHandler.ResetDamageVisual();
    }

    private void WeaponLogicManager_OnCharacterGotHurt(Transform shooter, CharacterIdentity character)
    {
        if(character.transform == _playableCharacterManager.CurrPlayableChara.transform)
        {
            _dmgUIHandler.CallArrow(shooter);
        }
    }
}

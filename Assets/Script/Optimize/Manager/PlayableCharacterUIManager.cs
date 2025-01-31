
using UnityEngine;

public class PlayableCharacterUIManager : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    private PlayableCharacterIdentity currPlayable;
    private WeaponLogicManager _weaponLogicManager;
    private PlayableCharacterManager _playableCharacterManager;
    private EnemyAIManager _enemyAIManager;
    [SerializeField] private DamageUIHandler _dmgUIHandler;
    [SerializeField] private CharacterProfileUIHandler _characterProfileUIHandler;
    [SerializeField] private ControlsManager _controlsManager;
    [Space(1)]
    [SerializeField] private GameObject[] _uiContainersCloseWhenEvent;
    
    public CharacterProfileUIHandler GetCharacterProfileUIHandler {get {return _characterProfileUIHandler;}}

    private void Awake() 
    {
        _playableCharacterManager = GetComponent<PlayableCharacterManager>();
        _playableCharacterManager.OnPlayerSwitch += _characterProfileUIHandler.UpdateHealthData;

        _controlsManager.OnAimModeChange += Settings_OnScopeModeChange;
        _controlsManager.OnSprintModeChange += Settings_OnSprintModeChange;
        _controlsManager.OnCrouchModeChange += Settings_OnCrouchModeChange;
    }



    private void Start() 
    {
        _weaponLogicManager = WeaponLogicManager.Instance;
        _weaponLogicManager.OnCharacterGotHurt += WeaponLogicManager_OnCharacterGotHurt;

        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnEnemyDead += _dmgUIHandler.StoreArrowBasedOnShooter;

        _gm = GameManager.instance;
        _gm.OnChangeGamePlayModeToNormal += ShowUIAfterEvent;
        _gm.OnChangeGamePlayModeToEvent += HideUIWhenEvent;
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

    private void Settings_OnScopeModeChange(bool change)
    {
        _playableCharacterManager.IsScopeModeHold = change;
    }

    private void Settings_OnSprintModeChange(bool change)
    {
        _playableCharacterManager.IsRunModeHold = change;
    }
    private void Settings_OnCrouchModeChange(bool change)
    {
        _playableCharacterManager.IsCrouchModeHold = change;
    }

    private void HideUIWhenEvent()
    {
        foreach(GameObject go in _uiContainersCloseWhenEvent)
        {
            go.SetActive(false);
        }
    }
    private void ShowUIAfterEvent()
    {
        foreach(GameObject go in _uiContainersCloseWhenEvent)
        {
            go.SetActive(true);
        }
    }

    public void UnsubscribeEvent()
    {
        _playableCharacterManager.OnPlayerSwitch -= _characterProfileUIHandler.UpdateHealthData;

        _controlsManager.OnAimModeChange -= Settings_OnScopeModeChange;
        _controlsManager.OnSprintModeChange -= Settings_OnSprintModeChange;
        _controlsManager.OnCrouchModeChange -= Settings_OnCrouchModeChange;
        _gm.OnChangeGamePlayModeToNormal -= ShowUIAfterEvent;
        _gm.OnChangeGamePlayModeToEvent -= HideUIWhenEvent;
    }
}

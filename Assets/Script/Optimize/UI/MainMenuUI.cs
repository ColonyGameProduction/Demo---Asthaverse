using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour, IUnsubscribeEvent
{
    private AudioManager _am;
    [ReadOnly(false), SerializeField] private bool _isClickingStartExitButton;
    [ReadOnly(false), SerializeField] private bool _isAnimatingButton;
    private SettingUIHandler _settingUIHandler;
    [SerializeField] private GameObject _mainMenuContainer;
    [SerializeField] private float _delayButtonAnimation = 1f;
    private FadeBGUIHandler _fadeUIHandler;
    [SerializeField] private Color _startGameBGColor;
    private SceneManagementManager _sceneManagementManager;
    private PlayerActionInput _inputActions;
    
    private Button[] _buttons;
    

    private void Awake() 
    {   
        _inputActions = new PlayerActionInput();  
        _inputActions.InputMenuAction.ExitSettingsMainMenu.performed += OnExitSettings_performed;

        _settingUIHandler = GetComponentInChildren<SettingUIHandler>();
        _settingUIHandler.HideSettingsUI();

        _inputActions.Enable();

        _buttons = _mainMenuContainer.GetComponentsInChildren<Button>();
    }

    private void Start() 
    {
        _fadeUIHandler = FadeBGUIHandler.Instance;
        _sceneManagementManager = SceneManagementManager.Instance;

        _am = AudioManager.Instance;
        foreach(Button button in _buttons)
        {
            button.onClick.AddListener(_am.PlayUIClick);
        }
    }


    private bool CanClickButton()
    {
        if(_isAnimatingButton || _isClickingStartExitButton) return false;

        return true;
    }
    public void LoadLevelButton(string levelToLoad)
    {
        if(!CanClickButton()) return;

        _isClickingStartExitButton = true;
        _sceneManagementManager.SaveLoadSceneName(levelToLoad);
        StartCoroutine(DelayButtonAnimation(StartGame));
    }

    public void ExitButton()
    {
        if(!CanClickButton()) return;

        _isClickingStartExitButton = true;
        StartCoroutine(DelayButtonAnimation(_sceneManagementManager.ExitGame));
    }

    public void SettingsButton()
    {
        if(!CanClickButton()) return;

        StartCoroutine(DelayButtonAnimation(OpenSettings));
    }

    private IEnumerator DelayButtonAnimation(Action action)
    {
        _isAnimatingButton = true;
        yield return new WaitForSeconds(_delayButtonAnimation);
        _isAnimatingButton = false;

        action();
    }
    private void StartGame()
    {
        _fadeUIHandler.ChangeBGColor(_startGameBGColor);
        _sceneManagementManager.GoToOtherScene();
        Debug.Log("I start the game - go to loading screen");
    }
    private void OpenSettings()
    {
        _settingUIHandler.ShowSettingsUI();
        _mainMenuContainer.SetActive(false);
    }
    private void CloseSettings()
    {
        _mainMenuContainer.SetActive(true);
        _settingUIHandler.HideSettingsUI();
    }
    private void OnExitSettings_performed(InputAction.CallbackContext context)
    {
        CloseSettings();
    }

    public void UnsubscribeEvent()
    {
        _inputActions.InputMenuAction.ExitSettingsMainMenu.performed -= OnExitSettings_performed;
    }
}


using UnityEngine;

public class PauseUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    [SerializeField] private GameObject _pauseUIContainer;
    private SceneManagementManager _sceneManagementManager;
    private SettingUIHandler _settingsUIHandler;
    private void Awake() 
    {
        _settingsUIHandler = GetComponent<SettingUIHandler>();

        _pauseUIContainer.SetActive(false);
    }

    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnPlayerPause += TogglePauseUI;
        _gm.OnQuitSettings += CloseSettingsUI;
        _sceneManagementManager = SceneManagementManager.Instance;
    }

    #region  Pause UI
    private void TogglePauseUI(bool change)
    {
        if(change) OpenPauseUI();
        else ClosePauseUI();
    }
    private void OpenPauseUI()
    {
        _pauseUIContainer.SetActive(true);
    }
    public void ClosePauseUI()
    {
        _pauseUIContainer.SetActive(false);
    }
    #endregion

    public void OpenSettingsUI()
    {
        _gm.OpenSettingsMenu();
        _settingsUIHandler.ShowSettingsUI();
        ClosePauseUI();
    }
    private void CloseSettingsUI()
    {
        OpenPauseUI();
        _settingsUIHandler.HideSettingsUI();
        _gm.OpenPauseMenu();
    }

    public void GoBackToMainMenu()
    {
        _sceneManagementManager.SaveLoadSceneName("Main Menu");
        _sceneManagementManager.GoToOtherScene();
    }

    public void UnsubscribeEvent()
    {
        _gm.OnPlayerPause -= TogglePauseUI;
        _gm.OnQuitSettings -= CloseSettingsUI;
    }
}

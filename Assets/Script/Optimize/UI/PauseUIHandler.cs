
using UnityEngine;

public class PauseUIHandler : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private GameObject _pauseUIContainer;
    [SerializeField] private GameObject _settingsUIContainer;
    private SettingUIHandler _settingsUIHandler;
    private void Awake() 
    {
        _settingsUIHandler = GetComponent<SettingUIHandler>();

        _pauseUIContainer.SetActive(false);
        _settingsUIContainer.SetActive(false);
    }

    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnPlayerPause += TogglePauseUI;
        _gm.OnQuitSettings += CloseSettingsUI;
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

    #region  Settings UI
    public void OpenSettingsUI()
    {
        _gm.OpenSettingsMenu();
        _settingsUIContainer.SetActive(true);
        ClosePauseUI();
    }
    private void CloseSettingsUI()
    {
        OpenPauseUI();
        _settingsUIContainer.SetActive(false);
        _gm.OpenPauseMenu();
    }
    #endregion
}


using UnityEngine;

public class PauseUIHandler : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private GameObject _pauseUIContainer;
    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnPlayerPause += TogglePauseUI;
    }

    private void TogglePauseUI(bool change)
    {
        if(change) ShowPauseUI();
        else HidePauseUI();
    }
    private void ShowPauseUI()
    {
        _pauseUIContainer.SetActive(true);
    }
    public void HidePauseUI()
    {
        _pauseUIContainer.SetActive(false);
    }
}

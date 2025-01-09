using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    private AudioManager _am;
    [SerializeField] private GameObject _gameOverContainer, _buttonsContainer;
    [SerializeField] private CanvasGroup _gameOverImageCanvasGroup;
    [SerializeField] private float _showDuration = 0.5f;
    private Button[] _buttons;
    private void Awake() 
    {
        _buttons = _buttonsContainer.GetComponentsInChildren<Button>();

        HideGameOverUI();
    }
    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnGameOver += ShowGameOverUI;

        _am = AudioManager.Instance;
        foreach(Button button in _buttons)
        {
            button.onClick.AddListener(_am.PlayUIClick);
        }
    }

    public void ShowGameOverUI()
    {
        _gameOverContainer.gameObject.SetActive(true);
        LeanTween.alphaCanvas(_gameOverImageCanvasGroup, 1, _showDuration).setOnComplete(
            ()=>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _buttonsContainer.SetActive(true);
            }
        );
    }
    public void HideGameOverUI()
    {
        _gameOverContainer.gameObject.SetActive(false);
        _gameOverImageCanvasGroup.alpha = 0f;
        _buttonsContainer.SetActive(false);
    }

    public void UnsubscribeEvent()
    {
        _gm.OnGameOver -= ShowGameOverUI;
    }
}

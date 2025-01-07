using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    [SerializeField] private GameObject _gameOverContainer, _buttonsContainer;
    [SerializeField] private CanvasGroup _gameOverImageCanvasGroup;
    [SerializeField] private float _showDuration = 0.5f;
    private void Awake() 
    {
        HideGameOverUI();
    }
    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnGameOver += ShowGameOverUI;
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

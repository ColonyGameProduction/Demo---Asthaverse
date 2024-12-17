using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUIHandler : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private CanvasGroup _gameOverContainer;
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
        LeanTween.alphaCanvas(_gameOverContainer, 1, _showDuration).setOnComplete(
            ()=>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }
        );
    }
    public void HideGameOverUI()
    {
        _gameOverContainer.gameObject.SetActive(false);
        _gameOverContainer.alpha = 0f;
    }
}

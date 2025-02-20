using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThankFinishUI : MonoBehaviour
{
    private SceneManagementManager SMM;
    private AudioManager _am;
    [SerializeField] private GameObject _textContainer;
    private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 2f;
    [SerializeField] private float _fadeWaitTimer = 1.5f;
    private void Awake()
    {
        _canvasGroup = _textContainer.GetComponent<CanvasGroup>();
        ToggleThanksFinishUI(0, 0, 0, null);
    }
    private void Start()
    {
        SMM = SceneManagementManager.Instance;
        _am = AudioManager.Instance;
    }
    public void ShowThanks(Action action)
    {
        _am.ChangeBGMMidGame(AudioBGMName.MainMenu);
        LeanTween.value(0, 1, _fadeWaitTimer).setOnComplete(
            ()=>
            {
                ToggleThanksFinishUI(0, 1, _fadeInDuration, ()=>{HideTimer(action);});
            }
        );
        
    }
    private void HideTimer(Action action)
    {
        LeanTween.value(0, 1, _fadeWaitTimer).setOnComplete(
            ()=>
            {
                ToggleThanksFinishUI(1, 0, _fadeOutDuration, action);
            }
        );
    }
    private void ToggleThanksFinishUI(float from, float to, float duration, Action action)
    {
        if(to > 0) _textContainer.gameObject.SetActive(true);
        LeanTween.value(from, to, _fadeInDuration).setOnUpdate((float value) =>
            {
                _canvasGroup.alpha = value;
            }
        ).setOnComplete(()=>
            {
                if(to == 0) _textContainer.gameObject.SetActive(false);
                if(action != null) action();
            }
        );

    }
}

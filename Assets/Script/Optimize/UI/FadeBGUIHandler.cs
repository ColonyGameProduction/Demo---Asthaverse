using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeBGUIHandler : MonoBehaviour
{
    public static FadeBGUIHandler Instance {get; private set;}
    [SerializeField] private CanvasGroup _fadeBG;
    private Image _fadeImage;
    [SerializeField] private float _fadeOutOnStartDuration = 0.5f;
    [SerializeField] private float _fadeInEndOfSceneDuration = 0.5f;

    [Header("False - if in Game or Want it to do something after it fades")]
    [SerializeField] private bool _canFadeOutOnStartOnItsOwn; 

    private void Awake() 
    {
        Instance = this;

        _fadeImage = GetComponentInChildren<Image>();
        _fadeBG = GetComponentInChildren<CanvasGroup>();
        _fadeBG.alpha = 1f;
    }
    private void Start() 
    {
        if(_canFadeOutOnStartOnItsOwn)
        {
            StartCoroutine(HideBGOnStart(null));
        }
    }

    public void ChangeBGColor(Color color)
    {
        _fadeImage.color = color;
    }

    public void HideBGStart(Action action)
    {
        if(_canFadeOutOnStartOnItsOwn) return;

        StartCoroutine(HideBGOnStart(action));
    }
    public void ShowBGAtEnd(Action action)
    {
        // Debug.Log("Haloo2??");
        FadeBG(1, _fadeInEndOfSceneDuration, action);
    }
    private IEnumerator HideBGOnStart(Action action)
    {
        yield return new WaitForEndOfFrame();

        FadeBG(0, _fadeOutOnStartDuration, action);
    }

    public void FadeBG(float to, float duration, Action action)
    {
        if(to > 0)
        {
            if(!_fadeBG.gameObject.activeSelf) _fadeBG.gameObject.SetActive(true);
            LeanTween.cancel(_fadeBG.gameObject);
            LeanTween.alphaCanvas(_fadeBG, to, duration).setOnComplete(
                ()=>
                {
                    // Debug.Log("Donee");
                    if(action != null) action();
                }
            );
        }
        else
        {
            LeanTween.cancel(_fadeBG.gameObject);
            LeanTween.alphaCanvas(_fadeBG, to, duration).setOnComplete(
                ()=>
                {
                    _fadeBG.gameObject.SetActive(false);
                    if(action != null) action();
                }
            );
        }
    }
}

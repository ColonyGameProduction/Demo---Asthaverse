using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBGUIHandler : MonoBehaviour
{
    public static FadeBGUIHandler Instance {get; private set;}
    [SerializeField] private CanvasGroup _fadeBG;

    private void Awake() 
    {
        Instance = this;
    }

    public void FadeBG(float to, float duration, Action action)
    {
        if(to > 0)
        {
            if(!_fadeBG.gameObject.activeSelf) _fadeBG.gameObject.SetActive(true);
            LeanTween.alphaCanvas(_fadeBG, to, duration).setOnComplete(
                ()=>
                {
                    if(action != null) action();
                }
            );
        }
        else
        {
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

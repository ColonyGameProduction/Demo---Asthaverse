using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject _holdInteractContainer;
    [SerializeField] private Image _holdFillImage;
    private HoldableObj_IntObj _currObj;
    public static Action<HoldableObj_IntObj> OnStartHoldInteracting, OnStopHoldInteracting;
    private void Awake() 
    {
        HideHoldInteractUI();
        OnStartHoldInteracting += StartHoldInteracting;
        OnStopHoldInteracting += StopHoldInteracting;
    }
    private void StartHoldInteracting(HoldableObj_IntObj obj)
    {
        _currObj = obj;
        ShowHoldInteractUI();
        obj.OnValueChange += ChangeFillVisual;
    }
    private void StopHoldInteracting(HoldableObj_IntObj obj)
    {
        if(_currObj != obj) return;
        obj.OnValueChange -= ChangeFillVisual;
        HideHoldInteractUI();
    }
    private void ChangeFillVisual(float fillValue)
    {
        fillValue = Mathf.Clamp01(fillValue);
        _holdFillImage.fillAmount = fillValue;

        if(fillValue == 1) HideHoldInteractUI();
    }
    public void ShowHoldInteractUI()
    {
        _holdFillImage.fillAmount = 0;
        _holdInteractContainer.SetActive(true);
    }
    private void HideHoldInteractUI()
    {
        _holdInteractContainer.SetActive(false);
    }
}

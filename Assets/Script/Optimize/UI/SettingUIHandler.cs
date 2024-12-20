using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingUIHandler : MonoBehaviour
{
    [Header("Sesuai urutan di UI")]
    [SerializeField] private GameObject[] _tabs;
    [SerializeField] private GameObject[] _buttonHighlights;
    [SerializeField] private TextMeshProUGUI[] _buttonTexts;
    
    [SerializeField] private float activeAlphaText = 1f;
    [SerializeField] private float inActiveAlphaText = 0.3f;
    private void Awake() 
    {
        SwitchTab(0);
    }

    public void SwitchTab(int pageNo)
    {
        for(int i=0; i<_tabs.Length; i++)
        {
            _tabs[i].SetActive(i==pageNo);
            _buttonHighlights[i].SetActive(i==pageNo);
            _tabs[i].SetActive(i==pageNo);
            SetTextAlpha(_buttonTexts[i], i==pageNo ? activeAlphaText : inActiveAlphaText);
        }
    }
    private void SetTextAlpha(TextMeshProUGUI buttonText, float alphaValueText)
    {
        if (buttonText != null)
        {
            Color newTextColor = buttonText.color;
            newTextColor.a = Mathf.Clamp01(alphaValueText);
            buttonText.color = newTextColor;
        }
    }


}

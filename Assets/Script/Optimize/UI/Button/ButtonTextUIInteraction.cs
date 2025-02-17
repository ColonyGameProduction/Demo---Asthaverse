using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonTextUIInteraction : ButtonUIInteraction
{
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private float _highlightedAlpha = 1f;
    [SerializeField] private float _normalAlpha = 0.1f;
    public override void ToggleHighlight(bool change)
    {
        _highlightContainer.SetActive(change);
        SetTextAlpha(change ? _highlightedAlpha : _normalAlpha);
    }
      
    protected virtual void SetTextAlpha(float alpha)
    {
        Color color = _buttonText.color;
        color.a = alpha;
        _buttonText.color = color;
    }
}

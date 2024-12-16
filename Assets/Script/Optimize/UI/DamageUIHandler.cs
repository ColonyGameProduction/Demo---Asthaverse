using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIHandler : MonoBehaviour
{
    [Header("Damage Visual Phase UI")]
    [SerializeField] private GameObject _dmgVisualContainer;
    [SerializeField] private Image  _dmgVisualPhase2;
    [SerializeField] private Image  _dmgVisualPhase3, _dmgVisualPhase4;

    [Header("Damage Direction Indicator UI")]
    [SerializeField] private GameObject _dmgDirContainer;
    [SerializeField] private GameObject _arrowIndicator;
    [SerializeField] private GameObject _upBloodDamage, _rightBloodDamage, _leftBloodDamage, _bottomBloodDamage;

    // [ReadOnly(false), SerializeField] private Transform _currPlayable;

    private void Awake() 
    {
        ResetDamageVisual();
        if(!_dmgVisualContainer.activeSelf)_dmgVisualContainer.SetActive(true);
    }
    

    public void SetDamageVisualPhase(float currHP, float maxHP)
    {
        float healthDifference = maxHP - currHP;

        float imageAlphaPhase2 = (healthDifference - (maxHP/4)) / (maxHP / 4);
        ChangeImageAlphaValue(_dmgVisualPhase2, imageAlphaPhase2);

        float imageAlphaPhase3 = (healthDifference - (maxHP / 2)) / (maxHP / 4);
        ChangeImageAlphaValue(_dmgVisualPhase3, imageAlphaPhase3);

        float imageAlphaPhase4 = (healthDifference - ((maxHP / 4) * 3)) / (maxHP / 4);
        ChangeImageAlphaValue(_dmgVisualPhase4, imageAlphaPhase4);
        
    }

    public void ResetDamageVisual()
    {
        ChangeImageAlphaValue(_dmgVisualPhase2, 0);
        ChangeImageAlphaValue(_dmgVisualPhase3, 0);
        ChangeImageAlphaValue(_dmgVisualPhase4, 0);
    }

    private void ChangeImageAlphaValue(Image chosen, float to)
    {
        to = Mathf.Clamp01(to);

        Color colorChosen = chosen.color;
        colorChosen.a = to;
        chosen.color = colorChosen;
    }

}

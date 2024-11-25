using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCanvasHandler : MonoBehaviour
{
    [Range(0f, 1f)]
    public float testHP;

    public float maxHP = 1f;
    public float currentValue;

    public float tempPhase2Value;
    public float tempPhase3Value;
    public float tempPhase4Value;

    public Image phase2;
    public Image phase3;
    public Image phase4;

    private void Update()
    {
        DamageVisual();
    }

    public void DamageVisual()
    {
        currentValue = maxHP - testHP;

        if(currentValue <= maxHP/2 && currentValue >= maxHP/4)
        {
            tempPhase2Value = (currentValue - 0.25f) / 0.25f;

            Color color = phase2.color;
            color.a = tempPhase2Value;
            phase2.color = color;

        }
        else if(currentValue >= maxHP/2 && currentValue <= (maxHP/4) * 3)
        {
            tempPhase3Value = (currentValue - 0.5f) / 0.25f;
            tempPhase2Value = 1;

            Color colorPhase2 = phase2.color;
            colorPhase2.a = tempPhase2Value;
            phase2.color = colorPhase2;

            Color colorPhase3 = phase3.color;
            colorPhase3.a = tempPhase3Value;
            phase3.color = colorPhase3;

        }
        else if(currentValue >= (maxHP/4) * 3)
        {
            tempPhase4Value = (currentValue - 0.75f) / 0.25f;
            tempPhase3Value = 1;

            Color colorPhase3 = phase3.color;
            colorPhase3.a = tempPhase3Value;
            phase3.color = colorPhase3;

            Color colorPhase4 = phase4.color;
            colorPhase4.a = tempPhase4Value;
            phase4.color = colorPhase4;

        }
        else if(currentValue <= maxHP/4)
        {
            tempPhase2Value = 0;
            tempPhase3Value = 0;
            tempPhase4Value = 0;

            Color colorPhase2 = phase2.color;
            Color colorPhase3 = phase3.color;
            Color colorPhase4 = phase4.color;

            colorPhase2.a = tempPhase2Value;
            colorPhase3.a = tempPhase3Value;
            colorPhase4.a = tempPhase4Value;

            phase2.color = colorPhase2;
            phase3.color = colorPhase3;
            phase4.color = colorPhase4;
        }
        
    }

    public void DamageDirectionIndicator()
    {

    }


}

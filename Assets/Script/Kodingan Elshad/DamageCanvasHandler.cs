using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCanvasHandler : MonoBehaviour
{
    [Header ("Damage Visual Phase")]

    public float currHP;
    public float maxHP;
    public float currentValue;

    public float tempPhase2Value;
    public float tempPhase3Value;
    public float tempPhase4Value;

    public Image phase2;
    public Image phase3;
    public Image phase4;

    [Header("Damage Direction Indicator")]

    public GameObject damageCanvas;
    public GameObject ArrowIndicator;
    public GameObject upBloodDamage;
    public GameObject rightBloodDamage;
    public GameObject leftBloodDamage;
    public GameObject bottomBloodDamage;

    public Transform player;

    public float stayTime;
    public float fadeTime;

    private void Update()
    {
        DamageVisual();
        //DamageDirectionArrowIndicator();
    }

    public void DamageVisual()
    {

        currentValue = maxHP - currHP;

        if(currentValue <= maxHP/2 && currentValue >= maxHP/4)
        {
            tempPhase2Value = (currentValue - (maxHP/4)) / (maxHP / 4);

            Color color = phase2.color;
            color.a = tempPhase2Value;
            phase2.color = color;

        }
        else if(currentValue >= maxHP/2 && currentValue <= (maxHP/4) * 3)
        {
            tempPhase3Value = (currentValue - (maxHP / 2)) / (maxHP / 4);
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
            tempPhase4Value = (currentValue - ((maxHP / 4) * 3)) / (maxHP / 4);
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

    public void DamageDirectionArrowIndicator(ArrowData arrow)
    {
        arrow.stayTime = stayTime;
        arrow.fadeTime = fadeTime;
        Vector3 damageLocation = arrow.whoShootMe.transform.position;

        damageLocation.y = player.position.y;
        Vector3 flatForward = player.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 direction = (damageLocation - player.position).normalized;

        float angle = (Vector3.SignedAngle(direction, flatForward, Vector3.up));
        arrow.arrow.transform.localEulerAngles = new Vector3(0, 0, angle);

        if(angle >= -45 && angle <= 45)
        {
            upBloodDamage.GetComponent<CanvasGroup>().alpha = 1;
            LeanTween.delayedCall(stayTime, () =>
            {
                LeanTween.alphaCanvas(upBloodDamage.GetComponent<CanvasGroup>(), 0, fadeTime);
            });
        }
        else if(angle >= 45 && angle <= 135)
        {
            leftBloodDamage.GetComponent<CanvasGroup>().alpha = 1;
            LeanTween.delayedCall(stayTime, () =>
            {
                LeanTween.alphaCanvas(leftBloodDamage.GetComponent<CanvasGroup>(), 0, fadeTime);
            });
        }
        else if(angle >= 135 || angle <= -135)
        {
            bottomBloodDamage.GetComponent<CanvasGroup>().alpha = 1;
            LeanTween.delayedCall(stayTime, () =>
            {
                LeanTween.alphaCanvas(bottomBloodDamage.GetComponent<CanvasGroup>(), 0, fadeTime);
            });
        }
        else if(angle <= -45 && angle >= -135)
        {
            rightBloodDamage.GetComponent<CanvasGroup>().alpha = 1;
            LeanTween.delayedCall(stayTime, () =>
            {
                LeanTween.alphaCanvas(rightBloodDamage.GetComponent<CanvasGroup>(), 0, fadeTime);
            });
        }
    }

    public void DamageArrow(ArrowData arrow)
    {
        Vector3 damageLocation = arrow.whoShootMe.transform.position;

        damageLocation.y = player.position.y;

        Vector3 flatForward = player.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 direction = (damageLocation - player.position).normalized;

        float angle = (Vector3.SignedAngle(direction, flatForward, Vector3.up));
        arrow.arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}

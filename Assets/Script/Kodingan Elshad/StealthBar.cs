using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StealthBar : MonoBehaviour
{
    public Image fillBarImage;
    public GameObject imageContainer;

    private void Start()
    {
        imageContainer.gameObject.SetActive(false);
    }

    public void FillingTheImage(float currStealth, float MaxStealth)
    {
        fillBarImage.fillAmount = currStealth / MaxStealth;

        if(fillBarImage.fillAmount <= 0)
        {
            imageContainer.gameObject.SetActive(false);
        }
        else if(fillBarImage.fillAmount > 0 && fillBarImage.fillAmount < 1)
        {
            imageContainer.gameObject.SetActive(true);
        }
        else if(fillBarImage.fillAmount >= 1)
        {
            imageContainer.gameObject.SetActive(false);
        }

    }
}

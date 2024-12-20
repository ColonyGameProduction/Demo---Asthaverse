using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HoldButtonInteract : MonoBehaviour
{
    public float curValue = 0;
    public float valueSpd = 5;
    public float maxValue = 10;

    public bool isComplete;
    public bool holdingButton;

    private void Update()
    {
        if (holdingButton)
        {
            if (curValue < maxValue)
            {
                curValue += Time.deltaTime * valueSpd;
            }
            else
            {
                isComplete = true;
            }
        }
    }

    public void HoldInteraction(bool holdButton)
    {
        if(!isComplete)
        {
            curValue = 0;
            holdingButton = holdButton;
        }   
    }
}

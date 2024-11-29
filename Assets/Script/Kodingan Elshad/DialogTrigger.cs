using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    public DialogCutsceneSO dialog;
    public InGameUIHandler inGameUIHandler;
    bool isActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isActivate)
            {
                inGameUIHandler.dialogCutscene = dialog;
                inGameUIHandler.DialogPlay();
                isActivate = true;
            }
        }
    }

}

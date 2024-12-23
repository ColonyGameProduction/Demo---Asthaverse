using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInGameTrigger : MonoBehaviour
{
    private DIalogInGameManager _dialogInGameManager;
    [SerializeField] private DialogCutsceneTitle _chosenTitleToPlay;
    private bool _isActivate;
    private bool _isTestAfterDialogDone;
    [SerializeField] private bool _canOnlyBeActivateOnce;
    private void Start() 
    {
        _dialogInGameManager = DIalogInGameManager.Instance;
        _dialogInGameManager.OnDialogFinish += Manager_OnDialogFinish;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponentInParent<PlayableCharacterIdentity>().IsPlayerInput)
            {
                if (!_isActivate)
                {
                    _dialogInGameManager.PlayDialogCutscene(_chosenTitleToPlay);
                    _isActivate = true;
                }
            }
        }
    }
    private void Manager_OnDialogFinish(DialogCutsceneTitle title)
    {
        if(title != _chosenTitleToPlay) return;
        
        if(!_canOnlyBeActivateOnce) _isActivate = false;

        if(!_isTestAfterDialogDone)
        {
            _isTestAfterDialogDone = true;
            Debug.Log(this.transform.name + " DONE");
        }
    }
}

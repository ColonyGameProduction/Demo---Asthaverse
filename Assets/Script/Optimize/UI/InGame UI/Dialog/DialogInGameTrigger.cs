using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogInGameTrigger : MonoBehaviour, IUnsubscribeEvent, IConnectToQuest
{
    private DIalogInGameManager _dialogInGameManager;
    [SerializeField] private DialogCutsceneTitle _chosenTitleToPlay;
    private bool _isActivate;
    private bool _isTestAfterDialogDone, _canTriggerOnce;
    [SerializeField] private bool _canOnlyBeActivateOnce;
    public Action<DialogCutsceneTitle> OnPlayerTriggerOnce;
    public UnityEvent OnTriggerEventEventFromOutside;
    public event Action OnTriggerQuestComplete;

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
                    _dialogInGameManager.PlayDialogCutsceneTitle(_chosenTitleToPlay);
                    _isActivate = true;

                    if(!_canTriggerOnce)
                    {
                        _canTriggerOnce = true;
                        OnPlayerTriggerOnce?.Invoke(_chosenTitleToPlay);
                    }
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
            OnTriggerQuestComplete?.Invoke();
            OnTriggerEventEventFromOutside?.Invoke();
        }
    }

    public void UnsubscribeEvent()
    {
        _dialogInGameManager.OnDialogFinish -= Manager_OnDialogFinish;
    }
}

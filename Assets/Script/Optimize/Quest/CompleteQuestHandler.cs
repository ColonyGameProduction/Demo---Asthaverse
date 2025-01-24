using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class CompleteQuestHandler : TriggerQuestHandler
{
    [SerializeField] private PauseUIHandler _pauseUIHandler;
    public override void EndedCallQuestContainerUI()
    {
        // Debug.Log("Load the Cutscene");

        GameManager.instance.GameCompleted();
        _pauseUIHandler.GoBackToMainMenu();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

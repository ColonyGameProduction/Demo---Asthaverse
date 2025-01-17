using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloQuestHandler : QuestHandlerParent
{
    // [SerializeField] protected bool _isOptional;
    [SerializeField] protected GameObject[] _checkerInMaps;
    public Action OnChangeDescBeforeComplete;
    public Action<QuestName, SoloQuestHandler> OnSoloQuestComplete;
    
    #region  Getter Setter
    // public bool IsOptional {get {return _isOptional;}}

    #endregion

    public override void ActivateQuest()
    {
        _isActivated = true;
        ToggleCheckerInMap(true);
    }
    public override void DeactivateQuest()
    {
        _isActivated = false;
        ToggleCheckerInMap(false);
    }
    public void ActivateQuest_isQuestShownAfterOtherQuest()
    {
        CallQuestContainerUI();
        ActivateQuest();
    }
    public override void CallQuestContainerUI()
    {
        _questGameUIHandler.CallQuestContainer(this, false, false);
    }

    public override void EndedCallQuestContainerUI()
    {
        _questGameUIHandler.HideCompletedQuestContainer(this);
    }
    protected override void HandleQuestComplete()
    {
        base.HandleQuestComplete();
        OnSoloQuestComplete?.Invoke(QuestName, this);
    }
    public virtual void ToggleCheckerInMap(bool isActivate)
    {
        foreach(GameObject checker in _checkerInMaps)
        {
            checker.SetActive(isActivate);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloQuestHandler : QuestHandlerParent
{
    [SerializeField] private bool _isOptional;
    public Action OnChangeDescBeforeComplete;
    public Action<QuestName, bool> OnSoloQuestComplete;
    
    #region  Getter Setter
    public bool IsOptional {get {return _isOptional;}}
    #endregion

    public override void CallQuestContainerUI()
    {
        _questGameUIHandler.CallQuestContainer(this);
    }

    public override void EndedCallQuestContainerUI()
    {
        _questGameUIHandler.HideCompletedQuestContainer(this);
    }
    protected override void QuestComplete()
    {
        base.QuestComplete();
        OnSoloQuestComplete?.Invoke(QuestName, _isOptional);
    }
}

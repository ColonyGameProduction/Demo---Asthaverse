using System;

using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class MultipleQuest
{
    public MultipleQuest(SoloQuestHandler quest)
    {
        soloQuestHead = quest;
        questName = quest.QuestName;
        questDescRaw = quest.QuestDesc;
        totalQuest = 1;
    }

    public SoloQuestHandler[] soloQuestsSameTitle;
    public bool isQuestShownAfterOtherQuest;
    [Tooltip("Change this if isQuestShownAfterOtherQuest = true")]
    public int otherMultipleQuestArrayIndexToShowThisQuestIdx;
    public bool isOptional;
    /// <summary>
    /// this just for connecting it with ui container
    /// </summary>
    [ReadOnly(false)] public SoloQuestHandler soloQuestHead;
    [ReadOnly(false)] public QuestName questName;
    [ReadOnly(false)] public string questDescRaw;
    [ReadOnly(false)] public int totalQuest = 1;
    [ReadOnly(false)] public int currTotalQuestCompleted;
    [HideInInspector] public QuestGameUIHandler questGameUIHandler;
    [HideInInspector] public MultipleQuestHandler multipleQuestHandler;
    public Action OnMultipleQuestCompleted; // JANGAN LUPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
    public void SetData()
    {
        soloQuestHead = soloQuestsSameTitle[0];
        questName = soloQuestHead.QuestName;
        questDescRaw = soloQuestHead.QuestDesc;
        currTotalQuestCompleted = 0;
        totalQuest = soloQuestsSameTitle.Length;
        ChangeDesc();
    }
    public void ChangeDesc()
    {
        if(totalQuest == 1)
        {
            if(currTotalQuestCompleted == totalQuest)
            {
                QuestGameUIHandler.Instance.HideCompletedQuestContainer(soloQuestHead);
                OnMultipleQuestCompleted?.Invoke(); 
            }
            return;
        }

        string questDescFinal = questDescRaw + " (" + currTotalQuestCompleted + "/" + totalQuest +")";
        soloQuestHead.QuestDesc = questDescFinal;
        soloQuestHead.OnChangeDescBeforeComplete?.Invoke();

        if(totalQuest == currTotalQuestCompleted)
        {
            QuestGameUIHandler.Instance.HideCompletedQuestContainer(soloQuestHead);
            OnMultipleQuestCompleted?.Invoke();
        }
    }

    public void AddTotalQuest()
    {
        totalQuest++;
        ChangeDesc();
    }
    public void AddCurrTotalQuestCompleted()
    {
        if(currTotalQuestCompleted < totalQuest)currTotalQuestCompleted++;
        ChangeDesc();
    }
    public void ActivateAllQuest_IsQuestShownAfterOtherQuest()
    {
        if(soloQuestHead.IsActivated || multipleQuestHandler.IsCompleted) return;

        CallQuestContainerUI();
        ActivateAllQuest();
    }
    public void ActivateAllQuest()
    {
        foreach(SoloQuestHandler quest in soloQuestsSameTitle)
        {
            quest.ActivateQuest();
        }
    }
    public void DeactivateAllQuest()
    {
        foreach(SoloQuestHandler quest in soloQuestsSameTitle)
        {
            quest.DeactivateQuest();
        }
    }
    public void CallQuestContainerUI()
    {
        questGameUIHandler.CallQuestContainer(soloQuestHead, false, isOptional);
    }
    public void EndedCallQuestContainerUI()
    {
        soloQuestHead.EndedCallQuestContainerUI();
    }
    public bool IsFromThisMultipleQuestData(SoloQuestHandler soloQuest)
    {
        foreach(SoloQuestHandler quest in soloQuestsSameTitle)
        {
            if(quest == soloQuest) return true;
        }
        return false;
    }
    public bool IsAllQuestCompleted()
    {
        if(isOptional) return true;

        foreach(SoloQuestHandler quest in soloQuestsSameTitle)
        {
            if(!quest.IsCompleted) return false;
        }
        return true;

    }
}
public class MultipleQuestHandler : QuestHandlerParent, IUnsubscribeEvent
{
    [Header("Solo Quest Array inside IS SAME TITLE ONLY - SAME TITLE DIFFERENT ISOPTIONAL IS ALSO DIFFERENT TITLE")]
    [SerializeField] private List<MultipleQuest> _multipleQuestData = new List<MultipleQuest>();

    private void Awake() 
    {
        for(int i=0; i < _multipleQuestData.Count; i++)
        {
            MultipleQuest multipleQuest = _multipleQuestData[i];
            multipleQuest.SetData();
            multipleQuest.multipleQuestHandler = this;
            foreach(SoloQuestHandler quest in multipleQuest.soloQuestsSameTitle)
            {
                quest.OnQuestCompleted += QuestComplete;
                quest.OnSoloQuestComplete += SoloQuestComplete_ChangeVisual;
                if(multipleQuest.isQuestShownAfterOtherQuest)
                {
                    if(multipleQuest.otherMultipleQuestArrayIndexToShowThisQuestIdx < _multipleQuestData.Count)
                    {
                        _multipleQuestData[multipleQuest.otherMultipleQuestArrayIndexToShowThisQuestIdx].OnMultipleQuestCompleted += multipleQuest.ActivateAllQuest_IsQuestShownAfterOtherQuest;
                    }
                }
            }
        }
    }
    protected override void Start() 
    {
        base.Start(); 
        foreach(MultipleQuest multipleQuest in _multipleQuestData)
        {
            multipleQuest.questGameUIHandler = _questGameUIHandler;
        } 
    }

    //yg dbwh naikin ke atas :D
    public override void ActivateQuest()
    {
        for(int i=0; i < _multipleQuestData.Count; i++)
        {
            MultipleQuest multipleQuest = _multipleQuestData[i];
            if(!multipleQuest.isQuestShownAfterOtherQuest)
            {
                foreach(SoloQuestHandler quest in multipleQuest.soloQuestsSameTitle)
                {
                    quest.ActivateQuest();
                }
            }
        }

        base.ActivateQuest();
    }
    public override void DeactivateQuest()
    {
        foreach(MultipleQuest multipleQuest in _multipleQuestData)
        {
            multipleQuest.DeactivateAllQuest();
        }
        base.DeactivateQuest();
    }
    protected override void HandleQuestComplete()
    {
        foreach(MultipleQuest multipleQuest in _multipleQuestData)
        {
            if(!multipleQuest.IsAllQuestCompleted()) return;
        }

        base.HandleQuestComplete();
    }

    public void UnsubscribeEvent()
    {
        for(int i=0; i < _multipleQuestData.Count; i++)
        {
            MultipleQuest multipleQuest = _multipleQuestData[i];
            foreach(SoloQuestHandler quest in multipleQuest.soloQuestsSameTitle)
            {
                quest.OnQuestCompleted -= QuestComplete;
                quest.OnSoloQuestComplete -= SoloQuestComplete_ChangeVisual;
                if(multipleQuest.isQuestShownAfterOtherQuest)
                {
                    _multipleQuestData[multipleQuest.otherMultipleQuestArrayIndexToShowThisQuestIdx].OnMultipleQuestCompleted -= multipleQuest.ActivateAllQuest_IsQuestShownAfterOtherQuest;
                }
            }
        }
    }

    public override void CallQuestContainerUI()
    {
        _questGameUIHandler.CallQuestContainer(this, true, false);
        foreach(MultipleQuest data in _multipleQuestData)
        {
            if(!data.isQuestShownAfterOtherQuest) _questGameUIHandler.CallQuestContainer(data.soloQuestHead, false, data.isOptional);
        }
    }

    public override void EndedCallQuestContainerUI()
    {
        foreach(MultipleQuest data in _multipleQuestData)
        {
            data.soloQuestHead.EndedCallQuestContainerUI();
        }
        _questGameUIHandler.HideCompletedQuestContainer(this);
    }

    #region MultipleQuestData Function
    // private MultipleQuest GetMultipleQuest(QuestName name, bool isOptional)
    // {
    //     foreach(MultipleQuest data in _multipleQuestData)
    //     {
    //         if(name == data.questName && data.isOptional == isOptional) return data;
    //     }
    //     return null;
    // }
    private void SoloQuestComplete_ChangeVisual(QuestName name, SoloQuestHandler quest)
    {
        // Debug.Log(name + " INI TOLONG MULTIPLE KENAPA" + quest + " " + quest.transform.name);
        foreach(MultipleQuest data in _multipleQuestData)
        {
            if(name == data.questName && data.IsFromThisMultipleQuestData(quest))
            {
                data.AddCurrTotalQuestCompleted();
            }
        }
    }
    #endregion
    
}

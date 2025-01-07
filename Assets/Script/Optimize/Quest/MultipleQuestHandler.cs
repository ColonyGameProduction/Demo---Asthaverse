using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultipleQuest
{
    public MultipleQuest(SoloQuestHandler quest)
    {
        soloQuestHead = quest;
        isOptional = quest.IsOptional;
        questName = quest.QuestName;
        questDescRaw = quest.QuestDesc;
        totalQuest = 1;
    }

    /// <summary>
    /// this just for connecting it with ui container
    /// </summary>
    public SoloQuestHandler soloQuestHead;
    public QuestName questName;
    public bool isOptional;
    public string questDescRaw;
    public int totalQuest;
    public int currTotalQuestCompleted;
    public void ChangeDesc()
    {
        if(totalQuest == 1)
        {
            if(currTotalQuestCompleted == totalQuest) QuestGameUIHandler.Instance.HideCompletedQuestContainer(soloQuestHead);
            return;
        }

        string questDescFinal = questDescRaw + " (" + currTotalQuestCompleted + "/" + totalQuest +")";
        soloQuestHead.QuestDesc = questDescFinal;
        soloQuestHead.OnChangeDescBeforeComplete?.Invoke();

        if(totalQuest == currTotalQuestCompleted)
        {
            QuestGameUIHandler.Instance.HideCompletedQuestContainer(soloQuestHead);
        }
    }

    public void AddTotalQuest()
    {
        totalQuest++;
        ChangeDesc();
    }
    public void AddCurrTotalQuestCompleted()
    {
        currTotalQuestCompleted++;
        ChangeDesc();
    }
}
public class MultipleQuestHandler : QuestHandlerParent, IUnsubscribeEvent
{
    [SerializeField] private List<SoloQuestHandler> _soloQuestList;
    private List<MultipleQuest> _multipleQuestData = new List<MultipleQuest>();

    private void Awake() 
    {
        foreach(SoloQuestHandler quest in _soloQuestList)
        {
            quest.OnQuestCompleted += QuestComplete;
            quest.OnSoloQuestComplete += SoloQuestComplete_ChangeVisual;

            MultipleQuest data = GetMultipleQuest(quest.QuestName, quest.IsOptional);
            if(data != null) data.AddTotalQuest();
            else
            {
                data = new MultipleQuest(quest);
                _multipleQuestData.Add(data);
            }
        }
    }

    public override void ActivateQuest()
    {
        foreach(SoloQuestHandler quest in _soloQuestList)
        {
            quest.ActivateQuest();
        }
        base.ActivateQuest();
    }
    public override void DeactivateQuest()
    {
        foreach(SoloQuestHandler quest in _soloQuestList)
        {
            quest.DeactivateQuest();
        }
        base.DeactivateQuest();
    }
    protected override void QuestComplete()
    {
        foreach(SoloQuestHandler quest in _soloQuestList)
        {
            if(!quest.IsCompleted && !quest.IsOptional) return;
        }

        base.QuestComplete();
        Debug.Log("Multiple Done NEXT");
    }

    public void UnsubscribeEvent()
    {
        foreach(SoloQuestHandler quest in _soloQuestList)
        {
            quest.OnQuestCompleted -= QuestComplete;
        }
    }

    public override void CallQuestContainerUI()
    {
        foreach(MultipleQuest data in _multipleQuestData)
        {
            _questGameUIHandler.CallQuestContainer(data.soloQuestHead);
        }
    }

    public override void EndedCallQuestContainerUI()
    {
        foreach(MultipleQuest data in _multipleQuestData)
        {
            _questGameUIHandler.HideCompletedQuestContainer(data.soloQuestHead);
        }
    }

    #region MultipleQuestData Function
    private MultipleQuest GetMultipleQuest(QuestName name, bool isOptional)
    {
        foreach(MultipleQuest data in _multipleQuestData)
        {
            if(name == data.questName && data.isOptional == isOptional) return data;
        }
        return null;
    }
    private void SoloQuestComplete_ChangeVisual(QuestName name, bool isOptional)
    {
        foreach(MultipleQuest data in _multipleQuestData)
        {
            if(name == data.questName && data.isOptional == isOptional)
            {
                data.AddCurrTotalQuestCompleted();
            }
        }
    }
    #endregion
}

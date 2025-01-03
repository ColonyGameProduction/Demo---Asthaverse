using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleQuestHandler : QuestHandlerParent, IUnsubscribeEvent
{
    [SerializeField] private List<SoloQuestHandler> _soloQuestList;

    private void Awake() 
    {
        foreach(SoloQuestHandler quest in _soloQuestList)
        {
            quest.OnQuestCompleted += QuestComplete;
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
}

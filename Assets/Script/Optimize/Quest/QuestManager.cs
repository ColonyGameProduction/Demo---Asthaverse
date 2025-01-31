using System;

using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IUnsubscribeEvent
{
    public static QuestManager Instance {get; private set;}
    [SerializeField] private List<QuestHandlerParent> _questList = new List<QuestHandlerParent>();

    [ReadOnly(false), SerializeField]private int _currQuestIdx;

    private QuestGameUIHandler _questGameUIHandler;

    #region  Event
    // public Action<QuestHandlerParent> OnQuestComplete;
    public Action OnLevelCompleted;
    public QuestHandlerParent CurrQuest {get {return _questList[_currQuestIdx];}}
    #endregion
    private void Awake() 
    {
        Instance = this;
        
        
    }
    private void Start() 
    {
        _questGameUIHandler = QuestGameUIHandler.Instance;
        foreach(QuestHandlerParent quest in _questList)
        {
            quest.OnQuestCompleted += NextQuest;
            quest.DeactivateQuest();
        }
    }

    //Handle UI yg list in di sini yaa
    public void ActivateQuest()
    {
        CurrQuest.CallQuestContainerUI();
        CurrQuest.ActivateQuest();
    }

    private void NextQuest()
    {
        // Debug.Log("NEXT QUEST");
        CurrQuest.EndedCallQuestContainerUI();
        
        if(_currQuestIdx == _questList.Count - 1)
        {
            // Debug.Log("Level Completed");
            _questGameUIHandler.HideQuestContainerUI();
            OnLevelCompleted?.Invoke();
            return;
        }

        _currQuestIdx++;
        _questGameUIHandler.ResetCurrActivatedChildIdx();
        ActivateQuest();
    }

    public void UnsubscribeEvent()
    {
        foreach(QuestHandlerParent quest in _questList)
        {
            quest.OnQuestCompleted -= NextQuest;
            quest.OnQuestCompleted -= _questGameUIHandler.ResetCurrActivatedChildIdx;
        }
    }

    
}

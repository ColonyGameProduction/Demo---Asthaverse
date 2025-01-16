using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class QuestHandlerParent : MonoBehaviour
{
    [Header("Quest Explanation")]
    [SerializeField] protected QuestName _questName;
    [TextArea(2 , 5)][SerializeField] protected string _questDesc;

    [Space(1)]
    [ReadOnly(false), SerializeField] protected bool _isActivated, _isCompleted;

    protected QuestManager _questManager;
    protected QuestGameUIHandler _questGameUIHandler;

    public Action OnQuestCompleted;
    public UnityEvent OnQuestCompletedEventFromOutside;

    #region  Getter Setter
    public bool IsActivated {get {return _isActivated;}}
    public bool IsCompleted {get {return _isCompleted;}}
    public QuestName QuestName {get {return _questName;}}
    public string QuestDesc {get {return _questDesc;} set{_questDesc = value;}}
    #endregion

    protected virtual void Start() 
    {
        _questManager = QuestManager.Instance;
        _questGameUIHandler = QuestGameUIHandler.Instance;
    }
    public virtual void ActivateQuest()
    {
        _isActivated = true;
    }
    public virtual void DeactivateQuest()
    {
        _isActivated = false;
    }

    protected virtual void QuestComplete()
    {
        if(!_isActivated) return;

        DeactivateQuest();
        _isCompleted = true;
        OnQuestCompleted?.Invoke();
        OnQuestCompletedEventFromOutside?.Invoke();
    }

    public abstract void CallQuestContainerUI();
    public abstract void EndedCallQuestContainerUI();
    
}

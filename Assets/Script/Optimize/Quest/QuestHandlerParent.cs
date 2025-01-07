using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestHandlerParent : MonoBehaviour
{
    [Header("Quest Explanation")]
    [SerializeField] protected QuestName _questName;
    [TextArea(2 , 5)][SerializeField] protected string _questDesc;

    [Space(1)]
    [ReadOnly(false), SerializeField] protected bool _isActivated, _isCompleted;

    protected QuestManager _questManager;

    public Action OnQuestCompleted;

    #region  Getter Setter
    public bool IsActivated {get {return _isActivated;}}
    public bool IsCompleted {get {return _isCompleted;}}
    public QuestName QuestName {get {return _questName;}}
    public string QuestDesc {get {return _questDesc;}}
    #endregion

    protected virtual void Start() 
    {
        _questManager = QuestManager.Instance;
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
    }
    
}

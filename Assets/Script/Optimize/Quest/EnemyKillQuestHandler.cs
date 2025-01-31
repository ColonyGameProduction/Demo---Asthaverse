
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillQuestHandler : SoloQuestHandler
{
    protected EnemyAIManager _enemyAIManager;
    [SerializeField]protected List<Transform> _enemyTargetList;
    protected int _totalTarget, _totalTargetDied;
    private string _questDescRaw;

    #region GETTER SETTER
    public int TotalTarget {get {return _totalTarget;}}
    public int TotalTargetDied {get {return _totalTargetDied;}}
    #endregion
    private void Awake() 
    {
        _totalTarget = _enemyTargetList.Count;
        _totalTargetDied = 0;
        _questDescRaw = _questDesc;
        ChangeDesc();
    }
    protected override void Start()
    {
        base.Start();
        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnEnemyDead += TargetDied;
    }

    private void TargetDied(Transform transform)
    {
        if(_enemyTargetList.Contains(transform))
        {
            EnemySnipingEventUIHandler snipingUI = transform.GetComponentInChildren<EnemySnipingEventUIHandler>();
            if(snipingUI != null) snipingUI.HideUI();

            _enemyTargetList.Remove(transform);
            _totalTargetDied++;
        }
        ChangeDesc();
        if(_totalTargetDied == _totalTarget) QuestComplete();
    }

    public override void ActivateQuest()
    {
        base.ActivateQuest();
        QuestComplete();
    }

    public override void DeactivateQuest()
    {
        base.DeactivateQuest();
    }
    
    protected override void HandleQuestComplete()
    {
        
        if(_totalTargetDied < _totalTarget) return;

        base.HandleQuestComplete();
    }

    private void ChangeDesc()
    {
        string questDescFinal = _questDescRaw + " (" + _totalTargetDied + "/" + TotalTarget +")";
        _questDesc = questDescFinal;
        OnChangeDescBeforeComplete?.Invoke();
    }
    public void ShowSnipingUITarget()
    {
        foreach(Transform target in _enemyTargetList)
        {
            EnemySnipingEventUIHandler snipingUI = target.GetComponentInChildren<EnemySnipingEventUIHandler>();
            if(snipingUI != null) snipingUI.ShowUI();
        }
    }

}

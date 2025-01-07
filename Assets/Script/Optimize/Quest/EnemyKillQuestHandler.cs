using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillQuestHandler : SoloQuestHandler
{
    protected EnemyAIManager _enemyAIManager;
    [SerializeField]protected List<Transform> _enemyTargetList;
    protected int _totalTarget, _totalTargetDied;

    #region GETTER SETTER
    public int TotalTarget {get {return _totalTarget;}}
    public int TotalTargetDied {get {return _totalTargetDied;}}
    #endregion
    private void Awake() 
    {
        _totalTarget = _enemyTargetList.Count;
        _totalTargetDied = 0;
    }
    protected override void Start()
    {
        base.Start();
        _enemyAIManager = EnemyAIManager.Instance;
        _enemyAIManager.OnEnemyDead += TargetDied;
    }

    private void TargetDied(Transform transform)
    {
        foreach(Transform target in _enemyTargetList)
        {
            if(_enemyTargetList.Contains(target))
            {
                _enemyTargetList.Remove(target);
                _totalTargetDied++;
                break;
            }
        }

        if(_totalTargetDied == _totalTarget) QuestComplete();
    }

    public override void ActivateQuest()
    {
        _isActivated = true;
        QuestComplete();
    }

    protected override void QuestComplete()
    {
        if(_totalTargetDied < _totalTarget) return;

        base.QuestComplete();
    }
}

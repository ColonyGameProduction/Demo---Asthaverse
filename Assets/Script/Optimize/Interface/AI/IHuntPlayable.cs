using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHuntPlayable
{
    public List<Transform> OtherVisibleTargets {get; }
    public Transform ClosestBreadCrumbs {get;}
    public Vector3 EnemyCharalastSeenPosition {get;set;}
    public bool HasToCheckEnemyLastSeenPosition {get;}
    void GetClosestBreadCrumbs();
    float GetMinimalPlayableStealth();
    void GoToEnemyLastSeenPosition(Vector3 enemyCharaLastSeenPosition);
    void IsCheckingEnemyLastPosition();
}

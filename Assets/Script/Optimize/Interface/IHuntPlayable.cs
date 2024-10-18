using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHuntPlayable
{
    public List<Transform> OtherVisibleTargets {get; }
    public Transform ClosestBreadCrumbs {get;}
    void GetClosestBreadCrumbs();
    float GetMinimalPlayableStealth();
}

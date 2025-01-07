using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps_HoldableObj : HoldableObj_IntObj
{
    protected override void WhenComplete()
    {
        base.WhenComplete();
        Debug.Log("Done laying trap!");
    }
}

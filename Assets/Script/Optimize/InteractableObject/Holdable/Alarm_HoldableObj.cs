using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm_HoldableObj : HoldableObj_IntObj
{
    protected override void WhenComplete()
    {
        Debug.Log("RINNGGGGGGGGGGGGGGGGGGGGG!!!!!");
    }
}

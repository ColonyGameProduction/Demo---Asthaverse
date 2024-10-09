using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStandMovementData
{
    public float WalkSpeed { get;}
    public float RunSpeed { get;}
    public bool IsIdle { get; set;}
    public bool IsWalking { get; set;}
    public bool IsRunning { get; set;}
}

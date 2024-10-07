using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStandMovement
{
    public float WalkSpeed { get;}
    public float RunSpeed { get;}
    public bool IsRunning { get; set;}
}

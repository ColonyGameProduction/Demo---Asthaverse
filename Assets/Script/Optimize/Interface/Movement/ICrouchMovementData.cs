using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGroundMovementData
{
    public float CrouchSpeed { get;}
    public bool IsCrouching { get; set;}
    public float CrawlSpeed { get;}
    public bool IsCrawling { get; set;}
}

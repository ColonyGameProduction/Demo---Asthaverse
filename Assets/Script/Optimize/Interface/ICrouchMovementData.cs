using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICrouchMovementData
{
    public float CrouchSpeed { get;}
    public bool IsCrouching { get; set;}
}

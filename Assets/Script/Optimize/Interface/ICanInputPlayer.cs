using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanInputPlayer
{
    public bool IsInputPlayer{ get; set; }
    event Action<bool> OnInputPlayerChange;
}

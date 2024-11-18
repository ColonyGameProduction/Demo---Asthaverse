using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReceiveInputFromPlayer
{
    public bool IsPlayerInput{ get; set; }
    event Action<bool> OnIsPlayerInputChange;
}

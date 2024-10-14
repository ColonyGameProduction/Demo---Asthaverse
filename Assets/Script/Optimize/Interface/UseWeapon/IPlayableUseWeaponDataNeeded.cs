using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayableUseWeaponDataNeeded
{
    event Action OnTurningOffScope;
    void TellToTurnOffScope();
}

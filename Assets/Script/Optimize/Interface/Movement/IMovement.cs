using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    void Move();
    void ForceStopMoving();
    public void CharaConDataToNormal();
    public void CharaConDataToCrouch();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayableMovementDataNeeded
{
    public Vector3 InputMovement { get; set;}
    bool IsMustLookForward{get; set;}
    void RotateToAim_Idle();
    public PlayableCharacterIdentity GetPlayableCharacterIdentity{ get ;}
    public PlayableMakeSFX GetPlayableMakeSFX {get ;}
    
}

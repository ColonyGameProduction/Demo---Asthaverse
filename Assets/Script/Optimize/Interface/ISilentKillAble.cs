using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISilentKillAble
{
    public Transform SilentKillAbleTransform{get;}
    public bool CanBeKill{get;}
    void GotSilentKill(PlayableCharacterIdentity characterIdentity);
}
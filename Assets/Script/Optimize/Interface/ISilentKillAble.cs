
using UnityEngine;

public interface ISilentKillAble
{
    public Transform GetSilentKillAbleTransform{get;}
    public bool CanBeKill{get;}
    void GotSilentKill(PlayableCharacterIdentity characterIdentity);
}

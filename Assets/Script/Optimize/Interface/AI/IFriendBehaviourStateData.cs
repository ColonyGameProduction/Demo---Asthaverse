using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFriendBehaviourStateData
{
    void GiveUpdateFriendDirection(PlayableCharacterIdentity currPlayer, Transform commandPos);
}

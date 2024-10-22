using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFriendBehaviourStateData
{
    void GiveUpdateFriendDirection(Transform currPlayer, Transform defaultPos, Transform commandPos);
}

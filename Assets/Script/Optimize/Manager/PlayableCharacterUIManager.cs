using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterUIManager : MonoBehaviour
{
    [SerializeField] private DamageUIHandler _dmgUIHandler;
    private PlayableCharacterIdentity currPlayable;

    public void ConnectUIHandler(PlayableCharacterIdentity curr)
    {
        currPlayable = curr;

        currPlayable.OnPlayableHealthChange += _dmgUIHandler.SetDamageVisualPhase;
    }
    public void DisconnectUIHandler()
    {
        if(currPlayable == null)
        {
            ResetUI();
            return;
        }

        currPlayable.OnPlayableHealthChange -= _dmgUIHandler.SetDamageVisualPhase;

        currPlayable = null;
        ResetUI();
    }

    public void ResetUI()
    {
        _dmgUIHandler.ResetDamageVisual();
    }
}

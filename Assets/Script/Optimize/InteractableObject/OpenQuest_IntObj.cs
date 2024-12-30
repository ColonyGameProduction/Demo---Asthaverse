using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenQuest_IntObj : InteractableObject
{
    [ReadOnly(false), SerializeField] protected bool _canInteract = true;
    public Action OnOpenQuest;
    public override bool CanInteract {get{ return _canInteract;}}
    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        characterIdentity.DoNormalInteractionAnimation();
        _canInteract = false;
        Debug.Log("Open Next Quest");
        characterIdentity.StopNormalInteractionAnimation();
        OnOpenQuest?.Invoke();
    }

}

using System;

using UnityEngine;

public class OpenQuest_IntObj : InteractableObject, IConnectToQuest
{
    [ReadOnly(false), SerializeField] protected bool _canInteract = true;
    public override bool CanInteract {get{ return _canInteract;}}

    public event Action OnTriggerQuestComplete;

    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        characterIdentity.DoNormalInteractionAnimation();
        _canInteract = false;
        // Debug.Log("Open Next Quest");
        characterIdentity.StopNormalInteractionAnimation();
        
        OnTriggerQuestComplete?.Invoke();
    }

}

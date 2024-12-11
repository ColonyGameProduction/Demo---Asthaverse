using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public Transform GetInteractableTransform{get;}
    public bool CanInteract{get;}
    void Interact(PlayableCharacterIdentity characterIdentity);
}


using UnityEngine;

public interface IInteractable
{
    public InteractObjType InteractObjType {get;}
    public Transform GetInteractableTransform{get;}
    public bool CanInteract{get;}
    void Interact(PlayableCharacterIdentity characterIdentity);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    // protected bool _canInteract = true;
    public Transform InteractableTransform {get{return transform;}}
    public virtual bool CanInteract {get;}

    public abstract void Interact(PlayableCharacterIdentity characterIdentity);
}

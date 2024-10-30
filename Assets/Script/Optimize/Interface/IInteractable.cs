using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public Transform InteractableTransform{get;}
    public bool CanInteract{get;}
    void Interact();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractObjType
{
    None, Revive, Pick_up, Interact
}
public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractObjType _intObjType;
    public InteractObjType InteractObjType {get {return _intObjType;}}
    // protected bool _canInteract = true;
    public Transform GetInteractableTransform {get{return transform;}}
    public virtual bool CanInteract {get;}

    public abstract void Interact(PlayableCharacterIdentity characterIdentity);
}

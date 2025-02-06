
using UnityEngine;

public enum InteractObjType
{
    None, Revive, Pick_up, Interact
}
public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractObjType _intObjType;
    [SerializeField] protected AudioSource _interactSFXAudioSource;
    public InteractObjType InteractObjType {get {return _intObjType;}}
    // protected bool _canInteract = true;
    public Transform GetInteractableTransform {get{return transform;}}
    public virtual bool CanInteract {get;}

    public virtual void Interact(PlayableCharacterIdentity characterIdentity)
    {
        ToggleInteractableObjAudio(_interactSFXAudioSource, true);
    }
    public virtual void ToggleInteractableObjAudio(AudioSource chosenAudioSource, bool play)
    {
        if(chosenAudioSource == null) return;

        if(play) chosenAudioSource.Play();
        else chosenAudioSource.Stop();
    }
}

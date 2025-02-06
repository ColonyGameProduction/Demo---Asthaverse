
using UnityEngine;


public abstract class OpenableObj_IntObj : InteractableObject
{
    protected bool _isAnimatingOpening;
    [ReadOnly(false), SerializeField] protected bool _isOpen;
    [SerializeField] protected float _animationOpenTimer = 0.5f;
    [SerializeField] protected GameObject _openObj;
    public bool IsOpen{get {return _isOpen;}}
    

    public override bool CanInteract {get {return !_isAnimatingOpening ? true : false;}}
    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        //jalankan animasi buka 
        // Debug.Log("Opening this");
        characterIdentity.DoNormalInteractionAnimation();
        ToggleOpenClose(characterIdentity.transform);
        characterIdentity.StopNormalInteractionAnimation();


        // StartCoroutine(OpenObj());
    }
    protected virtual void ToggleOpenClose(Transform chara)
    {
        _isOpen = !_isOpen;
        ToggleInteractableObjAudio(_interactSFXAudioSource, true);
    }

}

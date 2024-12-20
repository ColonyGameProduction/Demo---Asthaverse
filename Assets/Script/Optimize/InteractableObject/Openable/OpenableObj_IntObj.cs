using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
        ToggleOpenClose(characterIdentity.transform);
        // StartCoroutine(OpenObj());
    }
    protected virtual void ToggleOpenClose(Transform chara)
    {
        _isOpen = !_isOpen;
    }

}
